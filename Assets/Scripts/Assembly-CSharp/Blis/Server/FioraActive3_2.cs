using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.FioraActive3_2)]
	public class FioraActive3_2 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			float backDashDistance = Singleton<FioraSkillActive3Data>.inst.BackDashDistance;
			float backDashDuration = Singleton<FioraSkillActive3Data>.inst.BackDashDuration;
			Vector3 direction = GameUtil.DirectionOnPlane(Caster.Position, info.cursorPosition);
			Vector3 vector;
			bool flag;
			float seconds;
			Caster.MoveToDirectionForTime(direction, backDashDistance, backDashDuration, EasingFunction.Ease.Linear,
				false, out vector, out flag, out seconds);
			yield return WaitForSeconds(seconds);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		protected override void Start()
		{
			base.Start();
			Vector3 direction = GameUtil.DirectionOnPlane(info.cursorPosition, Caster.Position);
			LookAtDirection(Caster, direction);
			CasterLockRotation(true);
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
		}
	}
}