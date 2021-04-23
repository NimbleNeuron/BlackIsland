using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.GloveUppercutAttack)]
	public class GloveUppercutAttack : NormalAttackScript
	{
		
		private readonly SkillScriptParameterCollection parameterCollection = SkillScriptParameterCollection.Create();

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target);
			Vector3 position = Target.Position;
			Vector3 position2 = Caster.Position;
			float num = GameUtil.DistanceOnPlane(position, position2);
			float num2 = Singleton<GloveSkillActiveData>.inst.UppercutIncreaseAttackRange[info.SkillLevel];
			if (num > Caster.Stat.AttackRange - num2)
			{
				Vector3 a = GameUtil.DirectionOnPlane(position, position2);
				Vector3 vector = Target.Position;
				vector +=
					(Caster.Stat.Radius + Target.Stat.Radius + Singleton<GloveSkillActiveData>.inst.DashDistance) * a;
				Vector3 vector2;
				bool flag;
				float num3;
				Caster.MoveToDestinationForTime(vector, Singleton<GloveSkillActiveData>.inst.DashDuration,
					EasingFunction.Ease.Linear, false, out vector2, out flag, out num3);
			}

			if (Singleton<GloveSkillActiveData>.inst.DashDuration > 0f)
			{
				yield return WaitForSeconds(Singleton<GloveSkillActiveData>.inst.DashDuration);
			}

			parameterCollection.Clear();
			parameterCollection.Add(SkillScriptParameterType.DamageApCoef,
				Singleton<GloveSkillActiveData>.inst.GloveUppercutAttackApCoef);
			parameterCollection.Add(SkillScriptParameterType.FinalAddDamage,
				Singleton<GloveSkillActiveData>.inst.DamageByLevel[info.SkillLevel]);
			DamageTo(Target, DamageType.Normal, DamageSubType.Normal, 1, parameterCollection, 3020003);
			FinishNormalAttack();
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}