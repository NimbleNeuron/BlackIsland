using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ItemBoxWindow : BaseWindow, ISlotEventListener
	{
		[SerializeField] private Text title = default;

		private readonly List<Item> boxItems = new List<Item>();
		private readonly List<ItemSlot> boxSlots = new List<ItemSlot>();
		
		private readonly HashSet<ItemData> needItems = new HashSet<ItemData>();
		
		private readonly Dictionary<ItemData, int> ownItems = new Dictionary<ItemData, int>();
		private readonly Dictionary<ItemData, int> sourceItems = new Dictionary<ItemData, int>();

		private BoxWindowType boxWindowType;

		protected override void OnDestroy()
		{
			base.OnDestroy();
			UISystem.RemoveListener<NavigationAndCombineStore>(OnNavigationStoreUpdate);
			UISystem.RemoveListener<ItemBoxStore>(OnItemBoxStoreUpdate);
		}

		/// <summary>
		/// 클릭 시뮬레이션 가능
		/// </summary>
		/// <param name="slot"></param>
		public void OnSlotLeftClick(Slot slot)
		{
			ItemSlot itemSlot = slot as ItemSlot;
			if (itemSlot != null && itemSlot.GetItem() != null)
			{
				SingletonMonoBehaviour<PlayerController>.inst.TakeItem(itemSlot.GetItem());
				MonoBehaviourInstance<Tooltip>.inst.Hide(this);
			}
		}

		public void OnSlotRightClick(Slot slot) { }

		public void OnDropItem(Slot slot, BaseUI draggedUI)
		{
			ItemSlot itemSlot = draggedUI as ItemSlot;
			if (itemSlot != null && itemSlot.GetParentUI() is InventoryHud && itemSlot.GetItem() != null)
			{
				Item item = itemSlot.GetItem();
				SingletonMonoBehaviour<PlayerController>.inst.InsertItem(item);
			}
		}

		public void OnThrowItem(Slot slot) { }
		public void OnThrowItemPiece(Slot slot) { }
		public void OnPointerEnter(Slot slot) { }
		public void OnPointerExit(Slot slot) { }
		public void OnSlotDoubleClick(Slot slot) { }

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GetComponentsInChildren<ItemSlot>(true, boxSlots);
		}

		protected override void OnStartUI()
		{
			base.OnStartUI();
			boxSlots.ForEach(delegate(ItemSlot x)
			{
				x.SetEventListener(this);
				x.SetDraggable(false);
				x.SetItemBoxSlot(true);
			});
			
			UISystem.AddListener<NavigationAndCombineStore>(OnNavigationStoreUpdate);
			UISystem.AddListener<ItemBoxStore>(OnItemBoxStoreUpdate);
		}

		private void OnNavigationStoreUpdate(NavigationAndCombineStore navigationAndCombineStore)
		{
			sourceItems.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair in navigationAndCombineStore.GetNeedSourceItems())
			{
				sourceItems.Add(keyValuePair.Key, keyValuePair.Value);
			}

			ownItems.Clear();
			foreach (KeyValuePair<ItemData, int> keyValuePair2 in navigationAndCombineStore.GetOwnSourceItems())
			{
				ownItems.Add(keyValuePair2.Key, keyValuePair2.Value);
			}

			needItems.Clear();
			foreach (ItemData itemData in sourceItems.Keys)
			{
				if (ownItems.ContainsKey(itemData))
				{
					if (sourceItems[itemData] - ownItems[itemData] > 0)
					{
						needItems.Add(itemData);
					}
				}
				else
				{
					needItems.Add(itemData);
				}
			}

			if (IsOpen)
			{
				UpdateBoxUI();
			}
		}

		private void OnItemBoxStoreUpdate(ItemBoxStore itemBoxStore)
		{
			if (!itemBoxStore.IsBoxOpen)
			{
				Close();
				return;
			}

			if (!IsOpen)
			{
				Open();
			}

			boxWindowType = itemBoxStore.BoxWindowType;
			title.text = itemBoxStore.BoxWindowType == BoxWindowType.Corpse ? Ln.Get("시체") : Ln.Get("상자");
			boxItems.Clear();
			boxItems.AddRange(itemBoxStore.BoxItems);
			
			if (boxWindowType == BoxWindowType.AirSupply && boxItems.Count == 0)
			{
				Close();
				return;
			}

			UpdateBoxUI();
			
			if (MonoBehaviourInstance<ClientService>.inst.World.Find<LocalObject>(itemBoxStore.BoxId)
				.IsTypeOf<LocalStaticItemBox>())
			{
				LocalPlayerCharacter character = MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character;
				character.CharacterVoiceControl.PlayCharacterVoice(CharacterVoiceType.OpenBox, 15,
					character.GetPosition());
			}
		}

		private void UpdateBoxUI()
		{
			boxSlots.ForEach(x => x.ResetSlot());
			
			int num = boxWindowType == BoxWindowType.Corpse ? boxSlots.Count : boxSlots.Count / 2;
			for (int i = 0; i < boxSlots.Count; i++)
			{
				if (i < num)
				{
					boxSlots[i].gameObject.SetActive(true);
					boxSlots[i].ResetSlot();
				}
				else
				{
					boxSlots[i].gameObject.SetActive(false);
				}
			}

			ClientService inst = MonoBehaviourInstance<ClientService>.inst;
			for (int j = 0; j < boxItems.Count; j++)
			{
				Item item = boxItems[j];
				if (item != null)
				{
					boxSlots[j].SetItem(item);
					boxSlots[j].SetStackText(item.Amount > 1 ? item.Amount.ToString() : null);
					boxSlots[j].SetSprite(item.ItemData.GetSprite());
					boxSlots[j].SetBackground(item.ItemData.GetGradeSprite());
					
					if (inst != null && inst.IsPlayer)
					{
						List<ItemData> upperGradeItems = GameDB.item.GetUpperGradeItems(item.ItemData);
						
						foreach (ItemData t in upperGradeItems)
						{
							if ((t.itemType != ItemType.Weapon ||
							     inst.MyPlayer.Character.IsEquipableWeapon(t)) &&
							    inst.MyPlayer.IsCombinableWithMaterial(t, item.ItemData))
							{
								boxSlots[j].EnableCombineMark(true);
								break;
							}
						}
					}

					boxSlots[j].EnableNeedMark(needItems.Contains(item.ItemData));
				}
			}
		}

		protected override void OnClose()
		{
			base.OnClose();
			
			SingletonMonoBehaviour<PlayerController>.inst.CloseBox();
			
			if (DraggingUI != null)
			{
				boxSlots.ForEach(delegate(ItemSlot x)
				{
					if (x == DraggingUI)
					{
						x.OnEndDrag(new PointerEventData(EventSystem.current)
						{
							button = PointerEventData.InputButton.Left
						});
					}
				});
			}
		}
	}
}