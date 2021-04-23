using UnityEngine;

namespace SoxAnimation
{
	[ExecuteInEditMode]
	public class ConstraintTransform : MonoBehaviour
	{
		public enum ScaleMode
		{
			LocalScale,


			HierarchyScale
		}


		[HideInInspector] public float m_version = 1.101f;


		public Transform m_target;


		public ScaleMode scaleMode;


		private Transform m_transform;


		private void Start()
		{
			m_transform = transform;
		}


		private void Update()
		{
			if (m_target == null)
			{
				return;
			}

			if (Application.isPlaying)
			{
				m_transform.position = m_target.position;
				m_transform.rotation = m_target.rotation;
				if (scaleMode == ScaleMode.LocalScale)
				{
					m_transform.localScale = m_target.localScale;
					return;
				}

				m_transform.localScale = m_target.lossyScale;
			}
			else
			{
				transform.position = m_target.position;
				transform.rotation = m_target.rotation;
				if (scaleMode == ScaleMode.LocalScale)
				{
					transform.localScale = m_target.localScale;
					return;
				}

				transform.localScale = m_target.lossyScale;
			}
		}
	}
}