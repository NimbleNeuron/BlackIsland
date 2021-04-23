using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UINavFavoritesItem : BaseUI, ISlotEventListener
	{
		[SerializeField] private ItemDataSlot itemDataSlot = default;


		private Button deletebutton;


		private ItemData itemData;


		private Image selection;


		protected override void Awake()
		{
			base.Awake();
			selection = GameUtil.Bind<Image>(gameObject, "Selected");
			deletebutton = GameUtil.Bind<Button>(gameObject, "Delete");
		}


		public void OnSlotLeftClick(Slot slot)
		{
			OnSelection(this);
		}


		public void OnSlotDoubleClick(Slot slot) { }


		public void OnSlotRightClick(Slot slot) { }


		public void OnDropItem(Slot slot, BaseUI draggedUI) { }


		public void OnThrowItem(Slot slot) { }


		public void OnThrowItemPiece(Slot slot) { }


		public void OnPointerEnter(Slot slot) { }


		public void OnPointerExit(Slot slot) { }

		
		
		public event Action<UINavFavoritesItem> OnSelection = delegate { };


		
		
		public event Action<ItemData> OnRequestDelete = delegate { };


		protected override void OnStartUI()
		{
			base.OnStartUI();
			itemDataSlot.SetEventListener(this);
			deletebutton.onClick.AddListener(delegate
			{
				if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
				{
					MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("훈련 중에는 목표 아이템을 삭제할 수 없습니다."), new Popup.Button
					{
						text = Ln.Get("확인")
					});
					return;
				}

				if (itemData != null)
				{
					OnRequestDelete(itemData);
				}
			});
		}


		public void SetItemData(ItemData itemData)
		{
			this.itemData = itemData;
			itemDataSlot.ResetSlot();
			if (itemData != null)
			{
				itemDataSlot.SetItemData(itemData);
				itemDataSlot.SetSlotType(SlotType.Favorite);
				itemDataSlot.SetSprite(itemData.GetSprite());
				itemDataSlot.SetBackground(itemData.GetGradeSprite());
			}
		}


		public ItemData GetItemData()
		{
			return itemData;
		}


		public void EnableSelection(bool enable)
		{
			selection.enabled = enable;
			deletebutton.gameObject.SetActive(enable);
		}


		public void EnableOneMark(bool enable)
		{
			itemDataSlot.EnableOwnMark(enable);
		}
	}
}