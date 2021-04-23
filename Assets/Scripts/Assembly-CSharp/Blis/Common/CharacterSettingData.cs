namespace Blis.Common
{
	
	public class CharacterSettingData
	{
		public readonly int characterCode;
		public readonly int characterLevel;
		public readonly int code;
		public readonly int equipmentArm;
		public readonly int equipmentChest;
		public readonly int equipmentHead;
		public readonly int equipmentLeg;
		public readonly int equipmentTrinket;
		public readonly int equipmentWeapon;
		public readonly int equipmentWeaponCount;
		public readonly int explorationMasteryLevel1;
		public readonly int explorationMasteryLevel2;
		public readonly int explorationMasteryLevel3;
		public readonly int explorationMasteryLevel4;
		public readonly int inventoryItem1;
		public readonly int inventoryItem10;
		public readonly int inventoryItem2;
		public readonly int inventoryItem3;
		public readonly int inventoryItem4;
		public readonly int inventoryItem5;
		public readonly int inventoryItem6;
		public readonly int inventoryItem7;
		public readonly int inventoryItem8;
		public readonly int inventoryItem9;
		public readonly int inventoryItemCount1;
		public readonly int inventoryItemCount10;
		public readonly int inventoryItemCount2;
		public readonly int inventoryItemCount3;
		public readonly int inventoryItemCount4;
		public readonly int inventoryItemCount5;
		public readonly int inventoryItemCount6;
		public readonly int inventoryItemCount7;
		public readonly int inventoryItemCount8;
		public readonly int inventoryItemCount9;
		public readonly ObjectType objectType;
		public readonly int skillLevelActive1;
		public readonly int skillLevelActive2;
		public readonly int skillLevelActive3;
		public readonly int skillLevelActive4;
		public readonly int skillLevelPassive;
		public readonly int skillPoint;
		public readonly int startingArea;
		public readonly int startingIndex;
		public readonly int survivalMasteryLevel1;
		public readonly int survivalMasteryLevel2;
		public readonly int survivalMasteryLevel3;
		public readonly int survivalMasteryLevel4;
		public readonly int[] walkableAreaCodes;
		public readonly int weaponMasteryLevel1;
		public readonly int weaponMasteryLevel2;
		public readonly int weaponMasteryLevel3;
		public readonly int weaponMasteryLevel4;
		
		public CharacterSettingData(int code, int characterCode, ObjectType objectType, int startingArea,
			int startingIndex, int[] walkableAreaCodes, int characterLevel, int weaponMasteryLevel1,
			int weaponMasteryLevel2, int weaponMasteryLevel3, int weaponMasteryLevel4, int explorationMasteryLevel1,
			int explorationMasteryLevel2, int explorationMasteryLevel3, int explorationMasteryLevel4,
			int survivalMasteryLevel1, int survivalMasteryLevel2, int survivalMasteryLevel3, int survivalMasteryLevel4,
			int skillLevelPassive, int skillLevelActive1, int skillLevelActive2, int skillLevelActive3,
			int skillLevelActive4, int skillPoint, int equipmentWeapon, int equipmentWeaponCount, int equipmentHead,
			int equipmentChest, int equipmentArm, int equipmentLeg, int equipmentTrinket, int inventoryItem1,
			int inventoryItemCount1, int inventoryItem2, int inventoryItemCount2, int inventoryItem3,
			int inventoryItemCount3, int inventoryItem4, int inventoryItemCount4, int inventoryItem5,
			int inventoryItemCount5, int inventoryItem6, int inventoryItemCount6, int inventoryItem7,
			int inventoryItemCount7, int inventoryItem8, int inventoryItemCount8, int inventoryItem9,
			int inventoryItemCount9, int inventoryItem10, int inventoryItemCount10)
		{
			this.code = code;
			this.characterCode = characterCode;
			this.objectType = objectType;
			this.startingArea = startingArea;
			this.startingIndex = startingIndex;
			this.walkableAreaCodes = walkableAreaCodes;
			this.characterLevel = characterLevel;
			this.weaponMasteryLevel1 = weaponMasteryLevel1;
			this.weaponMasteryLevel2 = weaponMasteryLevel2;
			this.weaponMasteryLevel3 = weaponMasteryLevel3;
			this.weaponMasteryLevel4 = weaponMasteryLevel4;
			this.explorationMasteryLevel1 = explorationMasteryLevel1;
			this.explorationMasteryLevel2 = explorationMasteryLevel2;
			this.explorationMasteryLevel3 = explorationMasteryLevel3;
			this.explorationMasteryLevel4 = explorationMasteryLevel4;
			this.survivalMasteryLevel1 = survivalMasteryLevel1;
			this.survivalMasteryLevel2 = survivalMasteryLevel2;
			this.survivalMasteryLevel3 = survivalMasteryLevel3;
			this.survivalMasteryLevel4 = survivalMasteryLevel4;
			this.skillLevelPassive = skillLevelPassive;
			this.skillLevelActive1 = skillLevelActive1;
			this.skillLevelActive2 = skillLevelActive2;
			this.skillLevelActive3 = skillLevelActive3;
			this.skillLevelActive4 = skillLevelActive4;
			this.skillPoint = skillPoint;
			this.equipmentWeapon = equipmentWeapon;
			this.equipmentWeaponCount = equipmentWeaponCount;
			this.equipmentHead = equipmentHead;
			this.equipmentChest = equipmentChest;
			this.equipmentArm = equipmentArm;
			this.equipmentLeg = equipmentLeg;
			this.equipmentTrinket = equipmentTrinket;
			this.inventoryItem1 = inventoryItem1;
			this.inventoryItemCount1 = inventoryItemCount1;
			this.inventoryItem2 = inventoryItem2;
			this.inventoryItemCount2 = inventoryItemCount2;
			this.inventoryItem3 = inventoryItem3;
			this.inventoryItemCount3 = inventoryItemCount3;
			this.inventoryItem4 = inventoryItem4;
			this.inventoryItemCount4 = inventoryItemCount4;
			this.inventoryItem5 = inventoryItem5;
			this.inventoryItemCount5 = inventoryItemCount5;
			this.inventoryItem6 = inventoryItem6;
			this.inventoryItemCount6 = inventoryItemCount6;
			this.inventoryItem7 = inventoryItem7;
			this.inventoryItemCount7 = inventoryItemCount7;
			this.inventoryItem8 = inventoryItem8;
			this.inventoryItemCount8 = inventoryItemCount8;
			this.inventoryItem9 = inventoryItem9;
			this.inventoryItemCount9 = inventoryItemCount9;
			this.inventoryItem10 = inventoryItem10;
			this.inventoryItemCount10 = inventoryItemCount10;
		}
	}
}