using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive2AddAttack)]
	public class LukeActive2AddAttack : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Combine(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnFininshAddSkillDamage));
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForFrame();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnFinishNormalAttack = (Action<WorldCharacter, WorldCharacter>) Delegate.Remove(
				inst.OnFinishNormalAttack, new Action<WorldCharacter, WorldCharacter>(OnFininshAddSkillDamage));
		}

		
		private void OnFininshAddSkillDamage(WorldCharacter victim, WorldCharacter attacker)
		{
			if (attacker == null)
			{
				return;
			}

			if (!attacker.IsAlive)
			{
				return;
			}

			if (attacker.ObjectId != Caster.ObjectId)
			{
				return;
			}

			int skillLevel = Caster.GetSkillLevel(SkillSlotIndex.Active2);
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<LukeSkillActive2Data>.inst.BaseDamage[skillLevel]);
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<LukeSkillActive2Data>.inst.DamageApCoef);
			bool flag = Caster.IsSkillEvolution(SkillSlotIndex.Active2);
			int effectAndSoundCode =
				flag
					? Singleton<LukeSkillActive2Data>.inst.EvolutionEffectAddAttackSoundCode
					: Singleton<LukeSkillActive2Data>.inst.EffectAddAttackSoundCode;
			DamageTo(victim.SkillAgent, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
				effectAndSoundCode);
			AddState(Caster, Singleton<LukeSkillActive2Data>.inst.AddAttackBuffStackCode);
			if (flag)
			{
				ModifySkillCooldown(Caster, SkillSlotSet.Active1_1,
					-Singleton<LukeSkillActive2Data>.inst.EvolutionSkillActive1CoolDown);
			}
		}
	}
}