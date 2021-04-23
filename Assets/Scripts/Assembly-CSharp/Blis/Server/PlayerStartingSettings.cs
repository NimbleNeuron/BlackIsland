using System;
using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	public class PlayerStartingSettings
	{
		
		
		public KeyValuePair<int, int> StartingWeapon
		{
			get
			{
				return this.startingWeapon;
			}
		}

		
		
		public List<KeyValuePair<int, int>> StartingArmors
		{
			get
			{
				return this.startingArmors;
			}
		}

		
		
		public Dictionary<int, int> StartingInventoryItems
		{
			get
			{
				return this.startingInventoryItems;
			}
		}

		
		
		public int StartingAreaCode
		{
			get
			{
				return this.startingAreaCode;
			}
		}

		
		
		public int StartingAreaIndex
		{
			get
			{
				return this.startingAreaIndex;
			}
		}

		
		
		public int[] WalkableAreaCodes
		{
			get
			{
				return this.walkableAreaCodes;
			}
		}

		
		
		public int CharacterLevel
		{
			get
			{
				return this.characterLevel;
			}
		}

		
		
		public int WeaponMasteryLevel1
		{
			get
			{
				return this.weaponMasteryLevel1;
			}
		}

		
		
		public int WeaponMasteryLevel2
		{
			get
			{
				return this.weaponMasteryLevel2;
			}
		}

		
		
		public int WeaponMasteryLevel3
		{
			get
			{
				return this.weaponMasteryLevel3;
			}
		}

		
		
		public int WeaponMasteryLevel4
		{
			get
			{
				return this.weaponMasteryLevel4;
			}
		}

		
		
		public int ExplorationMasteryLevel1
		{
			get
			{
				return this.explorationMasteryLevel1;
			}
		}

		
		
		public int ExplorationMasteryLevel2
		{
			get
			{
				return this.explorationMasteryLevel2;
			}
		}

		
		
		public int ExplorationMasteryLevel3
		{
			get
			{
				return this.explorationMasteryLevel3;
			}
		}

		
		
		public int ExplorationMasteryLevel4
		{
			get
			{
				return this.explorationMasteryLevel4;
			}
		}

		
		
		public int SurvivalMasteryLevel1
		{
			get
			{
				return this.survivalMasteryLevel1;
			}
		}

		
		
		public int SurvivalMasteryLevel2
		{
			get
			{
				return this.survivalMasteryLevel2;
			}
		}

		
		
		public int SurvivalMasteryLevel3
		{
			get
			{
				return this.survivalMasteryLevel3;
			}
		}

		
		
		public int SurvivalMasteryLevel4
		{
			get
			{
				return this.survivalMasteryLevel4;
			}
		}

		
		
		public int SkillLevelPassive
		{
			get
			{
				return this.skillLevelPassive;
			}
		}

		
		
		public int SkillLevelActive1
		{
			get
			{
				return this.skillLevelActive1;
			}
		}

		
		
		public int SkillLevelActive2
		{
			get
			{
				return this.skillLevelActive2;
			}
		}

		
		
		public int SkillLevelActive3
		{
			get
			{
				return this.skillLevelActive3;
			}
		}

		
		
		public int SkillLevelActive4
		{
			get
			{
				return this.skillLevelActive4;
			}
		}

		
		
		public int SkillPoint
		{
			get
			{
				return this.skillPoint;
			}
		}

		
		
		public int StartItemGroupCode
		{
			get
			{
				return this.startItemGroupCode;
			}
		}

		
		public PlayerStartingSettings()
		{
			for (int i = 0; i < Enum.GetValues(typeof(ArmorType)).Length; i++)
			{
				this.startingArmors.Add(default(KeyValuePair<int, int>));
			}
		}

		
		private void SetCharacterLevel(int characterLevel)
		{
			this.characterLevel = characterLevel;
		}

		
		private void SetWeaponMasteryLevel(int weaponMasteryLevel1, int weaponMasteryLevel2, int weaponMasteryLevel3, int weaponMasteryLevel4)
		{
			this.weaponMasteryLevel1 = weaponMasteryLevel1;
			this.weaponMasteryLevel2 = weaponMasteryLevel2;
			this.weaponMasteryLevel3 = weaponMasteryLevel3;
			this.weaponMasteryLevel4 = weaponMasteryLevel4;
		}

		
		private void SetExplorationMasteryLevel(int explorationMasteryLevel1, int explorationMasteryLevel2, int explorationMasteryLevel3, int explorationMasteryLevel4)
		{
			this.explorationMasteryLevel1 = explorationMasteryLevel1;
			this.explorationMasteryLevel2 = explorationMasteryLevel2;
			this.explorationMasteryLevel3 = explorationMasteryLevel3;
			this.explorationMasteryLevel4 = explorationMasteryLevel4;
		}

		
		private void SetSurvivalMasteryLevel(int survivalMasteryLevel1, int survivalMasteryLevel2, int survivalMasteryLevel3, int survivalMasteryLevel4)
		{
			this.survivalMasteryLevel1 = survivalMasteryLevel1;
			this.survivalMasteryLevel2 = survivalMasteryLevel2;
			this.survivalMasteryLevel3 = survivalMasteryLevel3;
			this.survivalMasteryLevel4 = survivalMasteryLevel4;
		}

		
		private void SetSkillLevel(int skillLevelPassive, int skillLevelActive1, int skillLevelActive2, int skillLevelActive3, int skillLevelActive4)
		{
			this.skillLevelPassive = skillLevelPassive;
			this.skillLevelActive1 = skillLevelActive1;
			this.skillLevelActive2 = skillLevelActive2;
			this.skillLevelActive3 = skillLevelActive3;
			this.skillLevelActive4 = skillLevelActive4;
		}

		
		private void SetSkillPoint(int skillPoint)
		{
			this.skillPoint = skillPoint;
		}

		
		private void SetStartingWeapon(int itemCode, int amount)
		{
			if (itemCode == 0)
			{
				return;
			}
			this.startingWeapon = new KeyValuePair<int, int>(itemCode, amount);
		}

		
		private void SetStartingArmor(ArmorType armorType, int itemCode, int amount)
		{
			if (itemCode == 0)
			{
				return;
			}
			this.startingArmors[(int)armorType] = new KeyValuePair<int, int>(itemCode, amount);
		}

		
		public void AddStartingInventoryItem(int itemCode, int amount)
		{
			if (itemCode == 0)
			{
				return;
			}
			this.startingInventoryItems.Add(itemCode, amount);
		}

		
		public void SetStartItemGroupCode(int itemGroupCode)
		{
			this.startItemGroupCode = itemGroupCode;
		}

		
		public void SetStartingArea(int startingAreaCode)
		{
			this.startingAreaCode = startingAreaCode;
		}

		
		private void SetStartingAreaIndex(int startingAreaIndex)
		{
			this.startingAreaIndex = startingAreaIndex;
		}

		
		private void SetWalkableAreaCodes(int[] walkableAreaCodes)
		{
			this.walkableAreaCodes = walkableAreaCodes;
		}

		
		public void SetCharacterSettings(CharacterSettingData playerSettingData)
		{
			this.SetCharacterLevel(playerSettingData.characterLevel);
			this.SetStartingArea(playerSettingData.startingArea);
			this.SetStartingAreaIndex(playerSettingData.startingIndex);
			this.SetWalkableAreaCodes(playerSettingData.walkableAreaCodes);
			this.SetWeaponMasteryLevel(playerSettingData.weaponMasteryLevel1, playerSettingData.weaponMasteryLevel2, playerSettingData.weaponMasteryLevel3, playerSettingData.weaponMasteryLevel4);
			this.SetExplorationMasteryLevel(playerSettingData.explorationMasteryLevel1, playerSettingData.explorationMasteryLevel2, playerSettingData.explorationMasteryLevel3, playerSettingData.explorationMasteryLevel4);
			this.SetSurvivalMasteryLevel(playerSettingData.survivalMasteryLevel1, playerSettingData.survivalMasteryLevel2, playerSettingData.survivalMasteryLevel3, playerSettingData.survivalMasteryLevel4);
			this.SetSkillLevel(playerSettingData.skillLevelPassive, playerSettingData.skillLevelActive1, playerSettingData.skillLevelActive2, playerSettingData.skillLevelActive3, playerSettingData.skillLevelActive4);
			this.SetSkillPoint(playerSettingData.skillPoint);
			this.SetStartingWeapon(playerSettingData.equipmentWeapon, playerSettingData.equipmentWeaponCount);
			this.SetStartingArmor(ArmorType.Head, playerSettingData.equipmentHead, 1);
			this.SetStartingArmor(ArmorType.Chest, playerSettingData.equipmentChest, 1);
			this.SetStartingArmor(ArmorType.Arm, playerSettingData.equipmentArm, 1);
			this.SetStartingArmor(ArmorType.Leg, playerSettingData.equipmentLeg, 1);
			this.SetStartingArmor(ArmorType.Trinket, playerSettingData.equipmentTrinket, 1);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem1, playerSettingData.inventoryItemCount1);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem3, playerSettingData.inventoryItemCount3);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem2, playerSettingData.inventoryItemCount2);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem4, playerSettingData.inventoryItemCount4);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem5, playerSettingData.inventoryItemCount5);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem6, playerSettingData.inventoryItemCount6);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem7, playerSettingData.inventoryItemCount7);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem8, playerSettingData.inventoryItemCount8);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem9, playerSettingData.inventoryItemCount9);
			this.AddStartingInventoryItem(playerSettingData.inventoryItem10, playerSettingData.inventoryItemCount10);
		}

		
		public void SetStartingEquipment(ItemData itemData)
		{
			ItemType itemType = itemData.itemType;
			if (itemType == ItemType.Weapon)
			{
				this.SetStartingWeapon(itemData.code, itemData.initialCount);
				return;
			}
			if (itemType != ItemType.Armor)
			{
				return;
			}
			ItemArmorData itemArmorData = (ItemArmorData)itemData;
			this.SetStartingArmor(itemArmorData.armorType, itemArmorData.code, itemData.initialCount);
		}

		
		private KeyValuePair<int, int> startingWeapon;

		
		private readonly List<KeyValuePair<int, int>> startingArmors = new List<KeyValuePair<int, int>>();

		
		private readonly Dictionary<int, int> startingInventoryItems = new Dictionary<int, int>();

		
		private int startingAreaCode;

		
		private int startingAreaIndex;

		
		private int[] walkableAreaCodes;

		
		private int characterLevel = 1;

		
		private int weaponMasteryLevel1;

		
		private int weaponMasteryLevel2;

		
		private int weaponMasteryLevel3;

		
		private int weaponMasteryLevel4;

		
		private int explorationMasteryLevel1;

		
		private int explorationMasteryLevel2;

		
		private int explorationMasteryLevel3;

		
		private int explorationMasteryLevel4;

		
		private int survivalMasteryLevel1;

		
		private int survivalMasteryLevel2;

		
		private int survivalMasteryLevel3;

		
		private int survivalMasteryLevel4;

		
		private int skillLevelPassive = -1;

		
		private int skillLevelActive1 = -1;

		
		private int skillLevelActive2 = -1;

		
		private int skillLevelActive3 = -1;

		
		private int skillLevelActive4 = -1;

		
		private int skillPoint = -1;

		
		private int startItemGroupCode;
	}
}
