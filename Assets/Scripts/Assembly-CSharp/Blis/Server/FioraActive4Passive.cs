using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.FioraActive4Passive)]
	public class FioraActive4Passive : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		public override void OnUpgradePassiveSkill()
		{
			base.OnUpgradePassiveSkill();
			int skillLevel = SkillLevel;
			if (Caster.IsHaveStateByGroup(Singleton<FioraSkillActive4Data>.inst.BuffStateGroup, Caster.ObjectId) &&
			    Caster.FindStateByGroup(Singleton<FioraSkillActive4Data>.inst.BuffStateGroup, Caster.ObjectId).Level <
			    skillLevel)
			{
				Caster.OverwriteState(Singleton<FioraSkillActive4Data>.inst.BuffState[skillLevel], Caster.ObjectId);
			}
		}

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Combine(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnFinishNormalAttack));
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Remove(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnFinishNormalAttack));
		}

		
		private void OnFinishNormalAttack(WorldCharacter victim, WorldCharacter attacker)
		{
			if (Caster.ObjectId == victim.ObjectId)
			{
				return;
			}

			if (Caster.ObjectId != attacker.ObjectId)
			{
				return;
			}

			if (!victim.IsAlive)
			{
				return;
			}

			int skillLevel = SkillLevel;
			if (!Caster.IsHaveStateByGroup(Singleton<FioraSkillActive4Data>.inst.BuffStateGroup, Caster.ObjectId))
			{
				return;
			}

			int num = Singleton<FioraSkillActive4Data>.inst.ConsumeSp[skillLevel];
			if (Caster.Status.Sp >= num)
			{
				Caster.ConsumeSkillResources(info.SkillCostType, info.SkillCostKey, num);
				parameterCollection.Clear();
				parameterCollection.Add(SkillScriptParameterType.Damage,
					Singleton<FioraSkillActive4Data>.inst.SkillAttack[skillLevel]);
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<FioraSkillActive4Data>.inst.SkillAttackApCoef[skillLevel]);
				DamageTo(victim.SkillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0);
			}
		}
	}
}