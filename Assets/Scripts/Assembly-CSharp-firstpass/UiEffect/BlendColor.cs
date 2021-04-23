using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UiEffect
{
	[AddComponentMenu("UI/Effects/Blend Color")]
	[RequireComponent(typeof(Graphic))]
	public class BlendColor : BaseMeshEffect
	{
		public enum BlendMode
		{
			Multiply,


			Additive,


			Subtractive,


			Override
		}


		[SerializeField] private BlendMode m_blendMode;


		[SerializeField] private Color m_color = Color.white;


		
		public BlendMode blendMode {
			get => m_blendMode;
			set
			{
				if (m_blendMode != value)
				{
					m_blendMode = value;
					Refresh();
				}
			}
		}


		
		public Color color {
			get => m_color;
			set
			{
				if (m_color != value)
				{
					m_color = value;
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

			for (int i = 0; i < vList.Count; i++)
			{
				UIVertex uivertex = vList[i];
				byte a = uivertex.color.a;
				switch (m_blendMode)
				{
					case BlendMode.Multiply:
						uivertex.color *= m_color;
						break;
					case BlendMode.Additive:
						uivertex.color += m_color;
						break;
					case BlendMode.Subtractive:
						uivertex.color -= m_color;
						break;
					case BlendMode.Override:
						uivertex.color = m_color;
						break;
				}

				uivertex.color.a = a;
				vList[i] = uivertex;
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