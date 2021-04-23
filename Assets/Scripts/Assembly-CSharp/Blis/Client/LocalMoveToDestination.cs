using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Client
{
	public class LocalMoveToDestination : MoveToDestination
	{
		private readonly Queue<Vector3> corners = new Queue<Vector3>();


		private readonly NavMeshPath navMeshPath = new NavMeshPath();


		private readonly List<Vector3> path = new List<Vector3>();


		private Vector3 currentTarget;

		public IMoveStrategy Setup(int walkableNavMask, BlisVector startPosition, BlisVector destination,
			BlisVector[] corners, float moveSpeed)
		{
			Setup(walkableNavMask, startPosition, destination, moveSpeed);
			currentTarget = Vector3.zero;
			SetPath(corners);
			return this;
		}


		private void SetPath(BlisVector[] corners)
		{
			this.corners.Clear();
			if (corners != null)
			{
				for (int i = 0; i < corners.Length; i++)
				{
					this.corners.Enqueue(corners[i].ToVector3());
				}
			}

			currentTarget = 0 < this.corners.Count ? this.corners.Peek() : destination;
		}


		public IMoveStrategy Init(byte[] snapshotData)
		{
			MoveToDestinationSnapshot moveToDestinationSnapshot =
				Serializer.Default.Deserialize<MoveToDestinationSnapshot>(snapshotData);
			Setup(moveToDestinationSnapshot.walkableNavMask, moveToDestinationSnapshot.startPosition,
				moveToDestinationSnapshot.destination, moveToDestinationSnapshot.corners,
				moveToDestinationSnapshot.moveSpeed.Value);
			arrived = moveToDestinationSnapshot.arrived;
			return this;
		}


		public override Vector3 NextPosition(Vector3 position, float deltaTime)
		{
			if (arrived)
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


		private Vector3 _NextPosition(Vector3 position, float deltaTime)
		{
			Vector3 b = CalcDelta(position, currentTarget);
			float num = moveSpeed * deltaTime;
			if (b.sqrMagnitude <= Mathf.Pow(num, 2f))
			{
				if (corners.Count == 0)
				{
					arrived = true;
				}
				else
				{
					currentTarget = corners.Dequeue();
				}

				return position + b;
			}

			return position + num * b.normalized;
		}


		public override List<Vector3> GetCorners()
		{
			path.Clear();
			if (NavMesh.CalculatePath(startPosition, destination, walkableNavMask, navMeshPath))
			{
				path.AddRange(navMeshPath.corners);
			}
			else if (0 < corners.Count)
			{
				path.AddRange(corners);
			}

			return path;
		}
	}
}