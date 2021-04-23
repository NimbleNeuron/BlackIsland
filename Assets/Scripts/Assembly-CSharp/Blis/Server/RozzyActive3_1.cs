using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive3_1)]
	public class RozzyActive3_1 : SkillScript
	{
		
		private readonly SkillScriptParameterCollection damageParam = SkillScriptParameterCollection.Create();

		
		private List<SkillAgent> enemies = new List<SkillAgent>();

		
		private CollisionCircle3D sector;

		
		private Vector3 targetPosition;

		
		protected override void Start()
		{
			base.Start();
			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<RozziSkillActive1Data>.inst.Active1MoveStateCode).group,
				Caster.ObjectId);
			targetPosition = Target.Position;
			LookAtTarget(Caster, Target);
			AddState(Caster, Singleton<RozziSkillActive3Data>.inst.Active3BuffStateCode);
			if (sector == null)
			{
				sector = new CollisionCircle3D(Vector3.zero, 0f);
			}
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			PlaySkillAction(Caster, info.skillData.SkillId, 1);
			Vector3 direction = GameUtil.Direction(Caster.Position, Target.Position);
			float distance = GameUtil.Distance(Caster.Position, info.target.Position);
			float num = 0f;
			Vector3 vector;
			bool flag;
			Caster.MoveToDirectionForTime(direction, distance,
				Singleton<RozziSkillActive3Data>.inst.Active3Move1Duration, EasingFunction.Ease.EaseOutQuad, true,
				out vector, out flag, out num);
			if (num > 0f)
			{
				yield return WaitForSeconds(num);
			}

			PlaySkillAction(Caster, info.skillData.SkillId, 2, 0, new BlisVector(targetPosition));
			sector.UpdatePosition(targetPosition);
			sector.UpdateRadius(Singleton<RozziSkillActive3Data>.inst.Active3AttackRange);
			enemies.Clear();
			enemies = GetEnemyCharacters(sector);
			foreach (SkillAgent target in enemies)
			{
				damageParam.Clear();
				damageParam.Add(SkillScriptParameterType.Damage,
					Singleton<RozziSkillActive3Data>.inst.DamageActive3_1ByLevel[SkillLevel]);
				damageParam.Add(SkillScriptParameterType.DamageApCoef,
					Singleton<RozziSkillActive3Data>.inst.DamageActive3_1ApCoef);
				DamageTo(target, DamageType.Skill, DamageSubType.Normal, 0, damageParam,
					Singleton<RozziSkillActive3Data>.inst.Active3HitEffectCode);
			}

			if (Singleton<RozziSkillActive3Data>.inst.Active3ShotDuration > 0f)
			{
				yield return WaitForSeconds(Singleton<RozziSkillActive3Data>.inst.Active3ShotDuration);
			}

			float num2 = 0f;
			Caster.MoveToDirectionForTime(Caster.Forward, Singleton<RozziSkillActive3Data>.inst.Active3Move2Distance,
				Singleton<RozziSkillActive3Data>.inst.Active3Move2Duration, EasingFunction.Ease.EaseOutQuad, false,
				out vector, out flag, out num2);
			if (num2 > 0f)
			{
				yield return WaitForSeconds(num2);
			}

			Caster.RemoveStateByGroup(
				GameDB.characterState.GetData(Singleton<RozziSkillActive3Data>.inst.Active3BuffStateCode).group,
				Caster.ObjectId);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}

		
		protected override bool IsChangedSkillSequence()
		{
			if (enemies.Count > 0)
			{
				AddState(Caster, Singleton<RozziSkillActive3Data>.inst.Active3_2EnableStateCode);
				return true;
			}

			return false;
		}
	}
}