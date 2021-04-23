using UnityEngine;
using UnityEngine.AI;

namespace Blis.Common
{
	public abstract class MoveAgent : MonoBehaviour
	{
		private static readonly Vector3 planeVector = new Vector3(1f, 0f, 1f);


		private float angularPeriod = 0.3f;


		private bool isPaused;


		protected int lockRotation;


		protected MoveInCurve moveInCurve;


		protected MoveInDirection moveInDirection;


		protected MoveStraight moveStraight;


		protected MoveStraightWithoutNav moveStraightWithoutNav;


		protected IMoveStrategy moveStrategy;


		protected MoveToTargetWithoutNav moveToTargetWithoutNav;


		protected Quaternion startRotation;


		protected Quaternion targetRotation;


		protected float targetRotationPeriod;


		protected float targetRotationTimeStack;


		protected int walkableNavMask;


		public IMoveStrategy CurrentMoveStrategy => moveStrategy;


		public Quaternion TargetRotation => targetRotation;


		public int WalkableNavMask => walkableNavMask;


		public virtual bool IsInBush => false;


		public MoveStrategyType CurrentMoveStrategyType {
			get
			{
				if (moveStrategy == null)
				{
					return MoveStrategyType.None;
				}

				return moveStrategy.GetStrategyType();
			}
		}


		public bool IsMoving => !isPaused && moveStrategy != null;


		protected abstract MoveToDestination GetMovetoDestination();


		public void SetWalkableNavMask(int navMask)
		{
			walkableNavMask = navMask;
		}


		public void SetWalkableAreas(int[] areaCodes)
		{
			if (areaCodes == null)
			{
				return;
			}

			int num = 0;
			foreach (int num2 in areaCodes)
			{
				num |= 1 << (num2 + 9);
			}

			walkableNavMask = num;
		}


		public void AddWalkableAreas(int[] areaCodes)
		{
			if (areaCodes == null)
			{
				return;
			}

			foreach (int num in areaCodes)
			{
				walkableNavMask |= 1 << (num + 9);
			}
		}


		public virtual void Init(float angularSpeed, int walkableNavMask)
		{
			angularPeriod = angularSpeed;
			this.walkableNavMask = walkableNavMask;
			startRotation = transform.rotation;
			targetRotation = startRotation;
			targetRotationPeriod = 0f;
			targetRotationTimeStack = 0f;
		}


		public void MoveInCurve(Vector3 startDirection, float angularSpeed, float moveSpeed)
		{
			if (moveInCurve == null)
			{
				moveInCurve = new MoveInCurve();
			}

			moveStrategy = moveInCurve.Setup(walkableNavMask, startDirection, angularSpeed, moveSpeed);
		}


		public void MoveInDirection(Vector3 direction, float moveSpeed)
		{
			if (moveInDirection == null)
			{
				moveInDirection = new MoveInDirection();
			}

			moveStrategy = moveInDirection.Setup(walkableNavMask, direction, moveSpeed);
		}


		public void MoveStraight(BlisVector startPosition, BlisVector destination, float duration,
			EasingFunction.Ease ease, bool canRotate = false)
		{
			if (moveStraight == null)
			{
				moveStraight = new MoveStraight();
			}

			moveStrategy = moveStraight.Setup(walkableNavMask, startPosition, destination, duration, ease, canRotate);
		}


		public void MoveStraightWithoutNav(Vector3 moveStartPos, Vector3 moveEndPos, float duration)
		{
			if (moveStraightWithoutNav == null)
			{
				moveStraightWithoutNav = new MoveStraightWithoutNav();
			}

			moveStrategy = moveStraightWithoutNav.Setup(moveStartPos, moveEndPos, duration, walkableNavMask);
		}


		public void MoveToTargetWithoutNav(Vector3 moveStartPos, ObjectBase target, float moveSpeed, float arriveRadius)
		{
			if (moveToTargetWithoutNav == null)
			{
				moveToTargetWithoutNav = new MoveToTargetWithoutNav();
			}

			moveStrategy = moveToTargetWithoutNav.Setup(moveStartPos, target, moveSpeed, arriveRadius, walkableNavMask);
		}


		public void UpdateMoveSpeed(float moveSpeed)
		{
			IMoveStrategy moveStrategy = this.moveStrategy;
			if (moveStrategy == null)
			{
				return;
			}

			moveStrategy.UpdateMoveSpeed(moveSpeed);
		}


		public void Warp(BlisVector position)
		{
			Warp(position.ToVector3());
		}


		protected virtual void Warp(Vector3 position)
		{
			transform.position = position;
			moveStrategy = null;
		}


		public virtual void Stop()
		{
			moveStrategy = null;
		}


		public void Pause()
		{
			isPaused = true;
		}


		public void Resume()
		{
			isPaused = false;
		}


		public virtual void FrameUpdate(float deltaTime)
		{
			if (isPaused)
			{
				return;
			}

			if (moveStrategy == null)
			{
				if (!IsLockRotation())
				{
					transform.rotation = UpdateRotation(deltaTime);
				}

				return;
			}

			Vector3 position = transform.position;
			bool flag = moveStrategy.CanRotateWhileMoving();
			Vector3 vector = NextPosition(position, deltaTime);
			if (IsLockRotation())
			{
				transform.position = vector;
				return;
			}

			if (flag)
			{
				transform.SetPositionAndRotation(vector, UpdateRotation(deltaTime));
				return;
			}

			transform.SetPositionAndRotation(vector, UpdateRotationDependMoveDirection(position, vector, deltaTime));
		}


		public void LookAt(Quaternion lookFrom, Quaternion lookTo, float duration)
		{
			startRotation = lookFrom;
			targetRotation = lookTo;
			targetRotationTimeStack = 0f;
			if (duration <= 0.01f)
			{
				if (!IsLockRotation())
				{
					transform.rotation = targetRotation;
				}

				targetRotationPeriod = 0f;
				return;
			}

			targetRotationPeriod = duration;
		}


		private Quaternion UpdateRotationDependMoveDirection(Vector3 position, Vector3 nextPosition, float deltaTime)
		{
			if (GameUtil.DistanceOnPlane(position, nextPosition) < 1E-45f)
			{
				return UpdateRotation(deltaTime);
			}

			Quaternion b = GameUtil.LookRotation(Vector3.Scale((nextPosition - position).normalized, planeVector),
				Vector3.up);
			if (Quaternion.Angle(targetRotation, b) <= 1f)
			{
				return UpdateRotation(deltaTime);
			}

			startRotation = transform.rotation;
			if (angularPeriod == 0f)
			{
				targetRotationPeriod = 0f;
			}
			else
			{
				float num = angularPeriod - targetRotationTimeStack;
				if (num <= 0f)
				{
					targetRotationPeriod = angularPeriod;
				}
				else
				{
					Vector3 eulerAngles = startRotation.eulerAngles;
					float num2 = eulerAngles.y - b.eulerAngles.y;
					float num3 = eulerAngles.y - targetRotation.eulerAngles.y;
					if (num2 > 180f)
					{
						while (num2 > 180f)
						{
							num2 -= 360f;
						}
					}
					else if (num2 < -180f)
					{
						while (num2 < -180f)
						{
							num2 = 360f + num2;
						}
					}

					if (num3 > 180f)
					{
						while (num3 > 180f)
						{
							num3 -= 360f;
						}
					}
					else if (num3 < -180f)
					{
						while (num3 < -180f)
						{
							num3 = 360f + num3;
						}
					}

					if (num2 > 0f && num3 > 0f || num2 < 0f && num3 < 0f)
					{
						float num4 = num3 / num;
						targetRotationPeriod = Mathf.Min(angularPeriod, num2 / num4);
					}
					else
					{
						targetRotationPeriod = angularPeriod;
					}
				}
			}

			targetRotationTimeStack = 0f;
			targetRotation = b;
			return UpdateRotation(deltaTime);
		}


		private Quaternion UpdateRotation(float deltaTime)
		{
			if (targetRotationPeriod == 0f || transform.rotation.Equals(targetRotation))
			{
				startRotation = targetRotation;
				targetRotationPeriod = 0f;
				targetRotationTimeStack = 0f;
				return targetRotation;
			}

			targetRotationTimeStack += deltaTime;
			float num = targetRotationTimeStack / targetRotationPeriod;
			Quaternion result = Quaternion.Lerp(startRotation, targetRotation, num);
			if (num == 1f)
			{
				startRotation = targetRotation;
				targetRotationPeriod = 0f;
				targetRotationTimeStack = 0f;
			}

			return result;
		}


		protected Vector3 NextPosition(Vector3 currentPosition, float deltaTime)
		{
			if (CurrentMoveStrategy == null)
			{
				return currentPosition;
			}

			Vector3 vector = moveStrategy.NextPosition(currentPosition, deltaTime);
			NavMeshHit navMeshHit;
			if (NavMesh.SamplePosition(vector, out navMeshHit, 1f, walkableNavMask))
			{
				vector = new Vector3(vector.x, navMeshHit.position.y, vector.z);
			}

			if (moveStrategy.MoveFinished())
			{
				Stop();
			}

			return vector;
		}


		public bool IsLockRotation()
		{
			return 0 < lockRotation;
		}


		public void LockRotation(bool lockRotation)
		{
			if (lockRotation)
			{
				this.lockRotation++;
				return;
			}

			this.lockRotation--;
		}


		public static bool CanStandToElaboratePosition(Vector3 position, int walkableNavMask)
		{
			NavMeshHit navMeshHit;
			NavMesh.Raycast(position, position, out navMeshHit, walkableNavMask);
			return navMeshHit.mask != 0;
		}


		public static bool CanStandToPosition(Vector3 position, int walkableNavMask, out Vector3 sampledPosition)
		{
			return CanStandToPosition(position, walkableNavMask, 1f, out sampledPosition);
		}


		public static bool CanStandToPosition(Vector3 position, int walkableNavMask, float maxDistance,
			out Vector3 sampledPosition)
		{
			NavMeshHit navMeshHit;
			if (!NavMesh.SamplePosition(position, out navMeshHit, maxDistance, walkableNavMask))
			{
				sampledPosition = position;
				return false;
			}

			sampledPosition = navMeshHit.position;
			return true;
		}


		public static bool SamplePosition(Vector3 position, int walkableNavMask, out Vector3 sampledPosition)
		{
			NavMeshHit navMeshHit;
			if (!NavMesh.SamplePosition(position, out navMeshHit, 1f, walkableNavMask)
			    && !NavMesh.SamplePosition(position, out navMeshHit, 2f, walkableNavMask)
			    && !NavMesh.SamplePosition(position, out navMeshHit, 5f, walkableNavMask))
			{
				sampledPosition = position;
				return false;
			}

			sampledPosition = navMeshHit.position;
			return true;
		}


		public static bool SampleWidePosition(Vector3 position, int walkableNavMask, out Vector3 sampledPosition)
		{
			NavMeshHit navMeshHit;
			if (!NavMesh.SamplePosition(position, out navMeshHit, 5f, walkableNavMask) &&
			    !NavMesh.SamplePosition(position, out navMeshHit, 15f, walkableNavMask) &&
			    !NavMesh.SamplePosition(position, out navMeshHit, 25f, walkableNavMask))
			{
				sampledPosition = position;
				return false;
			}

			sampledPosition = navMeshHit.position;
			return true;
		}


		public static bool CanStraightMoveToDestination(Vector3 position, Vector3 destination, int walkableNavMask,
			out Vector3 nearestDestination)
		{
			NavMeshHit navMeshHit;
			if (NavMesh.Raycast(position, destination, out navMeshHit, walkableNavMask))
			{
				nearestDestination = navMeshHit.position;
				return false;
			}

			nearestDestination = destination;
			return true;
		}


		public void SetRotateWhileMoving(bool isRotation)
		{
			if (moveStrategy != null)
			{
				moveStrategy.SetRotateWhileMoving(isRotation);
			}
		}


		public void ResetDestination(Vector3 destination)
		{
			if (moveStrategy != null)
			{
				moveStrategy.ResetDestination(destination);
			}
		}
	}
}