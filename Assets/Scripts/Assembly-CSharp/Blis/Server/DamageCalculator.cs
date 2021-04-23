using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public abstract class DamageCalculator
	{
		
		
		public DamageType DamageType
		{
			get
			{
				return this.damageType;
			}
		}

		
		
		public DamageSubType DamageSubType
		{
			get
			{
				return this.damageSubType;
			}
		}

		
		public DamageCalculator(WeaponType weaponType, DamageType damageType, DamageSubType damageSubType, int damageDataCode, int damageId, int baseDamage, int minRemain, float damageMasteryModifier)
		{
			this.weaponType = weaponType;
			this.damageType = damageType;
			this.damageSubType = damageSubType;
			this.damageDataCode = damageDataCode;
			this.damageId = damageId;
			this.baseDamage = baseDamage;
			this.minRemain = minRemain;
			this.damageMasteryModifier = damageMasteryModifier;
		}

		
		private void AddCoefficient(StatType coefStatType, float coef)
		{
			if (this.coefMap == null)
			{
				this.coefMap = new Dictionary<StatType, float>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);
			}
			if (this.coefMap.ContainsKey(coefStatType))
			{
				Dictionary<StatType, float> dictionary = this.coefMap;
				dictionary[coefStatType] += coef;
				return;
			}
			this.coefMap.Add(coefStatType, coef);
		}

		
		
		private float finalMoreDamage
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.FinalMoreDamage);
			}
		}

		
		
		private float finalLessDamage
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.victimSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.FinalLessDamage);
			}
		}

		
		
		protected float FinalAddDamage
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.FinalAddDamage);
			}
		}

		
		
		private float externalCriticalChance
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.CriticalChance);
			}
		}

		
		
		protected float DamageCharacterLvCoef
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.DamageCharacterLvCoef);
			}
		}

		
		
		protected float DamageCasterLossHpCoef
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.DamageCasterLossHpCoef);
			}
		}

		
		
		protected float DamageCharacterSpeedCoef
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.DamageCharacterSpeedCoef);
			}
		}

		
		
		protected float DamageCasterMaxHpCoef
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.DamageCasterMaxHpCoef);
			}
		}

		
		
		protected float DamageTargetMaxHpCoef
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.DamageTargetMaxHpCoef);
			}
		}

		
		
		protected float DamageTargetLossHpCoef
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.DamageTargetLossHpCoef);
			}
		}

		
		
		protected float DamageCasterMaxSpCoef
		{
			get
			{
				SkillScriptParameterCollection skillScriptParameterCollection = this.attackerSkillScriptParameter;
				if (skillScriptParameterCollection == null)
				{
					return 0f;
				}
				return skillScriptParameterCollection.Get(SkillScriptParameterType.DamageCasterMaxSpCoef);
			}
		}

		
		public void SetAttackerSkillScriptParameter(SkillScriptParameterCollection attackerSkillScriptParameter)
		{
			this.attackerSkillScriptParameter = attackerSkillScriptParameter;
			float num = attackerSkillScriptParameter.Get(SkillScriptParameterType.DamageApCoef);
			if (0f < num)
			{
				this.AddCoefficient(StatType.AttackPower, num);
			}
			float num2 = attackerSkillScriptParameter.Get(SkillScriptParameterType.DamageDefCoef);
			if (0f < num2)
			{
				this.AddCoefficient(StatType.Defense, num2);
			}
		}

		
		public void SetVictimSkillScriptParameter(SkillScriptParameterCollection victimSkillScriptParameter)
		{
			this.victimSkillScriptParameter = victimSkillScriptParameter;
		}

		
		public DamageInfo Calculate(WorldCharacter self, AttackerInfo attackerInfo)
		{
			ObjectType objectType = self.ObjectType;
			if (objectType - ObjectType.SummonCamera <= 1)
			{
				return this.CalculateDamageToSummon((WorldSummonBase)self, attackerInfo);
			}
			return this.CalculateDamageToCharacter(self, attackerInfo);
		}

		
		private DamageInfo CalculateDamageToSummon(WorldSummonBase self, AttackerInfo attackerInfo)
		{
			int num = Math.Max(1, this.baseDamage);
			if (this.damageType != DamageType.RedZone)
			{
				if (this.weaponType != WeaponType.None)
				{
					WeaponTypeInfoData weaponTypeInfoData = GameDB.mastery.GetWeaponTypeInfoData(this.weaponType);
					if (weaponTypeInfoData != null)
					{
						num = weaponTypeInfoData.summonObjectHitDamage;
					}
				}
				if (self.SummonData.isInvincibility)
				{
					num = 0;
				}
			}
			int maxDamageOnHp = 0;
			if (this.damageType != DamageType.Sp)
			{
				maxDamageOnHp = ((self.Status.Hp >= num) ? num : self.Status.Hp);
			}
			DamageInfo damageInfo = DamageInfo.Create(num, this.damageType, this.damageSubType, this.damageDataCode, this.damageId, this.minRemain);
			damageInfo.SetAttacker(attackerInfo.Attacker);
			damageInfo.SetCritical(false);
			damageInfo.SetUndefendedDamage(num);
			damageInfo.SetMaxDamageOnHp(maxDamageOnHp);
			return damageInfo;
		}

		
		protected abstract DamageInfo CalculateDamageToCharacter(WorldCharacter self, AttackerInfo attackerInfo);

		
		protected int CalculateDamage(int attackerAttackPower, int baseDamage, int victimDefense, float attackerCriticalDamageRate)
		{
			return (int)((float)(attackerAttackPower + baseDamage) * attackerCriticalDamageRate * (100f / (100f + (float)victimDefense)));
		}

		
		protected float CriticalDamageRate(WorldCharacter self, AttackerInfo attackerInfo, DamageType damageType)
		{
			float result = 1f;
			if (attackerInfo.Attacker == null)
			{
				return result;
			}
			if (0f < this.externalCriticalChance)
			{
				if (this.IsCritical(this.externalCriticalChance))
				{
					result = 2f + attackerInfo.CachedStat.CriticalStrikeDamage - self.Stat.PreventCriticalStrikeDamaged;
				}
			}
			else if (damageType == DamageType.Normal && this.IsCritical(attackerInfo))
			{
				result = 2f + attackerInfo.CachedStat.CriticalStrikeDamage - self.Stat.PreventCriticalStrikeDamaged;
			}
			return result;
		}

		
		private bool IsCritical(AttackerInfo attackerInfo)
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			float actualCriticalChance = GameDB.character.GetActualCriticalChance(attackerInfo.CachedStat.CriticalStrikeChance);
			attackerInfo.Attacker.AddActualCriticalChance(actualCriticalChance);
			bool flag = attackerInfo.Attacker.ActualCriticalChance >= num;
			if (flag)
			{
				attackerInfo.Attacker.ResetActualCriticalChance();
			}
			return flag;
		}

		
		private bool IsCritical(float criticalChance)
		{
			float num = UnityEngine.Random.Range(0f, 1f);
			if (1f < criticalChance)
			{
				criticalChance = 1f;
			}
			return criticalChance >= num;
		}

		
		protected int AmplificationDamage(AttackerInfo attackerInfo, WorldCharacter victim, int damage)
		{
			float num = this.finalMoreDamage - this.finalLessDamage;
			if (victim.ObjectType.Equals(ObjectType.Monster))
			{
				num += attackerInfo.CachedStat.AmplifierToMonsterRatio;
			}
			damage = (int)((float)damage + (float)damage * num);
			return damage;
		}

		
		protected float GetRatioModeDamage(AttackerInfo attackerInfo, WorldCharacter self)
		{
			float b = 1f + (attackerInfo.CachedStat.IncreaseModeDamageRatio - self.Stat.PreventModeDamageRatio);
			return Mathf.Max(0f, b);
		}

		
		protected WeaponType weaponType;

		
		protected DamageType damageType;

		
		protected DamageSubType damageSubType;

		
		protected int damageDataCode;

		
		protected int damageId;

		
		protected int baseDamage;

		
		protected int minRemain;

		
		protected float damageMasteryModifier;

		
		protected Dictionary<StatType, float> coefMap;

		
		private SkillScriptParameterCollection attackerSkillScriptParameter;

		
		private SkillScriptParameterCollection victimSkillScriptParameter;
	}
}
