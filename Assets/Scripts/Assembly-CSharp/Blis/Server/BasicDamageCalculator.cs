using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class BasicDamageCalculator : DamageCalculator
	{
		
		public BasicDamageCalculator(WeaponType weaponType, DamageType damageType, DamageSubType damageSubType,
			int damageDataCode, int damageID, int baseDamage, int minRemain, float damageMasteryModifier) : base(
			weaponType, damageType, damageSubType, damageDataCode, damageID, baseDamage, minRemain,
			damageMasteryModifier) { }

		
		protected override DamageInfo CalculateDamageToCharacter(WorldCharacter self, AttackerInfo attackerInfo)
		{
			int num = 0;
			if (coefMap != null)
			{
				foreach (KeyValuePair<StatType, float> keyValuePair in coefMap)
				{
					if (keyValuePair.Key != StatType.None)
					{
						num += (int) (attackerInfo.CachedStat.GetValue(keyValuePair.Key) * keyValuePair.Value);
					}
				}
			}

			if (attackerInfo.Attacker != null)
			{
				num += (int) (attackerInfo.Attacker.Status.Level * DamageCharacterLvCoef);
				float num2 = 0f;
				if (0 < attackerInfo.CachedStat.MaxHp)
				{
					num2 = Mathf.Clamp(attackerInfo.Attacker.Status.Hp / (float) attackerInfo.CachedStat.MaxHp, 0f, 1f);
				}

				num += (int) ((1f - num2) * DamageCasterLossHpCoef * 100f);
				num += (int) (attackerInfo.Attacker.Status.MoveSpeed * DamageCharacterSpeedCoef);
			}

			num += (int) (attackerInfo.CachedStat.MaxHp * DamageCasterMaxHpCoef);
			num += (int) (attackerInfo.CachedStat.MaxSp * DamageCasterMaxSpCoef);
			num += (int) (self.Stat.MaxHp * DamageTargetMaxHpCoef);
			num += (int) ((1f - self.Status.Hp / (float) self.Stat.MaxHp) * DamageTargetLossHpCoef * 100f);
			float num3 = CriticalDamageRate(self, attackerInfo, this.damageType);
			bool critical = 1f < num3;
			int damage = CalculateDamage(num, baseDamage, self.Stat.Defense, num3);
			damage = AmplificationDamage(attackerInfo, self, damage);
			float num4 = 0f;
			float num5 = 1f;
			float num6 = 1f;
			DamageType damageType = this.damageType;
			if (damageType - DamageType.Normal <= 1)
			{
				num4 = GetFlatIncreaseDamage(attackerInfo, self, this.damageType == DamageType.Skill);
				num5 = GetRatioIncreaseDamage(attackerInfo, self, this.damageType == DamageType.Skill);
				num6 = GetRatioModeDamage(attackerInfo, self);
			}

			damage = Math.Max(1, Mathf.FloorToInt((damage + num4) * num5 * num6 + FinalAddDamage));
			int undefendedDamage = CalculateDamage(num, baseDamage, 0, num3);
			undefendedDamage = AmplificationDamage(attackerInfo, self, undefendedDamage);
			undefendedDamage = Math.Max(1, Mathf.FloorToInt((undefendedDamage + num4) * num5 * num6 + FinalAddDamage));
			self.IfTypeOf<WorldPlayerCharacter>(delegate(WorldPlayerCharacter player)
			{
				if (player.IsDyingCondition && this.damageType != DamageType.RedZone)
				{
					int val = (int) (damage * (0.25f - self.Status.Level * 0.008f));
					damage = Math.Max(1, val);
					int val2 = (int) (undefendedDamage * (0.2f - self.Status.Level * 0.075f));
					undefendedDamage = Math.Max(1, val2);
				}
			});
			int maxDamageOnHp = 0;
			if (this.damageType != DamageType.Sp)
			{
				maxDamageOnHp = self.Status.Hp >= damage ? damage : self.Status.Hp;
			}

			DamageInfo damageInfo = DamageInfo.Create(damage, this.damageType, damageSubType, damageDataCode, damageId,
				minRemain);
			damageInfo.SetAttacker(attackerInfo.Attacker);
			damageInfo.SetCritical(critical);
			damageInfo.SetUndefendedDamage(undefendedDamage);
			damageInfo.SetMaxDamageOnHp(maxDamageOnHp);
			damageInfo.SetDamageMasteryModifier(damageMasteryModifier);
			return damageInfo;
		}

		
		private float GetFlatIncreaseDamage(AttackerInfo attackerInfo, WorldCharacter self, bool isSkillDamage)
		{
			float result;
			if (isSkillDamage)
			{
				result = attackerInfo.CachedStat.IncreaseSkillDamage - self.Stat.PreventSkillDamaged;
			}
			else
			{
				result = attackerInfo.CachedStat.IncreaseBasicAttackDamage - self.Stat.PreventBasicAttackDamaged;
			}

			return result;
		}

		
		private float GetRatioIncreaseDamage(AttackerInfo attackerInfo, WorldCharacter self, bool isSkillDamage)
		{
			float b;
			if (isSkillDamage)
			{
				b = 1f + (attackerInfo.CachedStat.IncreaseSkillDamageRatio - self.Stat.PreventSkillDamagedRatio);
			}
			else
			{
				b = 1f + (attackerInfo.CachedStat.IncreaseBasicAttackDamageRatio -
				          self.Stat.PreventBasicAttackDamagedRatio);
			}

			return Mathf.Max(0f, b);
		}
	}
}