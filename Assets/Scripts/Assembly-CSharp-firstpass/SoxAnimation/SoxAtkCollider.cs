using UnityEngine;

namespace SoxAnimation
{
	public class SoxAtkCollider : MonoBehaviour
	{
		public enum Axis
		{
			X,


			Y,


			Z
		}


		public enum ColliderType
		{
			Sphere
		}


		[HideInInspector] public float m_version = 1.101f;


		public ColliderType m_colliderType;


		public Axis m_referenceAxisOfScale;


		public float m_sphereRadius = 0.1f;


		[HideInInspector] public float m_sphereRadiusScaled;


		[Range(0f, 1f)] public float m_friction;


		[HideInInspector] public float m_frictionInverse = 1f;


		public Color m_gizmoColor = Color.green;


		public bool m_showGizmoAtPlay;


		public bool m_showGizmoAtEditor = true;


		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				if (m_showGizmoAtPlay)
				{
					DrawGizmo();
				}
			}
			else if (m_showGizmoAtEditor)
			{
				DrawGizmo();
			}
		}


		private void OnDrawGizmosSelected()
		{
			DrawGizmo();
		}


		private void OnValidate()
		{
			m_sphereRadius = Mathf.Max(0f, m_sphereRadius);
			m_frictionInverse = 1f - m_friction;
		}


		private void DrawGizmo()
		{
			switch (m_referenceAxisOfScale)
			{
				case Axis.X:
					m_sphereRadiusScaled = m_sphereRadius * transform.lossyScale.x;
					break;
				case Axis.Y:
					m_sphereRadiusScaled = m_sphereRadius * transform.lossyScale.y;
					break;
				case Axis.Z:
					m_sphereRadiusScaled = m_sphereRadius * transform.lossyScale.z;
					break;
			}

			Gizmos.color = m_gizmoColor;
			Gizmos.DrawWireSphere(transform.position, m_sphereRadiusScaled);
		}
	}
}