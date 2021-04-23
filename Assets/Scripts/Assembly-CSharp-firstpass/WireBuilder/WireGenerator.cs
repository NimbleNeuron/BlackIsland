using UnityEngine;

namespace WireBuilder
{
	public class WireGenerator
	{
		public static Wire New(WireType wireType)
		{
			Wire wire = new GameObject
			{
				name = wireType.name + " Wire"
			}.AddComponent<Wire>();
			wire.wireType = wireType;
			Update(wire, null, true);
			return wire;
		}


		public static float CalculateWireSag(float gravity, float t)
		{
			return gravity * -Mathf.Sin(t * 3.1415927f);
		}


		public static void Update(Wire wire, WireType type, bool updateWind = false)
		{
			if (!wire || type == null)
			{
				return;
			}

			wire.wireType = type;
			wire.startPos = wire.startConnection ? wire.startConnection.transform.position : wire.transform.position;
			wire.endPos = wire.endConnection ? wire.endConnection.transform.position : wire.transform.position;
			wire.length = Vector3.Distance(wire.startPos, wire.endPos);
			wire.weight = wire.wireType.weight * 0.01f;
			wire.tension = wire.weight * wire.length;
			float gravity = wire.tension + wire.weight + wire.sagOffset;
			float num = CalculateWireSag(gravity, 0.5f);
			wire.sagDepth = num;
			int num2 = Mathf.RoundToInt(wire.wireType.pointsPerMeter * wire.length + wire.sagDepth);
			num2 = Mathf.Clamp(num2, 6, 50);
			wire.points = new Vector3[num2];
			Vector3 position = Vector3.Lerp(wire.startPos, wire.endPos, 0.5f);
			position.y += num;
			wire.gameObject.transform.position = position;
			Vector3 normalized = (wire.endPos - wire.startPos).normalized;
			if (normalized != Vector3.zero)
			{
				wire.gameObject.transform.forward = normalized;
			}

			for (int i = 0; i < num2; i++)
			{
				float num3 = i / (float) (num2 - 1);
				Vector3 b = (wire.endPos - wire.startPos) * num3;
				b.y += CalculateWireSag(gravity, num3);
				wire.points[i] = wire.transform.InverseTransformPoint(wire.startPos + b);
			}

			wire.startPos = wire.transform.position + wire.points[0];
			wire.endPos = wire.transform.position + wire.points[wire.points.Length - 1];
			if (updateWind || wire.windData == null)
			{
				wire.windData = NewWindData(wire);
			}

			ValidateComponents(wire);
			if (wire.wireType.geometryType == WireType.GeometryType.Line)
			{
				UpdateLineRenderer(wire, updateWind);
			}

			if (wire.wireType.geometryType == WireType.GeometryType.Mesh)
			{
				UpdateMesh(wire, updateWind);
			}

			wire.gameObject.layer = wire.wireType.layer;
			wire.gameObject.tag = wire.wireType.tag;
		}


		private static void ValidateComponents(Wire wire)
		{
			if (wire.wireType.geometryType == WireType.GeometryType.Line)
			{
				if (!wire.lineRenderer)
				{
					wire.lineRenderer = wire.gameObject.AddComponent<LineRenderer>();
					wire.lineRenderer.hideFlags = HideFlags.None;
				}

				if (wire.meshRenderer)
				{
					Object.DestroyImmediate(wire.meshRenderer);
				}

				if (wire.meshFilter)
				{
					Object.DestroyImmediate(wire.meshFilter);
				}

				wire.mesh = null;
			}

			if (wire.wireType.geometryType == WireType.GeometryType.Mesh)
			{
				if (!wire.meshRenderer)
				{
					wire.meshRenderer = wire.gameObject.AddComponent<MeshRenderer>();
				}

				if (!wire.meshFilter)
				{
					wire.meshFilter = wire.gameObject.AddComponent<MeshFilter>();
				}

				if (wire.lineRenderer)
				{
					Object.DestroyImmediate(wire.lineRenderer);
				}
			}
		}


		private static void UpdateLineRenderer(Wire wire, bool updateWind)
		{
			wire.lineRenderer.useWorldSpace = false;
			wire.lineRenderer.generateLightingData = true;
			if (wire.lineRenderer.colorGradient.alphaKeys[0].alpha == 1f || updateWind)
			{
				wire.lineRenderer.colorGradient = NewWindData(wire);
			}

			wire.lineRenderer.positionCount = wire.points.Length;
			wire.lineRenderer.SetPositions(wire.points);
			wire.lineRenderer.startWidth = wire.wireType.diameter * 2f;
			wire.lineRenderer.endWidth = wire.wireType.diameter * 2f;
			wire.lineRenderer.material = wire.wireType.material;
			wire.lineRenderer.textureMode = wire.wireType.textureMode;
			wire.lineRenderer.lightmapScaleOffset = new Vector4(1f / wire.length, 1f, 0f, 0f);
		}


		private static void UpdateMesh(Wire wire, bool updateWind)
		{
			wire.meshRenderer.sharedMaterial = wire.wireType.material;
			wire.meshFilter.mesh = GenerateMesh(wire, updateWind);
		}


		private static Gradient NewWindData(Wire wire)
		{
			Gradient gradient = new Gradient();
			float num = Random.Range(0.2f, 1f);
			float num2 = Random.Range(0.9f, 1f);
			float r = wire.tension * num;
			float g = num2 * Mathf.Abs(wire.sagDepth * 0.1f);
			Color col = new Color(r, g, 0f, 0f);
			GradientColorKey[] colorKeys =
			{
				new GradientColorKey(col, 0f),
				new GradientColorKey(col, 1f)
			};
			GradientAlphaKey[] array = new GradientAlphaKey[8];
			for (int i = 0; i < 8; i++)
			{
				float num3 = i / 7f;
				array[i] = new GradientAlphaKey(Mathf.Sin(num3 * 3.1415927f), num3);
			}

			gradient.SetKeys(colorKeys, array);
			return gradient;
		}


		private static Mesh GenerateMesh(Wire wire, bool updateWind)
		{
			if (wire.mesh == null)
			{
				wire.mesh = new Mesh();
			}

			wire.mesh.name = wire.name + " (Mesh)";
			int num = wire.wireType.radialSegments * wire.points.Length;
			Vector3[] array = new Vector3[num];
			Color[] colors = new Color[num];
			int[] triangles = GenerateIndices(wire);
			Vector2[] uv = GenerateUVs(wire);
			colors = GenerateColors(wire);
			if (num > wire.mesh.vertexCount)
			{
				wire.mesh.vertices = array;
				wire.mesh.triangles = triangles;
				wire.mesh.uv = uv;
			}
			else
			{
				wire.mesh.triangles = triangles;
				wire.mesh.vertices = array;
				wire.mesh.uv = uv;
			}

			int num2 = 0;
			for (int i = 0; i < wire.points.Length; i++)
			{
				foreach (Vector3 vector in VertexRing(i, wire))
				{
					array[num2++] = vector;
				}
			}

			wire.mesh.vertices = array;
			wire.mesh.colors = colors;
			wire.mesh.RecalculateNormals();
			wire.mesh.RecalculateTangents();
			wire.mesh.RecalculateBounds();
			return wire.mesh;
		}


		private static int[] GenerateIndices(Wire wire)
		{
			int[] array = new int[wire.points.Length * wire.wireType.radialSegments * 2 * 3];
			int num = 0;
			for (int i = 1; i < wire.points.Length; i++)
			{
				for (int j = 0; j < wire.wireType.radialSegments; j++)
				{
					int num2 = i * wire.wireType.radialSegments + j;
					int num3 = num2 - wire.wireType.radialSegments;
					array[num++] = num3;
					array[num++] = j == wire.wireType.radialSegments - 1
						? num2 - (wire.wireType.radialSegments - 1)
						: num2 + 1;
					array[num++] = num2;
					array[num++] = j == wire.wireType.radialSegments - 1
						? num3 - (wire.wireType.radialSegments - 1)
						: num3 + 1;
					array[num++] = j == wire.wireType.radialSegments - 1
						? num2 - (wire.wireType.radialSegments - 1)
						: num2 + 1;
					array[num++] = num3;
				}
			}

			return array;
		}


		private static Vector2[] GenerateUVs(Wire wire)
		{
			Vector2[] array = new Vector2[wire.points.Length * wire.wireType.radialSegments];
			for (int i = 0; i < wire.points.Length; i++)
			{
				for (int j = 0; j < wire.wireType.radialSegments; j++)
				{
					int num = i * wire.wireType.radialSegments + j;
					float y = j / (wire.wireType.radialSegments - 1f);
					float x = i / (wire.points.Length - 1f) * (wire.wireType.tiling * wire.length);
					array[num] = new Vector2(x, y);
				}
			}

			return array;
		}


		private static Color[] GenerateColors(Wire wire)
		{
			Color[] array = new Color[wire.points.Length * wire.wireType.radialSegments];
			for (int i = 0; i < wire.points.Length; i++)
			{
				float time = i / (float) (wire.points.Length - 1);
				for (int j = 0; j < wire.wireType.radialSegments; j++)
				{
					int num = i * wire.wireType.radialSegments + j;
					array[num] = wire.windData.Evaluate(time);
				}
			}

			return array;
		}


		private static Vector3[] VertexRing(int index, Wire wire)
		{
			int num = 0;
			Vector3 vector = Vector3.zero;
			if (index > 0)
			{
				vector += (wire.points[index] - wire.points[index - 1]).normalized;
				num++;
			}

			if (index < wire.points.Length - 1)
			{
				vector += (wire.points[index + 1] - wire.points[index]).normalized;
				num++;
			}

			vector = (vector / num).normalized;
			Vector3 normalized = Vector3.Cross(vector, vector + new Vector3(0.123564f, 0.34675f, 0.756892f)).normalized;
			Vector3 normalized2 = Vector3.Cross(vector, normalized).normalized;
			Vector3[] array = new Vector3[wire.wireType.radialSegments];
			float num2 = 0f;
			float num3 = 6.2831855f / wire.wireType.radialSegments;
			for (int i = 0; i < wire.wireType.radialSegments; i++)
			{
				float d = Mathf.Cos(num2);
				float d2 = Mathf.Sin(num2);
				array[i] = wire.points[index] + normalized * d * wire.wireType.diameter +
				           normalized2 * d2 * wire.wireType.diameter;
				num2 += num3;
			}

			return array;
		}
	}
}