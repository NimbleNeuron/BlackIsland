using UnityEngine;
using UnityEngine.Serialization;

namespace Blis.Client
{
	public class IndicatorPart : MonoBehaviour
	{
		[FormerlySerializedAs("renderer")] [SerializeField]
		private MeshRenderer m_renderer;

		private void Awake()
		{
			m_renderer = GetComponent<MeshRenderer>();
		}


		public void SetShaderFloat(string property, float value)
		{
			if (m_renderer != null)
			{
				foreach (Material material in m_renderer.materials)
				{
					if (material.HasProperty(property))
					{
						material.SetFloat(property, value);
					}
				}
			}
		}
	}
}