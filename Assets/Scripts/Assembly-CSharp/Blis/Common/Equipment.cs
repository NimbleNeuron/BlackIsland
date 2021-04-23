using System;
using System.Collections.Generic;
using System.Linq;

namespace Blis.Common
{
	public class Equipment
	{
		private readonly ItemStat equipmentStat;
		private readonly Item[] equips;
		private readonly List<EquipItem> ret = new List<EquipItem>();
		private readonly HashSet<EquipSlotType> updatedSlotTypes;

		public Equipment()
		{
			updatedSlotTypes = new HashSet<EquipSlotType>();
			equips = new Item[Enum.GetValues(typeof(EquipSlotType)).Length - 1];
			equipmentStat = new ItemStat();
		}


		public int Count {
			get { return equips.Count(x => x != null); }
		}


		public void UpdateEquipment(List<Item> equipments)
		{
			Array.Clear(equips, 0, equips.Length);
			for (int i = 0; i < equipments.Count; i++)
			{
				EquipSlotType equipSlotType = GetEquipSlotType(equipments[i]);
				if (equipSlotType != EquipSlotType.None)
				{
					equips[(int) equipSlotType] = equipments[i];
				}
			}

			UpdateEquipStat();
		}


		private EquipSlotType GetEquipSlotType(Item item)
		{
			if (item.ItemData.itemType == ItemType.Weapon)
			{
				return EquipSlotType.Weapon;
			}

			if (item.ItemData.itemType == ItemType.Armor)
			{
				return GetEquipSlotType(item.ItemData.GetSubTypeData<ItemArmorData>().armorType);
			}

			return EquipSlotType.None;
		}


		private EquipSlotType GetEquipSlotType(ArmorType armorType)
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


		public bool Equip(Item equip, out Item oldEquip)
		{
			oldEquip = null;
			if (equip == null)
			{
				return false;
			}

			EquipSlotType equipSlotType = GetEquipSlotType(equip);
			if (equipSlotType == EquipSlotType.None)
			{
				return false;
			}

			oldEquip = equips[(int) equipSlotType];
			Item item = oldEquip;
			equips[(int) equipSlotType] = equip;
			AddUpdateSlotType(equipSlotType);
			UpdateEquipStat();
			return true;
		}


		public bool Unequip(Item equip)
		{
			if (equip == null)
			{
				return false;
			}

			for (int i = 0; i < equips.Length; i++)
			{
				if (equips[i] != null && equips[i].id == equip.id)
				{
					AddUpdateSlotType(GetEquipSlotType(equip));
					equips[i] = null;
					UpdateEquipStat();
					return true;
				}
			}

			return false;
		}


		private void AddUpdateSlotType(EquipSlotType equipSlotType)
		{
			if (equipSlotType == EquipSlotType.None)
			{
				Log.E("[Hack Warning] AddUpdateSlotType type is None.");
			}

			updatedSlotTypes.Add(equipSlotType);
		}


		public List<EquipItem> FlushUpdates()
		{
			ret.Clear();
			for (int i = 0; i < equips.Length; i++)
			{
				if (equips[i] != null && equips[i].FlushDirty())
				{
					updatedSlotTypes.Add(GetEquipSlotType(equips[i]));
				}
			}

			foreach (EquipSlotType equipSlotType in updatedSlotTypes)
			{
				if (equipSlotType != EquipSlotType.None)
				{
					ret.Add(new EquipItem(equipSlotType, equips[(int) equipSlotType]));
				}
			}

			updatedSlotTypes.Clear();
			return ret;
		}


		public List<Item> GetEquips()
		{
			List<Item> list = new List<Item>();
			foreach (Item item in equips)
			{
				if (item != null)
				{
					list.Add(item);
				}
			}

			return list;
		}


		public Item FindEquip(int itemCode)
		{
			for (int i = 0; i < equips.Length; i++)
			{
				if (equips[i] != null && equips[i].itemCode == itemCode)
				{
					return equips[i];
				}
			}

			return null;
		}


		public Item FindEquipById(int id)
		{
			for (int i = 0; i < equips.Length; i++)
			{
				if (equips[i] != null && equips[i].id == id)
				{
					return equips[i];
				}
			}

			return null;
		}


		public Item GetWeapon()
		{
			return equips[0];
		}


		public Item GetArmor(ArmorType armorType)
		{
			EquipSlotType equipSlotType = GetEquipSlotType(armorType);
			if (equipSlotType != EquipSlotType.None)
			{
				return equips[(int) equipSlotType];
			}

			return null;
		}


		public bool CanEquip(Item item)
		{
			if (!item.ItemData.IsEquipItem())
			{
				return false;
			}

			EquipSlotType equipSlotType = item.ItemData.GetEquipSlotType();
			Item item2;
			if (equipSlotType == EquipSlotType.Weapon)
			{
				item2 = GetWeapon();
				if (item2 == null)
				{
					return true;
				}
			}
			else
			{
				item2 = GetArmor(equipSlotType.GetArmorType());
				if (item2 == null)
				{
					return true;
				}
			}

			return item2.itemCode == item.ItemData.code && item.Amount <= item2.ItemData.stackable - item2.Amount;
		}


		public float GetEquipStat(StatType statType)
		{
			ItemStat itemStat = equipmentStat;
			switch (statType)
			{
				case StatType.MaxHp:
					return itemStat.maxHp;
				case StatType.MaxSp:
					return itemStat.maxSp;
				case StatType.AttackPower:
					return itemStat.attackPower;
				case StatType.Defense:
					return itemStat.defense;
				case StatType.HpRegen:
					return itemStat.hpRegen;
				case StatType.SpRegen:
					return itemStat.spRegen;
				case StatType.AttackSpeed:
					return itemStat.attackSpeed;
				case StatType.MoveSpeed:
					return itemStat.moveSpeed;
				case StatType.SightRange:
					return itemStat.sightRange;
				case StatType.AttackRange:
					return itemStat.attackRange;
				case StatType.CriticalStrikeChance:
					return itemStat.criticalStrikeChance;
				case StatType.CriticalStrikeDamage:
					return itemStat.criticalStrikeDamage;
				case StatType.PreventCriticalStrikeDamaged:
					return itemStat.preventCriticalStrikeDamaged;
				case StatType.CooldownReduction:
					return itemStat.cooldownReduction;
				case StatType.LifeSteal:
					return itemStat.lifeSteal;
				case StatType.MoveSpeedOutOfCombat:
					return itemStat.outOfCombatMoveSpeed;
				case StatType.AttackPowerRatio:
					return 0f;
				case StatType.DefenseRatio:
					return 0f;
				case StatType.MaxHpRatio:
					return 0f;
				case StatType.MaxSpRatio:
					return 0f;
				case StatType.HpRegenRatio:
					return itemStat.hpRegenRatio;
				case StatType.SpRegenRatio:
					return itemStat.spRegenRatio;
				case StatType.AttackSpeedRatio:
					return itemStat.attackSpeedRatio;
				case StatType.MoveSpeedRatio:
					return 0f;
				case StatType.TrapDamageRatio:
					return 0f;
			}

			return 0f;
		}


		public ItemStat GetEquipStats()
		{
			return equipmentStat;
		}


		private ItemStat UpdateEquipStat()
		{
			equipmentStat.Clear();
			AddWeaponStat();
			AddArmorStat(EquipSlotType.Head);
			AddArmorStat(EquipSlotType.Chest);
			AddArmorStat(EquipSlotType.Arm);
			AddArmorStat(EquipSlotType.Leg);
			AddArmorStat(EquipSlotType.Trinket);
			return equipmentStat;
		}


		private void AddWeaponStat()
		{
			Item item = equips[0];
			if (item != null)
			{
				ItemWeaponData subTypeData = item.ItemData.GetSubTypeData<ItemWeaponData>();
				equipmentStat.attackPower += subTypeData.attackPower;
				equipmentStat.defense += subTypeData.defense;
				equipmentStat.maxHp += subTypeData.maxHp;
				equipmentStat.hpRegenRatio += subTypeData.hpRegenRatio;
				equipmentStat.hpRegen += subTypeData.hpRegen;
				equipmentStat.spRegenRatio += subTypeData.spRegenRatio;
				equipmentStat.spRegen += subTypeData.spRegen;
				equipmentStat.attackSpeedRatio += subTypeData.attackSpeedRatio;
				equipmentStat.moveSpeed += subTypeData.moveSpeed;
				equipmentStat.sightRange += subTypeData.sightRange;
				equipmentStat.criticalStrikeChance += subTypeData.criticalStrikeChance;
				equipmentStat.criticalStrikeDamage += subTypeData.criticalStrikeDamage;
				equipmentStat.cooldownReduction += subTypeData.cooldownReduction;
				equipmentStat.lifeSteal += subTypeData.lifeSteal;
				equipmentStat.attackRange += subTypeData.attackRange;
				equipmentStat.increaseBasicAttackDamage += subTypeData.increaseBasicAttackDamage;
				equipmentStat.increaseSkillDamage += subTypeData.increaseSkillDamage;
				equipmentStat.increaseSkillDamageRatio += subTypeData.increaseSkillDamageRatio;
				equipmentStat.decreaseRecoveryToBasicAttack += subTypeData.decreaseRecoveryToBasicAttack;
				equipmentStat.decreaseRecoveryToSkill += subTypeData.decreaseRecoveryToSkill;
				equipmentStat.weaponAttackSpeed += item.WeaponTypeInfoData.attackSpeed;
				equipmentStat.weaponAttackRange += item.WeaponTypeInfoData.attackRange;
			}
		}


		private void AddArmorStat(EquipSlotType equipSlotType)
		{
			Item item = equips[(int) equipSlotType];
			if (item != null)
			{
				ItemArmorData subTypeData = item.ItemData.GetSubTypeData<ItemArmorData>();
				equipmentStat.attackPower += subTypeData.attackPower;
				equipmentStat.defense += subTypeData.defense;
				equipmentStat.maxHp += subTypeData.maxHp;
				equipmentStat.maxSp += subTypeData.maxSp;
				equipmentStat.hpRegen += subTypeData.hpRegen;
				equipmentStat.spRegen += subTypeData.spRegen;
				equipmentStat.hpRegenRatio += subTypeData.hpRegenRatio;
				equipmentStat.spRegenRatio += subTypeData.spRegenRatio;
				equipmentStat.attackSpeedRatio += subTypeData.attackSpeedRatio;
				equipmentStat.moveSpeed += subTypeData.moveSpeed;
				equipmentStat.sightRange += subTypeData.sightRange;
				equipmentStat.criticalStrikeChance += subTypeData.criticalStrikeChance;
				equipmentStat.criticalStrikeDamage += subTypeData.criticalStrikeDamage;
				equipmentStat.preventCriticalStrikeDamaged += subTypeData.preventCriticalStrikeDamaged;
				equipmentStat.cooldownReduction += subTypeData.cooldownReduction;
				equipmentStat.lifeSteal += subTypeData.lifeSteal;
				equipmentStat.outOfCombatMoveSpeed += subTypeData.outOfCombatMoveSpeed;
				equipmentStat.increaseBasicAttackDamage += subTypeData.increaseBasicAttackDamage;
				equipmentStat.preventBasicAttackDamaged += subTypeData.preventBasicAttackDamaged;
				equipmentStat.increaseSkillDamage += subTypeData.increaseSkillDamage;
				equipmentStat.increaseSkillDamageRatio += subTypeData.increaseSkillDamageRatio;
				equipmentStat.preventSkillDamagedRatio += subTypeData.preventSkillDamagedRatio;
				equipmentStat.decreaseRecoveryToBasicAttack += subTypeData.decreaseRecoveryToBasicAttack;
				equipmentStat.decreaseRecoveryToSkill += subTypeData.decreaseRecoveryToSkill;
				equipmentStat.attackRange += subTypeData.attackRange;
			}
		}


		public Dictionary<string, string> GetEquipmentLogInfo()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (object obj in Enum.GetValues(typeof(EquipSlotType)))
			{
				EquipSlotType equipSlotType = (EquipSlotType) obj;
				if (equipSlotType != EquipSlotType.None)
				{
					Item item = equips[(int) equipSlotType];
					if (item != null)
					{
						dictionary.Add(equipSlotType.ToString(), item.ItemData.name);
					}
				}
			}

			return dictionary;
		}


		public bool HasItem(int itemCode)
		{
			return equips.Count(item => item != null && item.itemCode == itemCode) > 0;
		}


		public void UpdateItem(EquipItem item)
		{
			equips[(int) item.slotType] = item.item;
		}


		public float GetStatValue(StatType statType)
		{
			float num = 0f;
			foreach (Item item in equips)
			{
				if (item != null)
				{
					num += item.ItemData.GetStatValue(statType);
				}
			}

			return num;
		}
	}
}