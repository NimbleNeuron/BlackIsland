using System.Collections.Generic;
using UnityEngine;

namespace SoxAnimation
{
	[ExecuteInEditMode]
	public class SoxAtkLookAt : MonoBehaviour
	{
		public enum axisType
		{
			X,


			Y,


			Z
		}


		public enum lookType
		{
			Camera,


			Nodes
		}


		public enum upCtrType
		{
			LootAt,


			AxisAlignment
		}


		public enum upType
		{
			Camera,


			Node,


			World
		}


		[HideInInspector] public float m_version = 1.101f;


		public bool m_EditorLookAt;


		public lookType m_lookAtType;


		public List<Transform> m_lookAtNodeList = new List<Transform>();


		public bool m_lookAtFilp;


		public upType m_upAxisType = upType.World;


		public Transform m_upNode;


		public upCtrType m_upControl = upCtrType.AxisAlignment;


		public bool m_sourceAxisFilp;


		public axisType m_alignedToUpnodeAxis = axisType.Y;


		private Vector3 m_lookPos;


		private void Update()
		{
			if (!m_EditorLookAt && !Application.isPlaying)
			{
				return;
			}

			transform.rotation = Quaternion.LookRotation(GetForwardVec(), GetUpwardVec());
		}


		private Vector3 GetForwardVec()
		{
			SetLookPos();
			return m_lookPos - transform.position;
		}


		private Vector3 GetUpwardVec()
		{
			Vector3 position = new Vector3(0f, 0f, 0f);
			Vector3 a = new Vector3(0f, 1f, 0f);
			Vector3 vector = new Vector3(0f, 1f, 0f);
			switch (m_alignedToUpnodeAxis)
			{
				case axisType.X:
					vector = new Vector3(1f, 0f, 0f);
					break;
				case axisType.Y:
					vector = new Vector3(0f, 1f, 0f);
					break;
				case axisType.Z:
					vector = new Vector3(0f, 0f, 1f);
					break;
			}

			if (m_sourceAxisFilp)
			{
				vector *= -1f;
			}

			upCtrType upControl = m_upControl;
			if (upControl != upCtrType.LootAt)
			{
				if (upControl == upCtrType.AxisAlignment)
				{
					switch (m_upAxisType)
					{
						case upType.Camera:
							if (Camera.main != null)
							{
								position = Camera.main.transform.position;
								a = Camera.main.transform.TransformPoint(vector);
							}

							break;
						case upType.Node:
							if (m_upNode != null)
							{
								position = m_upNode.transform.position;
								a = m_upNode.TransformPoint(vector);
							}

							break;
						case upType.World:
							position = new Vector3(0f, 0f, 0f);
							a = vector;
							break;
					}
				}
			}
			else
			{
				position = transform.position;
				switch (m_upAxisType)
				{
					case upType.Camera:
						if (Camera.main != null)
						{
							a = Camera.main.transform.position;
						}

						break;
					case upType.Node:
						if (m_upNode != null)
						{
							a = m_upNode.transform.position;
						}

						break;
					case upType.World:
						a = new Vector3(0f, 0f, 0f);
						break;
				}
			}

			return a - position;
		}


		private void SetLookPos()
		{
			lookType lookAtType = m_lookAtType;
			if (lookAtType != lookType.Camera)
			{
				if (lookAtType == lookType.Nodes)
				{
					m_lookPos = this.transform.position + this.transform.forward;
					int num = 0;
					Vector3 a = new Vector3(0f, 0f, 0f);
					foreach (Transform transform in m_lookAtNodeList)
					{
						if (transform != null)
						{
							num++;
							a += transform.position;
						}
					}

					m_lookPos = a / num;
				}
			}
			else if (Camera.main)
			{
				m_lookPos = Camera.main.transform.position;
			}
			else
			{
				m_lookPos = transform.position + transform.forward;
			}

			if (m_lookAtFilp)
			{
				m_lookPos = transform.position + (transform.position - m_lookPos);
			}
		}
	}
}