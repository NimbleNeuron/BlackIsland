using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Layout/Better Axis Aligned Layout Group", 30)]
	public class BetterAxisAlignedLayoutGroup : HorizontalOrVerticalLayoutGroup, IBetterHorizontalOrVerticalLayoutGroup,
		IResolutionDependency
	{
		public enum Axis
		{
			Horizontal,


			Vertical
		}


		[SerializeField] private MarginSizeModifier paddingSizerFallback =
			new MarginSizeModifier(new Margin(), new Margin(), new Margin(1000, 1000, 1000, 1000));


		[SerializeField] private MarginSizeConfigCollection customPaddingSizers = new MarginSizeConfigCollection();


		[SerializeField] private FloatSizeModifier spacingSizerFallback = new FloatSizeModifier(0f, 0f, 300f);


		[SerializeField] private FloatSizeConfigCollection customSpacingSizers = new FloatSizeConfigCollection();


		[SerializeField] private Settings settingsFallback;


		[SerializeField] private SettingsConfigCollection customSettings = new SettingsConfigCollection();


		[SerializeField] private Axis orientation;


		public Settings CurrentSettings => customSettings.GetCurrentItem(settingsFallback);


		
		public Axis Orientation {
			get => orientation;
			set => orientation = value;
		}


		private bool isVertical => orientation == Axis.Vertical;


		protected override void OnEnable()
		{
			base.OnEnable();
			if (settingsFallback == null || string.IsNullOrEmpty(settingsFallback.ScreenConfigName))
			{
				StartCoroutine(InitDelayed());
				return;
			}

			CalculateCellSize();
		}


		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			SetDirty();
		}


		protected override void OnTransformChildrenChanged()
		{
			base.OnTransformChildrenChanged();
			if (isActiveAndEnabled)
			{
				StartCoroutine(SetDirtyDelayed());
			}
		}


		public MarginSizeModifier PaddingSizer => customPaddingSizers.GetCurrentItem(paddingSizerFallback);


		public FloatSizeModifier SpacingSizer => customSpacingSizers.GetCurrentItem(spacingSizerFallback);


		public void OnResolutionChanged()
		{
			CalculateCellSize();
		}


		private IEnumerator SetDirtyDelayed()
		{
			yield return null;
			SetDirty();
		}


		private IEnumerator InitDelayed()
		{
			yield return null;
			settingsFallback = new Settings(childAlignment, childForceExpandWidth, childForceExpandHeight, orientation)
			{
				ChildControlWidth = childControlWidth,
				ChildControlHeight = childControlHeight,
				ScreenConfigName = "Fallback"
			};
			CalculateCellSize();
		}


		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();
			CalcAlongAxis(0, isVertical);
		}


		public override void CalculateLayoutInputVertical()
		{
			CalcAlongAxis(1, isVertical);
		}


		public override void SetLayoutHorizontal()
		{
			SetChildrenAlongAxis(0, isVertical);
		}


		public override void SetLayoutVertical()
		{
			SetChildrenAlongAxis(1, isVertical);
		}


		public void CalculateCellSize()
		{
			Rect rect = rectTransform.rect;
			if (rect.width == float.NaN || rect.height == float.NaN)
			{
				return;
			}

			ApplySettings(CurrentSettings);
			m_Spacing = SpacingSizer.CalculateSize(this);
			PaddingSizer.CalculateSize(this).CopyValuesTo(m_Padding);
		}


		private void ApplySettings(Settings settings)
		{
			if (settingsFallback == null)
			{
				return;
			}

			m_ChildAlignment = settings.ChildAlignment;
			orientation = settings.Orientation;
			m_ChildForceExpandWidth = settings.ChildForceExpandWidth;
			m_ChildForceExpandHeight = settings.ChildForceExpandHeight;
			m_ChildControlWidth = settings.ChildControlWidth;
			m_ChildControlHeight = settings.ChildControlHeight;
		}


		[Serializable]
		public class Settings : IScreenConfigConnection
		{
			public TextAnchor ChildAlignment;


			public bool ChildForceExpandHeight = true;


			public bool ChildForceExpandWidth = true;


			public bool ChildControlWidth = true;


			public bool ChildControlHeight = true;


			public Axis Orientation;


			public bool UseReversedSiblingOrder;


			[SerializeField] private string screenConfigName;


			public Settings(TextAnchor childAlignment, bool expandWidth, bool expandHeight, Axis orientation)
			{
				ChildAlignment = childAlignment;
				ChildForceExpandWidth = expandWidth;
				ChildForceExpandHeight = expandHeight;
				Orientation = orientation;
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