﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityStandardAssets.CrossPlatformInput
{
	[RequireComponent(typeof(Image))]
	public class TouchPad : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		public enum AxisOption
		{
			Both,


			OnlyHorizontal,


			OnlyVertical
		}


		public enum ControlStyle
		{
			Absolute,


			Relative,


			Swipe
		}


		public AxisOption axesToUse;


		public ControlStyle controlStyle;


		public string horizontalAxisName = "Horizontal";


		public string verticalAxisName = "Vertical";


		public float Xsensitivity = 1f;


		public float Ysensitivity = 1f;


		private Vector3 m_Center;


		private bool m_Dragging;


		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;


		private int m_Id = -1;


		private Image m_Image;


		private Vector3 m_JoytickOutput;


		private Vector2 m_PreviousDelta;


		private Vector2 m_PreviousTouchPos;


		private Vector3 m_StartPos;


		private bool m_UseX;


		private bool m_UseY;


		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;


		private void Start()
		{
			m_Image = GetComponent<Image>();
			m_Center = m_Image.transform.position;
		}


		private void Update()
		{
			if (!m_Dragging)
			{
				return;
			}

			if (Input.touchCount >= m_Id + 1 && m_Id != -1)
			{
				if (controlStyle == ControlStyle.Swipe)
				{
					m_Center = m_PreviousTouchPos;
					m_PreviousTouchPos = Input.touches[m_Id].position;
				}

				Vector2 normalized = new Vector2(Input.touches[m_Id].position.x - m_Center.x,
					Input.touches[m_Id].position.y - m_Center.y).normalized;
				normalized.x *= Xsensitivity;
				normalized.y *= Ysensitivity;
				UpdateVirtualAxes(new Vector3(normalized.x, normalized.y, 0f));
			}
		}


		private void OnEnable()
		{
			CreateVirtualAxes();
		}


		private void OnDisable()
		{
			if (CrossPlatformInputManager.AxisExists(horizontalAxisName))
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(horizontalAxisName);
			}

			if (CrossPlatformInputManager.AxisExists(verticalAxisName))
			{
				CrossPlatformInputManager.UnRegisterVirtualAxis(verticalAxisName);
			}
		}


		public void OnPointerDown(PointerEventData data)
		{
			m_Dragging = true;
			m_Id = data.pointerId;
			if (controlStyle != ControlStyle.Absolute)
			{
				m_Center = data.position;
			}
		}


		public void OnPointerUp(PointerEventData data)
		{
			m_Dragging = false;
			m_Id = -1;
			UpdateVirtualAxes(Vector3.zero);
		}


		private void CreateVirtualAxes()
		{
			m_UseX = axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal;
			m_UseY = axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_HorizontalVirtualAxis);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_VerticalVirtualAxis);
			}
		}


		private void UpdateVirtualAxes(Vector3 value)
		{
			value = value.normalized;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(value.x);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(value.y);
			}
		}
	}
}