using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiEffect
{
	[AddComponentMenu("UI/Effects/Gradient Color")]
	[RequireComponent(typeof(Graphic))]
	public class GradientColor : BaseMeshEffect
	{
		private const int ONE_TEXT_VERTEX = 6;


		[SerializeField] private Color m_colorTop = Color.white;


		[SerializeField] private Color m_colorBottom = Color.white;


		[SerializeField] private Color m_colorLeft = Color.white;


		[SerializeField] private Color m_colorRight = Color.white;


		[SerializeField] [Range(-1f, 1f)] private float m_gradientOffsetVertical;


		[SerializeField] [Range(-1f, 1f)] private float m_gradientOffsetHorizontal;


		[SerializeField] private bool m_splitTextGradient;


		
		public Color colorTop {
			get => m_colorTop;
			set
			{
				if (m_colorTop != value)
				{
					m_colorTop = value;
					Refresh();
				}
			}
		}


		
		public Color colorBottom {
			get => m_colorBottom;
			set
			{
				if (m_colorBottom != value)
				{
					m_colorBottom = value;
					Refresh();
				}
			}
		}


		
		public Color colorLeft {
			get => m_colorLeft;
			set
			{
				if (m_colorLeft != value)
				{
					m_colorLeft = value;
					Refresh();
				}
			}
		}


		
		public Color colorRight {
			get => m_colorRight;
			set
			{
				if (m_colorRight != value)
				{
					m_colorRight = value;
					Refresh();
				}
			}
		}


		
		public float gradientOffsetVertical {
			get => m_gradientOffsetVertical;
			set
			{
				if (m_gradientOffsetVertical != value)
				{
					m_gradientOffsetVertical = Mathf.Clamp(value, -1f, 1f);
					Refresh();
				}
			}
		}


		
		public float gradientOffsetHorizontal {
			get => m_gradientOffsetHorizontal;
			set
			{
				if (m_gradientOffsetHorizontal != value)
				{
					m_gradientOffsetHorizontal = Mathf.Clamp(value, -1f, 1f);
					Refresh();
				}
			}
		}


		
		public bool splitTextGradient {
			get => m_splitTextGradient;
			set
			{
				if (m_splitTextGradient != value)
				{
					m_splitTextGradient = value;
					Refresh();
				}
			}
		}


		public override void ModifyMesh(VertexHelper vh)
		{
			if (!IsActive())
			{
				return;
			}

			List<UIVertex> list = UiEffectListPool<UIVertex>.Get();
			vh.GetUIVertexStream(list);
			ModifyVertices(list);
			vh.Clear();
			vh.AddUIVertexTriangleStream(list);
			UiEffectListPool<UIVertex>.Release(list);
		}


		private void ModifyVertices(List<UIVertex> vList)
		{
			if (!IsActive() || vList == null || vList.Count == 0)
			{
				return;
			}

			float num = 0f;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			for (int i = 0; i < vList.Count; i++)
			{
				if (i == 0 || m_splitTextGradient && i % 6 == 0)
				{
					num = vList[i].position.x;
					num2 = vList[i].position.y;
					float num5 = vList[i].position.x;
					float num6 = vList[i].position.y;
					int num7 = m_splitTextGradient ? i + 6 : vList.Count;
					int num8 = i;
					while (num8 < num7 && num8 < vList.Count)
					{
						UIVertex uivertex = vList[num8];
						num = Mathf.Min(num, uivertex.position.x);
						num2 = Mathf.Min(num2, uivertex.position.y);
						num5 = Mathf.Max(num5, uivertex.position.x);
						num6 = Mathf.Max(num6, uivertex.position.y);
						num8++;
					}

					num3 = num5 - num;
					num4 = num6 - num2;
				}

				UIVertex uivertex2 = vList[i];
				Color a = uivertex2.color;
				Color b = Color.Lerp(m_colorBottom, m_colorTop,
					(num4 > 0f ? (uivertex2.position.y - num2) / num4 : 0f) + m_gradientOffsetVertical);
				Color b2 = Color.Lerp(m_colorLeft, m_colorRight,
					(num3 > 0f ? (uivertex2.position.x - num) / num3 : 0f) + m_gradientOffsetHorizontal);
				uivertex2.color = a * b * b2;
				vList[i] = uivertex2;
			}
		}


		private void Refresh()
		{
			if (graphic != null)
			{
				graphic.SetVerticesDirty();
			}
		}
	}
}