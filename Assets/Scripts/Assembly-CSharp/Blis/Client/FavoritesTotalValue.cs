using System;
using System.Collections.Generic;
using Blis.Client.UI;
using Blis.Common;

namespace Blis.Client
{
	public class FavoritesTotalValue : BaseUI
	{
		private readonly List<UIFavoritesItem> favItems = new List<UIFavoritesItem>();


		private readonly ItemOptionValue itemOptionValue = new ItemOptionValue();


		private LnText txt_AttackPower;


		private LnText txt_AttackRange;


		private LnText txt_AttackSpeed;


		private LnText txt_CooldownReduction;


		private LnText txt_CriticalChance;


		private LnText txt_CriticalDamage;


		private LnText txt_Defense;


		private LnText txt_IncreaseBasicAttackDamage;


		private LnText txt_InCreaseSkillDamage;


		private LnText txt_InCreaseSkillDamageRatio;


		private LnText txt_LifeSteal;


		private LnText txt_MaxHP;


		private LnText txt_MaxSP;


		private LnText txt_MoveSpeed;


		private LnText txt_OutOfCombatMoveSpeed;


		private LnText txt_PreventBasicAttakDamage;


		private LnText txt_PreventSkillDamage;


		private LnText txt_PreventSkillDamageRatio;


		private LnText txt_RegenHP;


		private LnText txt_RegenHPRatio;


		private LnText txt_RegenSP;


		private LnText txt_RegenSPRatio;


		private LnText txt_SightRange;


		protected override void OnDestroy()
		{
			base.OnDestroy();
			UISystem.RemoveListener<FavoriteCommonViewStore>(OnFavoriteCommonViewStoreUpdate);
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			txt_AttackPower = GameUtil.Bind<LnText>(gameObject, "Category/Left/Value_AttackPower/TXT_AttackPower");
			txt_AttackSpeed = GameUtil.Bind<LnText>(gameObject, "Category/Left/Value_AttackSpeed/TXT_AttackSpeed");
			txt_CriticalChance =
				GameUtil.Bind<LnText>(gameObject, "Category/Left/Value_CriticalChance/TXT_CriticalChance");
			txt_CriticalDamage =
				GameUtil.Bind<LnText>(gameObject, "Category/Left/Value_CriticalDamage/TXT_CriticalDamage");
			txt_LifeSteal = GameUtil.Bind<LnText>(gameObject, "Category/Left/Value_LifeSteal/TXT_LifeSteal");
			txt_IncreaseBasicAttackDamage = GameUtil.Bind<LnText>(gameObject,
				"Category/Left/Value_IncreaseBasicAttackDamage/TXT_IncreaseBasicAttackDamage");
			txt_InCreaseSkillDamage = GameUtil.Bind<LnText>(gameObject,
				"Category/Left/Value_IncreaseSkillDamage/TXT_IncreaseSkillDamage");
			txt_InCreaseSkillDamageRatio = GameUtil.Bind<LnText>(gameObject,
				"Category/Left/Value_IncreaseSkillDamage/TXT_IncreaseSkillDamageRatio");
			txt_CooldownReduction =
				GameUtil.Bind<LnText>(gameObject, "Category/Left/Value_CooldownReduction/TXT_CooldownReduction");
			txt_RegenSP = GameUtil.Bind<LnText>(gameObject, "Category/Left/Value_RegenSP/TXT_RegenSP");
			txt_RegenSPRatio = GameUtil.Bind<LnText>(gameObject, "Category/Left/Value_RegenSP/TXT_RegenSPRatio");
			txt_PreventSkillDamage = GameUtil.Bind<LnText>(gameObject,
				"Category/Left/Value_PreventSkillDamage/TXT_PreventSkillDamage");
			txt_PreventSkillDamageRatio = GameUtil.Bind<LnText>(gameObject,
				"Category/Left/Value_PreventSkillDamage/TXT_PreventSkillDamageRatio");
			txt_Defense = GameUtil.Bind<LnText>(gameObject, "Category/Right/Value_Defense/TXT_Defense");
			txt_MaxHP = GameUtil.Bind<LnText>(gameObject, "Category/Right/Value_MaxHP/TXT_MaxHP");
			txt_MaxSP = GameUtil.Bind<LnText>(gameObject, "Category/Right/Value_MaxSP/TXT_MaxSP");
			txt_RegenHP = GameUtil.Bind<LnText>(gameObject, "Category/Right/Value_RegenHP/TXT_RegenHP");
			txt_RegenHPRatio = GameUtil.Bind<LnText>(gameObject, "Category/Right/Value_RegenHP/TXT_RegenHPRatio");
			txt_PreventBasicAttakDamage = GameUtil.Bind<LnText>(gameObject,
				"Category/Right/Value_PreventBasicAttackDamage/TXT_PreventBasicAttackDamage");
			txt_MoveSpeed = GameUtil.Bind<LnText>(gameObject, "Category/Right/Value_MoveSpeed/TXT_MoveSpeed");
			txt_OutOfCombatMoveSpeed = GameUtil.Bind<LnText>(gameObject,
				"Category/Right/Value_OutOfCombatMoveSpeed/TXT_OutOfCombatMoveSpeed");
			txt_SightRange = GameUtil.Bind<LnText>(gameObject, "Category/Right/Value_SightRange/TXT_SightRange");
			txt_AttackRange = GameUtil.Bind<LnText>(gameObject, "Category/Right/Value_AttackRange/TXT_AttackRange");
			UISystem.AddListener<FavoriteCommonViewStore>(OnFavoriteCommonViewStoreUpdate);
		}


		private void OnFavoriteCommonViewStoreUpdate(FavoriteCommonViewStore store)
		{
			favItems.Clear();
			favItems.AddRange(store.GetFavoritesItems());
			CalculateOptionValue();
			RenderView();
		}


		private void CalculateOptionValue()
		{
			itemOptionValue.Clear();
			foreach (UIFavoritesItem uifavoritesItem in favItems)
			{
				ItemData itemData = uifavoritesItem.GetItemData();
				if (itemData != null)
				{
					ItemType itemType = itemData.itemType;
					if (itemType != ItemType.Weapon)
					{
						if (itemType == ItemType.Armor)
						{
							ItemArmorData subTypeData = itemData.GetSubTypeData<ItemArmorData>();
							MergeOptionValue(subTypeData.attackPower, subTypeData.attackSpeedRatio,
								subTypeData.criticalStrikeChance, subTypeData.criticalStrikeDamage,
								subTypeData.lifeSteal, subTypeData.increaseBasicAttackDamage,
								subTypeData.increaseSkillDamage, subTypeData.increaseSkillDamageRatio,
								subTypeData.cooldownReduction, subTypeData.spRegen, subTypeData.spRegenRatio, 0f,
								subTypeData.preventSkillDamagedRatio, subTypeData.defense, subTypeData.maxHp,
								subTypeData.maxSp, subTypeData.hpRegen, subTypeData.hpRegenRatio,
								subTypeData.preventBasicAttackDamaged, subTypeData.moveSpeed,
								subTypeData.outOfCombatMoveSpeed, subTypeData.sightRange, subTypeData.attackRange);
						}
					}
					else
					{
						ItemWeaponData subTypeData2 = itemData.GetSubTypeData<ItemWeaponData>();
						MergeOptionValue(subTypeData2.attackPower, subTypeData2.attackSpeedRatio,
							subTypeData2.criticalStrikeChance, subTypeData2.criticalStrikeDamage,
							subTypeData2.lifeSteal, subTypeData2.increaseBasicAttackDamage,
							subTypeData2.increaseSkillDamage, subTypeData2.increaseSkillDamageRatio,
							subTypeData2.cooldownReduction, subTypeData2.spRegen, subTypeData2.spRegenRatio, 0f, 0f,
							subTypeData2.defense, subTypeData2.maxHp, 0, subTypeData2.hpRegen,
							subTypeData2.hpRegenRatio, 0f, subTypeData2.moveSpeed, 0f, subTypeData2.sightRange,
							subTypeData2.attackRange);
					}
				}
			}
		}


		private void MergeOptionValue(int attackPower, float attackSpeedRatio, float criticalChance,
			float criticalDamage, float lifeSteal, float increaseBasicAttackDamage, float increaseSkillDamage,
			float increaseSkillDamageRatio, float cooldownReduction, float regenSP, float regenSPRatio,
			float preventSkillDamage, float preventSkillDamageRatio, int defense, int maxHP, int maxSP, float regenHP,
			float regenHPRatio, float preventBasicAttackDamage, float moveSpeed, float outOfCombatMoveSpeed,
			float sightRange, float AttackRange)
		{
			itemOptionValue.AttackPower += attackPower;
			itemOptionValue.AttackSpeed += attackSpeedRatio * 100f;
			itemOptionValue.CriticalChance += criticalChance * 100f;
			itemOptionValue.CriticalDamage += criticalDamage;
			itemOptionValue.LifeSteal += lifeSteal * 100f;
			itemOptionValue.IncreaseBasicAttackDamage += increaseBasicAttackDamage;
			itemOptionValue.IncreaseSkillDamage += increaseSkillDamage;
			itemOptionValue.IncreaseSkillDamageRatio += increaseSkillDamageRatio * 100f;
			itemOptionValue.CooldownReduction += cooldownReduction * 100f;
			itemOptionValue.RegenSP += regenSP;
			itemOptionValue.RegenSPRatio += regenSPRatio * 100f;
			itemOptionValue.PreventSkillDamage += preventSkillDamage;
			itemOptionValue.PreventSkillDamageRatio += preventSkillDamageRatio * 100f;
			itemOptionValue.Defense += defense;
			itemOptionValue.MaxHP += maxHP;
			itemOptionValue.MaxSP += maxSP;
			itemOptionValue.RegenHP += regenHP;
			itemOptionValue.RegenHPRatio += regenHPRatio * 100f;
			itemOptionValue.PreventBasicAttackDamage += preventBasicAttackDamage;
			itemOptionValue.OutOfCombatMoveSpped += outOfCombatMoveSpeed;
			itemOptionValue.MoveSpeed += moveSpeed;
			itemOptionValue.SightRange += sightRange;
			itemOptionValue.AttackRange += AttackRange;
		}


		private void RenderView()
		{
			txt_AttackPower.text = itemOptionValue.AttackPower.ToString();
			txt_AttackSpeed.text = string.Format("{0}%", Math.Truncate(itemOptionValue.AttackSpeed));
			txt_CriticalChance.text = string.Format("{0}%", Math.Truncate(itemOptionValue.CriticalChance));
			txt_CriticalDamage.text = string.Format("{0:0.##}", itemOptionValue.CriticalDamage);
			txt_LifeSteal.text = string.Format("{0}%", Math.Truncate(itemOptionValue.LifeSteal));
			txt_IncreaseBasicAttackDamage.text = string.Format("{0}", itemOptionValue.IncreaseBasicAttackDamage);
			txt_InCreaseSkillDamage.text = string.Format("{0}", itemOptionValue.IncreaseSkillDamage);
			txt_InCreaseSkillDamageRatio.text =
				string.Format("{0}%", Math.Truncate(itemOptionValue.IncreaseSkillDamageRatio));
			txt_CooldownReduction.text = string.Format("{0}%", Math.Truncate(itemOptionValue.CooldownReduction));
			txt_RegenSP.text = string.Format("{0:0.##}", itemOptionValue.RegenSP);
			txt_RegenSPRatio.text = string.Format("{0}%", Math.Truncate(itemOptionValue.RegenSPRatio));
			txt_PreventSkillDamage.text = string.Format("{0}", itemOptionValue.PreventSkillDamage);
			txt_PreventSkillDamageRatio.text =
				string.Format("{0}%", Math.Truncate(itemOptionValue.PreventSkillDamageRatio));
			txt_Defense.text = string.Format("{0}", itemOptionValue.Defense);
			txt_MaxHP.text = string.Format("{0}", itemOptionValue.MaxHP);
			txt_MaxSP.text = string.Format("{0}", itemOptionValue.MaxSP);
			txt_RegenHP.text = string.Format("{0:0.##}", itemOptionValue.RegenHP);
			txt_RegenHPRatio.text = string.Format("{0}%", Math.Truncate(itemOptionValue.RegenHPRatio));
			txt_PreventBasicAttakDamage.text = string.Format("{0}", itemOptionValue.PreventBasicAttackDamage);
			txt_MoveSpeed.text = string.Format("{0:0.##}", itemOptionValue.MoveSpeed);
			txt_OutOfCombatMoveSpeed.text = string.Format("{0:0.##}", itemOptionValue.OutOfCombatMoveSpped);
			txt_SightRange.text = string.Format("{0}", itemOptionValue.SightRange);
			txt_AttackRange.text = string.Format("{0}", itemOptionValue.AttackRange);
		}


		private class ItemOptionValue
		{
			
			public int AttackPower { get; set; }


			
			public float AttackSpeed { get; set; }


			
			public float CriticalChance { get; set; }


			
			public float CriticalDamage { get; set; }


			
			public float LifeSteal { get; set; }


			
			public float IncreaseBasicAttackDamage { get; set; }


			
			public float IncreaseSkillDamage { get; set; }


			
			public float IncreaseSkillDamageRatio { get; set; }


			
			public float CooldownReduction { get; set; }


			
			public float RegenSP { get; set; }


			
			public float RegenSPRatio { get; set; }


			
			public float PreventSkillDamage { get; set; }


			
			public float PreventSkillDamageRatio { get; set; }


			
			public int Defense { get; set; }


			
			public int MaxHP { get; set; }


			
			public int MaxSP { get; set; }


			
			public float RegenHP { get; set; }


			
			public float RegenHPRatio { get; set; }


			
			public float PreventBasicAttackDamage { get; set; }


			
			public float MoveSpeed { get; set; }


			
			public float OutOfCombatMoveSpped { get; set; }


			
			public float SightRange { get; set; }


			
			public float AttackRange { get; set; }


			public void Clear()
			{
				AttackPower = 0;
				AttackSpeed = 0f;
				CriticalChance = 0f;
				CriticalDamage = 0f;
				LifeSteal = 0f;
				IncreaseBasicAttackDamage = 0f;
				IncreaseSkillDamage = 0f;
				IncreaseSkillDamageRatio = 0f;
				CooldownReduction = 0f;
				RegenSP = 0f;
				RegenSPRatio = 0f;
				PreventSkillDamage = 0f;
				PreventSkillDamageRatio = 0f;
				Defense = 0;
				MaxHP = 0;
				MaxSP = 0;
				RegenHP = 0f;
				RegenHPRatio = 0f;
				PreventBasicAttackDamage = 0f;
				MoveSpeed = 0f;
				OutOfCombatMoveSpped = 0f;
				SightRange = 0f;
				AttackRange = 0f;
			}
		}
	}
}