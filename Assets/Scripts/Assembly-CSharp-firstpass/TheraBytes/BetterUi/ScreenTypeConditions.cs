using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class ScreenTypeConditions
	{
		[SerializeField] private string name = "Screen";


		[SerializeField] private IsCertainScreenOrientation checkOrientation;


		[SerializeField] private IsScreenOfCertainSize checkScreenSize;


		[SerializeField] private IsCertainAspectRatio checkAspectRatio;


		[SerializeField] private IsScreenOfCertainDeviceInfo checkDeviceType;


		[SerializeField] private ScreenInfo optimizedScreenInfo;


		[SerializeField] private List<string> fallbacks = new List<string>();


		public ScreenTypeConditions(string displayName, bool optimizedScreenInfo = false, bool orientation = false,
			bool bigger = false, bool smaller = false, bool touch = false, bool vr = false, bool deviceType = false)
		{
			name = displayName;
			this.optimizedScreenInfo = new ScreenInfo(new Vector2(1920f, 1080f), 96f);
			checkOrientation = new IsCertainScreenOrientation(IsCertainScreenOrientation.Orientation.Landscape)
			{
				IsActive = orientation
			};
			checkScreenSize = new IsScreenOfCertainSize
			{
				IsActive = bigger
			};
			checkAspectRatio = new IsCertainAspectRatio
			{
				IsActive = bigger
			};
			checkDeviceType = new IsScreenOfCertainDeviceInfo
			{
				IsActive = deviceType
			};
		}


		
		public string Name {
			get => name;
			set => name = value;
		}


		
		public bool IsActive { get; private set; }


		public List<string> Fallbacks => fallbacks;


		public Vector2 OptimizedResolution {
			get
			{
				if (optimizedScreenInfo == null)
				{
					return ResolutionMonitor.OptimizedResolutionFallback;
				}

				return optimizedScreenInfo.Resolution;
			}
		}


		public int OptimizedWidth => (int) OptimizedResolution.x;


		public int OptimizedHeight => (int) OptimizedResolution.y;


		public float OptimizedDpi {
			get
			{
				if (optimizedScreenInfo == null)
				{
					return ResolutionMonitor.OptimizedDpiFallback;
				}

				return optimizedScreenInfo.Dpi;
			}
		}


		public IsCertainScreenOrientation CheckOrientation => checkOrientation;


		public IsScreenOfCertainSize CheckScreenSize => checkScreenSize;


		public IsCertainAspectRatio CheckAspectRatio => checkAspectRatio;


		public IsScreenOfCertainDeviceInfo CheckDeviceType => checkDeviceType;


		public ScreenInfo OptimizedScreenInfo => optimizedScreenInfo;


		public bool IsScreenType()
		{
			IsActive = (!checkOrientation.IsActive || checkOrientation.IsScreenType()) &&
			           (!checkScreenSize.IsActive || checkScreenSize.IsScreenType()) &&
			           (!checkAspectRatio.IsActive || checkAspectRatio.IsScreenType()) &&
			           (!checkDeviceType.IsActive || checkDeviceType.IsScreenType());
			return IsActive;
		}
	}
}