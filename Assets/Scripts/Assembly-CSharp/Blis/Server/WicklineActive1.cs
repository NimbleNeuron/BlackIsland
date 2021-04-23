using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WicklineActive1)]
	public class WicklineActive1 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtTarget(Caster, Target);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			StartConcentration();
			yield return WaitForSeconds(SkillConcentrationTime);
			FinishConcentration(false);
			Vector3 vector;
			bool flag;
			float num;
			Caster.MoveToDestinationAtSpeed(Target.Position, Singleton<WicklineSkillActive1Data>.inst.Speed,
				EasingFunction.Ease.Linear, true, out vector, out flag, out num);
			bool flag2 = Vector3.Distance(Target.Position, Caster.Position) <=
			             Singleton<WicklineSkillActive1Data>.inst.WarpDistance;
			while (!flag2 && Caster.IsMoving())
			{
				yield return WaitForFrame();
				flag2 = Vector3.Distance(Target.Position, Caster.Position) <=
				        Singleton<WicklineSkillActive1Data>.inst.WarpDistance;
			}

			if (flag2)
			{
				PlaySkillAction(Caster, 1);
				Vector3 vector2 = Target.Position;
				vector2 += (Caster.Stat.AttackRange + Target.Stat.Radius * 0.5f) * -Target.Forward;
				Caster.MoveToDestinationForTime(vector2, 0f, EasingFunction.Ease.Linear, true, out vector, out flag,
					out num);
				LookAtDirection(Caster, Target.Position);
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}