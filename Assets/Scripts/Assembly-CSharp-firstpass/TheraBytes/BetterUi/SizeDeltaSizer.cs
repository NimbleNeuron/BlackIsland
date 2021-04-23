using System;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("Better UI/Layout/Size Delta Sizer", 30)]
	public class SizeDeltaSizer : ResolutionSizer<Vector2>
	{
		[SerializeField] private Settings settingsFallback = new Settings();


		[SerializeField] private SettingsConfigCollection customSettings = new SettingsConfigCollection();


		[SerializeField] private Vector2SizeModifier deltaSizerFallback =
			new Vector2SizeModifier(100f * Vector2.one, Vector2.zero, 1000f * Vector2.one);


		[SerializeField] private Vector2SizeConfigCollection customDeltaSizers = new Vector2SizeConfigCollection();


		public Settings CurrentSettings => customSettings.GetCurrentItem(settingsFallback);


		public Vector2SizeModifier DeltaSizer => customDeltaSizers.GetCurrentItem(deltaSizerFallback);


		protected override ScreenDependentSize<Vector2> sizer => customDeltaSizers.GetCurrentItem(deltaSizerFallback);


		protected override void ApplySize(Vector2 newSize)
		{
			RectTransform rectTransform = transform as RectTransform;
			Vector2 sizeDelta = rectTransform.sizeDelta;
			Settings currentSettings = CurrentSettings;
			if (currentSettings.ApplyWidth)
			{
				sizeDelta.x = newSize.x;
			}

			if (currentSettings.ApplyHeight)
			{
				sizeDelta.y = newSize.y;
			}

			rectTransform.sizeDelta = sizeDelta;
		}


		[Serializable]
		public class Settings : IScreenConfigConnection
		{
			[SerializeField] private bool applyWidth;


			[SerializeField] private bool applyHeight;


			[SerializeField] private string screenConfigName;


			
			public bool ApplyWidth {
				get => applyWidth;
				set => applyWidth = value;
			}


			
			public bool ApplyHeight {
				get => applyHeight;
				set => applyHeight = value;
			}


			
			public string ScreenConfigName {
				get => screenConfigName;
				set => screenConfigName = value;
			}
		}


		[Serializable]
		public class SettingsConfigCollection : SizeConfigCollection<Settings> { }
	}
}