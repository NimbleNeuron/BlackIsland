using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class IvyContainer : ScriptableObject
	{
		public int lastNumberAssigned;


		public GameObject ivyGO;


		public List<BranchContainer> branches;


		public Vector3 firstVertexVector;


		private IvyContainer()
		{
			branches = new List<BranchContainer>();
			lastNumberAssigned = 0;
		}


		public void Clear()
		{
			lastNumberAssigned = 0;
			branches.Clear();
		}


		public void RemoveBranch(BranchContainer branchToDelete)
		{
			if (branchToDelete.originPointOfThisBranch != null)
			{
				branchToDelete.originPointOfThisBranch.branchContainer.ReleasePoint(branchToDelete
					.originPointOfThisBranch.index);
			}

			branches.Remove(branchToDelete);
		}


		public BranchContainer GetBranchContainerByBranchNumber(int branchNumber)
		{
			BranchContainer result = null;
			for (int i = 0; i < branches.Count; i++)
			{
				if (branches[i].branchNumber == branchNumber)
				{
					result = branches[i];
					break;
				}
			}

			return result;
		}


		public BranchPoint[] GetNearestSegmentSSBelowDistance(Vector2 pointSS, float distanceThreshold)
		{
			BranchPoint[] result = null;
			BranchPoint branchPoint = null;
			BranchPoint branchPoint2 = null;
			float num = distanceThreshold;
			for (int i = 0; i < branches.Count; i++)
			{
				for (int j = 1; j < branches[i].branchPoints.Count; j++)
				{
					BranchPoint branchPoint3 = branches[i].branchPoints[j - 1];
					BranchPoint branchPoint4 = branches[i].branchPoints[j];
					float num2 =
						RealIvyMathUtils.DistanceBetweenPointAndSegmentSS(pointSS, branchPoint3.pointSS,
							branchPoint4.pointSS);
					if (num2 <= num)
					{
						num = num2;
						branchPoint = branchPoint3;
						branchPoint2 = branchPoint4;
					}
				}
			}

			if (branchPoint != null && branchPoint2 != null)
			{
				result = new[]
				{
					branchPoint,
					branchPoint2
				};
			}

			return result;
		}


		public BranchPoint[] GetNearestSegmentSS(Vector2 pointSS)
		{
			return GetNearestSegmentSSBelowDistance(pointSS, float.MaxValue);
		}


		public void AddBranch(BranchContainer newBranchContainer)
		{
			newBranchContainer.branchNumber = lastNumberAssigned;
			lastNumberAssigned++;
			branches.Add(newBranchContainer);
		}


		public BranchPoint GetNearestPointAllBranchesSSFrom(Vector2 pointSS)
		{
			BranchPoint result = null;
			float num = float.MaxValue;
			for (int i = 0; i < branches.Count; i++)
			{
				for (int j = 0; j < branches[i].branchPoints.Count; j++)
				{
					float sqrMagnitude = (branches[i].branchPoints[j].pointSS - pointSS).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						result = branches[i].branchPoints[j];
						num = sqrMagnitude;
					}
				}
			}

			return result;
		}
	}
}