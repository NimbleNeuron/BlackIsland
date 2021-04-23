using UnityEngine;

namespace MLSpace
{
	public class BeamControl : MonoBehaviour
	{
		private float m_FadeDist = 10f;


		private bool m_initialized;


		private Material m_Mat;


		private void Start()
		{
			Initialize();
		}


		private void Update()
		{
			if (!m_initialized)
			{
				Debug.LogError("component not initialized.");
				return;
			}

			Ray ray = new Ray(transform.position, transform.forward);
			float value = m_FadeDist;
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit, m_FadeDist))
			{
				value = raycastHit.distance;
			}

			m_Mat.SetFloat("_FadeDist", value);
		}


		public void Initialize()
		{
			if (m_initialized)
			{
				return;
			}

			MeshRenderer component = GetComponent<MeshRenderer>();
			if (!component)
			{
				Debug.LogError("Cannot find 'MeshRenderer' component.");
				return;
			}

			m_Mat = component.material;
			if (!m_Mat)
			{
				Debug.LogError("Object cannot be null.");
				return;
			}

			m_FadeDist = m_Mat.GetFloat("_FadeDist");
			m_initialized = true;
		}
	}
}