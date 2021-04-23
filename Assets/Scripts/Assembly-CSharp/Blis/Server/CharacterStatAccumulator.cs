using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class CharacterStatAccumulator
	{
		
		private const float minMoveSpeed = 0.3f;

		
		private const float maxMoveSpeed = 7f;

		
		private readonly float[] characterStat = new float[GameConstants.Stat.STAT_LEN];

		
		private readonly float[] environmentStat = new float[GameConstants.Stat.STAT_LEN];

		
		private readonly float[] equipmentStat = new float[GameConstants.Stat.STAT_LEN];

		
		private readonly float[] masteryStat = new float[GameConstants.Stat.STAT_LEN];

		
		private readonly float[] stateEffectorStat = new float[GameConstants.Stat.STAT_LEN];

		
		private float GetStat(StatType statTypes)
		{
			if (statTypes == StatType.None)
			{
				return 0f;
			}

			return characterStat[(int) statTypes] + equipmentStat[(int) statTypes] + masteryStat[(int) statTypes] +
			       stateEffectorStat[(int) statTypes] + environmentStat[(int) statTypes];
		}

		
		public void UpdateDummy()
		{
			Array.Clear(characterStat, 0, characterStat.Length);
			characterStat[1] = 0.4f;
			characterStat[2] = 99999f;
			characterStat[4] = 99999f;
		}

		
		public void UpdateCharacterStat(CharacterData characterData, int level)
		{
			int num = level - 1;
			CharacterLevelUpStatData levelUpStatData = GameDB.character.GetLevelUpStatData(characterData.code);
			Array.Clear(characterStat, 0, characterStat.Length);
			characterStat[1] = characterData.radius;
			characterStat[2] = characterData.maxHp + levelUpStatData.maxHp * num;
			characterStat[4] = characterData.maxSp + levelUpStatData.maxSp * num;
			characterStat[5] = characterData.initExtraPoint;
			characterStat[6] = characterData.maxExtraPoint;
			characterStat[7] = characterData.attackPower + levelUpStatData.attackPower * num;
			characterStat[8] = characterData.defense + levelUpStatData.defense * num;
			characterStat[11] = characterData.attackSpeed + levelUpStatData.attackSpeed * num;
			characterStat[9] = characterData.hpRegen + levelUpStatData.hpRegen * num;
			characterStat[10] = characterData.spRegen + levelUpStatData.spRegen * num;
			characterStat[12] = characterData.moveSpeed + levelUpStatData.moveSpeed * num;
			characterStat[13] = characterData.sightRange;
			characterStat[14] = 360f;
			characterStat[24] = 2.5f;
		}

		
		public void UpdateCharacterStat(MonsterData monsterData, int level)
		{
			MonsterLevelUpStatData monsterLevelUpStatData = GameDB.monster.GetMonsterLevelUpStatData(monsterData.code);
			int num = level - 1;
			Array.Clear(characterStat, 0, characterStat.Length);
			characterStat[1] = monsterData.radius;
			characterStat[2] = monsterData.maxHp + monsterLevelUpStatData.maxHp * num;
			characterStat[7] = monsterData.attackPower + monsterLevelUpStatData.attackPower * num;
			characterStat[8] = monsterData.defense + monsterLevelUpStatData.defense * num;
			characterStat[11] = monsterData.attackSpeed;
			characterStat[15] = monsterData.attackRange;
			characterStat[12] = monsterData.moveSpeed + monsterLevelUpStatData.moveSpeed * num;
			characterStat[13] = monsterData.sightRange;
			characterStat[14] = 360f;
			characterStat[24] = 2.5f;
		}

		
		public void UpdateCharacterStat(SummonData summonData)
		{
			Array.Clear(characterStat, 0, characterStat.Length);
			characterStat[1] = summonData.radius;
			characterStat[2] = summonData.maxHp;
			characterStat[7] = summonData.attackPower;
			characterStat[11] = summonData.attackSpeed;
			characterStat[15] = summonData.attackRange;
			characterStat[13] = summonData.sightRange;
			characterStat[14] = 360f;
			characterStat[24] = 2.5f;
		}

		
		public void UpdateEquipmentStat(ItemStat itemStat)
		{
			Array.Clear(equipmentStat, 0, equipmentStat.Length);
			float itemBonus = GetItemBonus();
			equipmentStat[7] = itemStat.attackPower * itemBonus;
			equipmentStat[8] = itemStat.defense * itemBonus;
			equipmentStat[2] = itemStat.maxHp * itemBonus;
			equipmentStat[4] = itemStat.maxSp * itemBonus;
			equipmentStat[9] = itemStat.hpRegen * itemBonus;
			equipmentStat[10] = itemStat.spRegen * itemBonus;
			equipmentStat[33] = itemStat.hpRegenRatio * itemBonus;
			equipmentStat[34] = itemStat.spRegenRatio * itemBonus;
			equipmentStat[11] = itemStat.attackSpeed * itemBonus + itemStat.weaponAttackSpeed;
			equipmentStat[15] = itemStat.attackRange * itemBonus + itemStat.weaponAttackRange;
			equipmentStat[35] = itemStat.attackSpeedRatio * itemBonus;
			equipmentStat[12] = itemStat.moveSpeed * itemBonus;
			equipmentStat[13] = itemStat.sightRange * itemBonus;
			equipmentStat[17] = itemStat.criticalStrikeChance * itemBonus;
			equipmentStat[18] = itemStat.criticalStrikeDamage * itemBonus;
			equipmentStat[19] = itemStat.preventCriticalStrikeDamaged * itemBonus;
			equipmentStat[20] = itemStat.cooldownReduction * itemBonus;
			equipmentStat[21] = itemStat.lifeSteal * itemBonus;
			equipmentStat[22] = itemStat.outOfCombatMoveSpeed * itemBonus;
			equipmentStat[25] = itemStat.increaseBasicAttackDamage * itemBonus;
			equipmentStat[26] = itemStat.preventBasicAttackDamaged * itemBonus;
			equipmentStat[27] = itemStat.increaseSkillDamage * itemBonus;
			equipmentStat[45] = itemStat.increaseSkillDamageRatio * itemBonus;
			equipmentStat[28] = itemStat.preventSkillDamaged * itemBonus;
			equipmentStat[46] = itemStat.preventSkillDamagedRatio * itemBonus;
			equipmentStat[47] = itemStat.decreaseRecoveryToBasicAttack;
			equipmentStat[48] = itemStat.decreaseRecoveryToSkill;
		}

		
		public void UpdateMasteryStat(Dictionary<StatType, float> values)
		{
			UpdateValues(masteryStat, values);
		}

		
		public void UpdateStateStat(Dictionary<StatType, float> values)
		{
			UpdateValues(stateEffectorStat, values);
		}

		
		public void UpdateEnvironmentStat(Dictionary<StatType, float> values)
		{
			UpdateValues(environmentStat, values);
		}

		
		private void UpdateValues(float[] values, Dictionary<StatType, float> updateValues)
		{
			Array.Clear(values, 0, values.Length);
			foreach (KeyValuePair<StatType, float> keyValuePair in updateValues)
			{
				values[(int) keyValuePair.Key] = keyValuePair.Value;
			}
		}

		
		public float GetRadius()
		{
			return GetStat(StatType.Radius);
		}

		
		public int GetMaxHp()
		{
			return (int) (GetStat(StatType.MaxHp) * (1f + GetStat(StatType.MaxHpRatio)) + GetStat(StatType.MaxHpBonus));
		}

		
		public int GetMaxSp()
		{
			return (int) (GetStat(StatType.MaxSp) * (1f + GetStat(StatType.MaxSpRatio)));
		}

		
		public int GetInitExtraPoint()
		{
			return (int) GetStat(StatType.InitExtraPoint);
		}

		
		public int GetMaxExtraPoint()
		{
			return (int) GetStat(StatType.MaxExtraPoint);
		}

		
		public int GetAttackPower()
		{
			return (int) (GetStat(StatType.AttackPower) * (1f + GetStat(StatType.AttackPowerRatio)));
		}

		
		public int GetDefense()
		{
			return (int) (GetStat(StatType.Defense) * (1f + GetStat(StatType.DefenseRatio)));
		}

		
		public int GetCriticalStrikeChance()
		{
			float num = GetStat(StatType.CriticalStrikeChance) * 100f;
			return Math.Min(100, (int) num);
		}

		
		public float GetCriticalStrikeDamage()
		{
			return GetStat(StatType.CriticalStrikeDamage);
		}

		
		public float GetPreventCriticalStrikeDamaged()
		{
			return GetStat(StatType.PreventCriticalStrikeDamaged);
		}

		
		public float GetHpRegen()
		{
			return GetStat(StatType.HpRegen) * (1f + GetStat(StatType.HpRegenRatio)) *
			       (1f + GetStat(StatType.HpHealRatio));
		}

		
		public float GetSpRegen()
		{
			return GetStat(StatType.SpRegen) * (1f + GetStat(StatType.SpRegenRatio));
		}

		
		public float GetAttackSpeed()
		{
			return Math.Min(GetAttackSpeedLimit(),
				GetStat(StatType.AttackSpeed) * (1f + GetStat(StatType.AttackSpeedRatio)));
		}

		
		private float GetBaseMoveSpeed(out float calMinMoveSpeed, out float calMaxMoveSpeed)
		{
			float stat = GetStat(StatType.MoveSpeedRatioLimitIgnore);
			float result = GetStat(StatType.MoveSpeed) * (1f + GetStat(StatType.MoveSpeedRatio) + stat);
			calMinMoveSpeed = 0.3f;
			if (stat == 0f)
			{
				calMaxMoveSpeed = 7f;
				return result;
			}

			float num = 7f * (1f + stat);
			calMaxMoveSpeed = num;
			return result;
		}

		
		public float GetMoveSpeed()
		{
			float num2;
			float num3;
			float num = GetBaseMoveSpeed(out num2, out num3);
			if (num > num3)
			{
				num = num3;
			}
			else if (num < num2)
			{
				num = num2;
			}

			return num;
		}

		
		public float GetMoveSpeedOutOfCombat()
		{
			float num2;
			float num3;
			float num = GetBaseMoveSpeed(out num2, out num3);
			num += GetStat(StatType.MoveSpeedOutOfCombat) * (1f + GetStat(StatType.MoveSpeedOutOfCombatRatio));
			if (num > num3)
			{
				num = num3;
			}
			else if (num < num2)
			{
				num = num2;
			}

			return num;
		}

		
		public float GetSightRange()
		{
			return Math.Max(1f, GetStat(StatType.SightRange) * (1f + GetStat(StatType.SightRangeRatio)));
		}

		
		public int GetSightAngle()
		{
			return (int) GetStat(StatType.SightAngle);
		}

		
		public float GetAttackRange()
		{
			float stat = GetStat(StatType.FixedAttackRange);
			if (stat > 0f)
			{
				return stat;
			}

			return GetStat(StatType.Radius) + GetStat(StatType.AttackRange);
		}

		
		public float GetCooldownReduceRate()
		{
			return Mathf.Min(0.4f, GetStat(StatType.CooldownReduction));
		}

		
		public float GetLiftSteal()
		{
			return GetStat(StatType.LifeSteal);
		}

		
		public float GetAmplifierToMonster()
		{
			return GetStat(StatType.AmplifierToMonsterRatio);
		}

		
		public float GetItemBonus()
		{
			return 1f + GetStat(StatType.ItemBonusRatio);
		}

		
		public float GetAttackSpeedLimit()
		{
			return GetStat(StatType.AttackSpeedLimit);
		}

		
		public float GetTrapDamageRatio()
		{
			return GetStat(StatType.TrapDamageRatio);
		}

		
		public float GetIncreaseBasicAttackDamage()
		{
			return GetStat(StatType.IncreaseBasicAttackDamage);
		}

		
		public float GetIncreaseBasicAttackDamageRatio()
		{
			return GetStat(StatType.IncreaseBasicAttackDamageRatio);
		}

		
		public float GetPreventBasicAttackDamaged()
		{
			return GetStat(StatType.PreventBasicAttackDamaged);
		}

		
		public float GetPreventBasicAttackDamagedRatio()
		{
			return GetStat(StatType.PreventBasicAttackDamagedRatio);
		}

		
		public float GetIncreaseSkillDamage()
		{
			return GetStat(StatType.IncreaseSkillDamage);
		}

		
		public float GetIncreaseSkillDamageRatio()
		{
			return GetStat(StatType.IncreaseSkillDamageRatio);
		}

		
		public float GetPreventSkillDamaged()
		{
			return GetStat(StatType.PreventSkillDamaged);
		}

		
		public float GetPreventSkillDamagedRatio()
		{
			return GetStat(StatType.PreventSkillDamagedRatio);
		}

		
		public float GetInstallTrapCastingTimeReduce()
		{
			return GetStat(StatType.InstallTrapCastingTimeReduce);
		}

		
		public float GetDecreaseRecovery(bool bySkillAttack)
		{
			if (!bySkillAttack)
			{
				return GetStat(StatType.DecreaseRecoveryToBasicAttack);
			}

			return GetStat(StatType.DecreaseRecoveryToSkill);
		}

		
		public float GetHpHealRatio()
		{
			return GetStat(StatType.HpHealRatio);
		}

		
		public float GetIncreaseModeDamageRatio()
		{
			return GetStat(StatType.IncreaseModeDamageRatio);
		}

		
		public float GetPreventModeDamageRatio()
		{
			return GetStat(StatType.PreventModeDamageRatio);
		}

		
		public float GetIncreaseModeHealRatio()
		{
			return GetStat(StatType.IncreaseModeHealRatio);
		}

		
		public float GetIncreaseModeShieldRatio()
		{
			return GetStat(StatType.IncreaseModeShieldRatio);
		}
	}
}