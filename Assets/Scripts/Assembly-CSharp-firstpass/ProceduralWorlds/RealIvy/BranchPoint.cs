using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	[Serializable]
	public class BranchPoint
	{
		public Vector3 originalPoint;


		public Vector3 point;


		public Vector3 grabVector;


		public Vector2 pointSS;


		public float length;


		public Vector3 initialGrowDir;


		public BranchContainer branchContainer;


		public int index;


		public bool newBranch;


		public int newBranchNumber;


		public float radius;


		public float currentRadius;


		public Quaternion forwardRotation;


		public List<RTVertexData> verticesLoop;


		public Vector3 firstVector;


		public Vector3 axis;


		public BranchPoint() { }


		public BranchPoint(Vector3 point, Vector3 grabVector, int index, bool newBranch, int newBranchNumber,
			float length, BranchContainer branchContainer)
		{
			SetValues(point, grabVector, Vector3.zero, branchContainer, index, false, newBranch, newBranchNumber,
				length);
		}


		public BranchPoint(Vector3 point, Vector3 grabVector, int index, float length, BranchContainer branchContainer)
		{
			SetValues(point, grabVector, Vector3.zero, branchContainer, index, false, false, -1, length);
		}


		public BranchPoint(Vector3 point, int index, float length, BranchContainer branchContainer)
		{
			SetValues(point, Vector3.zero, Vector3.zero, branchContainer, index, false, false, -1, length);
		}


		public void SetValues(Vector3 point, Vector3 grabVector, Vector2 pointSS, BranchContainer branchContainer,
			int index, bool blocked, bool newBranch, int newBranchNumber, float length)
		{
			this.point = point;
			this.grabVector = grabVector;
			this.pointSS = pointSS;
			this.branchContainer = branchContainer;
			this.index = index;
			this.newBranch = newBranch;
			this.newBranchNumber = newBranchNumber;
			radius = 1f;
			currentRadius = 1f;
			this.length = length;
			initialGrowDir = Vector3.zero;
			if (index >= 1)
			{
				initialGrowDir = (point - branchContainer.branchPoints[index - 1].point).normalized;
			}
		}


		public void InitializeRuntime(IvyParameters ivyParameters)
		{
			verticesLoop = new List<RTVertexData>(ivyParameters.sides + 1);
		}


		public void SetOriginalPoint()
		{
			originalPoint = point;
		}


		public BranchPoint GetNextPoint()
		{
			BranchPoint result = null;
			if (index < branchContainer.branchPoints.Count - 1)
			{
				result = branchContainer.branchPoints[index + 1];
			}

			return result;
		}


		public BranchPoint GetPreviousPoint()
		{
			BranchPoint result = null;
			if (index > 0)
			{
				result = branchContainer.branchPoints[index - 1];
			}

			return result;
		}


		public void Move(Vector3 newPosition)
		{
			point = newPosition;
		}


		public void InitBranchInThisPoint(int branchNumber)
		{
			newBranch = true;
			newBranchNumber = branchNumber;
		}


		public void ReleasePoint()
		{
			newBranch = false;
			newBranchNumber = -1;
		}
	}
}