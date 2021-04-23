using UnityEngine;

namespace Blis.Common
{
	public class MoveStraight : IMoveStrategy
	{
		private bool arrived;


		private bool canRotate;


		private Vector3 destination;


		private float duration;


		private EasingFunction.Ease ease;


		private float elapsedTime;


		private Vector3 startPosition;


		private int walkableNavMask;

		public MoveStrategyType GetStrategyType()
		{
			return MoveStrategyType.Straight;
		}


		public Vector3 NextPosition(Vector3 position, float deltaTime)
		{
			if (arrived)
			{
				return position;
			}

			if (duration.Equals(0f))
			{
				arrived = true;
				return position;
			}

			if (deltaTime <= 0.033333335f)
			{
				return _NextPosition(deltaTime);
			}

			Vector3 result = position;
			float num = deltaTime;
			while (num > 0f)
			{
				result = _NextPosition(Mathf.Min(0.033333335f, num));
				num -= 0.033333335f;
				if (MoveFinished())
				{
					break;
				}
			}

			return result;
		}


		public void UpdateMoveSpeed(float moveSpeed) { }


		public bool MoveFinished()
		{
			return arrived;
		}


		public void SetRotateWhileMoving(bool isRotation)
		{
			canRotate = isRotation;
		}


		public bool CanRotateWhileMoving()
		{
			return canRotate;
		}


		public void ResetDestination(Vector3 destination)
		{
			this.destination = destination;
		}


		public byte[] CreateSnapshot()
		{
			return Serializer.Default.Serialize<MoveStraightSnapshot>(new MoveStraightSnapshot
			{
				arrived = arrived,
				startPosition = new BlisVector(startPosition),
				destination = new BlisVector(destination),
				duration = new BlisFixedPoint(duration),
				ease = ease,
				canRotate = canRotate,
				elapsedTime = new BlisFixedPoint(elapsedTime),
				walkableNavMask = walkableNavMask
			});
		}


		public IMoveStrategy Init(byte[] snapshotData)
		{
			MoveStraightSnapshot moveStraightSnapshot =
				Serializer.Default.Deserialize<MoveStraightSnapshot>(snapshotData);
			Setup(moveStraightSnapshot.walkableNavMask, moveStraightSnapshot.startPosition,
				moveStraightSnapshot.destination, moveStraightSnapshot.duration.Value, moveStraightSnapshot.ease,
				moveStraightSnapshot.canRotate);
			elapsedTime = moveStraightSnapshot.elapsedTime.Value;
			return this;
		}


		private float Easing(EasingFunction.Ease ease, float t, float e, float v)
		{
			return EasingFunction.GetEasingFunction(ease)(t, e, v);
		}


		public IMoveStrategy Setup(int walkableNavMask, BlisVector startPosition, BlisVector destination,
			float duration, EasingFunction.Ease ease, bool canRotate)
		{
			this.walkableNavMask = walkableNavMask;
			arrived = false;
			this.startPosition = startPosition.ToVector3();
			this.destination = destination.SamplePosition();
			this.duration = duration;
			this.ease = ease;
			elapsedTime = 0f;
			this.canRotate = canRotate;
			return this;
		}


		private Vector3 _NextPosition(float deltaTime)
		{
			float num;
			if (duration == 0f)
			{
				num = 1f;
			}
			else
			{
				num = elapsedTime / duration;
			}

			if (1f <= num)
			{
				arrived = true;
				canRotate = false;
				num = 1f;
			}

			Vector3 result = Vector3.Lerp(startPosition, destination, Easing(ease, 0f, 1f, num));
			elapsedTime += deltaTime;
			return result;
		}
	}
}