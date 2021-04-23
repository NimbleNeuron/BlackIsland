using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class DpiManager
	{
		[SerializeField] private List<DpiOverride> overrides = new List<DpiOverride>();


		public float GetDpi()
		{
			DpiOverride dpiOverride = overrides.FirstOrDefault(o => o.DeviceModel == SystemInfo.deviceModel);
			if (dpiOverride != null)
			{
				return dpiOverride.Dpi;
			}

			return Screen.dpi;
		}


		[Serializable]
		public class DpiOverride
		{
			[SerializeField] private float dpi = 96f;


			[SerializeField] private string deviceModel;


			public DpiOverride(string deviceModel, float dpi)
			{
				this.deviceModel = deviceModel;
				this.dpi = dpi;
			}


			public float Dpi => dpi;


			public string DeviceModel => deviceModel;
		}
	}
}