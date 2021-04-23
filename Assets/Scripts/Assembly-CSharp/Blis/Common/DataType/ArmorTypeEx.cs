namespace Blis.Common
{
	public static class ArmorTypeEx
	{
		public static EquipSlotType GetEquipSlotType(this ArmorType armorType)
		{
			switch (armorType)
			{
				case ArmorType.Head:
					return EquipSlotType.Head;
				case ArmorType.Chest:
					return EquipSlotType.Chest;
				case ArmorType.Arm:
					return EquipSlotType.Arm;
				case ArmorType.Leg:
					return EquipSlotType.Leg;
				case ArmorType.Trinket:
					return EquipSlotType.Trinket;
				default:
					return EquipSlotType.None;
			}
		}
	}
}