namespace Blis.Client
{
	public interface ISlotEventListener
	{
		void OnSlotLeftClick(Slot slot);


		void OnSlotDoubleClick(Slot slot);


		void OnSlotRightClick(Slot slot);


		void OnDropItem(Slot slot, BaseUI draggedUI);


		void OnThrowItem(Slot slot);


		void OnThrowItemPiece(Slot slot);


		void OnPointerEnter(Slot slot);


		void OnPointerExit(Slot slot);
	}
}