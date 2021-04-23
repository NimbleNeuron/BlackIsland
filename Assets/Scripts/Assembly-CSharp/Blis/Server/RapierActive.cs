using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RapierActive)]
	public class RapierActive : SkillScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target);
			CasterLockRotation(true);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			if (CheckCollision(Singleton<RapierSkillActiveData>.inst.DistanceCollisionStart))
			{
				RapierDamage();
			}
			else
			{
				Vector3 a = GameUtil.DirectionOnPlane(Target.Position, Caster.Position);
				Vector3 vector = Target.Position;
				vector += (Caster.Stat.Radius + Target.Stat.Radius) * a;
				float moveStartTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
				Vector3 vector2;
				bool flag;
				float finalDuration;
				Caster.MoveToDestinationAtSpeed(vector, Singleton<RapierSkillActiveData>.inst.DashSpeed,
					EasingFunction.Ease.Linear, true, out vector2, out flag, out finalDuration);
				while (Caster.IsMoving() &&
				       MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - moveStartTime <= finalDuration)
				{
					yield return WaitForFrame();
				}

				if (CheckCollision(Singleton<RapierSkillActiveData>.inst.DistanceCollisionEnd))
				{
					RapierDamage();
				}
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private bool CheckCollision(float distanceCollision)
		{
			return Caster.Stat.Radius + Target.Stat.Radius + distanceCollision >=
			       GameUtil.DistanceOnPlane(Caster.Position, Target.Position);
		}

		
		private void RapierDamage()
		{
			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<RapierSkillActiveData>.inst.SkillApCoef[SkillLevel] *
				(2f + Caster.Stat.CriticalStrikeDamage));
			DamageTo(Target, DamageType.Skill, DamageSubType.Normal, 0, parameterCollection,
				Singleton<RapierSkillActiveData>.inst.EffectAndSoundCode);
		}
	}
}