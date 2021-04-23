using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.RozziActive1Move)]
	public class RozzyActive1Move : SkillScript
	{
		
		protected override void Start()
		{
			base.Start();
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			Vector3 targetDirection = GameUtil.Direction(Caster.Position, info.cursorPosition);
			LookAtDirection(Caster, targetDirection);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
			}

			float num = 0f;
			Vector3 vector;
			bool flag;
			Caster.MoveToDirectionForTime(targetDirection, Singleton<RozziSkillActive1Data>.inst.MoveDistance,
				Singleton<RozziSkillActive1Data>.inst.MoveDuration, EasingFunction.Ease.EaseOutQuad, false, out vector,
				out flag, out num);
			PlaySkillAction(Caster, info.skillData.SkillId, 1);
			if (num > 0f)
			{
				yield return WaitForSeconds(num);
			}

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
	}
}