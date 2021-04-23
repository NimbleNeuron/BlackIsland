using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Server
{
	
	public class ServerMoveToDestination : MoveToDestination
	{
		
		
		
		public event ServerMoveToDestination.RefreshNextCornersEvent RefreshNextCorners = delegate()
		{
		};

		
		public override byte[] CreateSnapshot()
		{
			return Serializer.Default.Serialize<MoveToDestinationSnapshot>(new MoveToDestinationSnapshot
			{
				startPosition = new BlisVector(this.startPosition),
				destination = new BlisVector(this.destination),
				arrived = this.arrived,
				moveSpeed = new BlisFixedPoint(this.moveSpeed),
				walkableNavMask = this.walkableNavMask,
				corners = this.GetSnapshotCorners()
			});
		}

		
		public override IMoveStrategy Setup(int walkableNavMask, BlisVector startPosition, BlisVector destination, float moveSpeed)
		{
			base.Setup(walkableNavMask, startPosition, destination, moveSpeed);
			this.currentTarget = Vector3.zero;
			this.SetNavPath();
			return this;
		}

		
		private void SetNavPath()
		{
			this.navPath.Reset();
			this.navMeshPath.ClearCorners();
			if (!NavMesh.CalculatePath(this.startPosition, this.destination, this.walkableNavMask, this.navMeshPath))
			{
				Vector3 destination;
				if (!MoveAgent.SamplePosition(this.destination, this.walkableNavMask, out destination) && !MoveAgent.SampleWidePosition(this.destination, this.walkableNavMask, out destination))
				{
					this.arrived = true;
					Log.W("Failed to SetNavPath. PathInvalid. Start: {0}, Destination: {1}", new object[]
					{
						this.startPosition,
						this.destination
					});
					return;
				}
				this.destination = destination;
				if (!NavMesh.CalculatePath(this.startPosition, this.destination, this.walkableNavMask, this.navMeshPath))
				{
					this.arrived = true;
					Log.W("Failed to SetNavPath. PathInvalid. Start: {0}, Destination: {1}", new object[]
					{
						this.startPosition,
						this.destination
					});
					return;
				}
			}
			this.navPath.EnqueuePath(this.navMeshPath, this.destination);
			if (!this.navPath.PeekTarget(out this.currentTarget))
			{
				this.arrived = true;
			}
		}

		
		public override Vector3 NextPosition(Vector3 position, float deltaTime)
		{
			if (this.arrived)
			{
				return position;
			}
			if (deltaTime <= 0.033333335f)
			{
				return this._NextPosition(position, deltaTime);
			}
			Vector3 vector = position;
			float num = deltaTime;
			while (num > 0f)
			{
				vector = this._NextPosition(vector, Mathf.Min(0.033333335f, num));
				num -= 0.033333335f;
				if (base.MoveFinished())
				{
					break;
				}
			}
			return vector;
		}

		
		private Vector3 _NextPosition(Vector3 position, float deltaTime)
		{
			Vector3 b = base.CalcDelta(position, this.currentTarget);
			float num = this.moveSpeed * deltaTime;
			if (b.sqrMagnitude <= Mathf.Pow(num, 2f))
			{
				if (!this.navPath.NextTarget(out this.currentTarget))
				{
					this.arrived = true;
				}
				else
				{
					ServerMoveToDestination.RefreshNextCornersEvent refreshNextCorners = this.RefreshNextCorners;
					if (refreshNextCorners != null)
					{
						refreshNextCorners();
					}
				}
				return position + b;
			}
			return position + num * b.normalized;
		}

		
		public override List<Vector3> GetCorners()
		{
			this.corners.Clear();
			this.corners.AddRange(this.navPath.corners);
			return this.corners;
		}

		
		public BlisVector[] GetSnapshotCorners()
		{
			this.snapshotCorners.Clear();
			for (int i = 0; i < 2; i++)
			{
				if (this.navPath.index + i < this.navPath.corners.Count)
				{
					this.snapshotCorners.Add(new BlisVector(this.navPath.corners.ElementAt(this.navPath.index + i)));
				}
			}
			return this.snapshotCorners.ToArray();
		}

		
		private readonly NavPath navPath = new NavPath();

		
		private Vector3 currentTarget;

		
		private NavMeshPath navMeshPath = new NavMeshPath();

		
		private const int NextCornerMaxCount = 2;

		
		private readonly List<BlisVector> snapshotCorners = new List<BlisVector>();

		
		private readonly List<Vector3> corners = new List<Vector3>();

		
		public delegate void RefreshNextCornersEvent();
	}
}
