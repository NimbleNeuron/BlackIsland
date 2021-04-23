using System;
using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Layout/Better Layout Element", 30)]
	public class BetterLayoutElement : LayoutElement, IResolutionDependency
	{
		[SerializeField] private Settings settingsFallback = new Settings();


		[SerializeField] private SettingsConfigCollection customSettings = new SettingsConfigCollection();


		[SerializeField] private FloatSizeModifier minWidthSizerFallback = new FloatSizeModifier(0f, 0f, 5000f);


		[SerializeField] private FloatSizeConfigCollection customMinWidthSizers = new FloatSizeConfigCollection();


		[SerializeField] private FloatSizeModifier minHeightSizerFallback = new FloatSizeModifier(0f, 0f, 5000f);


		[SerializeField] private FloatSizeConfigCollection customMinHeightSizers = new FloatSizeConfigCollection();


		[SerializeField] private FloatSizeModifier preferredWidthSizerFallback = new FloatSizeModifier(100f, 0f, 5000f);


		[SerializeField] private FloatSizeConfigCollection customPreferredWidthSizers = new FloatSizeConfigCollection();


		[SerializeField]
		private FloatSizeModifier preferredHeightSizerFallback = new FloatSizeModifier(100f, 0f, 5000f);


		[SerializeField]
		private FloatSizeConfigCollection customPreferredHeightSizers = new FloatSizeConfigCollection();


		public Settings CurrentSettings => customSettings.GetCurrentItem(settingsFallback);


		public FloatSizeModifier MinWidthSizer => customMinWidthSizers.GetCurrentItem(minWidthSizerFallback);


		public FloatSizeModifier MinHeightSizer => customMinHeightSizers.GetCurrentItem(minHeightSizerFallback);


		public FloatSizeModifier PreferredWidthSizer =>
			customPreferredWidthSizers.GetCurrentItem(preferredWidthSizerFallback);


		public FloatSizeModifier PreferredHeightSizer =>
			customPreferredHeightSizers.GetCurrentItem(preferredHeightSizerFallback);


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
			Settings currentSettings = CurrentSettings;
			base.ignoreLayout = currentSettings.IgnoreLayout;
			base.minWidth = currentSettings.MinWidthEnabled ? MinWidthSizer.CalculateSize(this) : -1f;
			base.minHeight = currentSettings.MinHeightEnabled ? MinHeightSizer.CalculateSize(this) : -1f;
			base.preferredWidth = currentSettings.PreferredWidthEnabled ? PreferredWidthSizer.CalculateSize(this) : -1f;
			base.preferredHeight =
				currentSettings.PreferredHeightEnabled ? PreferredHeightSizer.CalculateSize(this) : -1f;
			base.flexibleWidth = currentSettings.FlexibleWidthEnabled ? currentSettings.FlexibleWidth : -1f;
			base.flexibleHeight = currentSettings.FlexibleHeightEnabled ? currentSettings.FlexibleHeight : -1f;
		}


		[Serializable]
		public class Settings : IScreenConfigConnection
		{
			public bool IgnoreLayout;


			public bool MinWidthEnabled;


			public bool MinHeightEnabled;


			public bool PreferredWidthEnabled;


			public bool PreferredHeightEnabled;


			public bool FlexibleWidthEnabled;


			public bool FlexibleHeightEnabled;


			public float FlexibleWidth = 1f;


			public float FlexibleHeight = 1f;


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