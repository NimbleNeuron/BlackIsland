using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.HyejinPassive)]
	public class HyejinPassive : SkillScript
	{
		
		public const int HYEJIN_SKILL_DAMAGE_ID = 1;

		
		private int passiveSamjeaImmuneStateGroup;

		
		private int passiveStackStateGroup;

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
			if (passiveStackStateGroup == 0)
			{
				passiveStackStateGroup =
					GameDB.characterState.GetData(Singleton<HyejinSkillData>.inst.PassiveStackState).group;
				passiveSamjeaImmuneStateGroup = GameDB.characterState
					.GetData(Singleton<HyejinSkillData>.inst.PassiveSamjeaImmuneState).group;
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}

		
		private void OnAfterDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
		{
			if (!Caster.IsAlive)
			{
				return;
			}

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

			if (damageInfo.DamageType != DamageType.Skill)
			{
				return;
			}

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			if (damageInfo.DamageId != 1)
			{
				return;
			}

			if (victim.StateEffector.IsHaveStateByGroup(passiveSamjeaImmuneStateGroup, Caster.ObjectId))
			{
				return;
			}

			if (victim.StateEffector.GetStackByGroup(passiveStackStateGroup, Caster.ObjectId) + 1 >=
			    Singleton<HyejinSkillData>.inst.PassiveTriggerStack)
			{
				CharacterState characterState = CreateState(victim.SkillAgent,
					Singleton<HyejinSkillData>.inst.PassiveFearState[SkillLevel]);
				characterState.AddExternalStat(StatType.MoveSpeedRatio,
					Singleton<HyejinSkillData>.inst.PassiveFearMoveSpeedRatio, StatType.None, 0f);
				AddState(victim.SkillAgent, characterState);
				AddState(victim.SkillAgent, Singleton<HyejinSkillData>.inst.PassiveSamjeaImmuneState);
				victim.SkillAgent.RemoveStateByGroup(passiveStackStateGroup, Caster.ObjectId);
				return;
			}

			AddState(victim.SkillAgent, Singleton<HyejinSkillData>.inst.PassiveStackState);
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return null;
		}
	}
}