using UnityEngine;
using UnityEngine.AI;

namespace Blis.Common
{
	
	public class MoveToTargetWithoutNav : IMoveStrategy
	{
		
		public MoveStrategyType GetStrategyType()
		{
			return MoveStrategyType.ToTargetWithoutNav;
		}

		
		public byte[] CreateSnapshot()
		{
			return Serializer.Default.Serialize<MoveToTargetWithoutNavSnapshot>(new MoveToTargetWithoutNavSnapshot
			{
				moveStartPos = new BlisVector(this.moveStartPos),
				target = this.target.ObjectId,
				speed = new BlisFixedPoint(this.speed),
				arriveRadius = new BlisFixedPoint(this.arriveRadius),
				arrived = this.arrived,
				walkableNavMask = this.walkableNavMask
			});
		}

		
		public IMoveStrategy Init<T>(byte[] snapshotData, WorldBase<T> world) where T : ObjectBase
		{
			MoveToTargetWithoutNavSnapshot moveToTargetWithoutNavSnapshot = Serializer.Default.Deserialize<MoveToTargetWithoutNavSnapshot>(snapshotData);
			this.moveStartPos = moveToTargetWithoutNavSnapshot.moveStartPos.ToVector3();
			this.target = world.Find<T>(moveToTargetWithoutNavSnapshot.target);
			this.speed = moveToTargetWithoutNavSnapshot.speed.Value;
			this.arriveRadius = moveToTargetWithoutNavSnapshot.arriveRadius.Value;
			this.arrived = moveToTargetWithoutNavSnapshot.arrived;
			this.walkableNavMask = moveToTargetWithoutNavSnapshot.walkableNavMask;
			return this;
		}

		
		public IMoveStrategy Setup(Vector3 moveStartPos, ObjectBase target, float speed, float arriveRadius, int walkableNavMask)
		{
			this.moveStartPos = moveStartPos;
			this.target = target;
			this.speed = speed;
			this.arriveRadius = arriveRadius;
			this.walkableNavMask = walkableNavMask;
			this.arrived = false;
			return this;
		}

		
		public Vector3 NextPosition(Vector3 position, float deltaTime)
		{
			if (this.arrived)
			{
				return position;
			}
			Vector3 vector = this.target.GetPosition() - position;
			float num = vector.magnitude - this.arriveRadius;
			if (num <= 0f)
			{
				this.arrived = true;
				return position;
			}
			Vector3 normalized = vector.normalized;
			float num2 = this.speed * deltaTime;
			if (num2 > num)
			{
				num2 = num;
				this.arrived = true;
			}
			Vector3 result = position + num2 * normalized;
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(position, out navMeshHit, 2f, this.walkableNavMask))
			{
				result.y = navMeshHit.position.y;
			}
			return result;
		}

		
		public void UpdateMoveSpeed(float moveSpeed)
		{
		}

		
		public bool MoveFinished()
		{
			return this.arrived;
		}

		
		public void SetRotateWhileMoving(bool isRotation)
		{
			this.canRotate = isRotation;
		}

		
		public bool CanRotateWhileMoving()
		{
			return this.canRotate;
		}

		
		public void ResetDestination(Vector3 destination)
		{
		}

		
		private bool canRotate;

		
		private Vector3 moveStartPos;

		
		private ObjectBase target;

		
		private float speed;

		
		private float arriveRadius;

		
		private bool arrived;

		
		private int walkableNavMask;
	}
}
