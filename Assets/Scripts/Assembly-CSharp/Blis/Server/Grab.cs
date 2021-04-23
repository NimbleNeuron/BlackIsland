using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	[SkillScript(SkillId.Grab)]
	public class Grab : CrowdControlScript
	{
		
		private const float CheckPeriodForMove = 0.1f;

		
		private GrabState grabState;

		
		private float lastMoveTime;

		
		private ServerSightAgent sharedSightAgent;

		
		private int sharedSightAgentTargetId;

		
		private float sumRadius;

		
		
		protected virtual bool ifNotOnNavDoWrap => true;

		
		protected override void Start()
		{
			base.Start();
			grabState = characterState as GrabState;
			isBegin = true;
			sumRadius = grabState.Graber.Stat.Radius + Target.Character.Stat.Radius;
			lastMoveTime = 0f;
			sharedSightAgentTargetId = Target.ObjectId;
			sharedSightAgent = Caster.AttachSight(Target.Character, 1f, 0f, false);
		}

		
		protected override void UpdateCrowdControl()
		{
			if (grabState.Graber == null || !grabState.Graber.IsAlive)
			{
				Finish();
			}

			if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastMoveTime < 0.1f)
			{
				return;
			}

			Vector3 b = (Target.Position - grabState.Graber.GetPosition()).normalized * (sumRadius * 0.98f);
			Target.MoveStraightWithoutNavSpeed(Target.Position, grabState.Graber.GetPosition() + b, grabState.Speed);
			lastMoveTime = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
		}

		
		protected override bool IsFinishedCrowdControl()
		{
			if (!isBegin)
			{
				return false;
			}

			float num = GameUtil.DistanceOnPlane(grabState.Graber.GetPosition(), Target.Position);
			return sumRadius >= num;
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			if (Target != null && grabState != null)
			{
				Target.RemoveStateByGroup(grabState.Group, Caster.ObjectId);
			}

			NavMeshHit navMeshHit;
			if (ifNotOnNavDoWrap && Target != null &&
			    !NavMesh.SamplePosition(Target.Position, out navMeshHit, 0.1f, 2147483640) &&
			    NavMesh.SamplePosition(Target.Position, out navMeshHit, 8f, 2147483640))
			{
				Target.WarpTo(navMeshHit.position);
			}

			if (sharedSightAgent != null)
			{
				Caster.RemoveSight(sharedSightAgent, sharedSightAgentTargetId);
			}
		}
	}
}