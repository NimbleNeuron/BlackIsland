using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class RTLeafPoint
	{
		public int chosenLeave;


		public int endSegmentIdx;


		public int initSegmentIdx;


		public Vector3 leafCenter;


		public Quaternion leafRotation;


		public float leafScale;


		public Vector3 left;


		public Vector3 lpForward;


		public float lpLength;


		public Vector3 lpUpward;


		public Vector3 point;


		public RTVertexData[] vertices;


		public RTLeafPoint() { }


		public RTLeafPoint(LeafPoint leafPoint, IvyParameters ivyParameters)
		{
			point = leafPoint.point;
			lpLength = leafPoint.lpLength;
			left = leafPoint.left;
			lpForward = leafPoint.lpForward;
			lpUpward = leafPoint.lpUpward;
			initSegmentIdx = leafPoint.initSegmentIdx;
			endSegmentIdx = leafPoint.endSegmentIdx;
			chosenLeave = leafPoint.chosenLeave;
			vertices = leafPoint.verticesLeaves.ToArray();
			leafCenter = leafPoint.leafCenter;
			leafRotation = leafPoint.leafRotation;
			leafScale = leafPoint.leafScale;
			CalculateLeafRotation(ivyParameters);
		}


		public void InitializeRuntime() { }


		public void PreInit(int numVertices)
		{
			vertices = new RTVertexData[numVertices];
		}


		public void PreInit(RTMeshData leafMeshData)
		{
			vertices = new RTVertexData[leafMeshData.vertices.Length];
		}


		public void SetValues(Vector3 point, float lpLength, Vector3 lpForward, Vector3 lpUpward, int chosenLeave,
			RTBranchPoint initSegment, RTBranchPoint endSegment, float leafScale, IvyParameters ivyParameters)
		{
			this.point = point;
			this.lpLength = lpLength;
			this.lpForward = lpForward;
			this.lpUpward = lpUpward;
			this.chosenLeave = chosenLeave;
			initSegmentIdx = initSegment.index;
			endSegmentIdx = endSegment.index;
			this.leafScale = leafScale;
			left = Vector3.Cross(lpForward, lpUpward).normalized;
			CalculateLeafRotation(ivyParameters);
		}


		private void CalculateLeafRotation(IvyParameters ivyParameters)
		{
			Vector3 globalRotation;
			Vector3 axis;
			if (!ivyParameters.globalOrientation)
			{
				globalRotation = lpForward;
				axis = left;
			}
			else
			{
				globalRotation = ivyParameters.globalRotation;
				axis = Vector3.Normalize(Vector3.Cross(ivyParameters.globalRotation, lpUpward));
			}

			leafRotation = Quaternion.LookRotation(lpUpward, globalRotation);
			leafRotation = Quaternion.AngleAxis(ivyParameters.rotation.x, axis) *
			               Quaternion.AngleAxis(ivyParameters.rotation.y, lpUpward) *
			               Quaternion.AngleAxis(ivyParameters.rotation.z, globalRotation) * leafRotation;
			leafRotation =
				Quaternion.AngleAxis(Random.Range(-ivyParameters.randomRotation.x, ivyParameters.randomRotation.x),
					axis) *
				Quaternion.AngleAxis(Random.Range(-ivyParameters.randomRotation.y, ivyParameters.randomRotation.y),
					lpUpward) * Quaternion.AngleAxis(
					Random.Range(-ivyParameters.randomRotation.z, ivyParameters.randomRotation.z), globalRotation) *
				leafRotation;
		}


		public void CreateVertices(IvyParameters ivyParameters, RTMeshData leafMeshData, GameObject ivyGO)
		{
			Vector3 vector = Vector3.zero;
			Vector3 normal = Vector3.zero;
			Vector2 uv = Vector2.zero;
			Color color = Color.black;
			Quaternion rotation = Quaternion.Inverse(ivyGO.transform.rotation);
			leafCenter = ivyGO.transform.InverseTransformPoint(point);
			for (int i = 0; i < leafMeshData.vertices.Length; i++)
			{
				Vector3 b = left * ivyParameters.offset.x + lpUpward * ivyParameters.offset.y +
				            lpForward * ivyParameters.offset.z;
				vector = leafRotation * leafMeshData.vertices[i] * leafScale + point + b;
				vector -= ivyGO.transform.position;
				vector = rotation * vector;
				normal = leafRotation * leafMeshData.normals[i];
				normal = rotation * normal;
				uv = leafMeshData.uv[i];
				color = leafMeshData.colors[i];
				vertices[i] = new RTVertexData(vector, normal, uv, Vector2.zero, color);
			}
		}
	}
}