using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoxAnimation
{
	public class SoxAtkTentacle : MonoBehaviour
	{
		public enum Axis
		{
			X,


			Y,


			Z
		}


		public const float mc_freqMul = 0.01f;


		[HideInInspector] public float m_version = 1.101f;


		[HideInInspector] public bool m_animated;


		[HideInInspector] public bool m_keepInitialRotation = true;


		public Transform[] m_nodes = new Transform[5];


		[HideInInspector] public List<SoxAtkJiggleBone> m_jiggleboneHeads = new List<SoxAtkJiggleBone>();


		public Waveset[] wavesets =
		{
			new Waveset(Axis.X, -20f, 5f, 40f, 5f, 0f, new float[5])
		};


		private float m_distanceAll;


		private bool m_initialized;


		private Transform[] m_nodesOriginal = new Transform[5];


		private float[] m_nodesSaveDistances = new float[5];


		private Quaternion[] m_nodesSaveLocalRotations = new Quaternion[5];


		private Transform m_rootNode;


		private Quaternion m_rootRotation;


		private Vector3[] m_wavesetMixEuler = new Vector3[5];


		private void Start()
		{
			if (!m_initialized && gameObject.activeInHierarchy && enabled)
			{
				Initialize();
			}
		}


		private void LateUpdate()
		{
			if (wavesets == null)
			{
				return;
			}

			if (m_rootNode != null)
			{
				m_rootRotation = m_rootNode.rotation;
			}

			if (m_animated)
			{
				SaveLocalRotations();
			}

			UpdateTentalces(Time.time, false);
			if (m_jiggleboneHeads.Count > 0)
			{
				for (int i = 0; i < m_jiggleboneHeads.Count; i++)
				{
					m_jiggleboneHeads[i].JiggleBoneUpdateTree();
				}
			}
		}


		private void OnEnable()
		{
			if (!m_initialized)
			{
				Initialize();
				return;
			}

			SaveNodesJigglebone();
		}


		private void OnDisable()
		{
			RevertNodesJigglebone();
		}


		public void Initialize()
		{
			InitArrays();
			if (m_nodes[0] != null)
			{
				m_rootNode = m_nodes[0].parent;
			}

			if (m_rootNode != null)
			{
				m_rootRotation = m_rootNode.rotation;
			}
			else
			{
				m_rootRotation = Quaternion.identity;
			}

			for (int i = 0; i < m_nodes.Length; i++)
			{
				m_nodesOriginal[i] = m_nodes[i];
			}

			SaveLocalRotations();
			SaveDistances();
			SaveStrengths();
			SaveNodesJigglebone();
			for (int j = 0; j < m_wavesetMixEuler.Length; j++)
			{
				m_wavesetMixEuler[j] = Vector3.zero;
			}

			m_initialized = true;
		}


		public Vector3[] UpdateTentalces(float time, bool bake)
		{
			for (int i = 0; i < m_nodes.Length; i++)
			{
				m_wavesetMixEuler[i] = Vector3.zero;
			}

			for (int j = 0; j < wavesets.Length; j++)
			{
				for (int k = 0; k < m_nodes.Length; k++)
				{
					float num = Mathf.Sin(
						(m_nodesSaveDistances[k] + wavesets[j].m_offset) * wavesets[j].m_frequencyProxy +
						time * wavesets[j].m_speed) * wavesets[j].m_nodesSaveStrengths[k];
					switch (wavesets[j].m_rotateAxis)
					{
						case Axis.X:
						{
							Vector3[] wavesetMixEuler = m_wavesetMixEuler;
							int num2 = k;
							wavesetMixEuler[num2].x = wavesetMixEuler[num2].x + num;
							break;
						}
						case Axis.Y:
						{
							Vector3[] wavesetMixEuler2 = m_wavesetMixEuler;
							int num3 = k;
							wavesetMixEuler2[num3].y = wavesetMixEuler2[num3].y + num;
							break;
						}
						case Axis.Z:
						{
							Vector3[] wavesetMixEuler3 = m_wavesetMixEuler;
							int num4 = k;
							wavesetMixEuler3[num4].z = wavesetMixEuler3[num4].z + num;
							break;
						}
					}
				}
			}

			for (int l = 0; l < m_nodes.Length; l++)
			{
				if (m_nodes[l] != null)
				{
					if (m_keepInitialRotation || m_animated)
					{
						m_nodes[l].rotation = m_rootRotation * m_nodesSaveLocalRotations[l] *
						                      Quaternion.Euler(m_wavesetMixEuler[l]);
					}
					else
					{
						m_nodes[l].rotation = m_rootRotation * Quaternion.Euler(m_wavesetMixEuler[l]);
					}
				}
			}

			return m_wavesetMixEuler;
		}


		private void InitArrays()
		{
			if (wavesets != null)
			{
				if (m_nodes.Length < 1)
				{
					m_nodes = new Transform[1];
				}

				if (m_nodes.Length != m_nodesOriginal.Length)
				{
					m_nodesOriginal = new Transform[m_nodes.Length];
				}

				for (int i = 0; i < m_nodes.Length; i++)
				{
					m_nodesOriginal[i] = m_nodes[i];
				}

				if (m_nodes.Length != m_nodesSaveLocalRotations.Length)
				{
					m_nodesSaveLocalRotations = new Quaternion[m_nodes.Length];
				}

				if (m_nodes.Length != m_nodesSaveDistances.Length)
				{
					m_nodesSaveDistances = new float[m_nodes.Length];
				}

				for (int j = 0; j < wavesets.Length; j++)
				{
					if (m_nodes.Length != wavesets[j].m_nodesSaveStrengths.Length)
					{
						wavesets[j].m_nodesSaveStrengths = new float[m_nodes.Length];
					}
				}

				if (m_nodes.Length != m_wavesetMixEuler.Length)
				{
					m_wavesetMixEuler = new Vector3[m_nodes.Length];
				}

				SaveFrequencies();
			}
		}


		public void MyValidate()
		{
			if (!m_initialized && gameObject.activeInHierarchy && enabled)
			{
				Initialize();
			}

			SaveFrequencies();
			SaveStrengths();
		}


		public void SaveLocalRotations()
		{
			if (wavesets == null)
			{
				return;
			}

			for (int i = 0; i < m_nodesOriginal.Length; i++)
			{
				if (m_nodesOriginal[i] != null)
				{
					m_nodesSaveLocalRotations[i] = Quaternion.Inverse(m_rootRotation) * m_nodesOriginal[i].rotation;
				}
				else
				{
					m_nodesSaveLocalRotations[i] = Quaternion.identity;
				}
			}
		}


		public void SaveDistances()
		{
			if (wavesets == null)
			{
				return;
			}

			m_distanceAll = 0f;
			m_nodesSaveDistances[0] = 0f;
			for (int i = 1; i < m_nodes.Length; i++)
			{
				if (m_nodes[i] != null && m_nodes[i - 1] != null)
				{
					float num = (Mathf.Abs(m_nodes[i].lossyScale.x) + Mathf.Abs(m_nodes[i].lossyScale.y) +
					             Mathf.Abs(m_nodes[i].lossyScale.z)) / 3f;
					m_nodesSaveDistances[i] =
						m_distanceAll + Vector3.Distance(m_nodes[i].position, m_nodes[i - 1].position) / num;
				}
				else
				{
					m_nodesSaveDistances[i] = m_distanceAll;
				}

				m_distanceAll = m_nodesSaveDistances[i];
			}
		}


		private void SaveFrequencies()
		{
			for (int i = 0; i < wavesets.Length; i++)
			{
				wavesets[i].m_frequencyProxy = wavesets[i].m_frequency * 0.01f;
			}
		}


		private void SaveStrengths()
		{
			if (wavesets == null)
			{
				return;
			}

			for (int i = 0; i < wavesets.Length; i++)
			{
				float num = wavesets[i].m_strengthEnd - wavesets[i].m_strengthStart;
				wavesets[i].m_nodesSaveStrengths[0] = wavesets[i].m_strengthStart;
				for (int j = 1; j < m_nodes.Length; j++)
				{
					float num2 = m_nodesSaveDistances[j] / m_distanceAll;
					wavesets[i].m_nodesSaveStrengths[j] = wavesets[i].m_strengthStart + num * num2;
				}
			}
		}


		private void SaveNodesJigglebone()
		{
			for (int i = 0; i < m_nodes.Length; i++)
			{
				if (m_nodes[i] != null)
				{
					SoxAtkJiggleBone component = m_nodes[i].GetComponent<SoxAtkJiggleBone>();
					if (component != null && component.gameObject.activeInHierarchy && component.enabled)
					{
						m_nodes[i] = component.SetMixedTentacle(this);
					}
				}
			}
		}


		private void RevertNodesJigglebone()
		{
			for (int i = 0; i < m_nodes.Length; i++)
			{
				if (m_nodes[i] != null)
				{
					SoxAtkJiggleBone component = m_nodesOriginal[i].GetComponent<SoxAtkJiggleBone>();
					if (component != null && component.gameObject.activeInHierarchy && component.enabled)
					{
						component.RemoveMixedTentacle();
					}

					m_nodes[i] = m_nodesOriginal[i];
				}
			}
		}


		[Serializable]
		public struct Waveset
		{
			public Axis m_rotateAxis;


			public float m_frequency;


			[HideInInspector] public float m_frequencyProxy;


			public float m_strengthStart;


			public float m_strengthEnd;


			public float m_speed;


			public float m_offset;


			[HideInInspector] public float[] m_nodesSaveStrengths;


			public Waveset(Axis rotateAxis, float frequency, float strengthStart, float strengthEnd, float speed,
				float offset, float[] nodesSaveStrengths)
			{
				m_rotateAxis = rotateAxis;
				m_frequency = frequency;
				m_frequencyProxy = frequency * 0.01f;
				m_strengthStart = strengthStart;
				m_strengthEnd = strengthEnd;
				m_speed = speed;
				m_offset = offset;
				m_nodesSaveStrengths = nodesSaveStrengths;
			}
		}
	}
}