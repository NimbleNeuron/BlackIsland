using System;
using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Layout/Better Aspect Ratio Fitter", 30)]
	public class BetterAspectRatioFitter : AspectRatioFitter, IResolutionDependency
	{
		[SerializeField] private Settings settingsFallback = new Settings();


		[SerializeField] private SettingsConfigCollection customSettings = new SettingsConfigCollection();


		public Settings CurrentSettings => customSettings.GetCurrentItem(settingsFallback);


		protected override void OnEnable()
		{
			base.OnEnable();
			Apply();
		}


		public void OnResolutionChanged()
		{
			Apply();
		}


		private void Apply()
		{
			aspectMode = CurrentSettings.AspectMode;
			aspectRatio = CurrentSettings.AspectRatio;
		}


		[Serializable]
		public class Settings : IScreenConfigConnection
		{
			public AspectMode AspectMode;


			public float AspectRatio = 1f;


			[SerializeField] private string screenConfigName;


			
			public string ScreenConfigName {
				get => screenConfigName;
				set => screenConfigName = value;
			}
		}


		[Serializable]
		public class SettingsConfigCollection : SizeConfigCollection<Settings> { }
	}
}