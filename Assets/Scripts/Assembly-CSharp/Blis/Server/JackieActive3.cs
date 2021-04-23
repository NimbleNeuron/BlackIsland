using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive3)]
	public class JackieActive3 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		private CollisionCircle3D sector;

		
		protected override void Start()
		{
			base.Start();
			if (sector == null)
			{
				sector = new CollisionCircle3D(Caster.Position, SkillInnerRange);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 destination = GetSkillPoint();
			LookAtPosition(Caster, destination);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			Vector3 vector;
			bool flag;
			float num;
			Caster.MoveToDestinationForTime(destination, Singleton<JackieSkillActive3Data>.inst.DashDuration,
				EasingFunction.Ease.EaseOutQuad, true, out vector, out flag, out num);
			yield return WaitForSeconds(Singleton<JackieSkillActive3Data>.inst.DashDuration);
			sector.UpdatePosition(Caster.Position);
			sector.UpdateRadius(SkillInnerRange);
			List<SkillAgent> enemyCharacters = GetEnemyCharacters(sector);
			int skillLevel = SkillLevel;
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<JackieSkillActive3Data>.inst.SkillApCoefByLevel[skillLevel]);
			parameterCollection.Add(SkillScriptParameterType.Damage,
				Singleton<JackieSkillActive3Data>.inst.DamageByLevel[skillLevel]);
			DamageTo(enemyCharacters, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection, 0);
			foreach (SkillAgent skillAgent in enemyCharacters)
			{
				if (skillAgent.IsHaveStateByGroup(Singleton<JackieSkillActive1Data>.inst.DebuffGroup,
					    Caster.ObjectId) ||
				    skillAgent.IsHaveStateByGroup(Singleton<JackieSkillActive4Data>.inst.DebuffGroup, Caster.ObjectId))
				{
					AddState(skillAgent, Singleton<JackieSkillActive3Data>.inst.ReinforcedDebuffState[skillLevel]);
				}
				else
				{
					AddState(skillAgent, Singleton<JackieSkillActive3Data>.inst.DebuffState);
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}