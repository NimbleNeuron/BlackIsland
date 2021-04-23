using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class EquipItem
	{
		[Key(1)] public readonly Item item;


		[Key(0)] public readonly EquipSlotType slotType;


		[SerializationConstructor]
		public EquipItem(EquipSlotType slotType, Item item)
		{
			this.slotType = slotType;
			this.item = item;
		}
	}
}