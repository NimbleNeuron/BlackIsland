using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class LobbyItemBookView : BaseUI
	{
		[SerializeField] private ToggleGroup toggleGroup = default;


		[SerializeField] private UISearch uiSearch = default;


		[SerializeField] private UIItemInfo uiItemInfo = default;


		[SerializeField] private UIMap uiMap = default;


		[SerializeField] private ExclamationMark exclamationMark = default;


		private readonly List<Toggle> toggles = new List<Toggle>();


		private ItemBookTab currentItemBookTab;


		private bool initUiMap;

		
		
		public event Action<ItemData> OnRightClickItem = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			uiSearch.OnClickListItem += OnClickCommonViewItem;
			uiSearch.OnRightClickItem += delegate(ItemData itemData) { OnRightClickItem(itemData); };
			uiItemInfo.OnRightClickItem += delegate(ItemData itemData) { OnRightClickItem(itemData); };
			uiItemInfo.OnClickItemHandler += OnChangeSelectItem;
			uiItemInfo.OnClickTreeItemHandler += OnChangeTreeItem;
			initUiMap = false;
			Transform transform = toggleGroup.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Toggle component = transform.GetChild(i).GetComponent<Toggle>();
				if (component != null)
				{
					toggles.Add(component);
				}
			}
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			InitUIMap();
			for (int i = 0; i < toggles.Count; i++)
			{
				Toggle toggle = toggles[i];
				ItemBookTab itemBookTab;
				if (Enum.TryParse<ItemBookTab>(toggle.name, out itemBookTab))
				{
					if (toggle.isOn)
					{
						SetTab(itemBookTab);
					}

					toggle.onValueChanged.AddListener(delegate(bool isOn)
					{
						if (isOn)
						{
							SetTab(itemBookTab);
						}
					});
				}
			}
		}


		private void InitUIMap()
		{
			if (!initUiMap)
			{
				uiMap.Init(GameDB.level.DefaultLevel);
				uiMap.SetMapMode(UIMap.MapModeFlag.SearchItem, UIMap.MapModeFlag.Restrict);
				initUiMap = true;
			}
		}


		public void SetTab(ItemBookTab itemBookTab)
		{
			currentItemBookTab = itemBookTab;
			for (int i = 0; i < toggles.Count; i++)
			{
				Toggle toggle = toggles[i];
				if (toggle.name == currentItemBookTab.ToString())
				{
					toggle.isOn = true;
				}
			}

			switch (itemBookTab)
			{
				case ItemBookTab.ItemTypeSearch:
					uiSearch.SetMode(UISearch.Mode.ItemType);
					return;
				case ItemBookTab.OptionSearch:
					uiSearch.SetMode(UISearch.Mode.ItemOption);
					return;
				case ItemBookTab.Recommend:
					break;
				case ItemBookTab.AreaSearch:
					uiSearch.SetMode(UISearch.Mode.Area);
					break;
				default:
					return;
			}
		}


		public void SetWeaponTypes(WeaponType[] weaponTypes)
		{
			uiSearch.SetWeaponTypes(weaponTypes);
			uiItemInfo.SetWeaponTypes(weaponTypes);
		}


		public void OnClickCommonViewItem(ItemData itemData)
		{
			uiItemInfo.ResetHistory();
			OnChangeSelectItem(itemData);
		}


		public void SetDraggable(bool draggable)
		{
			uiSearch.SetDraggable(draggable);
		}


		private void OnChangeSelectItem(ItemData itemData)
		{
			if (itemData != null)
			{
				uiSearch.SetFocusItem(itemData);
				uiItemInfo.SetTreeRootItem(itemData);
				InitUIMap();
				uiMap.SetSearchItem(itemData);
				ExclamationMark exclamationMark = this.exclamationMark;
				if (exclamationMark == null)
				{
					return;
				}

				exclamationMark.UpdateExcalmation(itemData);
			}
		}


		private void OnChangeTreeItem(ItemData itemData)
		{
			if (itemData != null)
			{
				uiItemInfo.SetFocusItem(itemData);
				InitUIMap();
				uiMap.SetSearchItem(itemData);
				ExclamationMark exclamationMark = this.exclamationMark;
				if (exclamationMark == null)
				{
					return;
				}

				exclamationMark.UpdateExcalmation(itemData);
			}
		}


		public void EmptyUI()
		{
			uiItemInfo.EmptyUI();
		}


		public void SetAnchoredPosition(Vector2 position)
		{
			transform.GetComponent<RectTransform>().anchoredPosition = position;
		}
	}
}