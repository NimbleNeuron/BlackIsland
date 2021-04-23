using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
	public class TiltInput : MonoBehaviour
	{
		public enum AxisOptions
		{
			ForwardAxis,


			SidewaysAxis
		}


		public AxisMapping mapping;


		public AxisOptions tiltAroundAxis;


		public float fullTiltAngle = 25f;


		public float centreAngleOffset;


		private CrossPlatformInputManager.VirtualAxis m_SteerAxis;


		private void Update()
		{
			float value = 0f;
			if (Input.acceleration != Vector3.zero)
			{
				AxisOptions axisOptions = tiltAroundAxis;
				if (axisOptions != AxisOptions.ForwardAxis)
				{
					if (axisOptions == AxisOptions.SidewaysAxis)
					{
						value = Mathf.Atan2(Input.acceleration.z, -Input.acceleration.y) * 57.29578f +
						        centreAngleOffset;
					}
				}
				else
				{
					value = Mathf.Atan2(Input.acceleration.x, -Input.acceleration.y) * 57.29578f + centreAngleOffset;
				}
			}

			float num = Mathf.InverseLerp(-fullTiltAngle, fullTiltAngle, value) * 2f - 1f;
			switch (mapping.type)
			{
				case AxisMapping.MappingType.NamedAxis:
					m_SteerAxis.Update(num);
					return;
				case AxisMapping.MappingType.MousePositionX:
					CrossPlatformInputManager.SetVirtualMousePositionX(num * Screen.width);
					return;
				case AxisMapping.MappingType.MousePositionY:
					CrossPlatformInputManager.SetVirtualMousePositionY(num * Screen.width);
					return;
				case AxisMapping.MappingType.MousePositionZ:
					CrossPlatformInputManager.SetVirtualMousePositionZ(num * Screen.width);
					return;
				default:
					return;
			}
		}


		private void OnEnable()
		{
			if (mapping.type == AxisMapping.MappingType.NamedAxis)
			{
				m_SteerAxis = new CrossPlatformInputManager.VirtualAxis(mapping.axisName);
				CrossPlatformInputManager.RegisterVirtualAxis(m_SteerAxis);
			}
		}


		private void OnDisable()
		{
			m_SteerAxis.Remove();
		}


		[Serializable]
		public class AxisMapping
		{
			public enum MappingType
			{
				NamedAxis,


				MousePositionX,


				MousePositionY,


				MousePositionZ
			}


			public MappingType type;


			public string axisName;
		}
	}
}