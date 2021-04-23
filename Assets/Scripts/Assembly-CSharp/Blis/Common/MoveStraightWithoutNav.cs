using UnityEngine;

namespace Blis.Common
{
	
	public class MoveStraightWithoutNav : IMoveStrategy
	{
		
		public MoveStrategyType GetStrategyType()
		{
			return MoveStrategyType.StraightWithoutNav;
		}

		
		public byte[] CreateSnapshot()
		{
			return Serializer.Default.Serialize<MoveStraightWithoutNavSnapshot>(new MoveStraightWithoutNavSnapshot
			{
				moveStartPos = new BlisVector(this.moveStartPos),
				moveEndPos = new BlisVector(this.moveEndPos),
				duration = new BlisFixedPoint(this.duration),
				lerpRate = new BlisFixedPoint(this.lerpRate),
				timeStack = new BlisFixedPoint(this.timeStack),
				walkableNavMask = this.walkableNavMask
			});
		}

		
		public IMoveStrategy Init(byte[] snapshotData)
		{
			MoveStraightWithoutNavSnapshot moveStraightWithoutNavSnapshot = Serializer.Default.Deserialize<MoveStraightWithoutNavSnapshot>(snapshotData);
			this.Setup(moveStraightWithoutNavSnapshot.moveStartPos.ToVector3(), moveStraightWithoutNavSnapshot.moveEndPos.ToVector3(), moveStraightWithoutNavSnapshot.duration.Value, moveStraightWithoutNavSnapshot.walkableNavMask);
			this.lerpRate = moveStraightWithoutNavSnapshot.lerpRate.Value;
			this.timeStack = moveStraightWithoutNavSnapshot.timeStack.Value;
			return this;
		}

		
		public IMoveStrategy Setup(Vector3 moveStartPos, Vector3 moveEndPos, float duration, int walkableNavMask)
		{
			this.moveStartPos = moveStartPos;
			this.moveEndPos = moveEndPos;
			this.duration = duration;
			this.timeStack = 0f;
			this.lerpRate = ((duration > 0f) ? 0f : 1f);
			this.walkableNavMask = walkableNavMask;
			return this;
		}

		
		public Vector3 NextPosition(Vector3 position, float deltaTime)
		{
			if (this.lerpRate >= 1f)
			{
				return this.moveEndPos;
			}
			this.timeStack += deltaTime;
			this.lerpRate = this.timeStack / this.duration;
			this.lerpRate = Mathf.Min(1f, this.lerpRate);
			Vector3 result = Vector3.Lerp(this.moveStartPos, this.moveEndPos, this.lerpRate);
			Vector3 vector;
			if (MoveAgent.SamplePosition(position, this.walkableNavMask, out vector))
			{
				result.y = vector.y;
			}
			else
			{
				result.y = position.y;
			}
			return result;
		}

		
		public void UpdateMoveSpeed(float moveSpeed)
		{
		}

		
		public bool MoveFinished()
		{
			return this.lerpRate >= 1f;
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
			this.moveEndPos = destination;
		}

		
		private bool canRotate;

		
		private Vector3 moveStartPos;

		
		private Vector3 moveEndPos;

		
		private float duration;

		
		private float lerpRate = 1f;

		
		private float timeStack;

		
		private int walkableNavMask;
	}
}
