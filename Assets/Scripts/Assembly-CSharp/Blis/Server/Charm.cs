using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[SkillScript(SkillId.Charm)]
	public class Charm : CrowdControlScript
	{
		
		private const float MoveToDirectionTick = 0.1f;

		
		private float lastMoveInDirectionTime;

		
		private WorldMovableCharacter movableTarget;

		
		protected override void Start()
		{
			base.Start();
			lastMoveInDirectionTime = 0f;
			movableTarget = Target.Character as WorldMovableCharacter;
		}

		
		protected override void UpdateCrowdControl()
		{
			if (movableTarget != null && movableTarget.SkillController.AnyPlayingSkill())
			{
				foreach (KeyValuePair<SkillId, SkillScript> keyValuePair in movableTarget.SkillController.playingScripts
				)
				{
					if (!keyValuePair.Value.CanMoveDuringSkillPlaying)
					{
						return;
					}
				}
			}

			if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastMoveInDirectionTime < 0.1f)
			{
				return;
			}

			Vector3 direction = GameUtil.DirectionOnPlane(Target.Position, Caster.Position);
			float num = GameUtil.DistanceOnPlane(Target.Position, Caster.Position);
			if (Target.Stat.Radius + Caster.Stat.Radius + 0.5f < num)
			{
				Target.MoveInDirection(direction);
			}
			else
			{
				Target.StopMove();
			}

			lastMoveInDirectionTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			Target.StopMove();
		}
	}
}