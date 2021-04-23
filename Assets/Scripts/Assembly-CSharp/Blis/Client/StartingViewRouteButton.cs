using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class StartingViewRouteButton : BaseUI, ISlotEventListener
	{
		private readonly List<ItemSlot> itemSlots = new List<ItemSlot>();


		private Button button;


		private Image check;


		private Transform itemList;


		private int routeIndex = -1;


		private Image selectedBg;


		private Text title;


		public void OnSlotLeftClick(Slot slot)
		{
			Action<int> action = onClickCallback;
			if (action == null)
			{
				return;
			}

			action(routeIndex);
		}


		public void OnSlotDoubleClick(Slot slot) { }


		public void OnSlotRightClick(Slot slot) { }


		public void OnDropItem(Slot slot, BaseUI draggedUI) { }


		public void OnThrowItem(Slot slot) { }


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot) { }


		public void OnPointerExit(Slot slot) { }

		
		
		public event Action<int> onClickCallback = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
		}


		public void BindingObjects()
		{
			GameUtil.Bind<Button>(gameObject, ref button);
			title = GameUtil.Bind<Text>(gameObject, "Title");
			itemList = GameUtil.Bind<Transform>(gameObject, "ItemList");
			itemList.GetComponentsInChildren<ItemSlot>(itemSlots);
			selectedBg = GameUtil.Bind<Image>(gameObject, "SelectedBg");
			check = GameUtil.Bind<Image>(gameObject, "Check");
			button.onClick.AddListener(delegate
			{
				if (routeIndex >= 0)
				{
					Action<int> action = onClickCallback;
					if (action == null)
					{
						return;
					}

					action(routeIndex);
				}
			});
			foreach (ItemSlot itemSlot in itemSlots)
			{
				itemSlot.SetEventListener(this);
			}
		}


		public void SetRouteButton(int index, Favorite favorite)
		{
			if (index < 0 || favorite == null)
			{
				return;
			}

			gameObject.SetActive(true);
			routeIndex = index;
			title.text = favorite.title;
			if (favorite.weaponCodes != null && favorite.weaponCodes.Count > 0)
			{
				FillItems(favorite);
			}
		}


		private void FillItems(Favorite favorite)
		{
			int count = favorite.weaponCodes.Count;
			for (int i = 0; i < itemSlots.Count; i++)
			{
				ItemSlot itemSlot = itemSlots[i];
				if (i >= count)
				{
					itemSlot.ResetSlot();
				}
				else
				{
					ItemData itemData = GameDB.item.FindItemByCode(favorite.weaponCodes[i]);
					itemSlot.SetItem(new Item(-1, itemData.code, itemData.initialCount, 0));
					itemSlot.SetSlotType(SlotType.None);
					itemSlot.SetSprite(itemData.GetSprite());
					itemSlot.SetBackground(itemData.GetGradeSprite());
					itemSlot.SetStackText("");
				}
			}
		}


		public void InitUI()
		{
			BindingObjects();
			routeIndex = -1;
			title.text = "";
			check.enabled = false;
			selectedBg.enabled = false;
			for (int i = 0; i < itemSlots.Count; i++)
			{
				itemSlots[i].ResetSlot();
			}

			gameObject.SetActive(false);
		}


		public void SetSelected(bool select)
		{
			check.enabled = select;
			selectedBg.enabled = select;
			button.interactable = !select;
			if (select)
			{
				button.Select();
			}
		}
	}
}