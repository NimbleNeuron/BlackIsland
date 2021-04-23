using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.Airborne)]
	public class Airborne : CrowdControlScript
	{
		
		private AirborneState airborne;

		
		private bool isLockRotation;

		
		protected override void Start()
		{
			base.Start();
			airborne = characterState as AirborneState;
			isLockRotation = false;
		}

		
		protected override void UpdateCrowdControl()
		{
			if (!isBegin)
			{
				isBegin = true;
				beginTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime +
				            MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime;
				if (!isLockRotation)
				{
					Target.LockRotation(true);
					isLockRotation = true;
				}

				Target.Airborne(airborne.Duration, airborne._Power);
				if (airborne._Moving)
				{
					Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
					float distance = Random.Range(0f, 0.5f);
					Vector3 vector;
					bool flag;
					float num;
					Target.MoveToDirectionForTime(direction, distance, airborne._Duration, EasingFunction.Ease.Linear,
						true, out vector, out flag, out num);
				}
			}
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (isLockRotation)
			{
				Target.LockRotation(false);
				isLockRotation = false;
			}
		}
	}
}