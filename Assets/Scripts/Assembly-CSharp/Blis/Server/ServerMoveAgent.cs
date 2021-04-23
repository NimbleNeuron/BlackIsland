using Blis.Common;
using UnityEngine;

namespace Blis.Server
{
	
	public class ServerMoveAgent : MoveAgent
	{
		
		
		public override bool IsInBush
		{
			get
			{
				return this.bushHashCode != 0;
			}
		}

		
		public MoveAgentSnapshot CreateSnapshot()
		{
			MoveAgentSnapshot moveAgentSnapshot = new MoveAgentSnapshot();
			if (this.moveStrategy != null)
			{
				moveAgentSnapshot.moveStrategyType = this.moveStrategy.GetStrategyType();
				switch (moveAgentSnapshot.moveStrategyType)
				{
				case MoveStrategyType.InCurve:
					moveAgentSnapshot.moveStrategySnapshot = this.moveInCurve.CreateSnapshot();
					break;
				case MoveStrategyType.InDirection:
					moveAgentSnapshot.moveStrategySnapshot = this.moveInDirection.CreateSnapshot();
					break;
				case MoveStrategyType.ToDestination:
				{
					MoveAgentSnapshot moveAgentSnapshot2 = moveAgentSnapshot;
					MoveToDestination movetoDestination = this.GetMovetoDestination();
					moveAgentSnapshot2.moveStrategySnapshot = ((movetoDestination != null) ? movetoDestination.CreateSnapshot() : null);
					break;
				}
				case MoveStrategyType.Straight:
					moveAgentSnapshot.moveStrategySnapshot = this.moveStraight.CreateSnapshot();
					break;
				case MoveStrategyType.StraightWithoutNav:
					moveAgentSnapshot.moveStrategySnapshot = this.moveStraightWithoutNav.CreateSnapshot();
					break;
				case MoveStrategyType.ToTargetWithoutNav:
					moveAgentSnapshot.moveStrategySnapshot = this.moveToTargetWithoutNav.CreateSnapshot();
					break;
				}
			}
			else
			{
				moveAgentSnapshot.moveStrategyType = MoveStrategyType.None;
			}
			moveAgentSnapshot.lockRotation = this.lockRotation;
			moveAgentSnapshot.startRotation = new BlisVector(this.startRotation.eulerAngles);
			moveAgentSnapshot.targetRotation = new BlisVector(this.targetRotation.eulerAngles);
			moveAgentSnapshot.targetRotationPeriod = new BlisFixedPoint(this.targetRotationPeriod);
			moveAgentSnapshot.targetRotationTimeStack = new BlisFixedPoint(this.targetRotationTimeStack);
			moveAgentSnapshot.walkableNavMask = this.walkableNavMask;
			moveAgentSnapshot.isInBush = (this.bushHashCode != 0);
			return moveAgentSnapshot;
		}

		
		public override void Init(float angularSpeed, int walkableNavMask)
		{
			base.Init(angularSpeed, walkableNavMask);
			this.CheckBush();
		}

		
		public override void FrameUpdate(float deltaTime)
		{
			base.FrameUpdate(deltaTime);
			this.CheckBush();
		}

		
		protected override MoveToDestination GetMovetoDestination()
		{
			return this.moveToDestination;
		}

		
		public void MoveToDestination(BlisVector startPosition, BlisVector destination, float moveSpeed)
		{
			if (this.moveToDestination == null)
			{
				this.moveToDestination = new ServerMoveToDestination();
			}
			this.moveToDestination.RefreshNextCorners -= this.OnRefreshNextCorners;
			this.moveToDestination.RefreshNextCorners += this.OnRefreshNextCorners;
			this.moveStrategy = this.moveToDestination.Setup(base.WalkableNavMask, startPosition, destination, moveSpeed);
		}

		
		private void OnRefreshNextCorners()
		{
			ServerMoveToDestination serverMoveToDestination;
			if ((serverMoveToDestination = (this.moveStrategy as ServerMoveToDestination)) != null)
			{
				ServerMoveAgent.RefreshNextCornersEvent refreshNextCorners = this.RefreshNextCorners;
				if (refreshNextCorners == null)
				{
					return;
				}
				refreshNextCorners(serverMoveToDestination.Destination, serverMoveToDestination.GetSnapshotCorners());
			}
		}

		
		protected override void Warp(Vector3 position)
		{
			base.Warp(position);
			this.CheckBush();
		}

		
		public override void Stop()
		{
			base.Stop();
			ServerMoveAgent.OnStopEvent onStop = this.OnStop;
			if (onStop == null)
			{
				return;
			}
			onStop();
		}

		
		public bool CompareBush(ServerMoveAgent targetMoveAgent)
		{
			return this.bushHashCode.CompareTo(targetMoveAgent.bushHashCode) == 0;
		}

		
		private void CheckBush()
		{
			int num = this.bushHashCode;
			Vector3 position = base.transform.position;
			Vector3 start = position + ServerMoveAgent.lineStartPositionY;
			Vector3 end = position + ServerMoveAgent.lineEndPositionY;
			RaycastHit raycastHit;
			if (Physics.Linecast(start, end, out raycastHit, GameConstants.LayerMask.BUSH_LAYER))
			{
				Vector3 position2 = raycastHit.collider.transform.position;
				this.bushHashCode = raycastHit.collider.name.GetHashCode();
				this.bushHashCode ^= (int)position2.x;
				this.bushHashCode ^= (int)position2.y;
				this.bushHashCode ^= (int)position2.z;
			}
			else
			{
				this.bushHashCode = 0;
			}
			if (num.Equals(0) && !this.bushHashCode.Equals(0))
			{
				ServerMoveAgent.InBushEvent inBush = this.InBush;
				if (inBush != null)
				{
					inBush();
				}
			}
			if (!num.Equals(0) && this.bushHashCode.Equals(0))
			{
				ServerMoveAgent.OutBushEvent outBush = this.OutBush;
				if (outBush == null)
				{
					return;
				}
				outBush();
			}
		}

		
		private ServerMoveToDestination moveToDestination;

		
		public ServerMoveAgent.RefreshNextCornersEvent RefreshNextCorners = delegate(Vector3 p0, BlisVector[] p1)
		{
		};

		
		private int bushHashCode;

		
		public ServerMoveAgent.OnStopEvent OnStop;

		
		public ServerMoveAgent.InBushEvent InBush;

		
		public ServerMoveAgent.OutBushEvent OutBush;

		
		private static readonly Vector3 lineStartPositionY = new Vector3(0f, -10f, 0f);

		
		private static readonly Vector3 lineEndPositionY = new Vector3(0f, 1f, 0f);

		
		public delegate void RefreshNextCornersEvent(Vector3 destination, BlisVector[] nextCorners);

		
		public delegate void OnStopEvent();

		
		public delegate void InBushEvent();

		
		public delegate void OutBushEvent();
	}
}
