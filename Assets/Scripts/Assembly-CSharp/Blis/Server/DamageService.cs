using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class DamageService : Singleton<DamageService>
	{
		
		public DamageInfo DamageTo(WorldCharacter victim, AttackerInfo attackerInfo, DamageCalculator damageCalculator, Vector3? damagePoint, SkillSlotSet skillSlotSet, bool isCheckAlly, int effectAndSoundCode, bool targetInCombat)
		{
			if (attackerInfo.Attacker != null && isCheckAlly && victim.GetHostileType(attackerInfo.Attacker) == HostileType.Ally)
			{
				return null;
			}
			if (damageCalculator.DamageType != DamageType.RedZone && damageCalculator.DamageType != DamageType.DyingCondition && victim.IsUntargetable())
			{
				return null;
			}
			if (damageCalculator.DamageType == DamageType.Normal && damageCalculator.DamageSubType == DamageSubType.Normal && victim.StateEffector.IsHaveStateByGroup(10007, 0))
			{
				victim.Evasion();
				return null;
			}
			SingletonMonoBehaviour<BattleEventCollector>.inst.BeforeDamageCaculator(victim, attackerInfo.Attacker, damageCalculator);
			DamageInfo damageInfo = damageCalculator.Calculate(victim, attackerInfo);
			damageInfo.SetDamagePoint(damagePoint);
			damageInfo.SetEffectAndSoundCode(effectAndSoundCode);
			damageInfo.SetSkillInfo(skillSlotSet);
			damageInfo.SetTargetInCombat(targetInCombat);
			SingletonMonoBehaviour<BattleEventCollector>.inst.BeforeDamageProcess(victim, damageInfo);
			this.DamageTo(victim, attackerInfo, damageInfo);
			return damageInfo;
		}

		
		public DamageInfo DamageTo(WorldCharacter victim, AttackerInfo attackerInfo, DamageCalculator damageCalculator, Vector3? damagePoint, int effectAndSoundCode, bool targetInCombat)
		{
			if (attackerInfo.Attacker != null && victim.GetHostileType(attackerInfo.Attacker) == HostileType.Ally)
			{
				return null;
			}
			if (damageCalculator.DamageType != DamageType.RedZone && damageCalculator.DamageType != DamageType.DyingCondition && victim.IsUntargetable())
			{
				return null;
			}
			if (damageCalculator.DamageType == DamageType.Normal && damageCalculator.DamageSubType == DamageSubType.Normal && victim.StateEffector.IsHaveStateByGroup(10007, 0))
			{
				victim.Evasion();
				return null;
			}
			DamageInfo damageInfo = damageCalculator.Calculate(victim, attackerInfo);
			damageInfo.SetDamagePoint(damagePoint);
			damageInfo.SetEffectAndSoundCode(effectAndSoundCode);
			damageInfo.SetTargetInCombat(targetInCombat);
			SingletonMonoBehaviour<BattleEventCollector>.inst.BeforeDamageProcess(victim, damageInfo);
			this.DamageTo(victim, attackerInfo, damageInfo);
			return damageInfo;
		}

		
		private void DamageTo(WorldCharacter victim, AttackerInfo attackerInfo, DamageInfo damageInfo)
		{
			victim.Damage(damageInfo);
			SingletonMonoBehaviour<BattleEventCollector>.inst.AfterDamageProcess(victim, damageInfo);
			this.LifeSteal(attackerInfo, damageInfo);
			this.DecreaseRecoveryAttack(attackerInfo, victim, damageInfo);
			this.AttackerProcess(attackerInfo, victim, damageInfo);
			this.VictimProcess(victim, attackerInfo, damageInfo);
		}

		
		private void LifeSteal(AttackerInfo attackerInfo, DamageInfo damageInfo)
		{
			if (attackerInfo.Attacker == null)
			{
				return;
			}
			DamageType damageType = damageInfo.DamageType;
			if (damageType != DamageType.Normal)
			{
				return;
			}
			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}
			float num = (float)damageInfo.Damage * attackerInfo.Attacker.Stat.LifeSteal;
			if (0f < num)
			{
				HealInfo healInfo = HealInfo.Create(Mathf.FloorToInt(num * (1f + attackerInfo.Attacker.Stat.IncreaseModeHealRatio)), 0);
				healInfo.SetHealer(attackerInfo.Attacker);
				healInfo.SetShowUI(false);
				attackerInfo.Attacker.Heal(healInfo);
			}
		}

		
		private void DecreaseRecoveryAttack(AttackerInfo attackerInfo, WorldCharacter victim, DamageInfo damageInfo)
		{
			if (attackerInfo.Attacker == null)
			{
				return;
			}
			DamageType damageType = damageInfo.DamageType;
			if (damageType != DamageType.Normal)
			{
				if (damageType != DamageType.Skill)
				{
					return;
				}
				if (attackerInfo.Attacker.Stat.DecreaseRecoveryToSkill)
				{
					victim.AddState(new CommonState(5001001, victim, attackerInfo.Attacker), attackerInfo.Attacker.ObjectId);
				}
			}
			else if (attackerInfo.Attacker.Stat.DecreaseRecoveryToBasicAttack)
			{
				victim.AddState(new CommonState(5001001, victim, attackerInfo.Attacker), attackerInfo.Attacker.ObjectId);
			}
		}

		
		private void AttackerProcess(AttackerInfo attackerInfo, WorldCharacter victim, DamageInfo damageInfo)
		{
			if (attackerInfo.Attacker == null)
			{
				return;
			}
			if (damageInfo.DamageType == DamageType.Sp)
			{
				return;
			}
			if (victim is WorldPlayerCharacter && (victim as WorldPlayerCharacter).IsDyingCondition)
			{
				return;
			}
			attackerInfo.Attacker.IfTypeOf<WorldPlayerCharacter>(delegate(WorldPlayerCharacter player)
			{
				DamageSubType damageSubType = damageInfo.DamageSubType;
				MasteryConditionType conditionType;
				MasteryType masteryType;
				if (damageSubType == DamageSubType.Trap)
				{
					conditionType = ((victim.ObjectType == ObjectType.PlayerCharacter || victim.ObjectType == ObjectType.BotPlayerCharacter) ? MasteryConditionType.TrapDamageToPlayer : MasteryConditionType.TrapDamageToMonster);
					masteryType = MasteryType.None;
				}
				else
				{
					conditionType = ((victim.ObjectType == ObjectType.PlayerCharacter || victim.ObjectType == ObjectType.BotPlayerCharacter) ? MasteryConditionType.AttackDamageToPlayer : MasteryConditionType.AttackDamageToMonster);
					masteryType = player.GetEquipWeaponMasteryType();
				}
				player.AddMasteryConditionExp(new UpdateMasteryInfo
				{
					conditionType = conditionType,
					takeMasteryValue = damageInfo.damageForMastery,
					masteryType = masteryType
				});
				if (victim.ObjectType == ObjectType.PlayerCharacter || victim.ObjectType == ObjectType.BotPlayerCharacter)
				{
					player.Status.AddDamageToPlayer(damageInfo);
					return;
				}
				player.Status.AddDamageToMonster(damageInfo.Damage);
			});
		}

		
		private void VictimProcess(WorldCharacter victim, AttackerInfo attackerInfo, DamageInfo damageInfo)
		{
			if (victim == null)
			{
				return;
			}
			if (damageInfo.DamageType == DamageType.Sp)
			{
				return;
			}
			bool attackerIsPlayer = attackerInfo.Attacker != null && (attackerInfo.Attacker.ObjectType == ObjectType.PlayerCharacter || attackerInfo.Attacker.ObjectType == ObjectType.BotPlayerCharacter);
			victim.IfTypeOf<WorldPlayerCharacter>(delegate(WorldPlayerCharacter player)
			{
				if (damageInfo.DamageSubType == DamageSubType.Trap)
				{
					player.AddMasteryConditionExp(new UpdateMasteryInfo
					{
						conditionType = MasteryConditionType.TrapDamaged,
						takeMasteryValue = damageInfo.Damage
					});
					return;
				}
				player.AddMasteryConditionExp(new UpdateMasteryInfo
				{
					conditionType = (attackerIsPlayer ? MasteryConditionType.UndefendedDamagedFromPlayer : MasteryConditionType.UndefendedDamagedFromMonster),
					takeMasteryValue = damageInfo.UndefendedDamage
				});
				player.AddMasteryConditionExp(new UpdateMasteryInfo
				{
					conditionType = MasteryConditionType.BeingDamaged,
					takeMasteryValue = damageInfo.Damage
				});
			});
		}

		
		public void EnvironmentDamageTo(WorldCharacter self, DamageCalculator damageCalculator, Vector3? damagePoint, int effectAndSoundCode)
		{
			DamageInfo damageInfo = damageCalculator.Calculate(self, AttackerInfo.Empty);
			damageInfo.SetDamagePoint(damagePoint);
			damageInfo.SetEffectAndSoundCode(effectAndSoundCode);
			SingletonMonoBehaviour<BattleEventCollector>.inst.BeforeDamageProcess(self, damageInfo);
			this.EnvironmentDamageTo(self, damageInfo);
		}

		
		private void EnvironmentDamageTo(WorldCharacter self, DamageInfo damageInfo)
		{
			self.Damage(damageInfo);
			SingletonMonoBehaviour<BattleEventCollector>.inst.AfterDamageProcess(self, damageInfo);
		}

		
		public void SelfDamageTo(WorldCharacter self, AttackerInfo attackerInfo, DamageCalculator damageCalculator, Vector3? damagePoint, int effectAndSoundCode)
		{
			DamageInfo damageInfo = damageCalculator.Calculate(self, attackerInfo);
			damageInfo.SetDamagePoint(damagePoint);
			damageInfo.SetEffectAndSoundCode(effectAndSoundCode);
			damageInfo.SetTargetInCombat(false);
			SingletonMonoBehaviour<BattleEventCollector>.inst.BeforeDamageProcess(self, damageInfo);
			this.SelfDamageTo(self, damageInfo);
		}

		
		private void SelfDamageTo(WorldCharacter self, DamageInfo damageInfo)
		{
			self.Damage(damageInfo);
			SingletonMonoBehaviour<BattleEventCollector>.inst.AfterDamageProcess(self, damageInfo);
		}
	}
}
