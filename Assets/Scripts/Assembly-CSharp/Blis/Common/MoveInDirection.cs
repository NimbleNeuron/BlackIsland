using UnityEngine;

namespace Blis.Common
{
	public class MoveInDirection : IMoveStrategy
	{
		private bool canRotate;


		private Vector3 direction;


		private bool moveFinished;


		private float moveSpeed;


		private int walkableNavMask;

		public MoveStrategyType GetStrategyType()
		{
			return MoveStrategyType.InDirection;
		}


		public Vector3 NextPosition(Vector3 position, float deltaTime)
		{
			if (deltaTime <= 0.033333335f)
			{
				return _NextPosition(position, deltaTime);
			}

			Vector3 vector = position;
			float num = deltaTime;
			while (num > 0f)
			{
				vector = _NextPosition(vector, Mathf.Min(0.033333335f, num));
				num -= 0.033333335f;
				if (MoveFinished())
				{
					break;
				}
			}

			return vector;
		}


		public void UpdateMoveSpeed(float moveSpeed)
		{
			this.moveSpeed = moveSpeed;
		}


		public bool MoveFinished()
		{
			return moveFinished;
		}


		public void SetRotateWhileMoving(bool isRotation)
		{
			canRotate = isRotation;
		}


		public bool CanRotateWhileMoving()
		{
			return canRotate;
		}


		public void ResetDestination(Vector3 destination) { }


		public byte[] CreateSnapshot()
		{
			return Serializer.Default.Serialize<MoveInDirectionSnapshot>(new MoveInDirectionSnapshot
			{
				direction = new BlisVector(direction),
				moveSpeed = new BlisFixedPoint(moveSpeed),
				moveFinished = moveFinished,
				walkableNavMask = walkableNavMask
			});
		}


		public IMoveStrategy Init(byte[] snapshotData)
		{
			MoveInDirectionSnapshot moveInDirectionSnapshot =
				Serializer.Default.Deserialize<MoveInDirectionSnapshot>(snapshotData);
			Setup(moveInDirectionSnapshot.walkableNavMask, moveInDirectionSnapshot.direction.ToVector3(),
				moveInDirectionSnapshot.moveSpeed.Value);
			moveFinished = moveInDirectionSnapshot.moveFinished;
			return this;
		}


		public IMoveStrategy Setup(int walkableNavMask, Vector3 direction, float moveSpeed)
		{
			this.walkableNavMask = walkableNavMask;
			this.direction = direction.normalized;
			this.moveSpeed = moveSpeed;
			moveFinished = false;
			return this;
		}


		private Vector3 _NextPosition(Vector3 position, float deltaTime)
		{
			Vector3 vector = position + moveSpeed * deltaTime * direction;
			Vector3 vector2;
			if (!MoveAgent.CanStraightMoveToDestination(position, vector, walkableNavMask, out vector2))
			{
				moveFinished = true;
				return position;
			}

			Vector3 vector3;
			if (!MoveAgent.CanStandToPosition(vector, walkableNavMask, out vector3))
			{
				moveFinished = true;
				return position;
			}

			vector.y = vector3.y;
			return vector;
		}
	}
}