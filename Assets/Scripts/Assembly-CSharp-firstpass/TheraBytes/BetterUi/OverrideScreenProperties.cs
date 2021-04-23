using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheraBytes.BetterUi
{
	[ExecuteAlways]
	[AddComponentMenu("Better UI/Layout/Override Screen Properties", 30)]
	public class OverrideScreenProperties : UIBehaviour, IResolutionDependency
	{
		public enum OverrideMode
		{
			Override,


			Inherit,


			ActualScreenProperty
		}


		public enum ScreenProperty
		{
			Width,


			Height,


			Dpi
		}


		[SerializeField] private Settings settingsFallback = new Settings();


		[SerializeField] private SettingsConfigCollection customSettings = new SettingsConfigCollection();


		public Settings CurrentSettings => customSettings.GetCurrentItem(settingsFallback);


		public ScreenInfo OptimizedOverride { get; } = new ScreenInfo();


		public ScreenInfo CurrentSize { get; } = new ScreenInfo();


		protected override void OnEnable()
		{
			base.OnEnable();
			OnResolutionChanged();
		}


		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			OnResolutionChanged();
		}


		protected override void OnTransformParentChanged()
		{
			base.OnTransformParentChanged();
			OnResolutionChanged();
		}


		public void OnResolutionChanged()
		{
			StopAllCoroutines();
			StartCoroutine(RecalculateRoutine());
		}


		private IEnumerator RecalculateRoutine()
		{
			yield return null;
			Settings currentItem = customSettings.GetCurrentItem(settingsFallback);
			Recalculate(currentItem);
			InformChildren();
		}


		private void Recalculate(Settings settings)
		{
			OverrideScreenProperties parent = settings.PropertyIterator().Any(o => o.Mode == OverrideMode.Inherit)
				? GetComponentInParent<OverrideScreenProperties>()
				: null;
			float x = CalculateOptimizedValue(settings, ScreenProperty.Width, parent);
			float y = CalculateOptimizedValue(settings, ScreenProperty.Height, parent);
			float dpi = CalculateOptimizedValue(settings, ScreenProperty.Dpi, parent);
			OptimizedOverride.Resolution = new Vector2(x, y);
			OptimizedOverride.Dpi = dpi;
			Rect rect = default;
			if (settings.PropertyIterator().Any(o => o.Mode == OverrideMode.Override))
			{
				rect = (transform as RectTransform).rect;
			}

			float x2 = CalculateCurrentValue(settings, ScreenProperty.Width, parent, rect);
			float y2 = CalculateCurrentValue(settings, ScreenProperty.Height, parent, rect);
			float dpi2 = CalculateCurrentValue(settings, ScreenProperty.Dpi, parent, rect);
			CurrentSize.Resolution = new Vector2(x2, y2);
			CurrentSize.Dpi = dpi2;
		}


		public float CalculateOptimizedValue(Settings settings, ScreenProperty property,
			OverrideScreenProperties parent)
		{
			switch (settings[property].Mode)
			{
				case OverrideMode.Override:
					return settings[property].Value;
				case OverrideMode.Inherit:
					if (parent != null)
					{
						switch (parent.CurrentSettings[property].Mode)
						{
							case OverrideMode.Override:
								return parent.CurrentSettings[property].Value;
							case OverrideMode.Inherit:
							{
								OverrideScreenProperties parent2 = parent
									.GetComponentsInParent<OverrideScreenProperties>()
									.FirstOrDefault(o => o.gameObject != gameObject);
								return parent.CalculateOptimizedValue(parent.CurrentSettings, property, parent2);
							}
						}
					}

					break;
				case OverrideMode.ActualScreenProperty:
					break;
				default:
					throw new ArgumentException();
			}

			ScreenInfo opimizedScreenInfo = ResolutionMonitor.GetOpimizedScreenInfo(settings.ScreenConfigName);
			switch (property)
			{
				case ScreenProperty.Width:
					return opimizedScreenInfo.Resolution.x;
				case ScreenProperty.Height:
					return opimizedScreenInfo.Resolution.y;
				case ScreenProperty.Dpi:
					return opimizedScreenInfo.Dpi;
				default:
					throw new ArgumentException();
			}
		}


		private float CalculateCurrentValue(Settings settings, ScreenProperty property, OverrideScreenProperties parent,
			Rect rect)
		{
			switch (settings[property].Mode)
			{
				case OverrideMode.Override:
					switch (property)
					{
						case ScreenProperty.Width:
							return rect.width;
						case ScreenProperty.Height:
							return rect.height;
					}

					break;
				case OverrideMode.Inherit:
					if (parent != null)
					{
						switch (parent.CurrentSettings[property].Mode)
						{
							case OverrideMode.Override:
							{
								Rect rect2 = (parent.transform as RectTransform).rect;
								return parent.CalculateCurrentValue(parent.CurrentSettings, property, null, rect2);
							}
							case OverrideMode.Inherit:
							{
								OverrideScreenProperties parent2 = parent
									.GetComponentsInParent<OverrideScreenProperties>()
									.FirstOrDefault(o => o.gameObject != gameObject);
								return parent.CalculateCurrentValue(parent.CurrentSettings, property, parent2, default);
							}
						}
					}

					break;
				case OverrideMode.ActualScreenProperty:
					break;
				default:
					throw new ArgumentException();
			}

			switch (property)
			{
				case ScreenProperty.Width:
					return ResolutionMonitor.CurrentResolution.x;
				case ScreenProperty.Height:
					return ResolutionMonitor.CurrentResolution.y;
				case ScreenProperty.Dpi:
					return ResolutionMonitor.CurrentDpi;
				default:
					throw new ArgumentException();
			}
		}


		public void InformChildren()
		{
			foreach (IResolutionDependency resolutionDependency in GetComponentsInChildren<Component>()
				.OfType<IResolutionDependency>())
			{
				if (!resolutionDependency.Equals(this))
				{
					resolutionDependency.OnResolutionChanged();
				}
			}
		}


		[Serializable]
		public class Settings : IScreenConfigConnection
		{
			public OverrideProperty OptimizedWidthOverride;


			public OverrideProperty OptimizedHeightOverride;


			public OverrideProperty OptimizedDpiOverride;


			[SerializeField] private string screenConfigName;


			public OverrideProperty this[ScreenProperty property] {
				get
				{
					switch (property)
					{
						case ScreenProperty.Width:
							return OptimizedWidthOverride;
						case ScreenProperty.Height:
							return OptimizedHeightOverride;
						case ScreenProperty.Dpi:
							return OptimizedDpiOverride;
						default:
							throw new ArgumentException();
					}
				}
			}


			
			public string ScreenConfigName {
				get => screenConfigName;
				set => screenConfigName = value;
			}


			public IEnumerable<OverrideProperty> PropertyIterator()
			{
				yield return OptimizedWidthOverride;
				yield return OptimizedHeightOverride;
				yield return OptimizedDpiOverride;
			}


			[Serializable]
			public class OverrideProperty
			{
				[SerializeField] private OverrideMode mode = default;


				[SerializeField] private float value = default;


				public OverrideMode Mode => mode;


				public float Value => value;
			}
		}


		[Serializable]
		public class SettingsConfigCollection : SizeConfigCollection<Settings> { }
	}
}