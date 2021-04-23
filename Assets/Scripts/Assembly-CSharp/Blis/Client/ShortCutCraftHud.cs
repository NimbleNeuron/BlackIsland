using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ShortCutCraftHud : BaseUI
	{
		private readonly HashSet<ItemData> needItemSet = new HashSet<ItemData>();
		private Transform boxParent;


		private CanvasAlphaTweener canvasAlphaTweener;


		private CanvasGroup canvasGroup;


		private GameObject highlightBox;


		private bool isEnabled;


		private ItemDataSlotTable itemDataSlotTable;


		private Image shortCutBg;


		private GameObject shortCutIcon;


		public List<ItemData> CombineableItems { get; } = new List<ItemData>();


		private void LateUpdate()
		{
			if (!SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
			{
				return;
			}

			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			if (inst == null)
			{
				return;
			}

			if (!inst.IsGameStarted)
			{
				return;
			}

			MyPlayerContext myPlayer = inst.MyPlayer;
			if (myPlayer == null)
			{
				return;
			}

			if (myPlayer.Character == null)
			{
				return;
			}

			GameUI inst2 = MonoBehaviourInstance<GameUI>.inst;
			if (inst2 == null)
			{
				return;
			}

			if (myPlayer.Character.IsRest || inst2.Caster.IsCasting() ||
			    !myPlayer.Character.CanAnyAction(ActionType.Craft))
			{
				EnableHud(false);
				return;
			}

			EnableHud(true);
		}


		protected override void OnDestroy()
		{
			base.OnDestroy();
			UISystem.RemoveListener<NavigationAndCombineStore>(OnNavigationStoreUpdate);
		}


		protected override void OnAwakeUI()
		{
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			GameUtil.Bind<CanvasAlphaTweener>(gameObject, ref canvasAlphaTweener);
			GameUtil.Bind<ItemDataSlotTable>(gameObject, ref itemDataSlotTable);
			UISystem.AddListener<NavigationAndCombineStore>(OnNavigationStoreUpdate);
			shortCutIcon = transform.FindRecursively("CraftIcon").gameObject;
			shortCutBg = GameUtil.Bind<Image>(gameObject, "Bg");
			boxParent = transform.FindRecursively("BoxParent");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			itemDataSlotTable.SetSlotEventListener(new ShortCutEventListener(this));
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				GameObject original =
					SingletonMonoBehaviour<ResourceManager>.inst.LoadTutorialPrefab("TutorialArrowBox");
				highlightBox = Instantiate<GameObject>(original, boxParent);
			}
		}


		private void OnNavigationStoreUpdate(NavigationAndCombineStore navigationAndCombineStore)
		{
			needItemSet.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair in navigationAndCombineStore.GetNeedSourceItems())
			{
				if (!keyValuePair.Key.IsLeafNodeItem())
				{
					needItemSet.Add(keyValuePair.Key);
				}
			}

			CombineableItems.Clear();
			CombineableItems.AddRange(navigationAndCombineStore.GetCombinableItems());
			CombineableItems.Sort(delegate(ItemData x, ItemData y)
			{
				int num = needItemSet.Contains(y).CompareTo(needItemSet.Contains(x));
				if (num == 0)
				{
					num = y.itemGrade.CompareTo(x.itemGrade);
				}

				if (num == 0)
				{
					num = x.code.CompareTo(y.code);
				}

				return num;
			});
			RenderView();
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				if (MonoBehaviourInstance<TutorialController>.inst.TutorialType == TutorialType.BasicGuide)
				{
					MonoBehaviourInstance<TutorialController>.inst.UpdateCombineableTutorialArrow(CombineableItems);
					return;
				}

				if (MonoBehaviourInstance<TutorialController>.inst.TutorialType == TutorialType.PowerUp)
				{
					bool flag = false;
					foreach (ItemData itemData in CombineableItems)
					{
						if (flag)
						{
							break;
						}

						foreach (ItemData itemData2 in needItemSet)
						{
							if (itemData.code == itemData2.code)
							{
								flag = true;
								MonoBehaviourInstance<TutorialController>.inst.ShowShortCutHighlight(itemData.code);
								break;
							}
						}
					}

					if (!flag)
					{
						MonoBehaviourInstance<GameUI>.inst.ShortCutCraftHud.ShowHighlightBox(false);
					}
				}
				else if (MonoBehaviourInstance<TutorialController>.inst.TutorialType == TutorialType.FinalSurvival)
				{
					MonoBehaviourInstance<TutorialController>.inst.ShowShortCutHighlight(117201);
				}
			}
		}


		public ItemDataSlot FindDataSlot(ItemData itemData)
		{
			return itemDataSlotTable.FindSlot(itemData);
		}


		private void RenderView()
		{
			itemDataSlotTable.Clear();
			for (int i = 0; i < CombineableItems.Count; i++)
			{
				ItemData itemData = CombineableItems[i];
				ItemDataSlot itemDataSlot = itemDataSlotTable.CreateSlot(itemData);
				if (itemDataSlot == null)
				{
					break;
				}

				itemDataSlot.SetItemData(itemData);
				itemDataSlot.SetSlotType(SlotType.ShortCut);
				itemDataSlot.SetSprite(itemData.GetSprite());
				itemDataSlot.SetBackground(itemData.GetGradeSprite());
				if (itemData.itemType == ItemType.Consume)
				{
					ItemConsumableData subTypeData = itemData.GetSubTypeData<ItemConsumableData>();
					itemDataSlot.EnableHPIcon(subTypeData.consumableType == ItemConsumableType.Food);
					itemDataSlot.EnableSPIcon(subTypeData.consumableType == ItemConsumableType.Beverage);
				}

				if (needItemSet.Contains(itemData))
				{
					itemDataSlot.EnableNeedMark(true);
				}

				if (itemData.initialCount > 1)
				{
					itemDataSlot.SetStackText(itemData.initialCount.ToString());
				}
			}

			shortCutIcon.SetActive(CombineableItems.Count > 0);
			shortCutBg.enabled = CombineableItems.Count > 0;
			foreach (ItemData t in needItemSet)
			{
				ItemDataSlot itemDataSlot2 = itemDataSlotTable.FindSlot(t);
				if (itemDataSlot2 != null)
				{
					itemDataSlot2.EnableNeedMark(true);
				}
			}
		}


		private void EnableHud(bool enable)
		{
			if (isEnabled != enable)
			{
				if (enable)
				{
					canvasAlphaTweener.from = 0.5f;
					canvasAlphaTweener.to = 1f;
					canvasAlphaTweener.PlayAnimation();
					canvasGroup.blocksRaycasts = true;
				}
				else
				{
					canvasAlphaTweener.from = 1f;
					canvasAlphaTweener.to = 0.5f;
					canvasAlphaTweener.PlayAnimation();
					canvasGroup.blocksRaycasts = false;
				}

				isEnabled = enable;
			}
		}


		public void ShowHighlight(bool active, int targetItemCode)
		{
			if (highlightBox == null)
			{
				return;
			}

			if (targetItemCode == 101201 || targetItemCode == 401211)
			{
				MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowTutorialBoxNavi(active, targetItemCode);
			}
			else
			{
				ShowHighlightBox(active, targetItemCode);
			}

			if (active && targetItemCode == 401303)
			{
				MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowTutorialBoxNavi(false, 401211);
				MonoBehaviourInstance<TutorialController>.inst.ShowTutorialMotor();
			}
		}


		public void ShowHighlightBox(bool active, int targetItemCode = -1)
		{
			if (highlightBox == null)
			{
				return;
			}

			ItemData itemData = CombineableItems.Find(x => x.code == targetItemCode);
			if (!active || itemData == null)
			{
				highlightBox.SetActive(false);
				return;
			}

			int num = itemDataSlotTable.FindSlotIndex(itemData);
			int num2 = (CombineableItems.Count > 5 ? 5 : CombineableItems.Count) - (num + 1);
			highlightBox.SetActive(active);
			highlightBox.transform.localPosition = new Vector3(51f - num2 * 55f, 0f, 0f);
		}


		private bool PlaySourceItemEquipFrame(Item source)
		{
			EquipItemSlot equalEquipItemId =
				MonoBehaviourInstance<GameUI>.inst.StatusHud.GetEqualEquipItemId(source.id);
			if (equalEquipItemId != null)
			{
				equalEquipItemId.PlaySourceItemFrame();
				return true;
			}

			return false;
		}


		private bool PlaySourceItemInvenFrame(Item source)
		{
			InvenItemSlot equalInventoryItemId =
				MonoBehaviourInstance<GameUI>.inst.InventoryHud.GetEqualInventoryItemId(source.id);
			if (equalInventoryItemId != null)
			{
				equalInventoryItemId.PlaySourceItemFrame();
				return true;
			}

			return false;
		}


		private void PlaySourceItemFrame(ItemType itemType, Item item)
		{
			if (itemType == ItemType.Weapon)
			{
				if (!PlaySourceItemEquipFrame(item))
				{
					PlaySourceItemInvenFrame(item);
				}
			}
			else if (!PlaySourceItemInvenFrame(item))
			{
				PlaySourceItemEquipFrame(item);
			}
		}


		public void PlayCraftItemWithSourceItemFrame(ItemData targetItemData)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			MyPlayerContext myPlayer = MonoBehaviourInstance<ClientService>.inst.MyPlayer;
			Item itemFromCharacter = GameDB.item.GetItemFromCharacter(targetItemData.makeMaterial1,
				targetItemData.itemType, myPlayer.Inventory, myPlayer.Character.Equipment);
			Item itemFromCharacter2 = GameDB.item.GetItemFromCharacter(targetItemData.makeMaterial2,
				targetItemData.itemType, myPlayer.Inventory, myPlayer.Character.Equipment);
			if (!GameDB.item.IsCombinable(targetItemData, myPlayer.Inventory, myPlayer.Character.Equipment,
				ref itemFromCharacter, ref itemFromCharacter2))
			{
				return;
			}

			PlaySourceItemFrame(targetItemData.itemType, itemFromCharacter);
			PlaySourceItemFrame(targetItemData.itemType, itemFromCharacter2);
		}


		private class ShortCutEventListener : ISlotEventListener
		{
			private readonly ShortCutCraftHud shortCutCraftHud;

			public ShortCutEventListener(ShortCutCraftHud shortCutCraftHud)
			{
				this.shortCutCraftHud = shortCutCraftHud;
			}


			public void OnSlotLeftClick(Slot slot)
			{
				ItemDataSlot itemDataSlot = slot as ItemDataSlot;
				if (itemDataSlot == null)
				{
					return;
				}

				ItemData itemData = itemDataSlot.GetItemData();
				if (itemData == null)
				{
					return;
				}

				SingletonMonoBehaviour<PlayerController>.inst.MakeItem(itemData);
				MonoBehaviourInstance<Tooltip>.inst.Hide();
			}


			public void OnSlotRightClick(Slot slot)
			{
				ItemDataSlot itemDataSlot = slot as ItemDataSlot;
				if (itemDataSlot == null)
				{
					return;
				}

				ItemData itemData = itemDataSlot.GetItemData();
				if (itemData == null)
				{
					return;
				}

				MonoBehaviourInstance<GameUI>.inst.OpenWindow(MonoBehaviourInstance<GameUI>.inst.CombineWindow);
				MonoBehaviourInstance<GameUI>.inst.CombineWindow.SelectItem(itemData);
			}


			public void OnDropItem(Slot slot, BaseUI draggedUI) { }


			public void OnThrowItem(Slot slot) { }


			public void OnThrowItemPiece(Slot slot) { }


			public void OnPointerEnter(Slot slot)
			{
				ItemDataSlot itemDataSlot = slot as ItemDataSlot;
				if (itemDataSlot == null)
				{
					return;
				}

				ItemData itemData = itemDataSlot.GetItemData();
				if (itemData == null)
				{
					return;
				}

				shortCutCraftHud.PlayCraftItemWithSourceItemFrame(itemData);
				EquipItemSlot equipItemEqualType =
					MonoBehaviourInstance<GameUI>.inst.StatusHud.GetEquipItemEqualType(itemData);
				if (equipItemEqualType != null)
				{
					equipItemEqualType.PlayFocusFrame();
				}
			}


			public void OnPointerExit(Slot slot)
			{
				if (!SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene)
				{
					return;
				}

				if (MonoBehaviourInstance<ClientService>.inst == null ||
				    MonoBehaviourInstance<ClientService>.inst.isCrafting)
				{
					return;
				}

				foreach (EquipItemSlot equipItemSlot in MonoBehaviourInstance<GameUI>.inst.StatusHud.EquipSlots)
				{
					equipItemSlot.StopFocusFrame();
					equipItemSlot.StopSourceItemFrame();
				}

				foreach (InvenItemSlot invenItemSlot in MonoBehaviourInstance<GameUI>.inst.InventoryHud.InvenSlots)
				{
					invenItemSlot.StopFocusFrame();
					invenItemSlot.StopSourceItemFrame();
				}
			}


			public void OnSlotDoubleClick(Slot slot) { }
		}
	}
}