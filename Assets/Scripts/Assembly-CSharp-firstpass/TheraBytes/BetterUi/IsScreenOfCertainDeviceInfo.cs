using System;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class IsScreenOfCertainDeviceInfo : IScreenTypeCheck, IIsActive
	{
		public enum DeviceInfo
		{
			Other,


			Handheld,


			Console,


			Desktop,


			TouchScreen,


			VirtualReality
		}


		[SerializeField] private DeviceInfo expectedDeviceInfo;


		[SerializeField] private bool isActive;


		
		public DeviceInfo ExpectedDeviceInfo {
			get => expectedDeviceInfo;
			set => expectedDeviceInfo = value;
		}


		
		public bool IsActive {
			get => isActive;
			set => isActive = value;
		}


		public bool IsScreenType()
		{
			switch (expectedDeviceInfo)
			{
				case DeviceInfo.Other:
					return SystemInfo.deviceType == DeviceType.Unknown && !TouchScreenKeyboard.isSupported;
				case DeviceInfo.Handheld:
					return SystemInfo.deviceType == DeviceType.Handheld;
				case DeviceInfo.Console:
					return SystemInfo.deviceType == DeviceType.Console;
				case DeviceInfo.Desktop:
					return SystemInfo.deviceType == DeviceType.Desktop;
				case DeviceInfo.TouchScreen:
					return TouchScreenKeyboard.isSupported;
				case DeviceInfo.VirtualReality:
					return false;
				default:
					throw new NotImplementedException();
			}
		}
	}
}