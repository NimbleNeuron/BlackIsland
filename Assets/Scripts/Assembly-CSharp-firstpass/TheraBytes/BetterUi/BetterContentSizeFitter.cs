using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Layout/Better Content Size Fitter", 30)]
	public class BetterContentSizeFitter : ContentSizeFitter, IResolutionDependency, ILayoutChildDependency
	{
		[SerializeField] private RectTransform source;


		[SerializeField] private Settings settingsFallback = new Settings();


		[SerializeField] private SettingsConfigCollection customSettings = new SettingsConfigCollection();


		[SerializeField] private FloatSizeModifier minWidthSizerFallback = new FloatSizeModifier(0f, 0f, 4000f);


		[SerializeField] private FloatSizeConfigCollection minWidthSizers = new FloatSizeConfigCollection();


		[SerializeField] private FloatSizeModifier minHeightSizerFallback = new FloatSizeModifier(0f, 0f, 4000f);


		[SerializeField] private FloatSizeConfigCollection minHeightSizers = new FloatSizeConfigCollection();


		[SerializeField] private FloatSizeModifier maxWidthSizerFallback = new FloatSizeModifier(1000f, 0f, 4000f);


		[SerializeField] private FloatSizeConfigCollection maxWidthSizers = new FloatSizeConfigCollection();


		[SerializeField] private FloatSizeModifier maxHeightSizerFallback = new FloatSizeModifier(1000f, 0f, 4000f);


		[SerializeField] private FloatSizeConfigCollection maxHeightSizers = new FloatSizeConfigCollection();


		[SerializeField] private Vector2SizeModifier paddingFallback =
			new Vector2SizeModifier(default, new Vector2(-5000f, -5000f), new Vector2(5000f, 5000f));


		[SerializeField] private Vector2SizeConfigCollection paddingSizers = new Vector2SizeConfigCollection();


		private readonly RectTransformData end = new RectTransformData();


		private readonly RectTransformData start = new RectTransformData();


		private bool isAnimating;


		private RectTransform rectTransform => transform as RectTransform;


		public Settings CurrentSettings => customSettings.GetCurrentItem(settingsFallback);


		
		public RectTransform Source {
			get
			{
				if (!(source != null))
				{
					return rectTransform;
				}

				return source;
			}
			set
			{
				source = value;
				SetDirty();
			}
		}


		protected override void OnEnable()
		{
			base.OnEnable();
			Apply();
		}


		protected override void OnDisable()
		{
			base.OnDisable();
			isAnimating = false;
		}


		public void ChildSizeChanged(Transform child)
		{
			ChildChanged();
		}


		public void ChildAddedOrEnabled(Transform child)
		{
			ChildChanged();
		}


		public void ChildRemovedOrDisabled(Transform child)
		{
			ChildChanged();
		}


		public void OnResolutionChanged()
		{
			Apply();
		}


		private void Apply()
		{
			Settings currentSettings = CurrentSettings;
			m_HorizontalFit = currentSettings.HorizontalFit;
			m_VerticalFit = currentSettings.VerticalFit;
			SetDirty();
		}


		public override void SetLayoutHorizontal()
		{
			SetLayout(0);
		}


		public override void SetLayoutVertical()
		{
			SetLayout(1);
		}


		private void SetLayout(int axis)
		{
			if (axis == 0 && CurrentSettings.HorizontalFit == FitMode.Unconstrained)
			{
				return;
			}

			if (axis == 1 && CurrentSettings.VerticalFit == FitMode.Unconstrained)
			{
				return;
			}

			if (isAnimating)
			{
				return;
			}

			if (CurrentSettings.IsAnimated)
			{
				start.PullFromTransform(transform as RectTransform);
			}

			if (axis == 0)
			{
				base.SetLayoutHorizontal();
			}
			else
			{
				base.SetLayoutVertical();
			}

			ApplyOffsetToDefaultSize(axis, axis == 0 ? m_HorizontalFit : m_VerticalFit);
			if (CurrentSettings.IsAnimated)
			{
				end.PullFromTransform(transform as RectTransform);
				start.PushToTransform(transform as RectTransform);
				Animate();
			}
		}


		private void ApplyOffsetToDefaultSize(int axis, FitMode fitMode)
		{
			Vector2 vector = paddingSizers.GetCurrentItem(paddingFallback).CalculateSize(this);
			bool flag = axis == 0 ? CurrentSettings.HasMaxWidth : CurrentSettings.HasMaxHeight;
			bool flag2 = axis == 0 ? CurrentSettings.HasMinWidth : CurrentSettings.HasMinHeight;
			if (flag || flag2 || !Mathf.Approximately(vector[axis], 0f) || source != null)
			{
				float num = fitMode == FitMode.MinSize
					? LayoutUtility.GetMinSize(Source, axis)
					: LayoutUtility.GetPreferredSize(Source, axis);
				num += vector[axis];
				num = ClampSize((RectTransform.Axis) axis, num);
				rectTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) axis, num);
			}
		}


		private float ClampSize(RectTransform.Axis axis, float size)
		{
			if (axis != RectTransform.Axis.Horizontal)
			{
				if (axis == RectTransform.Axis.Vertical)
				{
					if (CurrentSettings.HasMinHeight)
					{
						size = Mathf.Max(size,
							minHeightSizers.GetCurrentItem(minHeightSizerFallback).CalculateSize(this));
					}

					if (CurrentSettings.HasMaxHeight)
					{
						size = Mathf.Min(size,
							maxHeightSizers.GetCurrentItem(maxHeightSizerFallback).CalculateSize(this));
					}
				}
			}
			else
			{
				if (CurrentSettings.HasMinWidth)
				{
					size = Mathf.Max(size, minWidthSizers.GetCurrentItem(minWidthSizerFallback).CalculateSize(this));
				}

				if (CurrentSettings.HasMaxWidth)
				{
					size = Mathf.Min(size, maxWidthSizers.GetCurrentItem(maxWidthSizerFallback).CalculateSize(this));
				}
			}

			return size;
		}


		private Bounds GetChildBounds()
		{
			RectTransform root = transform as RectTransform;
			Bounds result = default;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					Bounds bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(root, child);
					result.Encapsulate(bounds);
				}
			}

			return result;
		}


		private void ChildChanged()
		{
			bool isAnimated = CurrentSettings.IsAnimated;
			CurrentSettings.IsAnimated = false;
			SetLayoutHorizontal();
			SetLayoutVertical();
			CurrentSettings.IsAnimated = isAnimated;
		}


		private void Animate()
		{
			if (!CurrentSettings.IsAnimated)
			{
				return;
			}

			StopAllCoroutines();
			StartCoroutine(CoAnimate());
		}


		private IEnumerator CoAnimate()
		{
			float t = 0f;
			isAnimating = true;
			yield return null;
			while (t < CurrentSettings.AnimationTime)
			{
				t += Time.deltaTime;
				float amount = Mathf.SmoothStep(0f, 1f, t / CurrentSettings.AnimationTime);
				RectTransformData.Lerp(start, end, amount).PushToTransform(transform as RectTransform);
				yield return null;
			}

			end.PushToTransform(transform as RectTransform);
			isAnimating = false;
			CurrentSettings.IsAnimated = false;
			SetLayoutHorizontal();
			SetLayoutVertical();
			CurrentSettings.IsAnimated = true;
		}


		[Serializable]
		public class Settings : IScreenConfigConnection
		{
			public FitMode HorizontalFit;


			public FitMode VerticalFit;


			public bool IsAnimated;


			public float AnimationTime = 0.2f;


			public bool HasMinWidth;


			public bool HasMinHeight;


			public bool HasMaxWidth;


			public bool HasMaxHeight;


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