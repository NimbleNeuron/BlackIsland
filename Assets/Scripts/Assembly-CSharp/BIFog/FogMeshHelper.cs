using System.Collections.Generic;
using UnityEngine;

namespace BIFog
{
	
	public class FogMeshHelper : Singleton<FogMeshHelper>
	{
		
		public Mesh BuildFogMesh(Transform transform, float viewRadius, float viewAngle, float height, float meshResolution, LayerMask layerMask, Mesh recycleMesh = null)
		{
			Mesh mesh = recycleMesh;
			if (mesh == null)
			{
				mesh = new Mesh();
				mesh.name = "fogMesh";
			}
			this.CreateFogMeta(this.sightMetas, transform, viewRadius, viewAngle, height, meshResolution, layerMask);
			int num = this.sightMetas.Count + 1;
			this.vertices.Clear();
			this.uvs.Clear();
			this.indexes.Clear();
			float num2 = Mathf.Pow(viewRadius, 2f);
			this.vertices.Add(Vector3.zero);
			this.uvs.Add(Vector2.zero);
			for (int i = 0; i < num - 1; i++)
			{
				ViewCastInfo viewCastInfo = this.sightMetas[i];
				this.vertices.Add(transform.InverseTransformPoint(viewCastInfo.point + viewCastInfo.reverseNormal * 0.2f));
				float num3 = Mathf.Abs((viewCastInfo.point - transform.position).sqrMagnitude / num2);
				this.uvs.Add(new Vector2(num3, num3));
				if (i < num - 2)
				{
					this.indexes.Add(0);
					this.indexes.Add(i + 1);
					this.indexes.Add(i + 2);
				}
			}
			mesh.Clear();
			mesh.SetVertices(this.vertices);
			mesh.SetTriangles(this.indexes, 0);
			mesh.SetUVs(0, this.uvs);
			mesh.RecalculateNormals();
			return mesh;
		}

		
		private void CreateFogMeta(List<ViewCastInfo> metaData, Transform transform, float viewRadius, float viewAngle, float height, float meshResolution, LayerMask layerMask)
		{
			int num = Mathf.RoundToInt(viewAngle * meshResolution);
			float num2 = viewAngle / (float)num;
			ViewCastInfo viewCastInfo = default(ViewCastInfo);
			metaData.Clear();
			float y = transform.eulerAngles.y;
			Vector3 position = transform.position;
			Vector3 eyePos = new Vector3(position.x, position.y + height, position.z);
			for (int i = 0; i <= num; i++)
			{
				float globalAngle = y - viewAngle / 2f + num2 * (float)i;
				ViewCastInfo viewCastInfo2 = this.ViewCast(position, eyePos, viewRadius, globalAngle, layerMask);
				if (i > 0)
				{
					bool flag = Mathf.Abs(viewCastInfo.dst - viewCastInfo2.dst) > 0.5f;
					if (viewCastInfo.hit != viewCastInfo2.hit || (viewCastInfo.hit && viewCastInfo2.hit && flag))
					{
						EdgeInfo edgeInfo = this.FindEdge(position, eyePos, viewRadius, layerMask, viewCastInfo, viewCastInfo2);
						if (edgeInfo.castedA)
						{
							metaData.Add(edgeInfo.viewCastInfoA);
						}
						if (edgeInfo.castedB)
						{
							metaData.Add(edgeInfo.viewCastInfoB);
						}
					}
				}
				metaData.Add(viewCastInfo2);
				viewCastInfo = viewCastInfo2;
			}
		}

		
		private EdgeInfo FindEdge(Vector3 position, Vector3 eyePos, float viewRadius, LayerMask layerMask, ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
		{
			float num = minViewCast.angle;
			float num2 = maxViewCast.angle;
			bool castedA = false;
			bool castedB = false;
			ViewCastInfo viewCastInfoA = minViewCast;
			ViewCastInfo viewCastInfoB = maxViewCast;
			for (int i = 0; i < 4; i++)
			{
				float num3 = (num + num2) / 2f;
				ViewCastInfo viewCastInfo = this.ViewCast(position, eyePos, viewRadius, num3, layerMask);
				bool flag = Mathf.Abs(minViewCast.dst - viewCastInfo.dst) > 0.5f;
				if (viewCastInfo.hit == minViewCast.hit && !flag)
				{
					num = num3;
					viewCastInfoA = viewCastInfo;
					castedA = true;
				}
				else
				{
					num2 = num3;
					viewCastInfoB = viewCastInfo;
					castedB = true;
				}
			}
			return new EdgeInfo(castedA, castedB, viewCastInfoA, viewCastInfoB);
		}

		
		private ViewCastInfo ViewCast(Vector3 pos, Vector3 eyePos, float viewRadius, float globalAngle, LayerMask obstacleMask)
		{
			Vector3 vector = this.DirFromAngle(globalAngle);
			RaycastHit raycastHit;
			if (Physics.Raycast(eyePos, vector, out raycastHit, viewRadius, obstacleMask))
			{
				return new ViewCastInfo(true, new Vector3(raycastHit.point.x, pos.y, raycastHit.point.z), raycastHit.distance, globalAngle, -raycastHit.normal);
			}
			return new ViewCastInfo(false, pos + vector * viewRadius, viewRadius, globalAngle, Vector3.zero);
		}

		
		private Vector3 DirFromAngle(float angleInDegrees)
		{
			float f = angleInDegrees * 0.017453292f;
			return new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
		}

		
		private const float FOG_SIGHT_LEN = 0.2f;

		
		private const int EDGE_RESOLVE_ITERATIONS = 4;

		
		private const float EDGE_DST_THRESHOLD = 0.5f;

		
		private List<ViewCastInfo> sightMetas = new List<ViewCastInfo>();

		
		private List<Vector3> vertices = new List<Vector3>();

		
		private List<Vector2> uvs = new List<Vector2>();

		
		private List<int> indexes = new List<int>();
	}
}
