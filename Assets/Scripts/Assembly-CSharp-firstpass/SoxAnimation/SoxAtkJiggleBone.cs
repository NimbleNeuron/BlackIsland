using System;
using UnityEngine;

namespace SoxAnimation
{
	public class SoxAtkJiggleBone : MonoBehaviour
	{
		public enum Axis
		{
			X,


			Y,


			Z
		}


		public enum SimType
		{
			Simple,


			KeepDistance
		}


		public enum UpnodeControl
		{
			LookAt,


			AxisAlignment
		}


		private const float mc_tensionMul = 0.1f;


		[HideInInspector] public static SoxAtkJiggleBone[] m_jiggleBoneAll;


		[HideInInspector] public static bool m_jiggleBoneAllSearched;


		[HideInInspector] public float m_version = 1.101f;


		[HideInInspector] public bool m_ifHead = true;


		[HideInInspector] public SoxAtkJiggleBone[] m_tree;


		public bool m_animated;


		public SimType m_simType;


		public float m_targetDistance = 3f;


		public bool m_targetFlip;


		public float m_tension = 30f;


		[Range(0f, 1f)] public float m_inercia = 0.85f;


		public Axis m_lookAxis = Axis.Z;


		public bool m_lookAxisFlip;


		public Axis m_sourceUpAxis = Axis.Y;


		public bool m_sourceUpAxisFlip;


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


		private Vector3 m_boneEndDragWPos;


		private Vector3 m_forceVec;


		private bool m_initialized;


		private Vector3 m_lookWPos;


		private Transform m_proxyAlign;


		private Transform m_proxyLook;


		private Vector3 m_resultVec;


		private Axis m_sourceUpAxisBefore;


		private SoxAtkTentacle m_soxAtkTentacle;


		private Transform m_target;


		private Transform m_targetRoot;


		private float m_tensionProxy;


		private Transform m_tentacle;


		private bool m_treeInit;


		private bool m_upNodeAutoSet;


		private Vector3 m_upVector;


		private Transform meTrans;


		private void Awake()
		{
			if (!m_jiggleBoneAllSearched)
			{
				m_jiggleBoneAll = FindObjectsOfType<SoxAtkJiggleBone>();
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


		private void LateUpdate()
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

			if (m_proxyLook != null)
			{
				DestroyImmediate(m_proxyLook.gameObject);
			}

			if (m_proxyAlign != null)
			{
				DestroyImmediate(m_proxyAlign.gameObject);
			}

			if (m_tentacle != null)
			{
				DestroyImmediate(m_tentacle.gameObject);
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
				switch (m_lookAxis)
				{
					case Axis.X:
						vector = transform.TransformPoint(new Vector3(m_targetDistance * num2, 0f, 0f));
						break;
					case Axis.Y:
						vector = transform.TransformPoint(new Vector3(0f, m_targetDistance * num2, 0f));
						break;
					case Axis.Z:
						vector = transform.TransformPoint(new Vector3(0f, 0f, m_targetDistance * num2));
						break;
				}

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
			m_boneEndDragWPos = Vector3.zero;
			m_forceVec = Vector3.zero;
			m_resultVec = Vector3.zero;
			m_lookWPos = Vector3.zero;
			m_proxyLook = new GameObject("SoxAtkJiggleboneProxyLook_" + transform.name).transform;
			m_proxyLook.parent = m_targetRoot;
			m_proxyLook.hideFlags = HideFlags.HideInHierarchy;
			m_proxyLook.localPosition = Vector3.zero;
			m_proxyLook.localRotation = Quaternion.identity;
			m_proxyLook.localScale = Vector3.one;
			m_proxyAlign = new GameObject("SoxAtkJiggleboneProxyAlign_" + transform.name).transform;
			m_proxyAlign.parent = m_proxyLook;
			m_proxyAlign.hideFlags = HideFlags.HideInHierarchy;
			m_proxyAlign.localPosition = Vector3.zero;
			m_proxyAlign.localRotation = Quaternion.identity;
			m_proxyAlign.localScale = Vector3.one;
			m_beforTargetWPos = m_target.position;
			m_beforeInerciaVec = Vector3.zero;
			SetOptions();
			EnsureGoodVars();
			m_initialized = true;
		}


		private void InitializeTentacle()
		{
			m_tentacle = new GameObject("SoxAtkJiggleboneTentacle_" + transform.name).transform;
			m_tentacle.parent = transform.parent;
			m_tentacle.hideFlags = HideFlags.HideInHierarchy;
			m_tentacle.localPosition = transform.localPosition;
			m_tentacle.localRotation = transform.localRotation;
			m_tentacle.localScale = transform.localScale;
		}


		private void SetHead()
		{
			if (meTrans.parent == null)
			{
				return;
			}

			SoxAtkJiggleBone soxAtkJiggleBone = meTrans.parent.GetComponent<SoxAtkJiggleBone>();
			if (soxAtkJiggleBone == null)
			{
				return;
			}

			if (!soxAtkJiggleBone.gameObject.activeInHierarchy || !soxAtkJiggleBone.enabled)
			{
				return;
			}

			bool flag = false;
			while (!flag)
			{
				if (soxAtkJiggleBone.m_ifHead)
				{
					flag = true;
				}
				else
				{
					soxAtkJiggleBone = soxAtkJiggleBone.m_tree[0];
				}
			}

			if (!soxAtkJiggleBone.m_treeInit)
			{
				soxAtkJiggleBone.m_tree = new[]
				{
					soxAtkJiggleBone
				};
				soxAtkJiggleBone.m_treeInit = true;
			}

			soxAtkJiggleBone.m_tree = ArrayAdd(soxAtkJiggleBone.m_tree, m_tree);
			m_ifHead = false;
			m_tree = new[]
			{
				soxAtkJiggleBone
			};
		}


		public void SetRealHead()
		{
			int num = 0;
			bool flag = false;
			SoxAtkJiggleBone soxAtkJiggleBone = m_tree[0];
			SoxAtkJiggleBone soxAtkJiggleBone2 = m_tree[0];
			while (!flag)
			{
				if (soxAtkJiggleBone.m_ifHead)
				{
					soxAtkJiggleBone2 = soxAtkJiggleBone;
					flag = true;
				}
				else
				{
					soxAtkJiggleBone = soxAtkJiggleBone.m_tree[0];
					num++;
				}

				if (num > 100000)
				{
					flag = true;
				}
			}

			m_tree[0] = soxAtkJiggleBone2;
		}


		private SoxAtkJiggleBone[] ArrayAdd(SoxAtkJiggleBone[] arrA, SoxAtkJiggleBone[] arrB)
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

			SoxAtkJiggleBone[] array = new SoxAtkJiggleBone[arrA.Length + arrB.Length];
			Array.Copy(arrA, 0, array, 0, arrA.Length);
			Array.Copy(arrB, 0, array, arrA.Length, arrB.Length);
			return array;
		}


		public void JiggleBoneUpdateTree()
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
			if (m_animated)
			{
				if (m_tentacle == null)
				{
					m_targetRoot.localRotation = meTrans.localRotation;
				}
				else
				{
					m_targetRoot.localRotation = m_tentacle.localRotation * meTrans.localRotation;
				}
			}
			else if (m_tentacle != null)
			{
				m_targetRoot.localRotation = m_tentacle.localRotation;
			}

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

			SimType simType = m_simType;
			if (simType != SimType.Simple)
			{
				if (simType == SimType.KeepDistance)
				{
					m_boneEndDragWPos = (m_beforTargetWPos - meTrans.position).normalized * m_targetDistance +
					                    meTrans.position;
					m_forceVec = a - m_boneEndDragWPos;
					m_resultVec = m_forceVec * m_tensionProxy * Time.smoothDeltaTime +
					              m_beforeInerciaVec * Mathf.Lerp(m_inercia, 0f, Time.smoothDeltaTime);
					m_resultVec += m_gravity * Time.smoothDeltaTime;
					if (flag)
					{
						m_resultVec *= d;
						m_beforeInerciaVec *= d;
					}

					m_lookWPos = m_boneEndDragWPos + m_resultVec;
				}
			}
			else
			{
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
			}

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
				m_proxyLook.LookAt(m_lookWPos, m_upVector);
				meTrans.rotation = m_proxyAlign.rotation;
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
			switch (m_lookAxis)
			{
				case Axis.X:
					m_target.localPosition = new Vector3(m_targetDistance * num, 0f, 0f);
					return;
				case Axis.Y:
					m_target.localPosition = new Vector3(0f, m_targetDistance * num, 0f);
					return;
				case Axis.Z:
					m_target.localPosition = new Vector3(0f, 0f, m_targetDistance * num);
					return;
				default:
					return;
			}
		}


		public void SetOptions()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			Vector3 worldPosition = Vector3.zero;
			Vector3 worldUp = Vector3.one;
			switch (m_lookAxis)
			{
				case Axis.X:
				{
					Axis sourceUpAxis = m_sourceUpAxis;
					if (sourceUpAxis != Axis.Y)
					{
						if (sourceUpAxis == Axis.Z)
						{
							if (!m_lookAxisFlip && !m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 1f, 0f));
								worldUp = m_proxyLook.TransformPoint(new Vector3(1f, 0f, 0f)) - m_proxyLook.position;
							}

							if (!m_lookAxisFlip && m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, -1f, 0f));
								worldUp = m_proxyLook.TransformPoint(new Vector3(-1f, 0f, 0f)) - m_proxyLook.position;
							}

							if (m_lookAxisFlip && !m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 1f, 0f));
								worldUp = m_proxyLook.TransformPoint(new Vector3(-1f, 0f, 0f)) - m_proxyLook.position;
							}

							if (m_lookAxisFlip && m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, -1f, 0f));
								worldUp = m_proxyLook.TransformPoint(new Vector3(1f, 0f, 0f)) - m_proxyLook.position;
							}
						}
					}
					else
					{
						if (!m_lookAxisFlip && !m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(-1f, 0f, 0f));
							worldUp = m_proxyLook.TransformPoint(new Vector3(0f, 1f, 0f)) - m_proxyLook.position;
						}

						if (!m_lookAxisFlip && m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(1f, 0f, 0f));
							worldUp = m_proxyLook.TransformPoint(new Vector3(0f, -1f, 0f)) - m_proxyLook.position;
						}

						if (m_lookAxisFlip && !m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(1f, 0f, 0f));
							worldUp = m_proxyLook.TransformPoint(new Vector3(0f, 1f, 0f)) - m_proxyLook.position;
						}

						if (m_lookAxisFlip && m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(-1f, 0f, 0f));
							worldUp = m_proxyLook.TransformPoint(new Vector3(0f, -1f, 0f)) - m_proxyLook.position;
						}
					}

					break;
				}
				case Axis.Y:
				{
					Axis sourceUpAxis = m_sourceUpAxis;
					if (sourceUpAxis != Axis.X)
					{
						if (sourceUpAxis == Axis.Z)
						{
							if (!m_lookAxisFlip && !m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 1f, 0f));
								worldUp = m_proxyLook.TransformPoint(new Vector3(0f, 0f, 1f)) - m_proxyLook.position;
							}

							if (!m_lookAxisFlip && m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, -1f, 0f));
								worldUp = m_proxyLook.TransformPoint(new Vector3(0f, 0f, 1f)) - m_proxyLook.position;
							}

							if (m_lookAxisFlip && !m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 1f, 0f));
								worldUp = -m_proxyLook.TransformPoint(new Vector3(0f, 0f, -1f)) - m_proxyLook.position;
							}

							if (m_lookAxisFlip && m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, -1f, 0f));
								worldUp = -m_proxyLook.TransformPoint(new Vector3(0f, 0f, -1f)) - m_proxyLook.position;
							}
						}
					}
					else
					{
						if (!m_lookAxisFlip && !m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(1f, 0f, 0f));
							worldUp = m_proxyLook.TransformPoint(new Vector3(0f, 0f, 1f)) - m_proxyLook.position;
						}

						if (!m_lookAxisFlip && m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(-1f, 0f, 0f));
							worldUp = m_proxyLook.TransformPoint(new Vector3(0f, 0f, 1f)) - m_proxyLook.position;
						}

						if (m_lookAxisFlip && !m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(-1f, 0f, 0f));
							worldUp = -m_proxyLook.TransformPoint(new Vector3(0f, 0f, -1f)) - m_proxyLook.position;
						}

						if (m_lookAxisFlip && m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(1f, 0f, 0f));
							worldUp = -m_proxyLook.TransformPoint(new Vector3(0f, 0f, -1f)) - m_proxyLook.position;
						}
					}

					break;
				}
				case Axis.Z:
				{
					Axis sourceUpAxis = m_sourceUpAxis;
					if (sourceUpAxis != Axis.X)
					{
						if (sourceUpAxis == Axis.Y)
						{
							if (!m_lookAxisFlip && !m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 0f, 1f));
								worldUp = m_proxyLook.TransformPoint(new Vector3(0f, 1f, 0f)) - m_proxyLook.position;
							}

							if (!m_lookAxisFlip && m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 0f, 1f));
								worldUp = -m_proxyLook.TransformPoint(new Vector3(0f, -1f, 0f)) - m_proxyLook.position;
							}

							if (m_lookAxisFlip && !m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 0f, -1f));
								worldUp = m_proxyLook.TransformPoint(new Vector3(0f, 1f, 0f)) - m_proxyLook.position;
							}

							if (m_lookAxisFlip && m_sourceUpAxisFlip)
							{
								worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 0f, -1f));
								worldUp = -m_proxyLook.TransformPoint(new Vector3(0f, -1f, 0f)) - m_proxyLook.position;
							}
						}
					}
					else
					{
						if (!m_lookAxisFlip && !m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 0f, 1f));
							worldUp = -m_proxyLook.TransformPoint(new Vector3(-1f, 0f, 0f)) - m_proxyLook.position;
						}

						if (!m_lookAxisFlip && m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 0f, 1f));
							worldUp = m_proxyLook.TransformPoint(new Vector3(1f, 0f, 0f)) - m_proxyLook.position;
						}

						if (m_lookAxisFlip && !m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 0f, -1f));
							worldUp = m_proxyLook.TransformPoint(new Vector3(1f, 0f, 0f)) - m_proxyLook.position;
						}

						if (m_lookAxisFlip && m_sourceUpAxisFlip)
						{
							worldPosition = m_proxyLook.TransformPoint(new Vector3(0f, 0f, -1f));
							worldUp = -m_proxyLook.TransformPoint(new Vector3(-1f, 0f, 0f)) - m_proxyLook.position;
						}
					}

					break;
				}
			}

			m_proxyAlign.LookAt(worldPosition, worldUp);
			if (m_upWorld || m_upNode == null)
			{
				switch (m_upNodeAxis)
				{
					case Axis.X:
						m_upVector = Vector3.right;
						return;
					case Axis.Y:
						m_upVector = Vector3.up;
						return;
					case Axis.Z:
						m_upVector = Vector3.forward;
						break;
					default:
						return;
				}
			}
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
				m_proxyLook.hideFlags = HideFlags.None;
				m_proxyAlign.hideFlags = HideFlags.None;
				if (m_tentacle != null)
				{
					m_tentacle.hideFlags = HideFlags.None;
				}
			}
			else
			{
				m_targetRoot.hideFlags = HideFlags.HideInHierarchy;
				m_target.hideFlags = HideFlags.HideInHierarchy;
				m_proxyLook.hideFlags = HideFlags.HideInHierarchy;
				m_proxyAlign.hideFlags = HideFlags.HideInHierarchy;
				if (m_tentacle != null)
				{
					m_tentacle.hideFlags = HideFlags.HideInHierarchy;
				}
			}

			m_hierarchyChanged = true;
		}


		public void MyValidate()
		{
			EnsureGoodVars();
			if (m_targetRoot != null)
			{
				SetTargetDistance();
				SetOptions();
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

			if (m_lookAxis == m_sourceUpAxis)
			{
				m_sourceUpAxis = m_sourceUpAxisBefore;
			}

			if (m_lookAxis == m_sourceUpAxis)
			{
				switch (m_lookAxis)
				{
					case Axis.X:
						m_sourceUpAxis = Axis.Y;
						break;
					case Axis.Y:
						m_sourceUpAxis = Axis.X;
						break;
					case Axis.Z:
						m_sourceUpAxis = Axis.Y;
						break;
				}
			}

			m_sourceUpAxisBefore = m_sourceUpAxis;
		}


		public Transform SetMixedTentacle(SoxAtkTentacle soxAtkTentacle)
		{
			if (m_tentacle == null)
			{
				InitializeTentacle();
			}

			m_soxAtkTentacle = soxAtkTentacle;
			m_tree[0].m_ifHead = false;
			if (!soxAtkTentacle.m_jiggleboneHeads.Contains(m_tree[0]))
			{
				soxAtkTentacle.m_jiggleboneHeads.Add(m_tree[0]);
			}

			return m_tentacle;
		}


		public void RemoveMixedTentacle()
		{
			if (m_tentacle != null)
			{
				DestroyImmediate(m_tentacle.gameObject);
			}

			m_soxAtkTentacle = null;
			m_tree[0].m_ifHead = true;
			if (m_soxAtkTentacle != null && m_soxAtkTentacle.m_jiggleboneHeads.Contains(m_tree[0]))
			{
				m_soxAtkTentacle.m_jiggleboneHeads.Remove(m_tree[0]);
			}
		}


		public Transform GetTentacle()
		{
			return m_tentacle;
		}
	}
}