using System;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server
{
	
	[ObjectAttr(ObjectType.SummonServant)]
	public class WorldSummonServant : WorldSummonBase, IServerMoveAgentOwner
	{
		
		protected override ObjectType GetObjectType()
		{
			return ObjectType.SummonServant;
		}

		
		
		public bool CanControl
		{
			get
			{
				return this.canControl;
			}
		}

		
		
		public ServerMoveAgent MoveAgent
		{
			get
			{
				return this.moveAgent;
			}
		}

		
		public bool IsStopped()
		{
			return !this.moveAgent.IsMoving;
		}

		
		
		public WorldSummonServant.WorldSummonServantData ServantData
		{
			get
			{
				return this.servantData;
			}
		}

		
		public override void Init(SummonData summonData, WorldPlayerCharacter owner)
		{
			base.Init(summonData, owner);
			GameUtil.BindOrAdd<ServerMoveAgent>(base.gameObject, ref this.moveAgent);
			this.moveAgent.Init(0f, 2147483640);
			ServerMoveAgent serverMoveAgent = this.moveAgent;
			serverMoveAgent.RefreshNextCorners = (ServerMoveAgent.RefreshNextCornersEvent)Delegate.Remove(serverMoveAgent.RefreshNextCorners, new ServerMoveAgent.RefreshNextCornersEvent(this.MoveToDestination));
			ServerMoveAgent serverMoveAgent2 = this.moveAgent;
			serverMoveAgent2.RefreshNextCorners = (ServerMoveAgent.RefreshNextCornersEvent)Delegate.Combine(serverMoveAgent2.RefreshNextCorners, new ServerMoveAgent.RefreshNextCornersEvent(this.MoveToDestination));
			ServerMoveAgent serverMoveAgent3 = this.moveAgent;
			serverMoveAgent3.OnStop = (ServerMoveAgent.OnStopEvent)Delegate.Remove(serverMoveAgent3.OnStop, new ServerMoveAgent.OnStopEvent(this.OnStopMove));
			ServerMoveAgent serverMoveAgent4 = this.moveAgent;
			serverMoveAgent4.OnStop = (ServerMoveAgent.OnStopEvent)Delegate.Combine(serverMoveAgent4.OnStop, new ServerMoveAgent.OnStopEvent(this.OnStopMove));
			ServerMoveAgent serverMoveAgent5 = this.moveAgent;
			serverMoveAgent5.InBush = (ServerMoveAgent.InBushEvent)Delegate.Remove(serverMoveAgent5.InBush, new ServerMoveAgent.InBushEvent(base.InBush));
			ServerMoveAgent serverMoveAgent6 = this.moveAgent;
			serverMoveAgent6.InBush = (ServerMoveAgent.InBushEvent)Delegate.Combine(serverMoveAgent6.InBush, new ServerMoveAgent.InBushEvent(base.InBush));
			ServerMoveAgent serverMoveAgent7 = this.moveAgent;
			serverMoveAgent7.OutBush = (ServerMoveAgent.OutBushEvent)Delegate.Remove(serverMoveAgent7.OutBush, new ServerMoveAgent.OutBushEvent(base.OutBush));
			ServerMoveAgent serverMoveAgent8 = this.moveAgent;
			serverMoveAgent8.OutBush = (ServerMoveAgent.OutBushEvent)Delegate.Combine(serverMoveAgent8.OutBush, new ServerMoveAgent.OutBushEvent(base.OutBush));
			this.canControl = true;
		}

		
		public override byte[] CreateSnapshot()
		{
			return WorldObject.serializer.Serialize<SummonSnapshot>(new SummonSnapshot
			{
				statusSnapshot = WorldObject.serializer.Serialize<SummonStatusSnapshot>(new SummonStatusSnapshot(base.Status)),
				initialStat = base.Stat.CreateSnapshot(),
				initialStateEffect = base.StateEffector.CreateSnapshot(),
				skillController = base.SkillController.CreateSnapshot(),
				moveAgentSnapshot = this.moveAgent.CreateSnapshot(),
				isInCombat = this.IsInCombat,
				isInvisible = base.IsInvisible,
				ownerId = ((base.Owner != null) ? base.Owner.ObjectId : 0),
				summonId = base.SummonData.code
			});
		}

		
		protected override int GetCharacterCode()
		{
			SummonData summonData = base.SummonData;
			if (summonData == null)
			{
				return 0;
			}
			return summonData.code;
		}

		
		protected override void OnFrameUpdate()
		{
			base.OnFrameUpdate();
			this.moveAgent.FrameUpdate(MonoBehaviourInstance<GameService>.inst.ServerFrameDeltaTime);
		}

		
		public void MoveStraightWithoutNavSpeed(Vector3 moveStartPos, Vector3 moveEndPos, float moveSpeed)
		{
			float num = GameUtil.DistanceOnPlane(moveStartPos, moveEndPos);
			this.MoveStraightWithoutNavDuration(moveStartPos, moveEndPos, num / moveSpeed);
		}

		
		public void MoveStraightWithoutNavDuration(Vector3 moveStartPos, Vector3 moveEndPos, float duration)
		{
			this.moveAgent.MoveStraightWithoutNav(moveStartPos, moveEndPos, duration);
			base.EnqueueCommand(new CmdMoveStraightWithoutNav
			{
				startPos = new BlisVector(moveStartPos),
				endPos = new BlisVector(moveEndPos),
				duration = new BlisFixedPoint(duration)
			});
		}

		
		public void MoveToTargetWithoutNavSpeed(Vector3 moveStartPos, WorldCharacter target, float moveSpeed, float arriveRadius)
		{
			this.moveAgent.MoveToTargetWithoutNav(moveStartPos, target, moveSpeed, arriveRadius);
			base.EnqueueCommand(new CmdMoveToTargetWithoutNav
			{
				startPos = new BlisVector(moveStartPos),
				targetId = target.ObjectId,
				moveSpeed = new BlisFixedPoint(moveSpeed),
				arriveRadius = new BlisFixedPoint(arriveRadius)
			});
		}

		
		public void WarpTo(Vector3 destination, bool needCheckNavMesh)
		{
			if (!this.isAlive)
			{
				return;
			}
			Vector3 vector;
			if (needCheckNavMesh && Blis.Common.MoveAgent.SamplePosition(destination, 2147483640, out vector))
			{
				destination = vector;
			}
			BlisVector blisVector = new BlisVector(destination);
			this.moveAgent.Warp(blisVector);
			base.EnqueueCommand(new CmdWarpTo
			{
				destination = blisVector
			});
		}

		
		public void StopMove()
		{
			this.moveAgent.Stop();
		}

		
		private void OnStopMove()
		{
			base.EnqueueCommand(new CmdStopMove());
		}

		
		public void LockRotation(bool isLock)
		{
			this.moveAgent.LockRotation(isLock);
			base.EnqueueCommand(new CmdLockRotation
			{
				isLock = isLock,
				rotationY = new BlisFixedPoint(base.transform.eulerAngles.y)
			});
		}

		
		public void MoveInCurve(float angularSpeed)
		{
			throw new NotImplementedException();
		}

		
		public void MoveInDirection(Vector3 direction)
		{
			throw new NotImplementedException();
		}

		
		public void MoveToDestination(Vector3 destination)
		{
			throw new NotImplementedException();
		}

		
		public void MoveToDestination(Vector3 destination, BlisVector[] nextCorners)
		{
			throw new NotImplementedException();
		}

		
		public void MoveStraight(Vector3 destination, float duration, EasingFunction.Ease ease, bool canRotate)
		{
			throw new NotImplementedException();
		}

		
		public void SetCanControl(bool canControl)
		{
			this.canControl = canControl;
		}

		
		public void SetServantData(WorldSummonServant.WorldSummonServantData servantData)
		{
			this.servantData = servantData;
		}

		
		private bool canControl = true;

		
		private ServerMoveAgent moveAgent;

		
		private WorldSummonServant.WorldSummonServantData servantData;

		
		public class WorldSummonServantData
		{
		}
	}
}
