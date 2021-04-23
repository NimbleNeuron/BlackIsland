using System.Collections.Generic;

namespace UnityEngine.UI
{
	
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/Flippable")]
	public class UIFlippable : MonoBehaviour, IMeshModifier
	{
		
		
		
		public bool horizontal
		{
			get
			{
				return this.m_Horizontal;
			}
			set
			{
				this.m_Horizontal = value;
				base.GetComponent<Graphic>().SetVerticesDirty();
			}
		}

		
		
		
		public bool vertical
		{
			get
			{
				return this.m_Veritical;
			}
			set
			{
				this.m_Veritical = value;
				base.GetComponent<Graphic>().SetVerticesDirty();
			}
		}

		
		protected void OnValidate()
		{
			base.GetComponent<Graphic>().SetVerticesDirty();
		}

		
		public void ModifyVertices(List<UIVertex> verts)
		{
			RectTransform rectTransform = base.transform as RectTransform;
			for (int i = 0; i < verts.Count; i++)
			{
				UIVertex uivertex = verts[i];
				uivertex.position = new Vector3(this.m_Horizontal ? (uivertex.position.x + (rectTransform.rect.center.x - uivertex.position.x) * 2f) : uivertex.position.x, this.m_Veritical ? (uivertex.position.y + (rectTransform.rect.center.y - uivertex.position.y) * 2f) : uivertex.position.y, uivertex.position.z);
				verts[i] = uivertex;
			}
		}

		
		public void ModifyMesh(Mesh mesh)
		{
		}

		
		public void ModifyMesh(VertexHelper verts)
		{
			List<UIVertex> list = new List<UIVertex>();
			verts.GetUIVertexStream(list);
			this.ModifyVertices(list);
			verts.AddUIVertexTriangleStream(list);
		}

		
		[SerializeField]
		private bool m_Horizontal;

		
		[SerializeField]
		private bool m_Veritical;
	}
}
