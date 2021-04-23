using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UILineRenderer : MaskableGraphic
	{
		[SerializeField] private Texture m_Texture;
		[SerializeField] private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

		public float LineThickness = 2f;
		public Vector2[] Points;
		public bool relativeSize;

		private readonly List<Vector2> pointList = new List<Vector2>();
		private readonly UIVertex[] VboVertices = new UIVertex[4];

		public override Texture mainTexture {
			get
			{
				if (!(m_Texture == null))
				{
					return m_Texture;
				}

				return s_WhiteTexture;
			}
		}
		
		public Texture texture {
			get => m_Texture;
			set
			{
				if (m_Texture == value)
				{
					return;
				}

				m_Texture = value;
				SetVerticesDirty();
				SetMaterialDirty();
			}
		}
		
		public Rect uvRect {
			get => m_UVRect;
			set
			{
				if (m_UVRect == value)
				{
					return;
				}

				m_UVRect = value;
				SetVerticesDirty();
			}
		}

		protected override void OnPopulateMesh(VertexHelper vbo)
		{
			if (Points == null || Points.Length < 2)
			{
				Points = new[]
				{
					new Vector2(0f, 0f),
					new Vector2(1f, 1f)
				};
			}

			int num = 2;
			
			RectTransform rt = this.rectTransform;
			
			float num2 = rt.rect.width;
			float num3 = rt.rect.height;
			float num4 = -rt.pivot.x * rt.rect.width;
			float num5 = -rt.pivot.y * rt.rect.height;
			if (!relativeSize)
			{
				num2 = 1f;
				num3 = 1f;
			}

			pointList.Clear();
			pointList.Add(Points[0]);
			Vector2 item = Points[0] + (Points[1] - Points[0]).normalized * num;
			pointList.Add(item);
			for (int i = 1; i < Points.Length - 1; i++)
			{
				pointList.Add(Points[i]);
			}

			item = Points[Points.Length - 1] - (Points[Points.Length - 1] - Points[Points.Length - 2]).normalized * num;
			pointList.Add(item);
			pointList.Add(Points[Points.Length - 1]);
			vbo.Clear();
			Vector2 vector = Vector2.zero;
			Vector2 vector2 = Vector2.zero;
			for (int j = 1; j < pointList.Count; j++)
			{
				Vector2 vector3 = pointList[j - 1];
				Vector2 vector4 = pointList[j];
				vector3 = new Vector2(vector3.x * num2 + num4, vector3.y * num3 + num5);
				vector4 = new Vector2(vector4.x * num2 + num4, vector4.y * num3 + num5);
				float z = Mathf.Atan2(vector4.y - vector3.y, vector4.x - vector3.x) * 180f / 3.1415927f;
				Vector2 vector5 = vector3 + new Vector2(0f, -LineThickness / 2f);
				Vector2 vector6 = vector3 + new Vector2(0f, LineThickness / 2f);
				Vector2 vector7 = vector4 + new Vector2(0f, LineThickness / 2f);
				Vector2 vector8 = vector4 + new Vector2(0f, -LineThickness / 2f);
				vector5 = RotatePointAroundPivot(vector5, vector3, new Vector3(0f, 0f, z));
				vector6 = RotatePointAroundPivot(vector6, vector3, new Vector3(0f, 0f, z));
				vector7 = RotatePointAroundPivot(vector7, vector4, new Vector3(0f, 0f, z));
				vector8 = RotatePointAroundPivot(vector8, vector4, new Vector3(0f, 0f, z));
				Vector2 zero = Vector2.zero;
				Vector2 vector9 = new Vector2(0f, 1f);
				Vector2 vector10 = new Vector2(0.5f, 0f);
				Vector2 vector11 = new Vector2(0.5f, 1f);
				Vector2 vector12 = new Vector2(1f, 0f);
				Vector2 vector13 = new Vector2(1f, 1f);
				Vector2[] uvs =
				{
					vector10,
					vector11,
					vector11,
					vector10
				};
				if (j > 1)
				{
					SetVbo(vbo, new[]
					{
						vector,
						vector2,
						vector5,
						vector6
					}, uvs);
				}

				if (j == 1)
				{
					uvs = new[]
					{
						zero,
						vector9,
						vector11,
						vector10
					};
				}
				else if (j == pointList.Count - 1)
				{
					uvs = new[]
					{
						vector10,
						vector11,
						vector13,
						vector12
					};
				}

				vbo.AddUIVertexQuad(SetVbo(vbo, new[]
				{
					vector5,
					vector6,
					vector7,
					vector8
				}, uvs));
				vector = vector7;
				vector2 = vector8;
			}
		}


		protected UIVertex[] SetVbo(VertexHelper vbo, Vector2[] vertices, Vector2[] uvs)
		{
			Array.Clear(VboVertices, 0, VboVertices.Length);
			for (int i = 0; i < vertices.Length; i++)
			{
				UIVertex simpleVert = UIVertex.simpleVert;
				simpleVert.color = color;
				simpleVert.position = vertices[i];
				simpleVert.uv0 = uvs[i];
				VboVertices[i] = simpleVert;
			}

			return VboVertices;
		}


		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
		{
			Vector3 vector = point - pivot;
			vector = Quaternion.Euler(angles) * vector;
			point = vector + pivot;
			return point;
		}
	}
}