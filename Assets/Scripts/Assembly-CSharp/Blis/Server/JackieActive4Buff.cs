using System;
using System.Collections;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive4Buff)]
	public class JackieActive4Buff : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Combine(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
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

			if (damageInfo.DamageType != DamageType.Normal)
			{
				return;
			}

			if (damageInfo.DamageSubType != DamageSubType.Normal)
			{
				return;
			}

			if (!victim.IsAlive)
			{
				return;
			}

			AddState(victim.SkillAgent, Singleton<JackieSkillActive4Data>.inst.DebuffState[SkillLevel]);
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
			if (!cancel)
			{
				SkillData skillData =
					GameDB.skill.GetSkillData(Singleton<JackieSkillActive4Data>.inst.FinishSkillDataCode[SkillLevel]);
				SkillUseInfo skillUseInfo = SkillUseInfo.Create(info.caster, info.target, skillData, info.skillSlotSet,
					MasteryType.None, info.skillEvolutionLevel, info.cursorPosition, info.releasePosition, null, true);
				(Caster.Character as WorldPlayerCharacter).UseInjectSkill(skillUseInfo);
			}

			BattleEventCollector inst = SingletonMonoBehaviour<BattleEventCollector>.inst;
			inst.OnAfterDamageProcess = (Action<WorldCharacter, DamageInfo>) Delegate.Remove(inst.OnAfterDamageProcess,
				new Action<WorldCharacter, DamageInfo>(OnAfterDamageProcess));
		}
	}
}