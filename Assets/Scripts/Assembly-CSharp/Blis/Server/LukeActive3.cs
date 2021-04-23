using System.Collections;
using Blis.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	[SkillScript(SkillId.LukeActive3)]
	public class LukeActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParameter = SkillScriptParameterCollection.Create();

		
		protected override void Start()
		{
			base.Start();
			if (Target != null)
			{
				Vector3 position = Caster.Position;
				Vector3 vector = Target.Position +
				                 Target.Forward * -1f * Singleton<LukeSkillActive3Data>.inst.WarpBackDistance;
				NavMeshHit navMeshHit;
				vector = NavMesh.SamplePosition(vector, out navMeshHit, SkillRange, 2147483640)
					? navMeshHit.position
					: Target.Position;
				Caster.WarpTo(vector);
				PlaySkillAction(Caster, 1, Target, position);
				if (vector != Caster.Position)
				{
					LookAtPosition(Caster, Target.Position);
				}
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (0f < SkillCastingTime1)
			{
				yield return FirstCastingTime();
			}

			if (Target != null)
			{
				damageParameter.Clear();
				damageParameter.Add(SkillScriptParameterType.Damage,
					Singleton<LukeSkillActive3Data>.inst.DamageBySkillLevel[SkillLevel]);
				damageParameter.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<LukeSkillActive3Data>.inst.DamageApCoef);
				int effectAndSoundCode = IsEvolution()
					? Singleton<LukeSkillActive3Data>.inst.EvolutionDamageEffectAndSoundCode
					: Singleton<LukeSkillActive3Data>.inst.DamageEffectAndSoundCode;
				DamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, damageParameter, effectAndSoundCode);
				PlaySkillAction(Caster, 2, Target);
				if (IsEvolution())
				{
					AddState(Target, Singleton<LukeSkillActive3Data>.inst.SilentVacuumCleanerMoveSpeedDownStateCode);
				}
			}

			if (0f < SkillFinishDelayTime)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish();
		}
	}
}