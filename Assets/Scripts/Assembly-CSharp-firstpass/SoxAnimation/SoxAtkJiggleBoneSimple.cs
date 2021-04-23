using System;
using UnityEngine;

namespace SoxAnimation
{
	public class SoxAtkJiggleBoneSimple : MonoBehaviour
	{
		public enum Axis
		{
			X,


			Y,


			Z
		}


		public enum UpnodeControl
		{
			LookAt,


			AxisAlignment
		}


		private const float mc_tensionMul = 0.1f;


		[HideInInspector] public static SoxAtkJiggleBoneSimple[] m_jiggleBoneAll;


		[HideInInspector] public static bool m_jiggleBoneAllSearched;


		[HideInInspector] public float m_version = 1.101f;


		[HideInInspector] public bool m_ifHead = true;


		[HideInInspector] public SoxAtkJiggleBoneSimple[] m_tree;


		public float m_targetDistance = 3f;


		public bool m_targetFlip;


		public float m_tension = 30f;


		[Range(0f, 1f)] public float m_inercia = 0.85f;


		public bool m_upWorld;


		public Transform m_upNode;


		public Axis m_upNodeAxis = Axis.Y;


		public UpnodeControl m_upnodeControl = UpnodeControl.AxisAlignment;


		public Vector3 m_gravity = Vector3.zero;


		public SoxAtkCollider[] m_colliders;


		public bool m_optShowGizmosAtPlaying;


		public bool m_optShowGizmosAtEditor = true;


		public float m_optGizmoSize = 0.1f;


		public bool m_optShowHiddenNodes;


		public bool m_hierarchyChanged;


		private Vector3 m_beforeInerciaVec;


		private Vector3 m_beforTargetWPos;


		private Vector3 m_forceVec;


		private bool m_initialized;


		private Vector3 m_lookWPos;


		private Vector3 m_resultVec;


		private SoxAtkTentacle m_soxAtkTentacle;


		private Transform m_target;


		private Transform m_targetRoot;


		private float m_tensionProxy;


		private bool m_treeInit;


		private bool m_upNodeAutoSet;


		private Vector3 m_upVector;


		private Transform meTrans;


		private void Awake()
		{
			if (!m_jiggleBoneAllSearched)
			{
				m_jiggleBoneAll = FindObjectsOfType<SoxAtkJiggleBoneSimple>();
				m_jiggleBoneAllSearched = true;
				for (int i = 0; i < m_jiggleBoneAll.Length; i++)
				{
					if (!m_jiggleBoneAll[i].m_initialized && m_jiggleBoneAll[i].gameObject.activeInHierarchy &&
					    m_jiggleBoneAll[i].enabled)
					{
						m_jiggleBoneAll[i].Initialize();
					}
				}

				for (int j = 0; j < m_jiggleBoneAll.Length; j++)
				{
					if (m_jiggleBoneAll[j].gameObject.activeInHierarchy && m_jiggleBoneAll[j].enabled)
					{
						m_jiggleBoneAll[j].SetRealHead();
					}
				}

				m_jiggleBoneAll = null;
			}
		}


		private void Update()
		{
			if (m_ifHead)
			{
				JiggleBoneUpdateTree();
			}
		}


		private void OnEnable()
		{
			if (!m_initialized)
			{
				Initialize();
				SetRealHead();
			}

			Clear();
		}


		private void OnDestroy()
		{
			if (m_target != null)
			{
				DestroyImmediate(m_target.gameObject);
			}

			if (m_targetRoot != null)
			{
				DestroyImmediate(m_targetRoot.gameObject);
			}
		}


		private void OnDrawGizmos()
		{
			if (!gameObject.activeInHierarchy || !enabled)
			{
				return;
			}

			float num = m_optGizmoSize * transform.lossyScale.x;
			if (Application.isPlaying)
			{
				if (m_optShowGizmosAtPlaying)
				{
					Gizmos.color = Color.yellow;
					Gizmos.DrawLine(meTrans.position, m_target.position);
					Gizmos.DrawWireSphere(m_target.position, num);
					Gizmos.color = Color.green;
					Gizmos.DrawLine(meTrans.position, m_lookWPos);
					Gizmos.DrawWireSphere(m_lookWPos, num * 0.6f);
				}
			}
			else if (m_optShowGizmosAtEditor)
			{
				Vector3 vector = Vector3.zero;
				float num2 = m_targetFlip ? -1f : 1f;
				vector = transform.TransformPoint(new Vector3(0f, 0f, m_targetDistance * num2));
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(transform.position, vector);
				Gizmos.DrawWireSphere(vector, num);
			}
		}


		public void Initialize()
		{
			if (m_initialized)
			{
				return;
			}

			meTrans = transform;
			if (!m_treeInit)
			{
				m_tree = new[]
				{
					this
				};
				m_treeInit = true;
			}

			SetHead();
			m_upVector = Vector3.up;
			m_targetRoot = new GameObject("SoxAtkJiggleboneTargetRoot_" + transform.name).transform;
			m_targetRoot.parent = meTrans.parent;
			m_targetRoot.hideFlags = HideFlags.HideInHierarchy;
			m_targetRoot.localPosition = meTrans.localPosition;
			m_targetRoot.localRotation = meTrans.localRotation;
			m_targetRoot.localScale = meTrans.localScale;
			m_target = new GameObject("SoxAtkJiggleboneTarget_" + transform.name).transform;
			m_target.parent = m_targetRoot;
			m_target.hideFlags = HideFlags.HideInHierarchy;
			SetTargetDistance();
			m_target.localRotation = Quaternion.identity;
			m_target.localScale = Vector3.one;
			m_forceVec = Vector3.zero;
			m_resultVec = Vector3.zero;
			m_lookWPos = Vector3.zero;
			m_beforTargetWPos = m_target.position;
			m_beforeInerciaVec = Vector3.zero;
			EnsureGoodVars();
			m_initialized = true;
		}


		private void SetHead()
		{
			if (meTrans.parent == null)
			{
				return;
			}

			SoxAtkJiggleBoneSimple soxAtkJiggleBoneSimple = meTrans.parent.GetComponent<SoxAtkJiggleBoneSimple>();
			if (soxAtkJiggleBoneSimple == null)
			{
				return;
			}

			if (!soxAtkJiggleBoneSimple.gameObject.activeInHierarchy || !soxAtkJiggleBoneSimple.enabled)
			{
				return;
			}

			bool flag = false;
			while (!flag)
			{
				if (soxAtkJiggleBoneSimple.m_ifHead)
				{
					flag = true;
				}
				else
				{
					soxAtkJiggleBoneSimple = soxAtkJiggleBoneSimple.m_tree[0];
				}
			}

			if (!soxAtkJiggleBoneSimple.m_treeInit)
			{
				soxAtkJiggleBoneSimple.m_tree = new[]
				{
					soxAtkJiggleBoneSimple
				};
				soxAtkJiggleBoneSimple.m_treeInit = true;
			}

			soxAtkJiggleBoneSimple.m_tree = ArrayAdd(soxAtkJiggleBoneSimple.m_tree, m_tree);
			m_ifHead = false;
			m_tree = new[]
			{
				soxAtkJiggleBoneSimple
			};
		}


		public void SetRealHead()
		{
			int num = 0;
			bool flag = false;
			SoxAtkJiggleBoneSimple soxAtkJiggleBoneSimple = m_tree[0];
			SoxAtkJiggleBoneSimple soxAtkJiggleBoneSimple2 = m_tree[0];
			while (!flag)
			{
				if (soxAtkJiggleBoneSimple.m_ifHead)
				{
					soxAtkJiggleBoneSimple2 = soxAtkJiggleBoneSimple;
					flag = true;
				}
				else
				{
					soxAtkJiggleBoneSimple = soxAtkJiggleBoneSimple.m_tree[0];
					num++;
				}

				if (num > 100000)
				{
					flag = true;
				}
			}

			m_tree[0] = soxAtkJiggleBoneSimple2;
		}


		private SoxAtkJiggleBoneSimple[] ArrayAdd(SoxAtkJiggleBoneSimple[] arrA, SoxAtkJiggleBoneSimple[] arrB)
		{
			if (arrA == null && arrB != null)
			{
				return arrB;
			}

			if (arrA != null && arrB == null)
			{
				return arrA;
			}

			if (arrA == null && arrB == null)
			{
				return null;
			}

			SoxAtkJiggleBoneSimple[] array = new SoxAtkJiggleBoneSimple[arrA.Length + arrB.Length];
			Array.Copy(arrA, 0, array, 0, arrA.Length);
			Array.Copy(arrB, 0, array, arrA.Length, arrB.Length);
			return array;
		}


		private void JiggleBoneUpdateTree()
		{
			for (int i = 0; i < m_tree.Length; i++)
			{
				m_tree[i].JiggleBoneUpdate();
			}
		}


		public void JiggleBoneUpdate()
		{
			if (!gameObject.activeInHierarchy || !enabled)
			{
				return;
			}

			m_targetRoot.position = meTrans.position;
			m_targetRoot.localScale = meTrans.localScale;
			Vector3 a = m_target.position;
			bool flag = false;
			float d = 1f;
			if (m_colliders.Length != 0)
			{
				for (int i = 0; i < m_colliders.Length; i++)
				{
					if (m_colliders[i] != null)
					{
						Vector3 vector = m_target.position - m_colliders[i].transform.position;
						if (vector.magnitude < m_colliders[i].m_sphereRadiusScaled)
						{
							a = m_colliders[i].transform.position +
							    vector.normalized * m_colliders[i].m_sphereRadiusScaled;
						}

						if (m_colliders[i].m_frictionInverse > 0f &&
						    Vector3.Distance(m_beforTargetWPos, m_colliders[i].transform.position) <
						    m_colliders[i].m_sphereRadiusScaled)
						{
							flag = true;
							d = m_colliders[i].m_frictionInverse;
						}
					}
				}
			}

			m_forceVec = a - m_beforTargetWPos;
			m_resultVec = m_forceVec * m_tensionProxy * Time.smoothDeltaTime +
			              m_beforeInerciaVec * Mathf.Lerp(m_inercia, 0f, Time.smoothDeltaTime);
			m_resultVec += m_gravity * Time.smoothDeltaTime;
			if (flag)
			{
				m_resultVec *= d;
				m_beforeInerciaVec *= d;
			}

			m_lookWPos = m_beforTargetWPos + m_resultVec;
			if (m_upnodeControl == UpnodeControl.AxisAlignment)
			{
				if (!m_upWorld && m_upNode != null)
				{
					switch (m_upNodeAxis)
					{
						case Axis.X:
							m_upVector = m_upNode.right;
							break;
						case Axis.Y:
							m_upVector = m_upNode.up;
							break;
						case Axis.Z:
							m_upVector = m_upNode.forward;
							break;
					}
				}
			}
			else if (!m_upWorld && m_upNode != null)
			{
				m_upVector = m_upNode.position - meTrans.position;
			}

			try
			{
				meTrans.LookAt(m_lookWPos, m_upVector);
			}
			catch { }

			m_beforTargetWPos = m_lookWPos;
			m_beforeInerciaVec = m_resultVec;
		}


		public bool Clear()
		{
			if (m_ifHead)
			{
				for (int i = 0; i < m_tree.Length; i++)
				{
					m_tree[i].m_beforTargetWPos = m_tree[i].m_target.position;
					m_tree[i].m_beforeInerciaVec = Vector3.zero;
				}

				return true;
			}

			return false;
		}


		public void SetTargetDistance()
		{
			if (m_target == null)
			{
				return;
			}

			float num = m_targetFlip ? -1f : 1f;
			m_target.localPosition = new Vector3(0f, 0f, m_targetDistance * num);
		}


		public void SetHiddenNodes()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			if (m_optShowHiddenNodes)
			{
				m_targetRoot.hideFlags = HideFlags.None;
				m_target.hideFlags = HideFlags.None;
			}
			else
			{
				m_targetRoot.hideFlags = HideFlags.HideInHierarchy;
				m_target.hideFlags = HideFlags.HideInHierarchy;
			}

			m_hierarchyChanged = true;
		}


		public void MyValidate()
		{
			EnsureGoodVars();
			if (m_targetRoot != null)
			{
				SetTargetDistance();
				SetHiddenNodes();
			}
		}


		public void EnsureGoodVars()
		{
			m_tensionProxy = Mathf.Max(0f, m_tension) * 0.1f;
			m_targetDistance = Mathf.Max(0f, m_targetDistance);
			m_optGizmoSize = Mathf.Max(0f, m_optGizmoSize);
			if (!m_upNodeAutoSet)
			{
				if (transform.parent != null)
				{
					m_upNode = transform.parent;
				}

				m_upNodeAutoSet = true;
			}

			if (m_upNode == transform)
			{
				m_upNode = null;
			}
		}
	}
}