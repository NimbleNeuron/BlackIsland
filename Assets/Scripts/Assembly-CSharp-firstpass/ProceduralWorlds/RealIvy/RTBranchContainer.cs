using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class RTBranchContainer
	{
		public int branchNumber;


		public List<RTBranchPoint> branchPoints;


		public int branchSense;


		public float currentHeight;


		public float deltaHeight;


		public bool falling;


		public float fallIteration;


		public Vector3 growDirection;


		public float heightParameter;


		public float heightVar;


		public RTLeafPoint[][] leavesOrderedByInitSegment;


		public float newHeight;


		public float randomizeHeight;


		public Quaternion rotationOnFallIteration;


		public float totalLength;


		public RTBranchContainer(BranchContainer branchContainer, IvyParameters ivyParameters,
			RTIvyContainer rtIvyContainer, GameObject ivyGO, RTMeshData[] leavesMeshesByChosenLeaf)
		{
			totalLength = branchContainer.totalLenght;
			growDirection = branchContainer.growDirection;
			randomizeHeight = branchContainer.randomizeHeight;
			heightVar = branchContainer.heightVar;
			newHeight = branchContainer.newHeight;
			heightParameter = branchContainer.heightParameter;
			deltaHeight = branchContainer.deltaHeight;
			currentHeight = branchContainer.currentHeight;
			branchSense = branchContainer.branchSense;
			falling = branchContainer.falling;
			rotationOnFallIteration = branchContainer.rotationOnFallIteration;
			branchNumber = branchContainer.branchNumber;
			branchPoints = new List<RTBranchPoint>(branchContainer.branchPoints.Count);
			for (int i = 0; i < branchContainer.branchPoints.Count; i++)
			{
				RTBranchPoint rtbranchPoint = new RTBranchPoint(branchContainer.branchPoints[i], this);
				rtbranchPoint.CalculateCenterLoop(ivyGO);
				rtbranchPoint.PreInit(ivyParameters);
				rtbranchPoint.CalculateVerticesLoop(ivyParameters, rtIvyContainer, ivyGO);
				branchPoints.Add(rtbranchPoint);
			}

			branchContainer.PrepareRTLeavesDict();
			if (ivyParameters.generateLeaves)
			{
				leavesOrderedByInitSegment = new RTLeafPoint[branchPoints.Count][];
				for (int j = 0; j < branchPoints.Count; j++)
				{
					List<LeafPoint> list = branchContainer.dictRTLeavesByInitSegment[j];
					int num = 0;
					if (list != null)
					{
						num = list.Count;
					}

					leavesOrderedByInitSegment[j] = new RTLeafPoint[num];
					for (int k = 0; k < num; k++)
					{
						RTLeafPoint rtleafPoint = new RTLeafPoint(list[k], ivyParameters);
						RTMeshData leafMeshData = leavesMeshesByChosenLeaf[rtleafPoint.chosenLeave];
						rtleafPoint.CreateVertices(ivyParameters, leafMeshData, ivyGO);
						leavesOrderedByInitSegment[j][k] = rtleafPoint;
					}
				}
			}
		}


		public RTBranchContainer(int numPoints, int numLeaves)
		{
			Init(numPoints, numLeaves);
		}


		public Vector2 GetLastUV(IvyParameters ivyParameters)
		{
			return new Vector2(totalLength * ivyParameters.uvScale.y + ivyParameters.uvOffset.y,
				0.5f * ivyParameters.uvScale.x + ivyParameters.uvOffset.x);
		}


		private void Init(int numPoints, int numLeaves)
		{
			branchPoints = new List<RTBranchPoint>(numPoints);
			leavesOrderedByInitSegment = new RTLeafPoint[numPoints][];
			for (int i = 0; i < numPoints; i++)
			{
				leavesOrderedByInitSegment[i] = new RTLeafPoint[1];
			}
		}


		public void AddBranchPoint(RTBranchPoint rtBranchPoint, float deltaLength)
		{
			totalLength += deltaLength;
			rtBranchPoint.length = totalLength;
			rtBranchPoint.index = branchPoints.Count;
			rtBranchPoint.branchContainer = this;
			branchPoints.Add(rtBranchPoint);
		}


		public RTBranchPoint GetLastBranchPoint()
		{
			return branchPoints[branchPoints.Count - 1];
		}


		public void AddLeaf(RTLeafPoint leafAdded)
		{
			if (leafAdded.initSegmentIdx >= leavesOrderedByInitSegment.Length)
			{
				Array.Resize<RTLeafPoint[]>(ref leavesOrderedByInitSegment, leavesOrderedByInitSegment.Length * 2);
				for (int i = leafAdded.initSegmentIdx; i < leavesOrderedByInitSegment.Length; i++)
				{
					leavesOrderedByInitSegment[i] = new RTLeafPoint[1];
				}
			}

			leavesOrderedByInitSegment[leafAdded.initSegmentIdx][0] = leafAdded;
		}
	}
}