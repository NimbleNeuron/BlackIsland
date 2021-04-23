using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.Knockback)]
	public class Knockback : CrowdControlScript
	{
		
		private bool canMoveToDestination;

		
		private float finalDuration;

		
		private bool isLockRotation;

		
		private KnockbackState knockback;

		
		protected override void Start()
		{
			base.Start();
			knockback = characterState as KnockbackState;
			isLockRotation = false;
			canMoveToDestination = false;
			finalDuration = 0f;
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

				if (0f < knockback._Distance)
				{
					Vector3 vector;
					Target.MoveToDirectionForTime(knockback._Direction, knockback._Distance, knockback._Duration,
						knockback._Ease, knockback._IsPassingWall, out vector, out canMoveToDestination,
						out finalDuration);
				}
			}
		}

		
		protected override bool IsFinishedCrowdControl()
		{
			return isBegin &&
			       finalDuration < MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - beginTime;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (!cancel)
			{
				SkillAgent target = Target;
				if (target != null)
				{
					target.RemoveStateByGroup(knockback.Group, knockback.Caster.ObjectId);
				}

				if (!canMoveToDestination)
				{
					KnockbackState knockbackState = knockback;
					if (knockbackState != null)
					{
						Action<SkillAgent> onCollisionWall = knockbackState._OnCollisionWall;
						if (onCollisionWall != null)
						{
							onCollisionWall(Target);
						}
					}
				}
			}

			knockback = null;
			if (isLockRotation)
			{
				SkillAgent target2 = Target;
				if (target2 != null)
				{
					target2.LockRotation(false);
				}

				isLockRotation = false;
			}
		}
	}
}