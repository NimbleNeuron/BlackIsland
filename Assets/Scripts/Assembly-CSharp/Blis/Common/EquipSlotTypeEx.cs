using System;

namespace Blis.Common
{
	public static class EquipSlotTypeEx
	{
		public static ArmorType GetArmorType(this EquipSlotType equipSlotType)
		{
			switch (equipSlotType)
			{
				case EquipSlotType.Chest:
					return ArmorType.Chest;
				case EquipSlotType.Head:
					return ArmorType.Head;
				case EquipSlotType.Arm:
					return ArmorType.Arm;
				case EquipSlotType.Leg:
					return ArmorType.Leg;
				case EquipSlotType.Trinket:
					return ArmorType.Trinket;
				default:
					throw new ArgumentOutOfRangeException("equipSlotType", equipSlotType, null);
			}
		}
	}
}