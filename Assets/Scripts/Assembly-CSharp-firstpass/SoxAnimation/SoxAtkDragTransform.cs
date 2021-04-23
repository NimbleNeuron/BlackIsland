using System;
using UnityEngine;

namespace SoxAnimation
{
	public class SoxAtkDragTransform : MonoBehaviour
	{
		[HideInInspector] public float m_version = 1.101f;


		[Header("Equal Tension - Tension equal to Element 0")]
		public bool m_equalTension = true;


		public float m_tensionMultiplier = 1f;


		[SerializeField] public DragTransformSet[] m_dragSet =
		{
			new DragTransformSet(null, null, 5f, 5f, Vector3.zero, Quaternion.identity, Vector3.zero,
				Quaternion.identity)
		};


		private bool m_initialized;


		private void Update()
		{
			if (m_dragSet == null)
			{
				return;
			}

			if (m_dragSet.Length == 0)
			{
				return;
			}

			for (int i = 0; i < m_dragSet.Length; i++)
			{
				if (m_dragSet[i].m_sourceObject != null && m_dragSet[i].m_dragObject != null)
				{
					Vector3 b = m_dragSet[i].m_sourceObject.TransformPoint(m_dragSet[i].m_localPosBak);
					m_dragSet[i].m_dragObject.position = Vector3.Lerp(m_dragSet[i].m_posBefore, b,
						m_dragSet[i].m_positionTension * Time.smoothDeltaTime * m_tensionMultiplier);
					Quaternion b2 = m_dragSet[i].m_sourceObject.rotation * m_dragSet[i].m_localRotBak;
					m_dragSet[i].m_dragObject.rotation = Quaternion.Lerp(m_dragSet[i].m_rotBefore, b2,
						m_dragSet[i].m_rotationTension * Time.smoothDeltaTime * m_tensionMultiplier);
					m_dragSet[i].m_posBefore = m_dragSet[i].m_dragObject.position;
					m_dragSet[i].m_rotBefore = m_dragSet[i].m_dragObject.rotation;
				}
			}
		}


		private void OnEnable()
		{
			if (!m_initialized)
			{
				Initialize();
			}

			Clear();
		}


		private void OnValidate()
		{
			m_tensionMultiplier = Mathf.Max(0f, m_tensionMultiplier);
			if (m_dragSet == null)
			{
				return;
			}

			if (m_dragSet.Length == 0)
			{
				return;
			}

			for (int i = 0; i < m_dragSet.Length; i++)
			{
				m_dragSet[i].m_positionTension = Mathf.Max(0f, m_dragSet[i].m_positionTension);
				m_dragSet[i].m_rotationTension = Mathf.Max(0f, m_dragSet[i].m_rotationTension);
			}

			if (m_equalTension && m_dragSet.Length >= 2)
			{
				for (int j = 1; j < m_dragSet.Length; j++)
				{
					m_dragSet[j].m_positionTension = m_dragSet[0].m_positionTension;
					m_dragSet[j].m_rotationTension = m_dragSet[0].m_rotationTension;
				}
			}
		}


		private void Initialize()
		{
			if (m_dragSet == null)
			{
				return;
			}

			if (m_dragSet.Length == 0)
			{
				return;
			}

			for (int i = 0; i < m_dragSet.Length; i++)
			{
				if (m_dragSet[i].m_sourceObject != null && m_dragSet[i].m_dragObject != null)
				{
					m_dragSet[i].m_localPosBak = m_dragSet[i].m_sourceObject
						.InverseTransformPoint(m_dragSet[i].m_dragObject.position);
					m_dragSet[i].m_localRotBak = Quaternion.Inverse(m_dragSet[i].m_sourceObject.rotation) *
					                             m_dragSet[i].m_dragObject.rotation;
					m_dragSet[i].m_posBefore = m_dragSet[i].m_dragObject.position;
					m_dragSet[i].m_rotBefore = m_dragSet[i].m_dragObject.rotation;
				}
			}

			m_initialized = true;
		}


		public void Clear()
		{
			for (int i = 0; i < m_dragSet.Length; i++)
			{
				if (m_dragSet[i].m_sourceObject != null && m_dragSet[i].m_dragObject != null)
				{
					m_dragSet[i].m_dragObject.position =
						m_dragSet[i].m_sourceObject.TransformPoint(m_dragSet[i].m_localPosBak);
					m_dragSet[i].m_dragObject.rotation =
						m_dragSet[i].m_sourceObject.rotation * m_dragSet[i].m_localRotBak;
					m_dragSet[i].m_posBefore = m_dragSet[i].m_dragObject.position;
					m_dragSet[i].m_rotBefore = m_dragSet[i].m_dragObject.rotation;
				}
			}
		}


		[Serializable]
		public struct DragTransformSet
		{
			public Transform m_sourceObject;


			public Transform m_dragObject;


			public float m_positionTension;


			public float m_rotationTension;


			[HideInInspector] public Vector3 m_localPosBak;


			[HideInInspector] public Quaternion m_localRotBak;


			[HideInInspector] public Vector3 m_posBefore;


			[HideInInspector] public Quaternion m_rotBefore;


			public DragTransformSet(Transform sourceObject, Transform dragObject, float positionTension,
				float rotationTension, Vector3 localPosBak, Quaternion localRotBak, Vector3 posBefore,
				Quaternion rotBefore)
			{
				m_sourceObject = sourceObject;
				m_dragObject = dragObject;
				m_positionTension = positionTension;
				m_rotationTension = rotationTension;
				m_localPosBak = localPosBak;
				m_localRotBak = localRotBak;
				m_posBefore = posBefore;
				m_rotBefore = rotBefore;
			}
		}
	}
}