using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class DirectDamageCalculator : DamageCalculator
	{
		
		public DirectDamageCalculator(WeaponType weaponType, DamageType damageType, DamageSubType damageSubType,
			int damageDataCode, int damageId, int baseDamage, int minRemain, float damageMasteryModifier) : base(
			weaponType, damageType, damageSubType, damageDataCode, damageId, baseDamage, minRemain,
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
					num2 = Mathf.Clamp(attackerInfo.Attacker.Status.Hp / attackerInfo.CachedStat.MaxHp, 0f, 1f);
				}

				num += (int) ((1f - num2) * DamageCasterLossHpCoef * 100f);
			}

			num += (int) (self.Stat.MaxHp * DamageTargetMaxHpCoef);
			num += (int) ((1f - self.Status.Hp / (float) self.Stat.MaxHp) * DamageTargetLossHpCoef * 100f);
			int damage = CalculateDamage(num, baseDamage, 0, 1f);
			float num3 = 1f;
			DamageType damageType = this.damageType;
			if (damageType - DamageType.Normal <= 1)
			{
				num3 = GetRatioModeDamage(attackerInfo, self);
			}

			damage = Mathf.FloorToInt(damage * num3 + FinalAddDamage);
			self.IfTypeOf<WorldPlayerCharacter>(delegate(WorldPlayerCharacter player)
			{
				if (player.IsDyingCondition && this.damageType != DamageType.RedZone)
				{
					int val = (int) (damage * (0.2f - self.Status.Level * 0.075f));
					damage = Math.Max(1, val);
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
			damageInfo.SetCritical(false);
			damageInfo.SetUndefendedDamage(damage);
			damageInfo.SetMaxDamageOnHp(maxDamageOnHp);
			damageInfo.SetDamageMasteryModifier(damageMasteryModifier);
			return damageInfo;
		}
	}
}