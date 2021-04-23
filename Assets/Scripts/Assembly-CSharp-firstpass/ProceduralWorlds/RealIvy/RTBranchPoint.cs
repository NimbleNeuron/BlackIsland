using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class RTBranchPoint
	{
		public Vector3 axis;


		public RTBranchContainer branchContainer;


		public Vector3 centerLoop;


		public Vector3 firstVector;


		public Vector3 grabVector;


		public int index;


		public Vector3 lastVectorNormal;


		public float length;


		public bool newBranch;


		public int newBranchNumber;


		public Vector3 point;


		public float radius;


		public RTVertexData[] verticesLoop;


		public RTBranchPoint() { }


		public RTBranchPoint(BranchPoint branchPoint, RTBranchContainer rtBranchContainer)
		{
			point = branchPoint.point;
			grabVector = branchPoint.grabVector;
			length = branchPoint.length;
			index = branchPoint.index;
			newBranch = branchPoint.newBranch;
			newBranchNumber = branchPoint.newBranchNumber;
			branchContainer = rtBranchContainer;
			radius = branchPoint.radius;
			firstVector = branchPoint.firstVector;
			axis = branchPoint.axis;
		}


		public void PreInit(IvyParameters ivyParameters)
		{
			verticesLoop = new RTVertexData[ivyParameters.sides + 1];
		}


		public void SetValues(Vector3 point, Vector3 grabVector)
		{
			SetValues(point, grabVector, false, -1);
		}


		public void SetValues(Vector3 point, Vector3 grabVector, bool newBranch, int newBranchNumber)
		{
			this.point = point;
			this.grabVector = grabVector;
			this.newBranch = newBranch;
			this.newBranchNumber = newBranchNumber;
		}


		public void InitBranchInThisPoint(int branchNumber)
		{
			newBranch = true;
			newBranchNumber = branchNumber;
		}


		public void CalculateVerticesLoop(IvyParameters ivyParameters, RTIvyContainer rtIvyContainer, GameObject ivyGO,
			Vector3 firstVector, Vector3 axis, float radius)
		{
			this.firstVector = firstVector;
			this.axis = axis;
			this.radius = radius;
			CalculateVerticesLoop(ivyParameters, rtIvyContainer, ivyGO);
		}


		public void CalculateVerticesLoop(IvyParameters ivyParameters, RTIvyContainer rtIvyContainer, GameObject ivyGO)
		{
			float num;
			if (!ivyParameters.halfgeom)
			{
				num = 360f / ivyParameters.sides;
			}
			else
			{
				num = 360f / ivyParameters.sides / 2f;
			}

			Vector3 vector = Vector3.zero;
			Vector3 normal = Vector3.zero;
			Vector2 zero = Vector2.zero;
			Quaternion identity = Quaternion.identity;
			Vector3 vector2 = Vector3.zero;
			Quaternion rotation = Quaternion.Inverse(ivyGO.transform.rotation);
			for (int i = 0; i < ivyParameters.sides + 1; i++)
			{
				vector2 = Quaternion.AngleAxis(num * i, axis) * firstVector;
				if (ivyParameters.halfgeom && ivyParameters.sides == 1)
				{
					normal = -grabVector;
				}
				else
				{
					normal = vector2;
				}

				normal = rotation * normal;
				vector = vector2 * radius + point;
				vector -= ivyGO.transform.position;
				vector = rotation * vector;
				zero = new Vector2(length * ivyParameters.uvScale.y + ivyParameters.uvOffset.y - ivyParameters.stepSize,
					1f / ivyParameters.sides * i * ivyParameters.uvScale.x + ivyParameters.uvOffset.x);
				verticesLoop[i] = new RTVertexData(vector, normal, zero, Vector2.zero, Color.black);
			}
		}


		public void CalculateCenterLoop(GameObject ivyGO)
		{
			centerLoop = Quaternion.Inverse(ivyGO.transform.rotation) * (point - ivyGO.transform.position);
			lastVectorNormal = ivyGO.transform.InverseTransformVector(grabVector);
		}


		public RTBranchPoint GetNextPoint()
		{
			return branchContainer.branchPoints[index + 1];
		}


		public RTBranchPoint GetPreviousPoint()
		{
			return branchContainer.branchPoints[index - 1];
		}
	}
}