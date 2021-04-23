using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HartPassive)]
	public class HartPassive : HartSkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Combine(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnFinishNormalAttack));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst2.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Remove(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnFinishNormalAttack));
			BattleEventCollector inst2 = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst2.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst2.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		private void OnFinishNormalAttack(WorldCharacter victim, WorldCharacter attacker)
		{
			if (!IsCorrectEvent(victim))
			{
				return;
			}

			if (Caster.ObjectId != attacker.ObjectId)
			{
				return;
			}

			if (Evolution(1))
			{
				StartCoroutine(CoroutineUtil.DelayedAction(0.1f, delegate
				{
					int projectileCode = IsEnchanted()
						? Singleton<HartSkillPassiveData>.inst.EnchantedEvolutionProjectileCode_1
						: Singleton<HartSkillPassiveData>.inst.EvolutionProjectileCode_1;
					LaunchProjectile(victim.ObjectId, projectileCode, 0,
						Singleton<HartSkillPassiveData>.inst.PassiveBonusAttackApCoef);
				}));
			}

			if (Evolution(2))
			{
				StartCoroutine(CoroutineUtil.DelayedAction(0.2f, delegate
				{
					int projectileCode = IsEnchanted()
						? Singleton<HartSkillPassiveData>.inst.EnchantedEvolutionProjectileCode_2
						: Singleton<HartSkillPassiveData>.inst.EvolutionProjectileCode_2;
					LaunchProjectile(victim.ObjectId, projectileCode, 0,
						Singleton<HartSkillPassiveData>.inst.PassiveBonusAttackApCoef);
				}));
			}
		}

		
		private void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!IsCorrectEvent(victim))
			{
				return;
			}

			if (damageInfo.Attacker == null)
			{
				return;
			}

			if (Caster.ObjectId != damageInfo.Attacker.ObjectId)
			{
				return;
			}

			if (damageInfo.DamageType != DamageType.Normal)
			{
				return;
			}

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			SpHealTo(Caster, 0, 0f,
				(int) (Caster.Stat.MaxSp * (Singleton<HartSkillPassiveData>.inst.RecoverySpRatio[SkillLevel] * 0.01f)),
				true, 1008109);
		}

		
		private bool IsCorrectEvent(WorldCharacter victim)
		{
			return Caster.ObjectId != victim.ObjectId;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}