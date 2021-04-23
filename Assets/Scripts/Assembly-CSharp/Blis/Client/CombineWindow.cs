using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class CombineWindow : BaseWindow
	{
		private const string ActiveMapKey = "ActiveCombineWindowMap";


		[SerializeField] private GameObject tutorialBoxCombine = default;


		[SerializeField] private UIItemRecommend uiItemRecommend = default;


		[SerializeField] private UISearch uiSearch = default;


		[SerializeField] private UIItemInfo uiItemInfo = default;


		[SerializeField] private ToggleGroup toggleGroup = default;


		private readonly List<Item> equipment = new List<Item>();


		private readonly List<Item> inventory = new List<Item>();


		private readonly List<Toggle> toggles = new List<Toggle>();


		private CanvasGroup canvasGroup;


		private ItemBookTab currentItemBookTab;


		private ItemData currentSelectedItem;


		private bool initUiMap;


		private Camera mainCamera;


		private RectTransform mapArrow;


		private Button mapButton;


		private RectTransform mapRect;


		private UIMap uiMap;


		public UIMap UIMap => uiMap;

		
		
		public event Action<ItemData> OnRequestAddItem = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}

			mapButton = GameUtil.Bind<Button>(gameObject, "MapBtn");
			mapArrow = GameUtil.Bind<RectTransform>(gameObject, "MapBtn/Arrow");
			mapRect = GameUtil.Bind<RectTransform>(gameObject, "MapRect");
			uiMap = GameUtil.Bind<UIMap>(gameObject, "MapRect/Map");
			initUiMap = false;
			mapButton.onClick.AddListener(ToggleMap);
			uiMap.OnDragEvent += OnDragMap;
			uiMap.OnEndDragEvent += OnEndDragMap;
			uiItemRecommend.OnClickListItem += OnChangeSelectListItemAndReset;
			uiItemRecommend.OnRequestCombine += CombineItem;
			uiSearch.OnClickListItem += OnChangeSelectListItemAndReset;
			uiSearch.OnRightClickItem += CombineItem;
			uiSearch.SetDraggable(false);
			uiItemInfo.OnClickItemHandler += OnChangeSelectItem;
			uiItemInfo.OnClickTreeItemHandler += OnChangeTreeItem;
			uiItemInfo.OnRightClickItem += CombineItem;
			uiItemInfo.OnRequestCombineItem += CombineItem;
			uiItemInfo.OnClickNavItem += AddNavigationItem;
			uiItemInfo.OnClickAdminItem += AdminItem;
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
			EnableMap(PlayerPrefs.GetInt("ActiveCombineWindowMap", 1) == 1);
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

			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				SetTab(ItemBookTab.Recommend);
				return;
			}

			SetTab(ItemBookTab.ItemTypeSearch);
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
					uiItemRecommend.Hide();
					uiSearch.Show();
					uiSearch.SetMode(UISearch.Mode.ItemType);
					return;
				case ItemBookTab.OptionSearch:
					uiSearch.Show();
					uiItemRecommend.Hide();
					uiSearch.SetMode(UISearch.Mode.ItemOption);
					return;
				case ItemBookTab.Recommend:
					uiItemRecommend.Show();
					uiSearch.Hide();
					return;
				default:
					return;
			}
		}


		protected override void OnOpen()
		{
			base.OnOpen();
			uiSearch.SetLockInputSearch(true);
			uiSearch.OnUpdateInventory(inventory);
			uiItemInfo.OnUpdateInventory(inventory);
			uiSearch.OnUpdateEquipment(equipment);
			uiItemInfo.OnUpdateEquipment(equipment);
			for (int i = 0; i < toggles.Count; i++)
			{
				if (toggles[i].name == ItemBookTab.Recommend.ToString())
				{
					toggles[i].gameObject.SetActive(MonoBehaviourInstance<ClientService>.inst.IsPlayer);
				}
			}

			if (currentSelectedItem != null)
			{
				ShowTutorialSquareBoxCombine(currentSelectedItem);
			}
		}


		protected override void OnClose()
		{
			base.OnClose();
			MonoBehaviourInstance<Tooltip>.inst.Hide(this);
			uiSearch.SetLockInputSearch(false);
			uiItemInfo.HighLightUpperGrade(false);
			uiItemInfo.ResetHistory();
		}


		public void SelectItem(ItemData rootItem, ItemData treeSelectionItem = null)
		{
			OnChangeSelectListItemAndReset(rootItem);
			OnChangeTreeItem(treeSelectionItem);
		}


		public void SetWeaponTypes(WeaponType[] weaponTypes)
		{
			uiSearch.SetWeaponTypes(weaponTypes);
			uiSearch.SetMode(UISearch.Mode.ItemType);
			uiItemInfo.SetWeaponTypes(weaponTypes);
		}


		private void OnChangeSelectListItemAndReset(ItemData itemData)
		{
			uiItemInfo.ResetHistory();
			OnChangeSelectItem(itemData);
		}


		private void OnChangeSelectItem(ItemData itemData)
		{
			if (itemData != null)
			{
				uiItemRecommend.SetFocusItem(itemData);
				uiSearch.SetFocusItem(itemData);
				uiItemInfo.SetTreeRootItem(itemData);
				InitUIMap();
				uiMap.SetSearchItem(itemData);
				currentSelectedItem = itemData;
				ShowTutorialSquareBoxCombine(itemData);
			}
		}


		private void InitUIMap()
		{
			if (!initUiMap)
			{
				uiMap.Init(MonoBehaviourInstance<ClientService>.inst.CurrentLevel);
				uiMap.SetMapMode(UIMap.MapModeFlag.Restrict, UIMap.MapModeFlag.SearchItem);
				initUiMap = true;
			}
		}


		private bool IsHave(int itemCode)
		{
			return MonoBehaviourInstance<ClientService>.inst.IsPlayer &&
			       (MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.GetEquipments()
				       .Exists(x => x.ItemData.code == itemCode) || MonoBehaviourInstance<ClientService>.inst.MyPlayer
				       .Inventory.GetItems().Exists(x => x.ItemData.code == itemCode));
		}


		private void OnChangeTreeItem(ItemData itemData)
		{
			if (itemData != null)
			{
				uiItemInfo.SetFocusItem(itemData);
				InitUIMap();
				uiMap.SetSearchItem(itemData);
				currentSelectedItem = itemData;
				if (MonoBehaviourInstance<GameUI>.inst.CombineWindow.IsOpen)
				{
					ShowTutorialSquareBoxCombine(itemData);
				}
			}
		}


		private void CombineItem(ItemData itemData)
		{
			if (itemData != null && MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				SingletonMonoBehaviour<PlayerController>.inst.MakeItem(itemData);
			}
		}


		public void CombineSelectedItem()
		{
			CombineItem(currentSelectedItem);
		}


		private void AddNavigationItem(ItemData itemData)
		{
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("훈련 중에는 목표 아이템을 추가할 수 없습니다."), new Popup.Button
				{
					text = Ln.Get("확인")
				});
				return;
			}

			if (itemData != null)
			{
				OnRequestAddItem(itemData);
			}
		}


		private void ShowTutorialSquareBoxCombine(ItemData itemData)
		{
			tutorialBoxCombine.SetActive(false);
			if (SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene &&
			    MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
			    (itemData.code == 101201 || itemData.code == 401211) && IsHave(itemData.makeMaterial1) &&
			    IsHave(itemData.makeMaterial2))
			{
				MonoBehaviourInstance<TutorialController>.inst.ShowTutorialBoxCombine(itemData.code);
			}
		}


		public void OnInventoryUpdate(List<Item> items)
		{
			inventory.Clear();
			inventory.AddRange(items);
			if (IsOpen)
			{
				uiSearch.OnUpdateInventory(items);
				uiItemInfo.OnUpdateInventory(items);
			}
		}


		public void OnUpdateEquipment(List<Item> items)
		{
			equipment.Clear();
			equipment.AddRange(items);
			if (IsOpen)
			{
				uiSearch.OnUpdateEquipment(items);
				uiItemInfo.OnUpdateEquipment(items);
			}
		}


		public void OnUpdateButtonState()
		{
			uiItemInfo.OnUpdateButtonState();
		}


		public void AdminItem(ItemData itemData)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				SingletonMonoBehaviour<PlayerController>.inst.AdminCreateItem(itemData);
			}
		}


		public void ShowTutorialBoxCombine(bool show)
		{
			tutorialBoxCombine.SetActive(show);
		}


		public void Init(int characterCode)
		{
			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(characterCode);
			List<WeaponType> list = new List<WeaponType>();
			list.Add(characterMasteryData.weapon1.GetWeaponType());
			list.Add(characterMasteryData.weapon2.GetWeaponType());
			list.Add(characterMasteryData.weapon3.GetWeaponType());
			list.Add(characterMasteryData.weapon4.GetWeaponType());
			list.RemoveAll(x => x == WeaponType.None);
			uiSearch.SetWeaponTypes(list.ToArray());
		}


		public void SetRecommendData(Dictionary<RecommendItemType, List<ItemData>> recommendDatas,
			List<ItemData> preferItems)
		{
			uiItemRecommend.SetRecommendData(recommendDatas);
			uiSearch.SetPreferItems(preferItems);
			uiItemInfo.SetPreferItems(preferItems);
			if (0 < preferItems.Count)
			{
				SelectItem(preferItems[0]);
			}
		}


		public void HighLightUpperGrade()
		{
			uiItemInfo.HighLightUpperGrade(true);
		}


		public void ToggleMap()
		{
			EnableMap(!mapRect.gameObject.activeSelf);
		}


		public void EnableMap(bool enable)
		{
			mapRect.gameObject.SetActive(enable);
			PlayerPrefs.SetInt("ActiveCombineWindowMap", enable ? 1 : 0);
			mapArrow.localRotation = enable ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.Euler(0f, 0f, 180f);
		}


		private void OnDragMap(BaseControl control, PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				BaseWindow parentWindow = control.GetParentWindow();
				if (parentWindow != null)
				{
					parentWindow.transform.Translate(eventData.delta);
				}
			}
		}


		private void OnClickMap(BaseControl control, PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Right &&
			    MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				Vector3 destination = GameUIUtility.MapSpaceUVToWorldPos(
					(GameUIUtility.ScreenToRectPos(mainCamera,
						(RectTransform) MonoBehaviourInstance<GameUI>.inst.transform, control.rectTransform,
						eventData.position) + control.rectTransform.sizeDelta * 0.5f) /
					control.rectTransform.sizeDelta);
				SingletonMonoBehaviour<PlayerController>.inst.MoveTo(destination, false);
			}
		}


		private void OnEndDragMap(BaseControl control, PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				BaseWindow parentWindow = control.GetParentWindow();
				RectTransform rectTransform = (RectTransform) parentWindow.transform;
				Vector3[] array = new Vector3[4];
				rectTransform.GetWorldCorners(array);
				Vector2 size = rectTransform.rect.size;
				Vector2 zero = Vector2.zero;
				if (array[1].x + size.x < GameConstants.SCREEN_DRAG_MARGIN.x)
				{
					zero.x = GameConstants.SCREEN_DRAG_MARGIN.x - (array[1].x + size.x);
				}
				else if (Screen.width - array[3].x + size.x < GameConstants.SCREEN_DRAG_MARGIN.x)
				{
					zero.x = Screen.width - array[3].x + size.x - GameConstants.SCREEN_DRAG_MARGIN.x;
				}

				if (array[3].y + size.y < GameConstants.SCREEN_DRAG_MARGIN.y)
				{
					zero.y = GameConstants.SCREEN_DRAG_MARGIN.y - (array[3].y + size.y);
				}
				else if (Screen.height - array[1].y < 0f)
				{
					zero.y = Screen.height - array[1].y;
				}

				parentWindow.transform.Translate(zero);
			}
		}


		public void OnUpdateRestrictedArea(Dictionary<int, AreaRestrictionState> areaStateMap)
		{
			InitUIMap();
			uiMap.OnUpdateRestrictedArea(areaStateMap);
		}


		public void UpdateMapPlayerPosition(int objectId, bool isAlly, Vector3 worldPos)
		{
			if (!IsOpen)
			{
				return;
			}

			InitUIMap();
			uiMap.UpdatePlayerPosition(objectId, isAlly, worldPos);
		}
	}
}