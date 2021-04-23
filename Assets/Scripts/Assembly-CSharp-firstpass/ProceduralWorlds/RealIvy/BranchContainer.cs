using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class BranchContainer : ScriptableObject
	{
		public List<BranchPoint> branchPoints;


		public Vector3 growDirection;


		public List<LeafPoint> leaves;


		public float totalLenght;


		public float fallIteration;


		public bool falling;


		public Quaternion rotationOnFallIteration;


		public int branchSense;


		public float heightParameter;


		public float randomizeHeight;


		public float heightVar;


		public float currentHeight;


		public float deltaHeight;


		public float newHeight;


		public BranchPoint originPointOfThisBranch;


		public int branchNumber;


		public Dictionary<int, List<LeafPoint>> dictRTLeavesByInitSegment;


		public int GetNumLeaves()
		{
			return leaves.Count;
		}


		public void SetValues(Vector3 growDirection, float randomizeHeight, float currentHeight, float heightParameter,
			int branchSense, BranchPoint originPointOfThisBranch)
		{
			branchPoints = new List<BranchPoint>(1000);
			this.growDirection = growDirection;
			leaves = new List<LeafPoint>(1000);
			totalLenght = 0f;
			fallIteration = 0f;
			falling = false;
			rotationOnFallIteration = Quaternion.identity;
			this.branchSense = branchSense;
			this.heightParameter = heightParameter;
			this.randomizeHeight = randomizeHeight;
			heightVar = 0f;
			this.currentHeight = currentHeight;
			deltaHeight = 0f;
			newHeight = 0f;
			this.originPointOfThisBranch = originPointOfThisBranch;
			branchNumber = -1;
		}


		public void AddBranchPoint(BranchPoint branchPoint, float length, float stepSize)
		{
			branchPoint.branchContainer = this;
			branchPoints.Add(branchPoint);
		}


		public void Init(int branchPointsSize, int numLeaves)
		{
			branchPoints = new List<BranchPoint>(branchPointsSize * 2);
			leaves = new List<LeafPoint>(numLeaves * 2);
		}


		public void Init()
		{
			Init(0, 0);
		}


		public void PrepareRTLeavesDict()
		{
			dictRTLeavesByInitSegment = new Dictionary<int, List<LeafPoint>>();
			for (int i = 0; i < branchPoints.Count; i++)
			{
				List<LeafPoint> list = new List<LeafPoint>();
				GetLeavesInSegment(branchPoints[i], list);
				dictRTLeavesByInitSegment[i] = list;
			}
		}


		public void UpdateLeavesDictEntry(int initSegmentIdx, LeafPoint leaf)
		{
			if (dictRTLeavesByInitSegment.ContainsKey(initSegmentIdx))
			{
				dictRTLeavesByInitSegment[initSegmentIdx].Add(leaf);
				return;
			}

			List<LeafPoint> list = new List<LeafPoint>();
			list.Add(leaf);
			dictRTLeavesByInitSegment[initSegmentIdx] = list;
		}


		public void AddBranchPoint(BranchPoint branchPoint)
		{
			branchPoint.index = branchPoints.Count;
			branchPoint.newBranch = false;
			branchPoint.newBranchNumber = -1;
			branchPoint.branchContainer = this;
			branchPoint.length = totalLenght;
			branchPoints.Add(branchPoint);
		}


		public void AddBranchPoint(Vector3 point, Vector3 grabVector)
		{
			AddBranchPoint(point, grabVector, false, -1);
		}


		public void AddBranchPoint(Vector3 point, Vector3 grabVector, bool isNewBranch, int newBranchIndex)
		{
			BranchPoint item = new BranchPoint(point, grabVector, branchPoints.Count, isNewBranch, newBranchIndex,
				totalLenght, this);
			branchPoints.Add(item);
		}


		public BranchPoint InsertBranchPoint(Vector3 point, Vector3 grabVector, int index)
		{
			float length = Mathf.Lerp(branchPoints[index - 1].length, branchPoints[index].length, 0.5f);
			BranchPoint branchPoint = new BranchPoint(point, grabVector, index, length, this);
			branchPoints.Insert(index, branchPoint);
			for (int i = index + 1; i < branchPoints.Count; i++)
			{
				branchPoints[i].index++;
			}

			return branchPoint;
		}


		public void GetLeavesInSegmentRT(int initSegmentIdx, int endSegmentIdx, List<LeafPoint> res)
		{
			for (int i = initSegmentIdx; i <= endSegmentIdx; i++)
			{
				if (dictRTLeavesByInitSegment.ContainsKey(i))
				{
					res.AddRange(dictRTLeavesByInitSegment[i]);
				}
			}
		}


		public void GetLeavesInSegment(BranchPoint initSegment, List<LeafPoint> res)
		{
			for (int i = 0; i < leaves.Count; i++)
			{
				if (leaves[i].initSegmentIdx == initSegment.index)
				{
					res.Add(leaves[i]);
				}
			}
		}


		public List<LeafPoint> GetLeavesInSegment(BranchPoint initSegment)
		{
			List<LeafPoint> list = new List<LeafPoint>();
			GetLeavesInSegment(initSegment, list);
			return list;
		}


		public LeafPoint AddRandomLeaf(Vector3 pointWS, BranchPoint initSegment, BranchPoint endSegment, int leafIndex,
			InfoPool infoPool)
		{
			int chosenLeave = Random.Range(0, infoPool.ivyParameters.leavesPrefabs.Length);
			Vector3 initialGrowDir = initSegment.initialGrowDir;
			float lpLength = initSegment.length + Vector3.Distance(pointWS, initSegment.point);
			return InsertLeaf(pointWS, lpLength, initialGrowDir, -initSegment.grabVector, chosenLeave, leafIndex,
				initSegment, endSegment);
		}


		public void RepositionLeavesAfterAdd02(BranchPoint newPoint)
		{
			BranchPoint previousPoint = newPoint.GetPreviousPoint();
			BranchPoint nextPoint = newPoint.GetNextPoint();
			List<LeafPoint> list = new List<LeafPoint>();
			GetLeavesInSegment(previousPoint, list);
			Vector3 normalized = (newPoint.point - previousPoint.point).normalized;
			Vector3 normalized2 = (nextPoint.point - newPoint.point).normalized;
			for (int i = 0; i < list.Count; i++)
			{
				Vector3 lhs = list[i].point - branchPoints[list[i].initSegmentIdx].point;
				Vector3 lhs2 = list[i].point - branchPoints[list[i].endSegmentIdx].point;
				Vector3 vector = previousPoint.point + normalized * Vector3.Dot(lhs, normalized);
				Vector3 point = nextPoint.point + normalized2 * Vector3.Dot(lhs2, normalized2);
				if (Vector3.Dot(newPoint.point - vector, normalized) >= 0f)
				{
					list[i].SetValues(vector, list[i].lpLength, normalized, list[i].lpUpward, list[i].chosenLeave,
						previousPoint, newPoint);
				}
				else
				{
					list[i].SetValues(point, list[i].lpLength, normalized2, list[i].lpUpward, list[i].chosenLeave,
						newPoint, nextPoint);
				}
			}
		}


		public void RepositionLeavesAfterRemove02(BranchPoint removedPoint)
		{
			BranchPoint previousPoint = removedPoint.GetPreviousPoint();
			BranchPoint nextPoint = removedPoint.GetNextPoint();
			List<LeafPoint> leavesInSegment = GetLeavesInSegment(previousPoint);
			leavesInSegment.AddRange(GetLeavesInSegment(removedPoint));
			for (int i = 0; i < leavesInSegment.Count; i++)
			{
				Vector3 lhs = leavesInSegment[i].point - previousPoint.point;
				Vector3 normalized = (nextPoint.point - previousPoint.point).normalized;
				float d = Vector3.Dot(lhs, normalized);
				Vector3 point = previousPoint.point + normalized * d;
				leavesInSegment[i].SetValues(point, leavesInSegment[i].lpLength, previousPoint.initialGrowDir,
					-previousPoint.grabVector, leavesInSegment[i].chosenLeave, previousPoint, nextPoint);
			}
		}


		public void RemoveBranchPoint(int indexToRemove)
		{
			RepositionLeavesAfterRemove02(branchPoints[indexToRemove]);
			for (int i = indexToRemove + 1; i < branchPoints.Count; i++)
			{
				List<LeafPoint> list = new List<LeafPoint>();
				GetLeavesInSegment(branchPoints[i], list);
				for (int j = 0; j < list.Count; j++)
				{
					list[j].initSegmentIdx--;
					list[j].endSegmentIdx--;
				}

				branchPoints[i].index--;
			}

			branchPoints.RemoveAt(indexToRemove);
		}


		public void RemoveRange(int index, int count)
		{
			List<LeafPoint> list = new List<LeafPoint>();
			for (int i = index; i < index + count; i++)
			{
				GetLeavesInSegment(branchPoints[i], list);
			}

			for (int j = 0; j < list.Count; j++)
			{
				leaves.Remove(list[j]);
			}

			for (int k = index + count; k < branchPoints.Count; k++)
			{
				branchPoints[k].index--;
			}

			totalLenght = branchPoints[index - 1].length;
			branchPoints.RemoveRange(index, count);
			if (leaves[leaves.Count - 1].endSegmentIdx >= branchPoints.Count)
			{
				leaves.RemoveAt(leaves.Count - 1);
			}
		}


		public BranchPoint GetNearestPointFrom(Vector3 from)
		{
			BranchPoint result = null;
			float num = float.MaxValue;
			for (int i = 0; i < branchPoints.Count; i++)
			{
				float sqrMagnitude = (branchPoints[i].point - from).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					result = branchPoints[i];
					num = sqrMagnitude;
				}
			}

			return result;
		}


		public BranchPoint GetNearestPointWSFrom(Vector3 from)
		{
			BranchPoint result = null;
			float num = float.MaxValue;
			for (int i = 0; i < branchPoints.Count; i++)
			{
				float sqrMagnitude = (branchPoints[i].point - from).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					result = branchPoints[i];
					num = sqrMagnitude;
				}
			}

			return result;
		}


		public BranchPoint GetNearestPointSSFrom(Vector2 from)
		{
			BranchPoint result = null;
			float num = float.MaxValue;
			for (int i = 0; i < branchPoints.Count; i++)
			{
				float sqrMagnitude = (branchPoints[i].pointSS - from).sqrMagnitude;
				if (sqrMagnitude <= num)
				{
					result = branchPoints[i];
					num = sqrMagnitude;
				}
			}

			return result;
		}


		public Vector3[] GetSegmentPoints(Vector3 worldPoint)
		{
			Vector3[] array = new Vector3[2];
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			BranchPoint nearestPointFrom = GetNearestPointFrom(worldPoint);
			BranchPoint nextPoint = nearestPointFrom.GetNextPoint();
			BranchPoint previousPoint = nearestPointFrom.GetPreviousPoint();
			if (nextPoint != null && previousPoint != null)
			{
				float magnitude = (worldPoint - nextPoint.point).magnitude;
				float magnitude2 = (worldPoint - previousPoint.point).magnitude;
				if (magnitude <= magnitude2)
				{
					vector = nearestPointFrom.point;
					vector2 = nextPoint.point;
				}
				else
				{
					vector = previousPoint.point;
					vector2 = nearestPointFrom.point;
				}
			}

			array[0] = vector;
			array[1] = vector2;
			return array;
		}


		public BranchPoint GetLastBranchPoint()
		{
			return branchPoints[branchPoints.Count - 1];
		}


		public void AddLeaf(LeafPoint leafPoint)
		{
			leaves.Add(leafPoint);
		}


		public LeafPoint AddLeaf(Vector3 leafPoint, float lpLength, Vector3 lpForward, Vector3 lpUpward,
			int chosenLeave, BranchPoint initSegment, BranchPoint endSegment)
		{
			LeafPoint leafPoint2 = new LeafPoint(leafPoint, lpLength, lpForward, lpUpward, chosenLeave, initSegment,
				endSegment);
			leaves.Add(leafPoint2);
			return leafPoint2;
		}


		public LeafPoint InsertLeaf(Vector3 leafPoint, float lpLength, Vector3 lpForward, Vector3 lpUpward,
			int chosenLeave, int leafIndex, BranchPoint initSegment, BranchPoint endSegment)
		{
			LeafPoint leafPoint2 = new LeafPoint(leafPoint, lpLength, lpForward, lpUpward, chosenLeave, initSegment,
				endSegment);
			int index = Mathf.Clamp(leafIndex, 0, int.MaxValue);
			leaves.Insert(index, leafPoint2);
			return leafPoint2;
		}


		public void RemoveLeaves(List<LeafPoint> leaves)
		{
			for (int i = 0; i < leaves.Count; i++)
			{
				this.leaves.Remove(leaves[i]);
			}
		}


		public void DrawLeavesVectors(List<BranchPoint> branchPointsToFilter)
		{
			for (int i = 0; i < leaves.Count; i++)
			{
				leaves[i].DrawVectors();
			}
		}


		public void GetInitIdxEndIdxLeaves(int initIdxBranchPoint, float stepSize, out int initIdxLeaves,
			out int endIdxLeaves)
		{
			bool flag = false;
			bool flag2 = false;
			initIdxLeaves = -1;
			endIdxLeaves = -1;
			for (int i = 0; i < leaves.Count; i++)
			{
				if (!flag && leaves[i].lpLength > initIdxBranchPoint * stepSize)
				{
					flag = true;
					initIdxLeaves = i;
				}

				if (!flag2 && leaves[i].lpLength >= totalLenght)
				{
					endIdxLeaves = i;
					return;
				}
			}
		}


		public void ReleasePoint(int indexPoint)
		{
			if (indexPoint < branchPoints.Count)
			{
				branchPoints[indexPoint].ReleasePoint();
			}
		}


		public void GetInitIdxEndIdxLeaves(int initIdxBranchPoint, int endIdxBranchPoint, float stepSize,
			out int initIdxLeaves, out int endIdxLeaves)
		{
			bool flag = false;
			bool flag2 = false;
			initIdxLeaves = -1;
			endIdxLeaves = -1;
			for (int i = 0; i < leaves.Count; i++)
			{
				if (!flag && leaves[i].lpLength >= initIdxBranchPoint * stepSize)
				{
					flag = true;
					initIdxLeaves = i;
				}

				if (!flag2 && leaves[i].lpLength >= endIdxBranchPoint * stepSize)
				{
					endIdxLeaves = i - 1;
					return;
				}
			}
		}
	}
}