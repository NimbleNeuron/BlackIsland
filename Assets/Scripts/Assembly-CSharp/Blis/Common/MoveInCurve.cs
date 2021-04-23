using UnityEngine;

namespace Blis.Common
{
	public class MoveInCurve : IMoveStrategy
	{
		private float angularSpeed;


		private bool canRotate;


		private Vector3 currentDirection;


		private bool moveFinished;


		private float moveSpeed;


		private int walkableNavMask;

		public MoveStrategyType GetStrategyType()
		{
			return MoveStrategyType.InCurve;
		}


		public Vector3 NextPosition(Vector3 position, float deltaTime)
		{
			if (moveFinished)
			{
				return position;
			}

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
			return Serializer.Default.Serialize<MoveInCurveSnapshot>(new MoveInCurveSnapshot
			{
				moveSpeed = new BlisFixedPoint(moveSpeed),
				angularSpeed = new BlisFixedPoint(angularSpeed),
				currentDirection = new BlisVector(currentDirection),
				moveFinished = moveFinished,
				walkableNavMask = walkableNavMask
			});
		}


		public IMoveStrategy Init(byte[] snapshotData)
		{
			MoveInCurveSnapshot moveInCurveSnapshot = Serializer.Default.Deserialize<MoveInCurveSnapshot>(snapshotData);
			Setup(moveInCurveSnapshot.walkableNavMask, moveInCurveSnapshot.currentDirection.ToVector3(),
				moveInCurveSnapshot.angularSpeed.Value, moveInCurveSnapshot.moveSpeed.Value);
			moveFinished = moveInCurveSnapshot.moveFinished;
			return this;
		}


		public IMoveStrategy Setup(int walkableNavMask, Vector3 startDirection, float angularSpeed, float moveSpeed)
		{
			this.walkableNavMask = walkableNavMask;
			this.moveSpeed = moveSpeed;
			this.angularSpeed = angularSpeed;
			currentDirection = startDirection;
			moveFinished = false;
			return this;
		}


		private Vector3 _NextPosition(Vector3 position, float deltaTime)
		{
			currentDirection = Quaternion.AngleAxis(angularSpeed * deltaTime, Vector3.up) * currentDirection;
			Vector3 vector = position + moveSpeed * deltaTime * currentDirection.normalized;
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