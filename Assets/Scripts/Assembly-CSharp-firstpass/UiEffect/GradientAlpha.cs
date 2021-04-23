using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiEffect
{
	[AddComponentMenu("UI/Effects/Gradient Alpha")]
	[RequireComponent(typeof(Graphic))]
	public class GradientAlpha : BaseMeshEffect
	{
		private const int ONE_TEXT_VERTEX = 6;


		[SerializeField] [Range(0f, 1f)] private float m_alphaTop = 1f;


		[SerializeField] [Range(0f, 1f)] private float m_alphaBottom = 1f;


		[SerializeField] [Range(0f, 1f)] private float m_alphaLeft = 1f;


		[SerializeField] [Range(0f, 1f)] private float m_alphaRight = 1f;


		[SerializeField] [Range(-1f, 1f)] private float m_gradientOffsetVertical;


		[SerializeField] [Range(-1f, 1f)] private float m_gradientOffsetHorizontal;


		[SerializeField] private bool m_splitTextGradient;


		
		public float alphaTop {
			get => m_alphaTop;
			set
			{
				if (m_alphaTop != value)
				{
					m_alphaTop = Mathf.Clamp01(value);
					Refresh();
				}
			}
		}


		
		public float alphaBottom {
			get => m_alphaBottom;
			set
			{
				if (m_alphaBottom != value)
				{
					m_alphaBottom = Mathf.Clamp01(value);
					Refresh();
				}
			}
		}


		
		public float alphaLeft {
			get => m_alphaLeft;
			set
			{
				if (m_alphaLeft != value)
				{
					m_alphaLeft = Mathf.Clamp01(value);
					Refresh();
				}
			}
		}


		
		public float alphaRight {
			get => m_alphaRight;
			set
			{
				if (m_alphaRight != value)
				{
					m_alphaRight = Mathf.Clamp01(value);
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
				float num9 = uivertex2.color.a / 255f;
				float num10 = Mathf.Lerp(m_alphaBottom, m_alphaTop,
					(num4 > 0f ? (uivertex2.position.y - num2) / num4 : 0f) + m_gradientOffsetVertical);
				float num11 = Mathf.Lerp(m_alphaLeft, m_alphaRight,
					(num3 > 0f ? (uivertex2.position.x - num) / num3 : 0f) + m_gradientOffsetHorizontal);
				uivertex2.color.a = (byte) (Mathf.Clamp01(num9 * num10 * num11) * 255f);
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