using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class CharacterStat : CharacterStatBase
	{
		
		public CharacterStat()
		{
			this.updates = new Dictionary<StatType, CharacterStatValue>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);
		}

		
		public void InitDummyStat()
		{
			this.accumulator.UpdateDummy();
			this.CheckUpdateAccumulatedValues();
		}

		
		public void UpdateCharacterStat(CharacterData characterData, int level)
		{
			this.accumulator.UpdateCharacterStat(characterData, level);
			this.CheckUpdateAccumulatedValues();
		}

		
		public void UpdateCharacterStat(MonsterData monsterData, int level)
		{
			this.accumulator.UpdateCharacterStat(monsterData, level);
			this.CheckUpdateAccumulatedValues();
		}

		
		public void UpdateCharacterStat(SummonData summonData)
		{
			this.accumulator.UpdateCharacterStat(summonData);
			this.CheckUpdateAccumulatedValues();
		}

		
		public void UpdateEquipmentStat(ItemStat itemStat)
		{
			this.accumulator.UpdateEquipmentStat(itemStat);
			this.CheckUpdateAccumulatedValues();
		}

		
		public void UpdateMasteryStat(Dictionary<StatType, float> values)
		{
			this.accumulator.UpdateMasteryStat(values);
			this.CheckUpdateAccumulatedValues();
		}

		
		public void UpdateAddMaxHp(int addMaxHp)
		{
			Dictionary<StatType, float> dictionary = new Dictionary<StatType, float>();
			dictionary.Add(StatType.MaxHp, (float)(base.MaxHp + addMaxHp));
			this.accumulator.UpdateStateStat(dictionary);
		}

		
		public void UpdateStateStat(Dictionary<StatType, float> values)
		{
			this.accumulator.UpdateStateStat(values);
			this.CheckUpdateAccumulatedValues();
		}

		
		public void UpdateEnvironmentStat(Dictionary<StatType, float> values)
		{
			this.accumulator.UpdateEnvironmentStat(values);
			this.CheckUpdateAccumulatedValues();
		}

		
		private bool IsDifferent(float src, float dest)
		{
			src = ServerCharacterStatValue.GetBiasedValue(src);
			dest = ServerCharacterStatValue.GetBiasedValue(dest);
			return Mathf.Abs(dest - src) > float.Epsilon;
		}

		
		private void UpdateValue(StatType statType, float value)
		{
			float value2 = this.GetValue(statType);
			if (this.IsDifferent(value2, value))
			{
				this.SetValue(statType, value);
				if (this.updates.ContainsKey(statType))
				{
					this.updates.Remove(statType);
				}
				CharacterStatValue statValue = this.GetStatValue(statType);
				this.updates.Add(statType, statValue);
				if (statType.IsRequiredByClient() && !this.snapshot.Contains(statValue))
				{
					this.snapshot.Add(statValue);
				}
			}
		}

		
		private void CheckUpdateAccumulatedValues()
		{
			foreach (object obj in CharacterStatBase.StatTypes)
			{
				StatType statType = (StatType)obj;
				float value = 0f;
				if (this.GetAccumulatedValue(statType, out value))
				{
					this.UpdateValue(statType, value);
				}
			}
		}

		
		public List<CharacterStatValue> FlushUpdates()
		{
			this.ret.Clear();
			foreach (KeyValuePair<StatType, CharacterStatValue> keyValuePair in this.updates)
			{
				if (keyValuePair.Key == StatType.MaxHp)
				{
					Action<int> changeMaxHpEvent = this.ChangeMaxHpEvent;
					if (changeMaxHpEvent != null)
					{
						changeMaxHpEvent(base.MaxHp);
					}
				}
				if (keyValuePair.Key.IsRequiredByClient())
				{
					this.ret.Add(keyValuePair.Value);
				}
			}
			this.updates.Clear();
			return this.ret;
		}

		
		public List<CharacterStatValue> CreateSnapshot()
		{
			if (!this.isSnapshoted)
			{
				this.isSnapshoted = true;
				this.FlushUpdates();
			}
			return this.snapshot;
		}

		
		private bool GetAccumulatedValue(StatType statType, out float value)
		{
			bool result = true;
			switch (statType)
			{
			case StatType.Radius:
				value = this.accumulator.GetRadius();
				return result;
			case StatType.MaxHp:
				value = (float)this.accumulator.GetMaxHp();
				return result;
			case StatType.MaxSp:
				value = (float)this.accumulator.GetMaxSp();
				return result;
			case StatType.InitExtraPoint:
				value = (float)this.accumulator.GetInitExtraPoint();
				return result;
			case StatType.MaxExtraPoint:
				value = (float)this.accumulator.GetMaxExtraPoint();
				return result;
			case StatType.AttackPower:
				value = (float)this.accumulator.GetAttackPower();
				return result;
			case StatType.Defense:
				value = (float)this.accumulator.GetDefense();
				return result;
			case StatType.HpRegen:
				value = this.accumulator.GetHpRegen();
				return result;
			case StatType.SpRegen:
				value = this.accumulator.GetSpRegen();
				return result;
			case StatType.AttackSpeed:
				value = this.accumulator.GetAttackSpeed();
				return result;
			case StatType.MoveSpeed:
				value = this.accumulator.GetMoveSpeed();
				return result;
			case StatType.SightRange:
				value = this.accumulator.GetSightRange();
				return result;
			case StatType.SightAngle:
				value = (float)this.accumulator.GetSightAngle();
				return result;
			case StatType.AttackRange:
				value = this.accumulator.GetAttackRange();
				return result;
			case StatType.CriticalStrikeChance:
				value = (float)this.accumulator.GetCriticalStrikeChance();
				return result;
			case StatType.CriticalStrikeDamage:
				value = this.accumulator.GetCriticalStrikeDamage();
				return result;
			case StatType.PreventCriticalStrikeDamaged:
				value = this.accumulator.GetPreventCriticalStrikeDamaged();
				return result;
			case StatType.CooldownReduction:
				value = this.accumulator.GetCooldownReduceRate();
				return result;
			case StatType.LifeSteal:
				value = this.accumulator.GetLiftSteal();
				return result;
			case StatType.MoveSpeedOutOfCombat:
				value = this.accumulator.GetMoveSpeedOutOfCombat();
				return result;
			case StatType.ItemBonus:
				value = this.accumulator.GetItemBonus();
				return result;
			case StatType.AttackSpeedLimit:
				value = this.accumulator.GetAttackSpeedLimit();
				return result;
			case StatType.IncreaseBasicAttackDamage:
				value = this.accumulator.GetIncreaseBasicAttackDamage();
				return result;
			case StatType.PreventBasicAttackDamaged:
				value = this.accumulator.GetPreventBasicAttackDamaged();
				return result;
			case StatType.IncreaseSkillDamage:
				value = this.accumulator.GetIncreaseSkillDamage();
				return result;
			case StatType.PreventSkillDamaged:
				value = this.accumulator.GetPreventSkillDamaged();
				return result;
			case StatType.TrapDamageRatio:
				value = this.accumulator.GetTrapDamageRatio();
				return result;
			case StatType.AmplifierToMonsterRatio:
				value = this.accumulator.GetAmplifierToMonster();
				return result;
			case StatType.IncreaseBasicAttackDamageRatio:
				value = this.accumulator.GetIncreaseBasicAttackDamageRatio();
				return result;
			case StatType.PreventBasicAttackDamagedRatio:
				value = this.accumulator.GetPreventBasicAttackDamagedRatio();
				return result;
			case StatType.IncreaseSkillDamageRatio:
				value = this.accumulator.GetIncreaseSkillDamageRatio();
				return result;
			case StatType.PreventSkillDamagedRatio:
				value = this.accumulator.GetPreventSkillDamagedRatio();
				return result;
			case StatType.DecreaseRecoveryToBasicAttack:
				value = this.accumulator.GetDecreaseRecovery(false);
				return result;
			case StatType.DecreaseRecoveryToSkill:
				value = this.accumulator.GetDecreaseRecovery(true);
				return result;
			case StatType.HpHealRatio:
				value = this.accumulator.GetHpHealRatio();
				return result;
			case StatType.InstallTrapCastingTimeReduce:
				value = this.accumulator.GetInstallTrapCastingTimeReduce();
				return result;
			case StatType.IncreaseModeDamageRatio:
				value = this.accumulator.GetIncreaseModeDamageRatio();
				return result;
			case StatType.PreventModeDamageRatio:
				value = this.accumulator.GetPreventModeDamageRatio();
				return result;
			case StatType.IncreaseModeHealRatio:
				value = this.accumulator.GetIncreaseModeHealRatio();
				return result;
			case StatType.IncreaseModeShieldRatio:
				value = this.accumulator.GetIncreaseModeShieldRatio();
				return result;
			}
			value = 0f;
			result = false;
			return result;
		}

		
		protected override void CheckNull(StatType statType)
		{
			if (!this.values.ContainsKey(statType))
			{
				this.values.Add(statType, new ServerCharacterStatValue(statType, 0));
			}
		}

		
		public override float GetValue(StatType statType)
		{
			this.CheckNull(statType);
			return this.values[statType].GetValue();
		}

		
		public override float GetValue(StatType statType, bool shuffle)
		{
			return this.GetValue(statType);
		}

		
		protected override int GetIntValue(StatType statType)
		{
			this.CheckNull(statType);
			return this.values[statType].GetIntValue();
		}

		
		protected override int GetIntValue(StatType statType, bool shuffle)
		{
			return this.GetIntValue(statType);
		}

		
		protected override void SetValue(StatType statType, float value)
		{
			this.CheckNull(statType);
			this.values[statType].Set(value);
		}

		
		protected override void SetIntValue(StatType statType, int value)
		{
			this.CheckNull(statType);
			this.values[statType].Set(value);
		}

		
		public override CharacterStatValue GetStatValue(StatType statType)
		{
			this.CheckNull(statType);
			return this.values[statType];
		}

		
		public void CopyValues(ref Dictionary<StatType, ServerCharacterStatValue> otherValues)
		{
			otherValues.Clear();
			foreach (KeyValuePair<StatType, ServerCharacterStatValue> keyValuePair in this.values)
			{
				otherValues.Add(keyValuePair.Key, new ServerCharacterStatValue(keyValuePair.Key, keyValuePair.Value.InternalValue));
			}
		}

		
		private readonly CharacterStatAccumulator accumulator = new CharacterStatAccumulator();

		
		private Dictionary<StatType, CharacterStatValue> updates;

		
		private readonly List<CharacterStatValue> snapshot = new List<CharacterStatValue>();

		
		private bool isSnapshoted;

		
		private readonly Dictionary<StatType, ServerCharacterStatValue> values = new Dictionary<StatType, ServerCharacterStatValue>(SingletonComparerEnum<StatTypeComparer, StatType>.Instance);

		
		private readonly List<CharacterStatValue> ret = new List<CharacterStatValue>();
	}
}
