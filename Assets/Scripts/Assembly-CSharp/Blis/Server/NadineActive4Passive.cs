using System;
using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.NadineActive4Passive)]
	public class NadineActive4Passive : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Combine(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnPlayNormalAttack));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Remove(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnPlayNormalAttack));
		}

		
		private void OnPlayNormalAttack(WorldCharacter victim, WorldCharacter attacker)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			if (Caster.ObjectId == victim.ObjectId)
			{
				return;
			}

			if (attacker == null)
			{
				return;
			}

			if (Caster.ObjectId != attacker.ObjectId)
			{
				return;
			}

			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive4Data>.inst.BuffState1[SkillLevel]);
			if (!Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				return;
			}

			CharacterStateData data2 =
				GameDB.characterState.GetData(Singleton<NadineSkillActive4Data>.inst.BuffState2[SkillLevel]);
			CharacterState characterState = Caster.FindStateByGroup(data2.group, Caster.ObjectId);
			if (characterState != null && data2.maxStack <= characterState.StackCount)
			{
				Caster.RemoveStateByGroup(data2.group, Caster.ObjectId);
				PlaySkillAction(Caster, info.skillData.PassiveSkillId, 1);
				StartCoroutine(LaunchWolf(victim));
				return;
			}

			AddState(Caster, data2.code);
		}

		
		private IEnumerator LaunchWolf(WorldCharacter victim)
		{
			yield return WaitForSeconds(0.4f);
			PlaySkillAction(Caster, info.skillData.PassiveSkillId, 2);
			ProjectileProperty projectile =
				PopProjectileProperty(Caster, Singleton<NadineSkillActive4Data>.inst.ProjectileCode);
			projectile.SetTargetObject(victim.ObjectId);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<NadineSkillActive4Data>.inst.PassiveBuffState);
			int stackByGroup = Caster.GetStackByGroup(data.group, Caster.ObjectId);
			int damage = Singleton<NadineSkillActive4Data>.inst.DamageByLevel[SkillLevel];
			damage += stackByGroup * Singleton<NadineSkillActive4Data>.inst.DamagePerPassiveStackCount;
			projectile.SetActionOnCollisionCharacter(
				delegate(SkillAgent targetAgent, AttackerInfo attackerInfo, Vector3 damagePoint,
					Vector3 damageDirection)
				{
					parameterCollection.Clear();
					parameterCollection.Add(SkillScriptParameterType.Damage, damage);
					parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
						Singleton<NadineSkillActive4Data>.inst.DamageAp);
					DamageTo(targetAgent, attackerInfo, projectile.ProjectileData.damageType,
						projectile.ProjectileData.damageSubType, 0, parameterCollection,
						projectile.SkillUseInfo.skillSlotSet, damagePoint, damageDirection, 0);
					AddState(targetAgent, Singleton<NadineSkillActive4Data>.inst.DebuffState);
				});
			LaunchProjectile(projectile);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}
	}
}