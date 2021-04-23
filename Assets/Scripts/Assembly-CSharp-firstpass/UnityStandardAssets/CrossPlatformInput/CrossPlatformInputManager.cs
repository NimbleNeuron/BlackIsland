using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput.PlatformSpecific;

namespace UnityStandardAssets.CrossPlatformInput
{
	public static class CrossPlatformInputManager
	{
		public enum ActiveInputMethod
		{
			Hardware,


			Touch
		}


		private static VirtualInput activeInput;


		private static readonly VirtualInput s_TouchInput = new MobileInput();


		private static readonly VirtualInput s_HardwareInput = new StandaloneInput();


		static CrossPlatformInputManager()
		{
			activeInput = s_HardwareInput;
		}


		public static Vector3 mousePosition => activeInput.MousePosition();


		public static void SwitchActiveInputMethod(ActiveInputMethod activeInputMethod)
		{
			if (activeInputMethod == ActiveInputMethod.Hardware)
			{
				activeInput = s_HardwareInput;
				return;
			}

			if (activeInputMethod != ActiveInputMethod.Touch)
			{
				return;
			}

			activeInput = s_TouchInput;
		}


		public static bool AxisExists(string name)
		{
			return activeInput.AxisExists(name);
		}


		public static bool ButtonExists(string name)
		{
			return activeInput.ButtonExists(name);
		}


		public static void RegisterVirtualAxis(VirtualAxis axis)
		{
			activeInput.RegisterVirtualAxis(axis);
		}


		public static void RegisterVirtualButton(VirtualButton button)
		{
			activeInput.RegisterVirtualButton(button);
		}


		public static void UnRegisterVirtualAxis(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			activeInput.UnRegisterVirtualAxis(name);
		}


		public static void UnRegisterVirtualButton(string name)
		{
			activeInput.UnRegisterVirtualButton(name);
		}


		public static VirtualAxis VirtualAxisReference(string name)
		{
			return activeInput.VirtualAxisReference(name);
		}


		public static float GetAxis(string name)
		{
			return GetAxis(name, false);
		}


		public static float GetAxisRaw(string name)
		{
			return GetAxis(name, true);
		}


		private static float GetAxis(string name, bool raw)
		{
			return activeInput.GetAxis(name, raw);
		}


		public static bool GetButton(string name)
		{
			return activeInput.GetButton(name);
		}


		public static bool GetButtonDown(string name)
		{
			return activeInput.GetButtonDown(name);
		}


		public static bool GetButtonUp(string name)
		{
			return activeInput.GetButtonUp(name);
		}


		public static void SetButtonDown(string name)
		{
			activeInput.SetButtonDown(name);
		}


		public static void SetButtonUp(string name)
		{
			activeInput.SetButtonUp(name);
		}


		public static void SetAxisPositive(string name)
		{
			activeInput.SetAxisPositive(name);
		}


		public static void SetAxisNegative(string name)
		{
			activeInput.SetAxisNegative(name);
		}


		public static void SetAxisZero(string name)
		{
			activeInput.SetAxisZero(name);
		}


		public static void SetAxis(string name, float value)
		{
			activeInput.SetAxis(name, value);
		}


		public static void SetVirtualMousePositionX(float f)
		{
			activeInput.SetVirtualMousePositionX(f);
		}


		public static void SetVirtualMousePositionY(float f)
		{
			activeInput.SetVirtualMousePositionY(f);
		}


		public static void SetVirtualMousePositionZ(float f)
		{
			activeInput.SetVirtualMousePositionZ(f);
		}


		public class VirtualAxis
		{
			private float m_Value;


			public VirtualAxis(string name) : this(name, true) { }


			public VirtualAxis(string name, bool matchToInputSettings)
			{
				this.name = name;
				matchWithInputManager = matchToInputSettings;
			}


			
			public string name { get; }


			
			public bool matchWithInputManager { get; }


			public float GetValue => m_Value;


			public float GetValueRaw => m_Value;


			public void Remove()
			{
				UnRegisterVirtualAxis(name);
			}


			public void Update(float value)
			{
				m_Value = value;
			}
		}


		public class VirtualButton
		{
			private int m_LastPressedFrame = -5;


			private bool m_Pressed;


			private int m_ReleasedFrame = -5;


			public VirtualButton(string name) : this(name, true) { }


			public VirtualButton(string name, bool matchToInputSettings)
			{
				this.name = name;
				matchWithInputManager = matchToInputSettings;
			}


			
			public string name { get; }


			
			public bool matchWithInputManager { get; }


			public bool GetButton => m_Pressed;


			public bool GetButtonDown => m_LastPressedFrame - Time.frameCount == -1;


			public bool GetButtonUp => m_ReleasedFrame == Time.frameCount - 1;


			public void Pressed()
			{
				if (m_Pressed)
				{
					return;
				}

				m_Pressed = true;
				m_LastPressedFrame = Time.frameCount;
			}


			public void Released()
			{
				m_Pressed = false;
				m_ReleasedFrame = Time.frameCount;
			}


			public void Remove()
			{
				UnRegisterVirtualButton(name);
			}
		}
	}
}