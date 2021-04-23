using System;
using System.Collections.Generic;

namespace Blis.Common
{
	public abstract class CharacterStatBase
	{
		public static readonly Array StatTypes = Enum.GetValues(typeof(StatType));


		public Action<int> ChangeMaxHpEvent;


		public virtual float SightRange => GetValue(StatType.SightRange);


		public virtual float SightRangeRatio => GetValue(StatType.SightRangeRatio);


		public virtual int SightAngle => GetIntValue(StatType.SightAngle);


		public virtual float AttackRange => GetValue(StatType.AttackRange);


		public int MaxHp => GetIntValue(StatType.MaxHp, false);


		public int MaxSp => GetIntValue(StatType.MaxSp, false);


		public int InitExtraPoint => GetIntValue(StatType.InitExtraPoint, false);


		public int MaxExtraPoint => GetIntValue(StatType.MaxExtraPoint, false);


		public float Radius => GetValue(StatType.Radius);


		public int AttackPower => GetIntValue(StatType.AttackPower, false);


		public int Defense => GetIntValue(StatType.Defense, false);


		public float HpRegen => GetValue(StatType.HpRegen, false);


		public float SpRegen => GetValue(StatType.SpRegen, false);


		public float AttackSpeed => GetValue(StatType.AttackSpeed, false);


		public float AttackDelay => 1f / AttackSpeed;


		public float MoveSpeed => GetValue(StatType.MoveSpeed, false);


		public int CriticalStrikeChance => GetIntValue(StatType.CriticalStrikeChance, false);


		public float CriticalStrikeDamage => GetValue(StatType.CriticalStrikeDamage, false);


		public float PreventCriticalStrikeDamaged => GetValue(StatType.PreventCriticalStrikeDamaged, false);


		public float CooldownReduction => GetValue(StatType.CooldownReduction, false);


		public float LifeSteal => GetValue(StatType.LifeSteal, false);


		public float AmplifierToMonsterRatio => GetValue(StatType.AmplifierToMonsterRatio, false);


		public float MoveSpeedOutOfCombat => GetValue(StatType.MoveSpeedOutOfCombat, false);


		public float TrapDamageRatio => GetValue(StatType.TrapDamageRatio, false);


		public float IncreaseBasicAttackDamage => GetValue(StatType.IncreaseBasicAttackDamage, false);


		public float PreventBasicAttackDamaged => GetValue(StatType.PreventBasicAttackDamaged, false);


		public float IncreaseBasicAttackDamageRatio => GetValue(StatType.IncreaseBasicAttackDamageRatio, false);


		public float PreventBasicAttackDamagedRatio => GetValue(StatType.PreventBasicAttackDamagedRatio, false);


		public float IncreaseSkillDamage => GetValue(StatType.IncreaseSkillDamage, false);


		public float PreventSkillDamaged => GetValue(StatType.PreventSkillDamaged, false);


		public float IncreaseSkillDamageRatio => GetValue(StatType.IncreaseSkillDamageRatio, false);


		public float PreventSkillDamagedRatio => GetValue(StatType.PreventSkillDamagedRatio, false);


		public bool DecreaseRecoveryToBasicAttack => GetValue(StatType.DecreaseRecoveryToBasicAttack, false) >= 1f;


		public bool DecreaseRecoveryToSkill => GetValue(StatType.DecreaseRecoveryToSkill, false) >= 1f;


		public float HpHealRatio => GetValue(StatType.HpHealRatio, false);


		public float InstallTrapCastingTimeReduce => GetValue(StatType.InstallTrapCastingTimeReduce, false);


		public float IncreaseModeDamageRatio => GetValue(StatType.IncreaseModeDamageRatio, false);


		public float PreventModeDamageRatio => GetValue(StatType.PreventModeDamageRatio, false);


		public float IncreaseModeHealRatio => GetValue(StatType.IncreaseModeHealRatio, false);


		public float IncreaseModeShieldRatio => GetValue(StatType.IncreaseModeShieldRatio, false);

		protected abstract void CheckNull(StatType statType);


		public abstract float GetValue(StatType statType);


		public abstract float GetValue(StatType statType, bool shuffle);


		protected abstract int GetIntValue(StatType statType);


		protected abstract int GetIntValue(StatType statType, bool shuffle);


		protected abstract void SetValue(StatType statType, float value);


		protected abstract void SetIntValue(StatType statType, int value);


		public abstract CharacterStatValue GetStatValue(StatType statType);


		public void Update(List<CharacterStatValue> updates)
		{
			foreach (CharacterStatValue characterStatValue in updates)
			{
				SetValue(characterStatValue.statType, characterStatValue.GetValue());
			}
		}
	}
}