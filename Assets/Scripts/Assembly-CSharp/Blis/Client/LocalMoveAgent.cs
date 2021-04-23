using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class LocalMoveAgent : MoveAgent
	{
		private CorrectionWarpContext correctionWarpContext;


		private bool isInBush;


		private LocalMoveToDestination moveToDestination;


		public override bool IsInBush => isInBush;


		public void ApplySnapshot(MoveAgentSnapshot snapshot, LocalWorld world)
		{
			switch (snapshot.moveStrategyType)
			{
				case MoveStrategyType.None:
					moveStrategy = null;
					break;
				case MoveStrategyType.InCurve:
					if (moveInCurve == null)
					{
						moveInCurve = new MoveInCurve();
					}

					moveStrategy = moveInCurve.Init(snapshot.moveStrategySnapshot);
					break;
				case MoveStrategyType.InDirection:
					if (moveInDirection == null)
					{
						moveInDirection = new MoveInDirection();
					}

					moveStrategy = moveInDirection.Init(snapshot.moveStrategySnapshot);
					break;
				case MoveStrategyType.ToDestination:
					if (moveToDestination == null)
					{
						moveToDestination = new LocalMoveToDestination();
					}

					moveStrategy = moveToDestination.Init(snapshot.moveStrategySnapshot);
					break;
				case MoveStrategyType.Straight:
					if (moveStraight == null)
					{
						moveStraight = new MoveStraight();
					}

					moveStrategy = moveStraight.Init(snapshot.moveStrategySnapshot);
					break;
				case MoveStrategyType.StraightWithoutNav:
					if (moveStraightWithoutNav == null)
					{
						moveStraightWithoutNav = new MoveStraightWithoutNav();
					}

					moveStrategy = moveStraightWithoutNav.Init(snapshot.moveStrategySnapshot);
					break;
				case MoveStrategyType.ToTargetWithoutNav:
					if (moveToTargetWithoutNav == null)
					{
						moveToTargetWithoutNav = new MoveToTargetWithoutNav();
					}

					moveStrategy = moveToTargetWithoutNav.Init<LocalObject>(snapshot.moveStrategySnapshot, world);
					break;
			}

			correctionWarpContext = null;
			lockRotation = snapshot.lockRotation;
			startRotation = Quaternion.Euler(snapshot.startRotation.ToVector3());
			targetRotation = Quaternion.Euler(snapshot.targetRotation.ToVector3());
			targetRotationPeriod = snapshot.targetRotationPeriod.Value;
			targetRotationTimeStack = snapshot.targetRotationTimeStack.Value;
			walkableNavMask = snapshot.walkableNavMask;
			isInBush = snapshot.isInBush;
		}


		protected override MoveToDestination GetMovetoDestination()
		{
			return moveToDestination;
		}


		public void MoveToDestination(BlisVector startPosition, BlisVector destination, BlisVector[] corners,
			float moveSpeed)
		{
			if (moveToDestination == null)
			{
				moveToDestination = new LocalMoveToDestination();
			}

			moveStrategy = moveToDestination.Setup(walkableNavMask, startPosition, destination, corners, moveSpeed);
		}


		public override void FrameUpdate(float deltaTime)
		{
			if (correctionWarpContext != null)
			{
				transform.position = correctionWarpContext.NextPosition(deltaTime);
				float remainTick = correctionWarpContext.RemainTick;
				if (remainTick < 0f)
				{
					return;
				}

				deltaTime = remainTick;
				correctionWarpContext = null;
			}

			base.FrameUpdate(deltaTime);
		}


		public bool PositioningCorrection(Vector3 startPosition, float timeDelta)
		{
			Vector3 vector = startPosition;
			if (moveStrategy != null && timeDelta > 0f)
			{
				vector = NextPosition(startPosition, timeDelta);
			}

			float num = GameUtil.DistanceOnPlane(vector, transform.position);
			if (num > 1f)
			{
				Log.W("[WARP] duration: {0}, currentDistance: {1}, correctedDistance: {2}", timeDelta,
					(startPosition - transform.position).magnitude,
					Vector3.Scale(vector - transform.position, new Vector3(1f, 0f, 1f)).magnitude);
				StartCorrectionWarp(transform.position, vector, num / 10f);
				return true;
			}

			return false;
		}


		protected void StartCorrectionWarp(Vector3 start, Vector3 target, float duration)
		{
			Vector3 target2 = target;
			if (moveStrategy != null)
			{
				target2 = NextPosition(target, duration);
			}

			correctionWarpContext = new CorrectionWarpContext(start, target2, duration);
		}


		public void InstanceLookAt(Quaternion rotation)
		{
			transform.rotation = rotation;
		}


		private class CorrectionWarpContext
		{
			private readonly float duration;


			private readonly Vector3 start;


			private readonly Vector3 target;


			private float tick;


			public CorrectionWarpContext(Vector3 start, Vector3 target, float duration)
			{
				this.start = start;
				this.target = target;
				this.duration = duration;
				tick = 0f;
			}


			public float RemainTick => tick - duration;


			public Vector3 NextPosition(float deltaTime)
			{
				tick += deltaTime;
				return Vector3.Lerp(start, target, Mathf.Min(1f, tick / duration));
			}
		}
	}
}