using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive2Buff)]
	public class JackieActive2Buff : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override SkillScriptParameterCollection GetParameters(SkillAgent target, DamageType type,
			DamageSubType subType, int damageId)
		{
			parameterCollection.Clear();
			if (subType == DamageSubType.Normal &&
			    (target.IsHaveStateByGroup(Singleton<JackieSkillActive1Data>.inst.DebuffGroup, Caster.ObjectId) ||
			     target.IsHaveStateByGroup(Singleton<JackieSkillActive4Data>.inst.DebuffGroup, Caster.ObjectId)))
			{
				parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<JackieSkillActive2Data>.inst.DamageApCoef[SkillLevel]);
			}

			return parameterCollection;
		}

		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(
				inst.OnBeforeDamageProcess, new Action<WorldCharacter, DamageInfo>(OnBeforeDamageProcess));
		}

		
		private void OnBeforeDamageProcess(WorldCharacter victim, DamageInfo damageInfo)
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

			if (damageInfo.DamageSubType == DamageSubType.Dot || damageInfo.DamageSubType == DamageSubType.Trap ||
			    damageInfo.DamageSubType == DamageSubType.Summon)
			{
				return;
			}

			if (victim.IsAlive && victim.SkillAgent.AnyHaveStateByType(StateType.Bleed))
			{
				float healApCoef = Singleton<JackieSkillActive2Data>.inst.HealApCoef;
				int fixAmount = Singleton<JackieSkillActive2Data>.inst.HealByLevel[SkillLevel];
				HpHealTo(Caster, Caster.Stat.AttackPower, healApCoef, fixAmount, true,
					Singleton<JackieSkillActive2Data>.inst.Active2BuffEffectAndSoundCode);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			yield return WaitForSeconds(StateDuration);
			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnBeforeDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(
				inst.OnBeforeDamageProcess, new Action<WorldCharacter, DamageInfo>(OnBeforeDamageProcess));
		}
	}
}