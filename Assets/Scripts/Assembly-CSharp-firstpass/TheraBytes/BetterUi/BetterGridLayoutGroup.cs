using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Layout/Better Grid Layout Group", 30)]
	public class BetterGridLayoutGroup : GridLayoutGroup, IResolutionDependency
	{
		[FormerlySerializedAs("paddingSizer")] [SerializeField]
		private MarginSizeModifier paddingSizerFallback =
			new MarginSizeModifier(new Margin(), new Margin(), new Margin(1000, 1000, 1000, 1000));


		[SerializeField] private MarginSizeConfigCollection customPaddingSizers = new MarginSizeConfigCollection();


		[FormerlySerializedAs("cellSizer")] [SerializeField]
		private Vector2SizeModifier cellSizerFallback =
			new Vector2SizeModifier(new Vector2(100f, 100f), new Vector2(10f, 10f), new Vector2(300f, 300f));


		[SerializeField] private Vector2SizeConfigCollection customCellSizers = new Vector2SizeConfigCollection();


		[FormerlySerializedAs("spacingSizer")] [SerializeField]
		private Vector2SizeModifier spacingSizerFallback =
			new Vector2SizeModifier(Vector2.zero, Vector2.zero, new Vector2(300f, 300f));


		[SerializeField] private Vector2SizeConfigCollection customSpacingSizers = new Vector2SizeConfigCollection();


		[SerializeField] private Settings settingsFallback;


		[SerializeField] private SettingsConfigCollection customSettings = new SettingsConfigCollection();


		[SerializeField] private bool fit;


		public MarginSizeModifier PaddingSizer => customPaddingSizers.GetCurrentItem(paddingSizerFallback);


		public Vector2SizeModifier CellSizer => customCellSizers.GetCurrentItem(cellSizerFallback);


		public Vector2SizeModifier SpacingSizer => customSpacingSizers.GetCurrentItem(spacingSizerFallback);


		public Settings CurrentSettings => customSettings.GetCurrentItem(settingsFallback);


		
		public bool Fit {
			get => fit;
			set
			{
				if (fit == value)
				{
					return;
				}

				fit = value;
				CalculateCellSize();
			}
		}


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
			CalculateCellSize();
		}


		public void OnResolutionChanged()
		{
			CalculateCellSize();
			if (fit)
			{
				SetDirty();
				CalculateCellSize();
			}
		}


		private IEnumerator InitDelayed()
		{
			yield return null;
			settingsFallback = new Settings(this)
			{
				ScreenConfigName = "Fallback"
			};
			CalculateCellSize();
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
			CellSizer.CalculateSize(this);
			if (fit)
			{
				Vector2 lastCalculatedSize = CellSizer.LastCalculatedSize;
				Constraint constraint = this.constraint;
				if (constraint != Constraint.FixedColumnCount)
				{
					if (constraint == Constraint.FixedRowCount)
					{
						lastCalculatedSize.y = GetCellHeight();
					}
				}
				else
				{
					lastCalculatedSize.x = GetCellWidth();
				}

				CellSizer.OverrideLastCalculatedSize(lastCalculatedSize);
			}

			m_CellSize = CellSizer.LastCalculatedSize;
		}


		public float GetCellWidth()
		{
			return (rectTransform.rect.width - padding.horizontal - constraintCount * spacing.x) / constraintCount;
		}


		public float GetCellHeight()
		{
			return (rectTransform.rect.height - padding.vertical - constraintCount * spacing.y) / constraintCount;
		}


		private void ApplySettings(Settings settings)
		{
			if (settingsFallback == null)
			{
				return;
			}

			m_Constraint = settings.Constraint;
			m_ConstraintCount = settings.ConstraintCount;
			m_ChildAlignment = settings.ChildAlignment;
			m_StartAxis = settings.StartAxis;
			m_StartCorner = settings.StartCorner;
			fit = settings.Fit;
		}


		[Serializable]
		public class Settings : IScreenConfigConnection
		{
			public Constraint Constraint;


			public int ConstraintCount;


			public TextAnchor ChildAlignment;


			public Axis StartAxis;


			public Corner StartCorner;


			public bool Fit;


			[SerializeField] private string screenConfigName;


			public Settings(BetterGridLayoutGroup grid)
			{
				Constraint = grid.m_Constraint;
				ConstraintCount = grid.m_ConstraintCount;
				ChildAlignment = grid.childAlignment;
				StartAxis = grid.m_StartAxis;
				StartCorner = grid.m_StartCorner;
				Fit = grid.fit;
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