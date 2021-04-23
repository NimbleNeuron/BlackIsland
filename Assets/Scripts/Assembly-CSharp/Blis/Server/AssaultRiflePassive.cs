using System;
using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AssaultRiflePassive)]
	public class AssaultRiflePassive : SkillScript
	{
		
		private float lastNormalAttackTime;

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Start()
		{
			base.Start();
			lastNormalAttackTime = 0f;
			StartCoroutine(AttackIdle());
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		private IEnumerator AttackIdle()
		{
			int stateGroup = Singleton<AssaultRifleSkillActiveData>.inst.AssaultPassiveBuffGroup;
			for (;;)
			{
				float seconds = 0f;
				if (Singleton<AssaultRifleSkillActiveData>.inst.OverHeatCheckTime <
				    MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastNormalAttackTime &&
				    Caster.IsHaveStateByGroup(stateGroup, Caster.ObjectId))
				{
					Caster.ModifyStateValue(stateGroup, Caster.ObjectId, 0f,
						-Singleton<AssaultRifleSkillActiveData>.inst.OverHeatRemoveStack, false);
					seconds = Singleton<AssaultRifleSkillActiveData>.inst.OverHeatRemoveTime;
				}

				yield return WaitForSeconds(seconds);
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<AssaultRifleSkillActiveData>.inst.BuffState[SkillLevel]);
			Caster.RemoveStateByGroup(data.group, Caster.ObjectId);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterNormalDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnAfterNormalDamageProcess, new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		private void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (Caster.ObjectId == victim.ObjectId)
			{
				return;
			}

			if (damageInfo == null || damageInfo.Attacker == null)
			{
				return;
			}

			if (Caster.ObjectId != damageInfo.Attacker.ObjectId)
			{
				return;
			}

			CharacterStateData data =
				GameDB.characterState.GetData(Singleton<AssaultRifleSkillActiveData>.inst.BuffState[SkillLevel]);
			if (Caster.IsHaveStateByGroup(data.group, Caster.ObjectId))
			{
				return;
			}

			int assaultPassiveBuffGroup = Singleton<AssaultRifleSkillActiveData>.inst.AssaultPassiveBuffGroup;
			if (Caster.IsHaveStateByGroup(assaultPassiveBuffGroup, Caster.ObjectId))
			{
				Caster.ModifyStateValue(assaultPassiveBuffGroup, Caster.ObjectId, 0f,
					Singleton<AssaultRifleSkillActiveData>.inst.OverHeatAddStack, true);
			}
			else
			{
				AddState(Caster, Singleton<AssaultRifleSkillActiveData>.inst.AssaultPassiveBuff);
			}

			lastNormalAttackTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}
	}
}