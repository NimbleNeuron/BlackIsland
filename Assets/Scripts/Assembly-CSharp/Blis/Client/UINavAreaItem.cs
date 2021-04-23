using System;
using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UINavAreaItem : BaseUI, ILnEventHander, ISlotEventListener
	{
		private const int MAX_SLOT_COUNT = 15;


		private const int MAX_COLLECT_AND_HUNT_SLOT_COUNT = 5;


		[SerializeField] private Text title = default;


		[SerializeField] private Transform extendView = default;


		[SerializeField] private GameObject extenViewBTNMax = default;


		[SerializeField] private ItemDataSlotTable itemDataSlotTable = default;


		[SerializeField] private ItemDataSlotTable collectAndHuntItemDataSlotTable = default;


		[SerializeField] private GameObject tutorialSquareNaviArea = default;


		[SerializeField] private GameObject tutorialBoxLeather = default;


		private readonly Dictionary<ItemData, int> collectAndHuntOwnItems = new Dictionary<ItemData, int>();


		private readonly Dictionary<ItemData, int> collectAndHuntSourceItems = new Dictionary<ItemData, int>();


		private readonly Dictionary<ItemData, int> ownItems = new Dictionary<ItemData, int>();


		private readonly Dictionary<ItemData, int> sourceItems = new Dictionary<ItemData, int>();


		private readonly List<ItemData> targetItems = new List<ItemData>();


		private Dictionary<int, AreaRestrictionState> areaStateMap = new Dictionary<int, AreaRestrictionState>();


		private int currentArea;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			UISystem.RemoveListener<NavigationAndCombineStore>(OnNavigationStoreUpdate);
		}


		public void OnLnDataChange()
		{
			UpdateTitle();
		}


		public void OnSlotLeftClick(Slot slot)
		{
			if (slot is ItemDataSlot)
			{
				ItemDataSlot itemDataSlot = (ItemDataSlot) slot;
				Action<ItemData> onNavMaterialItemClick = OnNavMaterialItemClick;
				if (onNavMaterialItemClick == null)
				{
					return;
				}

				onNavMaterialItemClick(itemDataSlot.GetItemData());
			}
		}


		public void OnSlotDoubleClick(Slot slot) { }


		public void OnSlotRightClick(Slot slot) { }


		public void OnDropItem(Slot slot, BaseUI draggedUI) { }


		public void OnThrowItem(Slot slot) { }


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot) { }


		public void OnPointerExit(Slot slot) { }

		
		
		public event Action<ItemData> OnNavMaterialItemClick = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			UISystem.AddListener<NavigationAndCombineStore>(OnNavigationStoreUpdate);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			itemDataSlotTable.SetSlotEventListener(this);
			collectAndHuntItemDataSlotTable.SetSlotEventListener(this);
		}


		private void OnNavigationStoreUpdate(NavigationAndCombineStore navigationAndCombineStore)
		{
			targetItems.Clear();
			targetItems.AddRange(navigationAndCombineStore.GetTargetItems());
			sourceItems.Clear();
			collectAndHuntSourceItems.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair in navigationAndCombineStore.GetNeedSourceItems())
			{
				if (GameDB.item.IsCollectAndHuntItem(keyValuePair.Key.code))
				{
					collectAndHuntSourceItems.Add(keyValuePair.Key, keyValuePair.Value);
				}
				else
				{
					sourceItems.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}

			ownItems.Clear();
			collectAndHuntOwnItems.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair2 in navigationAndCombineStore.GetOwnSourceItems())
			{
				if (GameDB.item.IsCollectAndHuntItem(keyValuePair2.Key.code))
				{
					collectAndHuntOwnItems.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
				else
				{
					ownItems.Add(keyValuePair2.Key, keyValuePair2.Value);
				}
			}

			RenderView();
		}


		private void RenderView()
		{
			RenderSlotTable(itemDataSlotTable, sourceItems, ownItems, 15, true);
			RenderSlotTable(collectAndHuntItemDataSlotTable, collectAndHuntSourceItems, collectAndHuntOwnItems, 5,
				false);
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
			    MonoBehaviourInstance<TutorialController>.inst.TutorialType == TutorialType.PowerUp)
			{
				bool flag = false;
				using (Dictionary<ItemData, int>.KeyCollection.Enumerator enumerator =
					collectAndHuntOwnItems.Keys.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.code == 401103)
						{
							flag = true;
							break;
						}
					}
				}

				if (!flag)
				{
					ShowTutorialSquareBoxLeather(false);
				}
			}

			UpdateTitle();
			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateButtonState();
		}


		private void RenderSlotTable(ItemDataSlotTable slotTable, Dictionary<ItemData, int> source,
			Dictionary<ItemData, int> own, int MaxSlotCount, bool tutorial)
		{
			slotTable.Clear();
			int num = 0;
			int num2 = 0;
			foreach (KeyValuePair<ItemData, int> keyValuePair in source)
			{
				if (keyValuePair.Value > 0 &&
				    (Singleton<ItemService>.inst.IsDropArea(currentArea, keyValuePair.Key.code) ||
				     GameDB.item.IsCollectAndHuntItem(keyValuePair.Key.code)) && keyValuePair.Key.IsLeafNodeItem() &&
				    MaxSlotCount > num)
				{
					ItemDataSlot itemDataSlot = slotTable.CreateSlot(keyValuePair.Key);
					int num3 = 0;
					if (own.ContainsKey(keyValuePair.Key))
					{
						num3 = own[keyValuePair.Key];
					}

					int num4 = keyValuePair.Value - num3;
					if (num4 <= 0)
					{
						RenderSlot(itemDataSlot, keyValuePair.Key, null, true);
						num2++;
					}
					else if (keyValuePair.Value > 1)
					{
						RenderSlot(itemDataSlot, keyValuePair.Key,
							string.Format("{0}/{1}", keyValuePair.Value - num4, keyValuePair.Value), false);
					}
					else
					{
						RenderSlot(itemDataSlot, keyValuePair.Key, null, false);
					}

					num++;
				}
			}

			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial && tutorial && num > 0 && num2 == num)
			{
				MonoBehaviourInstance<TutorialController>.inst.CreateHaveAllOwnItemTutorial();
			}
		}


		private void RenderSlot(ItemDataSlot itemDataSlot, ItemData itemData, string stackText, bool isOwn)
		{
			itemDataSlot.SetItemData(itemData);
			itemDataSlot.SetSlotType(SlotType.Navigation);
			itemDataSlot.SetSprite(itemData.GetSprite());
			itemDataSlot.SetBackground(itemData.GetGradeSprite());
			itemDataSlot.EnableOwnMark(isOwn);
			itemDataSlot.SetStackText(stackText);
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			foreach (int num in Singleton<ItemService>.inst.GetDropArea(itemData.code))
			{
				flag = true;
				flag3 = flag3 || areaStateMap[num] != AreaRestrictionState.Restricted;
				flag2 = flag2 || num == currentArea;
			}

			if (flag2)
			{
				itemDataSlot.EnableNeedMark(true);
				return;
			}

			if (!flag)
			{
				itemDataSlot.EnableRandomMark(true);
				return;
			}

			if (!flag3)
			{
				itemDataSlot.EnableBlockLine(true);
			}
		}


		private void UpdateTitle()
		{
			AreaData areaData = GameDB.level.GetAreaData(currentArea);
			title.text = areaData == null ? Ln.Get("현재 지역") : LnUtil.GetAreaName(areaData.code);
		}


		public void SetCurrentArea(int areaCode)
		{
			currentArea = areaCode;
			RenderView();
		}


		public void SetAreaStateMap(Dictionary<int, AreaRestrictionState> areaStateMap)
		{
			this.areaStateMap = areaStateMap;
			RenderView();
		}


		public void ToggleExtendView()
		{
			extendView.gameObject.SetActive(!extendView.gameObject.activeSelf);
			extenViewBTNMax.SetActive(!extendView.gameObject.activeSelf);
		}


		public void ShowTutorialSquareNaviArea(bool show)
		{
			tutorialSquareNaviArea.SetActive(show);
		}


		public void ShowTutorialSquareBoxLeather(bool show)
		{
			if (show)
			{
				int num = 0;
				using (Dictionary<ItemData, int>.Enumerator enumerator = collectAndHuntSourceItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<ItemData, int> keyValuePair = enumerator.Current;
						if (keyValuePair.Key.code == 401103)
						{
							Vector3 localPosition = tutorialBoxLeather.transform.localPosition;
							tutorialBoxLeather.transform.localPosition = new Vector3(localPosition.x + 51.5f * num,
								localPosition.y, localPosition.z);
							tutorialBoxLeather.transform.SetAsLastSibling();
							tutorialBoxLeather.SetActive(true);
							break;
						}

						num++;
					}

					return;
				}
			}

			tutorialBoxLeather.SetActive(false);
		}
	}
}