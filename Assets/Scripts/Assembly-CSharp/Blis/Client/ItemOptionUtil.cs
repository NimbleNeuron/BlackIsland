using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class ItemOptionUtil
	{
		private const string Heal = "체력 회복";


		private const string HpRecover = "체력 재생";


		private const string SpRecover = "SP 재생";


		private const string TrapDamage = "트랩 데미지";

		public static List<ItemOptionText> GetItemOptionTexts(ItemData itemData)
		{
			List<ItemOptionText> list = new List<ItemOptionText>();
			if (itemData.itemType == ItemType.Weapon)
			{
				BuildWeaponUI(list, itemData);
			}
			else if (itemData.itemType == ItemType.Armor)
			{
				BuildArmorUI(list, itemData);
			}
			else if (itemData.itemType == ItemType.Consume)
			{
				BuildConsumeUI(list, itemData);
			}
			else if (itemData.itemType == ItemType.Special)
			{
				BuildSpecialUI(list, itemData);
			}

			return list;
		}


		private static void GetItemOption(List<ItemOptionText> optionList, StatType statType, float value,
			Color? forceToColor = null)
		{
			if (value == 0f)
			{
				return;
			}

			GetItemOption(optionList, Ln.Get(LnType.StatType, statType.ToString()), statType.IsRatio(), value,
				forceToColor);
		}


		private static void GetItemOption(List<ItemOptionText> optionList, string labelName, bool isRatio, float value,
			Color? forceToColor = null)
		{
			if (value == 0f)
			{
				return;
			}

			optionList.Add(new ItemOptionText(labelName, isRatio, value, forceToColor));
		}


		private static void BuildWeaponUI(List<ItemOptionText> optionList, ItemData itemData)
		{
			ItemWeaponData subTypeData = itemData.GetSubTypeData<ItemWeaponData>();
			GetItemOption(optionList, StatType.AttackPower, subTypeData.attackPower);
			GetItemOption(optionList, StatType.Defense, subTypeData.defense);
			GetItemOption(optionList, StatType.MaxHp, subTypeData.maxHp);
			GetItemOption(optionList, StatType.HpRegenRatio, subTypeData.hpRegenRatio);
			GetItemOption(optionList, StatType.HpRegen, subTypeData.hpRegen);
			GetItemOption(optionList, StatType.SpRegenRatio, subTypeData.spRegenRatio);
			GetItemOption(optionList, StatType.SpRegen, subTypeData.spRegen);
			GetItemOption(optionList, StatType.AttackSpeedRatio, subTypeData.attackSpeedRatio);
			GetItemOption(optionList, StatType.MoveSpeed, subTypeData.moveSpeed);
			GetItemOption(optionList, StatType.SightRange, subTypeData.sightRange);
			GetItemOption(optionList, StatType.CriticalStrikeChance, subTypeData.criticalStrikeChance);
			GetItemOption(optionList, StatType.CriticalStrikeDamage, subTypeData.criticalStrikeDamage);
			GetItemOption(optionList, StatType.CooldownReduction, subTypeData.cooldownReduction);
			GetItemOption(optionList, StatType.LifeSteal, subTypeData.lifeSteal);
			GetItemOption(optionList, StatType.AttackRange, subTypeData.attackRange);
			GetItemOption(optionList, StatType.IncreaseBasicAttackDamage, subTypeData.increaseBasicAttackDamage);
			GetItemOption(optionList, StatType.IncreaseSkillDamage, subTypeData.increaseSkillDamage);
			GetItemOption(optionList, StatType.IncreaseSkillDamageRatio, subTypeData.increaseSkillDamageRatio);
			if (subTypeData.decreaseRecoveryToBasicAttack != 0)
			{
				GetItemOption(optionList, StatType.DecreaseRecoveryToBasicAttack, -0.4f, ItemOptionText.Green);
			}

			if (subTypeData.decreaseRecoveryToSkill != 0)
			{
				GetItemOption(optionList, StatType.DecreaseRecoveryToSkill, -0.4f, ItemOptionText.Green);
			}
		}


		private static void BuildArmorUI(List<ItemOptionText> optionList, ItemData itemData)
		{
			ItemArmorData subTypeData = itemData.GetSubTypeData<ItemArmorData>();
			GetItemOption(optionList, StatType.AttackPower, subTypeData.attackPower);
			GetItemOption(optionList, StatType.Defense, subTypeData.defense);
			GetItemOption(optionList, StatType.MaxHp, subTypeData.maxHp);
			GetItemOption(optionList, StatType.MaxSp, subTypeData.maxSp);
			GetItemOption(optionList, StatType.HpRegen, subTypeData.hpRegen);
			GetItemOption(optionList, StatType.HpRegenRatio, subTypeData.hpRegenRatio);
			GetItemOption(optionList, StatType.SpRegen, subTypeData.spRegen);
			GetItemOption(optionList, StatType.SpRegenRatio, subTypeData.spRegenRatio);
			GetItemOption(optionList, StatType.AttackSpeedRatio, subTypeData.attackSpeedRatio);
			GetItemOption(optionList, StatType.MoveSpeed, subTypeData.moveSpeed);
			GetItemOption(optionList, StatType.SightRange, subTypeData.sightRange);
			GetItemOption(optionList, StatType.CriticalStrikeChance, subTypeData.criticalStrikeChance);
			GetItemOption(optionList, StatType.CriticalStrikeDamage, subTypeData.criticalStrikeDamage);
			GetItemOption(optionList, StatType.PreventCriticalStrikeDamaged, subTypeData.preventCriticalStrikeDamaged);
			GetItemOption(optionList, StatType.CooldownReduction, subTypeData.cooldownReduction);
			GetItemOption(optionList, StatType.LifeSteal, subTypeData.lifeSteal);
			GetItemOption(optionList, StatType.MoveSpeedOutOfCombat, subTypeData.outOfCombatMoveSpeed);
			GetItemOption(optionList, StatType.IncreaseBasicAttackDamage, subTypeData.increaseBasicAttackDamage);
			GetItemOption(optionList, StatType.PreventBasicAttackDamaged, subTypeData.preventBasicAttackDamaged);
			GetItemOption(optionList, StatType.IncreaseSkillDamage, subTypeData.increaseSkillDamage);
			GetItemOption(optionList, StatType.IncreaseSkillDamageRatio, subTypeData.increaseSkillDamageRatio);
			GetItemOption(optionList, StatType.PreventSkillDamagedRatio, subTypeData.preventSkillDamagedRatio);
			GetItemOption(optionList, StatType.AttackRange, subTypeData.attackRange);
			if (subTypeData.decreaseRecoveryToBasicAttack != 0)
			{
				GetItemOption(optionList, StatType.DecreaseRecoveryToBasicAttack, -0.4f, ItemOptionText.Green);
			}

			if (subTypeData.decreaseRecoveryToSkill != 0)
			{
				GetItemOption(optionList, StatType.DecreaseRecoveryToSkill, -0.4f, ItemOptionText.Green);
			}
		}


		private static void BuildConsumeUI(List<ItemOptionText> optionList, ItemData itemData)
		{
			ItemConsumableData subTypeData = itemData.GetSubTypeData<ItemConsumableData>();
			GetItemOption(optionList, Ln.Get("체력 회복"), false, subTypeData.heal);
			GetItemOption(optionList, Ln.Get("체력 재생"), false, subTypeData.hpRecover);
			GetItemOption(optionList, Ln.Get("SP 재생"), false, subTypeData.spRecover);
		}


		private static void BuildSpecialUI(List<ItemOptionText> optionList, ItemData itemData)
		{
			ItemSpecialData subTypeData = itemData.GetSubTypeData<ItemSpecialData>();
			if (subTypeData.summonCode > 0)
			{
				SummonData summonData = GameDB.character.GetSummonData(subTypeData.summonCode);
				if (summonData != null)
				{
					GetItemOption(optionList, Ln.Get("트랩 데미지"), false, summonData.attackPower);
				}
			}
		}


		public class ItemOptionText
		{
			public static readonly Color Green = new Color32(0, 176, 0, byte.MaxValue);


			public static readonly Color Red = new Color32(176, 0, 0, byte.MaxValue);


			public readonly Color color;


			public readonly string text;

			public ItemOptionText(string labelName, bool isRatio, float value, Color? forceToColor)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				if (isRatio)
				{
					value *= 100f;
					text2 = "%";
				}

				if (value >= 0f)
				{
					text = "+";
					color = Green;
				}
				else
				{
					text = "-";
					color = Red;
				}

				if (forceToColor != null)
				{
					color = forceToColor.Value;
				}

				this.text = string.Format("{0} {1}{2}{3}", labelName, text, Mathf.Abs(value), text2);
			}
		}
	}
}