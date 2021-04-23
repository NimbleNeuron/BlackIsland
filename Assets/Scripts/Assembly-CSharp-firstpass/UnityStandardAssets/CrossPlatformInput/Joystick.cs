using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class Joystick : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler
	{
		public enum AxisOption
		{
			Both,


			OnlyHorizontal,


			OnlyVertical
		}


		public int MovementRange = 100;


		public AxisOption axesToUse;


		public string horizontalAxisName = "Horizontal";


		public string verticalAxisName = "Vertical";


		private CrossPlatformInputManager.VirtualAxis m_HorizontalVirtualAxis;


		private Vector3 m_StartPos;


		private bool m_UseX;


		private bool m_UseY;


		private CrossPlatformInputManager.VirtualAxis m_VerticalVirtualAxis;


		private void Start()
		{
			m_StartPos = transform.position;
		}


		private void OnEnable()
		{
			CreateVirtualAxes();
		}


		private void OnDisable()
		{
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Remove();
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Remove();
			}
		}


		public void OnDrag(PointerEventData data)
		{
			Vector3 zero = Vector3.zero;
			if (m_UseX)
			{
				int num = (int) (data.position.x - m_StartPos.x);
				num = Mathf.Clamp(num, -MovementRange, MovementRange);
				zero.x = num;
			}

			if (m_UseY)
			{
				int num2 = (int) (data.position.y - m_StartPos.y);
				num2 = Mathf.Clamp(num2, -MovementRange, MovementRange);
				zero.y = num2;
			}

			transform.position = new Vector3(m_StartPos.x + zero.x, m_StartPos.y + zero.y, m_StartPos.z + zero.z);
			UpdateVirtualAxes(transform.position);
		}


		public void OnPointerDown(PointerEventData data) { }


		public void OnPointerUp(PointerEventData data)
		{
			transform.position = m_StartPos;
			UpdateVirtualAxes(m_StartPos);
		}


		private void UpdateVirtualAxes(Vector3 value)
		{
			Vector3 vector = m_StartPos - value;
			vector.y = -vector.y;
			vector /= (float) MovementRange;
			if (m_UseX)
			{
				m_HorizontalVirtualAxis.Update(-vector.x);
			}

			if (m_UseY)
			{
				m_VerticalVirtualAxis.Update(vector.y);
			}
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
	}
}