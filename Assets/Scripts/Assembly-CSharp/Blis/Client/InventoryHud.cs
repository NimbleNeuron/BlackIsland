using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class InventoryHud : BaseUI, ISlotEventListener
	{
		[SerializeField] private GameObject tutorialSquareInven = default;


		private Dictionary<ItemData, float> cooldownMap = new Dictionary<ItemData, float>();


		private Item[] inventory;


		public List<InvenItemSlot> InvenSlots { get; } = new List<InvenItemSlot>();


		protected override void OnDestroy()
		{
			base.OnDestroy();
			UISystem.RemoveListener<NavigationAndCombineStore>(UpdateInvenNeedMarkUI);
		}


		public void OnSlotLeftClick(Slot slot)
		{
			SlotLeftClick(slot, false, Vector3.zero);
		}


		public void OnSlotRightClick(Slot slot)
		{
			ItemSlot itemSlot = slot as ItemSlot;
			if (itemSlot != null && itemSlot.GetItem() != null)
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					OnThrowItem(slot);
					return;
				}

				if (Input.GetKey(KeyCode.LeftControl))
				{
					OnThrowItemPiece(slot);
					return;
				}

				ItemData itemData = itemSlot.GetItem().ItemData;
				MonoBehaviourInstance<GameUI>.inst.CombineWindow.SelectItem(itemData);
				MonoBehaviourInstance<GameUI>.inst.OpenWindow(MonoBehaviourInstance<GameUI>.inst.CombineWindow);
			}
		}


		public void OnDropItem(Slot slot, BaseUI draggedObject)
		{
			if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.CheckDyingCondition())
			{
				return;
			}

			ItemSlot itemSlot = draggedObject as ItemSlot;
			ItemSlot itemSlot2 = slot as ItemSlot;
			if (itemSlot == null || itemSlot2 == null)
			{
				Log.E("OnDropItem error.");
				return;
			}

			if (itemSlot.GetParentUI() is InventoryHud && itemSlot2.GetParentUI() is InventoryHud)
			{
				InvenItemSlot invenItemSlot = itemSlot as InvenItemSlot;
				InvenItemSlot invenItemSlot2 = itemSlot2 as InvenItemSlot;
				invenItemSlot.StopBulletCooldown();
				invenItemSlot2.StopBulletCooldown();
				SingletonMonoBehaviour<PlayerController>.inst.SwapInvenItemSlot(InvenSlots.IndexOf(invenItemSlot2),
					InvenSlots.IndexOf(invenItemSlot));
				Item item = itemSlot.GetItem();
				Item item2 = itemSlot2.GetItem();
				itemSlot2.SetItem(item);
				itemSlot.SetItem(item2);
				if (item != null && item.ItemData.IsThrowType() && !item.IsFullBullet())
				{
					MonoBehaviourInstance<ClientService>.inst.MyPlayer.StartBulletCooldown(item, true);
				}

				if (item2 != null && item2.ItemData.IsThrowType() && !item2.IsFullBullet())
				{
					MonoBehaviourInstance<ClientService>.inst.MyPlayer.StartBulletCooldown(item2, true);
				}
			}
			else if (itemSlot.GetParentUI() is StatusHud && itemSlot2.GetParentUI() is InventoryHud)
			{
				if (itemSlot.GetItem() != null)
				{
					SingletonMonoBehaviour<PlayerController>.inst.UnequipItem(itemSlot.GetItem());
				}
			}
			else if (itemSlot.GetParentUI() is ItemBoxWindow && itemSlot2.GetParentUI() is InventoryHud &&
			         itemSlot.GetItem() != null)
			{
				SingletonMonoBehaviour<PlayerController>.inst.TakeItem(itemSlot.GetItem());
			}
		}


		public void OnThrowItem(Slot slot)
		{
			ItemSlot itemSlot = slot as ItemSlot;
			if (itemSlot != null && itemSlot.GetItem() != null)
			{
				Item item = itemSlot.GetItem();
				if (MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
				    MonoBehaviourInstance<TutorialController>.inst.DontThrowItems(item.itemCode))
				{
					MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("아이템을 버릴 수 없습니다."));
					return;
				}

				SingletonMonoBehaviour<PlayerController>.inst.DropItem(itemSlot.GetSlotType(), item);
				if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
				{
					MonoBehaviourInstance<TutorialController>.inst.SuccessDiscardItemTutorial();
				}
			}
		}


		public void OnThrowItemPiece(Slot slot)
		{
			ItemSlot itemSlot = slot as ItemSlot;
			if (itemSlot != null && itemSlot.GetItem() != null)
			{
				Item item = itemSlot.GetItem();
				if (item.Amount == 1)
				{
					OnThrowItem(slot);
					return;
				}

				SingletonMonoBehaviour<PlayerController>.inst.DropItemPiece(itemSlot.GetSlotType(), item);
			}
		}


		public void OnPointerEnter(Slot slot)
		{
			Item item = (slot as InvenItemSlot).GetItem();
			if (item != null)
			{
				ItemData itemData = item.ItemData;
				EquipItemSlot equipItemEqualType =
					MonoBehaviourInstance<GameUI>.inst.StatusHud.GetEquipItemEqualType(itemData);
				if (MonoBehaviourInstance<GameUI>.inst != null && equipItemEqualType != null)
				{
					equipItemEqualType.PlayFocusFrame();
				}
			}
		}


		public void OnPointerExit(Slot slot)
		{
			foreach (EquipItemSlot equipItemSlot in MonoBehaviourInstance<GameUI>.inst.StatusHud.EquipSlots)
			{
				equipItemSlot.StopFocusFrame();
				equipItemSlot.StopSourceItemFrame();
			}

			foreach (InvenItemSlot invenItemSlot in InvenSlots)
			{
				invenItemSlot.StopFocusFrame();
				invenItemSlot.StopSourceItemFrame();
			}
		}


		public void OnSlotDoubleClick(Slot slot) { }


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			InvenSlots.AddRange(GetComponentsInChildren<InvenItemSlot>());
			foreach (InvenItemSlot invenItemSlot in InvenSlots)
			{
				invenItemSlot.SetEventListener(this);
				invenItemSlot.SetSlotType(SlotType.Inventory);
			}

			inventory = new Item[InvenSlots.Count];
			UISystem.AddListener<NavigationAndCombineStore>(UpdateInvenNeedMarkUI);
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			UpdateInvenUI();
			for (int i = 0; i < InvenSlots.Count; i++)
			{
				InvenSlots[i].SetGameInputEventKey(GameInputEvent.Alpha1 + i);
			}
		}


		public void ShowTutorialSquareInven(bool show)
		{
			tutorialSquareInven.SetActive(show);
		}


		public InvenItemSlot GetInvenItemSlot(int itemId)
		{
			return InvenSlots.Find(x => x.GetItem() != null && x.GetItem().id == itemId);
		}


		public InvenItemSlot GetInvenItemSlot(Item item)
		{
			return InvenSlots.Find(x => x.GetItem() == item);
		}


		public void OnInventoryUpdate(List<Item> inventoryItems, List<int> indexList)
		{
			Array.Clear(inventory, 0, inventory.Length);
			int num = 0;
			while (num < inventoryItems.Count && num < indexList.Count)
			{
				if (0 <= indexList[num] && indexList[num] < inventory.Length)
				{
					inventory[indexList[num]] = inventoryItems[num];
				}

				num++;
			}

			UpdateInvenUI();
		}


		private void UpdateInvenUI()
		{
			for (int i = 0; i < inventory.Length; i++)
			{
				Item item = inventory[i];
				InvenSlots[i].ResetSlot();
				if (item != null)
				{
					UpdateSlotItem(item, i);
					if (item.ItemData.IsThrowType() && !item.IsFullBullet())
					{
						StopBulletCooldown(item.id);
						MonoBehaviourInstance<ClientService>.inst.MyPlayer.StartBulletCooldown(item, true);
					}
					else
					{
						float cooldown = GetCooldown(item.ItemData);
						InvenSlots[i].Cooldown.Init();
						InvenSlots[i].Cooldown
							.SetCooldown(cooldown, cooldown, UICooldown.FillAmountType.FORWARD, false);
					}
				}
			}
		}


		private void UpdateInventoryCooldown(ItemData itemData)
		{
			for (int i = 0; i < inventory.Length; i++)
			{
				Item item = inventory[i];
				if (item != null && item.itemCode == itemData.code)
				{
					InvenSlots[i].ResetCooldownSlot();
					if (item != null)
					{
						UpdateSlotItem(item, i);
					}
				}
			}
		}


		public void StopBulletCooldown(int itemId)
		{
			InvenItemSlot invenItemSlot = InvenSlots.Find(x => x.GetItem() != null && x.GetItem().id == itemId);
			if (invenItemSlot != null)
			{
				invenItemSlot.StopBulletCooldown();
			}
		}


		private void UpdateSlotItem(Item item, int index)
		{
			if (item != null)
			{
				InvenSlots[index].SetItem(item);
				InvenSlots[index].SetSprite(item.ItemData.GetSprite());
				InvenSlots[index].SetBackground(item.ItemData.GetGradeSprite());
				if (item.ItemData.IsThrowType())
				{
					InvenSlots[index].SetBulletStackText(item.itemCode, item.Bullet);
				}
				else if (item.Amount > 1)
				{
					InvenSlots[index].SetStackText(item.Amount.ToString());
				}

				ItemConsumableData itemData = item.GetItemData<ItemConsumableData>();
				if (itemData != null)
				{
					InvenSlots[index].EnableHPIcon(itemData.consumableType == ItemConsumableType.Food);
					InvenSlots[index].EnableSPIcon(itemData.consumableType == ItemConsumableType.Beverage);
				}

				if (item.madeType == ItemMadeType.XiukaiMade)
				{
					InvenSlots[index].EnableXiukaiIcon(true);
				}

				if (item.ItemData.itemType == ItemType.Weapon && !item.ItemData.IsLeafNodeItem() &&
				    !MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsEquipableWeapon(item.ItemData))
				{
					InvenSlots[index].SetLock(true);
				}
			}
		}


		private void UpdateInvenNeedMarkUI(NavigationAndCombineStore navigationAndCombineStore)
		{
			Dictionary<ItemData, int> dictionary = new Dictionary<ItemData, int>();
			List<ItemData> list = navigationAndCombineStore.GetTargetItems().ToList<ItemData>();
			for (int i = 0; i < list.Count; i++)
			{
				List<ItemData> list2 = GameDB.item.AnalyzeItem(list[i]);
				for (int j = 0; j < list2.Count; j++)
				{
					ItemData itemData = list2[j];
					if (dictionary.ContainsKey(itemData))
					{
						Dictionary<ItemData, int> dictionary2 = dictionary;
						ItemData key = itemData;
						int num = dictionary2[key];
						dictionary2[key] = num + 1;
					}
					else
					{
						dictionary.Add(itemData, 1);
					}
				}
			}

			foreach (Item item in GetItemListGradeOrderBy(navigationAndCombineStore.GetEquipItems()))
			{
				SubtrackEquipItemsInSourceItems(dictionary, item);
			}

			foreach (Item item2 in GetItemListGradeOrderBy(navigationAndCombineStore.GetInventoryItems()))
			{
				bool enable = CheckNeedMarkInventorySlot(dictionary, item2);
				foreach (ItemSlot itemSlot in InvenSlots)
				{
					Item item3 = itemSlot.GetItem();
					if (item3 != null && item3.id == item2.id)
					{
						itemSlot.EnableNeedMark(enable);
						break;
					}
				}
			}

			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateNeedItem(new List<ItemData>(dictionary.Keys));
		}


		private IEnumerable<Item> GetItemListGradeOrderBy(IEnumerable<Item> list)
		{
			List<Item> list2 = new List<Item>();
			foreach (Item item2 in list)
			{
				if (item2 != null)
				{
					list2.Add(item2);
				}
			}

			return from item in list2
				orderby item.ItemData.itemGrade descending, item.ItemData.name, item.Amount descending
				select item;
		}


		private void SubtrackEquipItemsInSourceItems(Dictionary<ItemData, int> sourceItems, Item item)
		{
			foreach (KeyValuePair<ItemData, int> keyValuePair in sourceItems)
			{
				if (item.itemCode == keyValuePair.Key.code)
				{
					Queue<ItemData> queue = new Queue<ItemData>();
					queue.Enqueue(item.ItemData);
					while (queue.Count > 0)
					{
						ItemData itemData = queue.Dequeue();
						if (sourceItems.ContainsKey(itemData))
						{
							int num;
							if (item.itemCode == itemData.code)
							{
								num = sourceItems[itemData] - item.Amount;
							}
							else
							{
								num = sourceItems[itemData] - 1;
							}

							if (num <= 0)
							{
								sourceItems.Remove(itemData);
							}
							else
							{
								sourceItems[itemData] = num;
							}

							if (itemData.makeMaterial1 > 0)
							{
								queue.Enqueue(GameDB.item.FindItemByCode(itemData.makeMaterial1));
							}

							if (itemData.makeMaterial2 > 0)
							{
								queue.Enqueue(GameDB.item.FindItemByCode(itemData.makeMaterial2));
							}
						}
					}

					break;
				}
			}
		}


		private bool CheckNeedMarkInventorySlot(Dictionary<ItemData, int> sourceItems, Item item)
		{
			bool result = false;
			foreach (KeyValuePair<ItemData, int> keyValuePair in sourceItems)
			{
				if (item.itemCode == keyValuePair.Key.code)
				{
					result = true;
					Queue<ItemData> queue = new Queue<ItemData>();
					queue.Enqueue(item.ItemData);
					while (queue.Count > 0)
					{
						ItemData itemData = queue.Dequeue();
						if (sourceItems.ContainsKey(itemData))
						{
							int num;
							int num2;
							if (item.itemCode == itemData.code)
							{
								num = sourceItems[itemData] - item.Amount;
								num2 = Math.Min(sourceItems[itemData], item.Amount);
							}
							else
							{
								num = sourceItems[itemData] - 1;
								num2 = 1;
							}

							if (num <= 0)
							{
								sourceItems.Remove(itemData);
							}
							else
							{
								sourceItems[itemData] = num;
							}

							for (int i = 0; i < num2; i++)
							{
								if (itemData.makeMaterial1 > 0)
								{
									queue.Enqueue(GameDB.item.FindItemByCode(itemData.makeMaterial1));
								}

								if (itemData.makeMaterial2 > 0)
								{
									queue.Enqueue(GameDB.item.FindItemByCode(itemData.makeMaterial2));
								}
							}
						}
					}

					break;
				}
			}

			return result;
		}


		public InvenItemSlot GetEqualInventoryItemId(int id)
		{
			InvenItemSlot result = null;
			int num = 100;
			for (int i = 0; i < InvenSlots.Count; i++)
			{
				Item item = InvenSlots[i].GetItem();
				if (item != null && !item.IsEmpty() && item.id == id && num > item.Amount)
				{
					result = InvenSlots[i];
					num = item.Amount;
				}
			}

			return result;
		}


		public void OnPressKey(GameInputEvent gameInputEvent, Vector3 mousePos)
		{
			int num = -1;
			switch (gameInputEvent)
			{
				case GameInputEvent.Alpha1:
					num = 0;
					break;
				case GameInputEvent.Alpha2:
					num = 1;
					break;
				case GameInputEvent.Alpha3:
					num = 2;
					break;
				case GameInputEvent.Alpha4:
					num = 3;
					break;
				case GameInputEvent.Alpha5:
					num = 4;
					break;
				case GameInputEvent.Alpha6:
					num = 5;
					break;
				case GameInputEvent.Alpha7:
					num = 6;
					break;
				case GameInputEvent.Alpha8:
					num = 7;
					break;
				case GameInputEvent.Alpha9:
					num = 8;
					break;
				case GameInputEvent.Alpha0:
					num = 9;
					break;
			}

			if (0 <= num && num < InvenSlots.Count)
			{
				if (!Singleton<LocalSetting>.inst.setting.quickCastKeys.ContainsKey(gameInputEvent))
				{
					return;
				}

				SlotLeftClick(InvenSlots[num], Singleton<LocalSetting>.inst.setting.quickCastKeys[gameInputEvent],
					mousePos);
			}
		}


		private void SlotLeftClick(Slot slot, bool isQuickCast, Vector3 mousePos)
		{
			ItemSlot itemSlot = slot as ItemSlot;
			if (itemSlot != null && itemSlot.GetItem() != null)
			{
				if (!itemSlot.IsUsable())
				{
					return;
				}

				Item item = itemSlot.GetItem();
				if (item.ItemData.itemType == ItemType.Weapon || item.ItemData.itemType == ItemType.Armor)
				{
					SingletonMonoBehaviour<PlayerController>.inst.EquipItem(item);
					MonoBehaviourInstance<Tooltip>.inst.Hide();
					return;
				}

				if (item.ItemData.itemType == ItemType.Consume)
				{
					SingletonMonoBehaviour<PlayerController>.inst.UseItem(item, isQuickCast, mousePos);
					return;
				}

				if (item.ItemData.itemType == ItemType.Special)
				{
					SingletonMonoBehaviour<PlayerController>.inst.UseItem(item, isQuickCast, mousePos);
				}
			}
		}


		public void OnNavigationClear(ItemData itemData)
		{
			ItemSlot itemSlot = InvenSlots.FindLast(x => x.GetItem() != null && x.GetItem().itemCode == itemData.code);
			if (itemSlot != null)
			{
				itemSlot.PlayEffect();
			}
		}


		public void UpdateCooldown(Dictionary<ItemData, float> cooldownMap)
		{
			this.cooldownMap = cooldownMap;
			foreach (KeyValuePair<ItemData, float> keyValuePair in cooldownMap)
			{
				UpdateInventoryCooldown(keyValuePair.Key);
			}

			for (int i = 0; i < inventory.Length; i++)
			{
				Item item = inventory[i];
				if (item != null)
				{
					InvenSlots[i].SetItem(item);
					if (item.ItemData.itemType == ItemType.Weapon && !item.ItemData.IsLeafNodeItem() &&
					    !MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsEquipableWeapon(item.ItemData))
					{
						InvenSlots[i].SetLock(true);
					}

					float cooldown = GetCooldown(item.ItemData);
					InvenSlots[i].Cooldown.Init();
					InvenSlots[i].Cooldown.SetCooldown(cooldown, cooldown, UICooldown.FillAmountType.FORWARD, false);
				}
			}
		}


		private float GetCooldown(ItemData itemData)
		{
			foreach (ItemData itemData2 in cooldownMap.Keys)
			{
				if (itemData2.IsShareCooldown(itemData))
				{
					return Mathf.Max(0f, cooldownMap[itemData2] - Time.time);
				}
			}

			return 0f;
		}
	}
}