using System;

namespace Blis.Common
{
	public static class StatTypeEx
	{
		public static bool IsSightRange(this StatType statType)
		{
			return statType == StatType.SightRange || statType == StatType.SightRangeRatio;
		}


		public static bool IsAttackRangeRange(this StatType statType)
		{
			return statType == StatType.AttackRange || statType == StatType.FixedAttackRange ||
			       statType == StatType.Radius;
		}


		public static bool IsRatio(this StatType statType)
		{
			switch (statType)
			{
				case StatType.None:
				case StatType.Radius:
				case StatType.MaxHp:
				case StatType.MaxHpBonus:
				case StatType.MaxSp:
				case StatType.AttackPower:
				case StatType.Defense:
				case StatType.HpRegen:
				case StatType.SpRegen:
				case StatType.AttackSpeed:
				case StatType.MoveSpeed:
				case StatType.SightRange:
				case StatType.SightAngle:
				case StatType.AttackRange:
				case StatType.FixedAttackRange:
				case StatType.MoveSpeedOutOfCombat:
				case StatType.ItemBonus:
				case StatType.AttackSpeedLimit:
				case StatType.IncreaseBasicAttackDamage:
				case StatType.PreventBasicAttackDamaged:
				case StatType.IncreaseSkillDamage:
				case StatType.PreventSkillDamaged:
				case StatType.InstallTrapCastingTimeReduce:
					return false;
				case StatType.CriticalStrikeChance:
				case StatType.CriticalStrikeDamage:
				case StatType.PreventCriticalStrikeDamaged:
				case StatType.CooldownReduction:
				case StatType.LifeSteal:
				case StatType.AttackPowerRatio:
				case StatType.DefenseRatio:
				case StatType.MaxHpRatio:
				case StatType.MaxSpRatio:
				case StatType.HpRegenRatio:
				case StatType.SpRegenRatio:
				case StatType.AttackSpeedRatio:
				case StatType.MoveSpeedRatio:
				case StatType.MoveSpeedRatioLimitIgnore:
				case StatType.TrapDamageRatio:
				case StatType.SightRangeRatio:
				case StatType.AmplifierToMonsterRatio:
				case StatType.ItemBonusRatio:
				case StatType.MoveSpeedOutOfCombatRatio:
				case StatType.IncreaseBasicAttackDamageRatio:
				case StatType.PreventBasicAttackDamagedRatio:
				case StatType.IncreaseSkillDamageRatio:
				case StatType.PreventSkillDamagedRatio:
				case StatType.DecreaseRecoveryToBasicAttack:
				case StatType.DecreaseRecoveryToSkill:
				case StatType.HpHealRatio:
				case StatType.IncreaseModeDamageRatio:
				case StatType.PreventModeDamageRatio:
				case StatType.IncreaseModeHealRatio:
				case StatType.IncreaseModeShieldRatio:
				case StatType.ReduceAirborneDurationRatio:
				case StatType.ReduceSlowDurationRatio:
				case StatType.ReduceFetterDurationRatio:
				case StatType.ReduceStunDurationRatio:
				case StatType.ReduceSuppressedDurationRatio:
					return true;
			}

			throw new ArgumentOutOfRangeException("statType", statType, null);
		}


		public static bool IsRequiredByClient(this StatType statType)
		{
			if (statType != StatType.None)
			{
				switch (statType)
				{
					case StatType.ItemBonus:
					case StatType.AttackSpeedLimit:
					case StatType.PreventSkillDamaged:
					case StatType.AttackPowerRatio:
					case StatType.DefenseRatio:
					case StatType.MaxHpRatio:
					case StatType.MaxSpRatio:
					case StatType.HpRegenRatio:
					case StatType.SpRegenRatio:
					case StatType.AttackSpeedRatio:
					case StatType.MoveSpeedRatio:
					case StatType.MoveSpeedRatioLimitIgnore:
					case StatType.SightRangeRatio:
					case StatType.AmplifierToMonsterRatio:
					case StatType.ItemBonusRatio:
					case StatType.MoveSpeedOutOfCombatRatio:
					case StatType.DecreaseRecoveryToBasicAttack:
					case StatType.DecreaseRecoveryToSkill:
					case StatType.HpHealRatio:
					case StatType.InstallTrapCastingTimeReduce:
						return false;
				}

				return true;
			}

			return false;
		}


		public static bool IsBroadcastType(this StatType statType)
		{
			switch (statType)
			{
				case StatType.Radius:
				case StatType.MaxHp:
				case StatType.MaxSp:
				case StatType.AttackPower:
				case StatType.Defense:
				case StatType.AttackSpeed:
				case StatType.MoveSpeed:
				case StatType.SightRange:
				case StatType.SightAngle:
				case StatType.CriticalStrikeChance:
				case StatType.CooldownReduction:
					return true;
			}

			return false;
		}


		public static bool IsReduceCrowdControlDurationType(this StatType statType, StateType targetType)
		{
			switch (statType)
			{
				case StatType.ReduceAirborneDurationRatio:
					return targetType == StateType.Airborne;
				case StatType.ReduceSlowDurationRatio:
					return targetType == StateType.Slow;
				case StatType.ReduceFetterDurationRatio:
					return targetType == StateType.Fetter;
				case StatType.ReduceStunDurationRatio:
					return targetType == StateType.Stun;
				case StatType.ReduceSuppressedDurationRatio:
					return targetType == StateType.Suppressed;
				default:
					return false;
			}
		}
	}
}