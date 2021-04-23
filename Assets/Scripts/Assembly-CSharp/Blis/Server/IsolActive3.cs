using System.Collections;
using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.IsolActive3)]
	public class IsolActive3 : SkillScript
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
			float stealthSpeedUpDelay = Singleton<IsolSkillActive3Data>.inst.StealthSpeedUpDelay;
			if (stealthSpeedUpDelay > 0f)
			{
				StartCoroutine(CoroutineUtil.DelayedAction(stealthSpeedUpDelay, StartStealthAndSpeedUp), false);
			}
			else
			{
				StartStealthAndSpeedUp();
			}

			float dashDuration = Singleton<IsolSkillActive3Data>.inst.DashDuration;
			Vector3 vector;
			bool flag;
			float num;
			Caster.MoveToDirectionForTime(direction, Singleton<IsolSkillActive3Data>.inst.DashDistance, dashDuration,
				EasingFunction.Ease.EaseOutQuad, false, out vector, out flag, out num);
			yield return WaitForSeconds(dashDuration);
			if (SkillFinishDelayTime > 0f)
			{
				yield return FinishDelayTime();
			}

			Finish();
		}

		
		private void StartStealthAndSpeedUp()
		{
			if (!Caster.IsAlive)
			{
				return;
			}

			PlaySkillAction(Caster, 1);
			AddState(Caster, Singleton<IsolSkillActive3Data>.inst.BuffState[SkillLevel]);
		}
	}
}