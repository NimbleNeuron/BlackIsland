using System.Collections.Generic;
using UnityEngine;

namespace Blis.Common
{
	public abstract class MoveToDestination : IMoveStrategy
	{
		private static readonly Vector3 planeVector = new Vector3(1f, 0f, 1f);


		protected bool arrived;


		private bool canRotate;


		protected Vector3 destination;


		protected float moveSpeed;


		protected Vector3 startPosition;


		protected int walkableNavMask;


		public Vector3 Destination => destination;

		public MoveStrategyType GetStrategyType()
		{
			return MoveStrategyType.ToDestination;
		}


		public abstract Vector3 NextPosition(Vector3 position, float deltaTime);


		public void UpdateMoveSpeed(float moveSpeed)
		{
			this.moveSpeed = moveSpeed;
		}


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


		public virtual byte[] CreateSnapshot()
		{
			return null;
		}


		public abstract List<Vector3> GetCorners();


		public virtual IMoveStrategy Setup(int walkableNavMask, BlisVector startPosition, BlisVector destination,
			float moveSpeed)
		{
			this.walkableNavMask = walkableNavMask;
			this.startPosition = startPosition.SamplePosition();
			this.destination = destination.SamplePosition();
			arrived = false;
			this.moveSpeed = moveSpeed;
			return this;
		}


		protected Vector3 CalcDelta(Vector3 position, Vector3 target)
		{
			return Vector3.Scale(target - position, planeVector);
		}
	}
}