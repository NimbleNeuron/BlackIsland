using System.Collections;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.AyaActive3)]
	public class AyaActive3 : SkillScript
	{
		
		public override IEnumerator Play(object extraData = null)
		{
			Start();
			LookAtPosition(Caster, info.cursorPosition);
			if (SkillCastingTime1 > 0f)
			{
				yield return FirstCastingTime();
				LookAtPosition(Caster, info.cursorPosition);
			}

			Vector3 direction = GameUtil.Direction(Caster.Position, GetSkillPoint());
			Vector3 vector;
			bool flag;
			float finalDuration;
			Caster.MoveToDirectionForTime(direction, Singleton<AyaSkillActive3Data>.inst.DashDistance,
				Singleton<AyaSkillActive3Data>.inst.DashDuration, EasingFunction.Ease.EaseOutQuad, true, out vector,
				out flag, out finalDuration);
			while (finalDuration > 0f && Caster.IsMoving())
			{
				yield return WaitForFrame();
				finalDuration -= MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
			}

			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}
	}
}