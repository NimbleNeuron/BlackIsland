using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Blis.Common
{
	public class NavPath
	{
		public const float StoppingDistance = 0.5f;


		public readonly List<Vector3> corners = new List<Vector3>();


		private Vector3 destination;


		public int index;

		public Vector3 GetDestination()
		{
			return destination;
		}


		public void EnqueuePath(NavMeshPath path, Vector3 destination)
		{
			corners.Clear();
			corners.AddRange(path.corners);
			corners.RemoveAt(0);
			index = 0;
			this.destination = destination;
		}


		public bool PeekTarget(out Vector3 target)
		{
			target = Vector3.zero;
			if (corners.Count == 0 || corners.Count <= index)
			{
				return false;
			}

			target = corners.ElementAt(index);
			return true;
		}


		public bool NextTarget(out Vector3 target)
		{
			target = Vector3.zero;
			index++;
			return index < corners.Count && PeekTarget(out target);
		}


		public void Reset()
		{
			corners.Clear();
			index = 0;
		}


		public void DrawDebug(Vector3 position)
		{
			if (corners.Count > 0)
			{
				Debug.DrawLine(position, corners.ElementAt(0));
				for (int i = 1; i < corners.Count; i++)
				{
					Debug.DrawLine(corners.ElementAt(i - 1), corners.ElementAt(i), Color.red);
				}
			}
		}


		public bool HasPath()
		{
			return corners.Count > 0;
		}


		public bool CheckSkipCorner(Vector3 position, ref Vector3 target)
		{
			if (index + 1 < corners.Count)
			{
				Vector3 vector = corners.ElementAt(index + 1);
				NavMeshHit navMeshHit;
				if (!NavMesh.Raycast(position, vector, out navMeshHit, 2147483640))
				{
					index++;
					target = vector;
					return true;
				}
			}

			return false;
		}
	}
}