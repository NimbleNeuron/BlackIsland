using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Controls/Better Text", 30)]
	public class BetterText : Text, IResolutionDependency
	{
		public enum FittingMode
		{
			SizerOnly,


			StayInBounds,


			BestFit
		}


		[SerializeField] private FittingMode fitting = FittingMode.StayInBounds;


		[FormerlySerializedAs("fontSizer")] [SerializeField]
		private FloatSizeModifier fontSizerFallback = new FloatSizeModifier(40f, 0f, 500f);


		[SerializeField] private FloatSizeConfigCollection customFontSizers = new FloatSizeConfigCollection();


		private bool isCalculatingSize;


		public FloatSizeModifier FontSizer => customFontSizers.GetCurrentItem(fontSizerFallback);


		
		public FittingMode Fitting {
			get => fitting;
			set
			{
				fitting = value;
				CalculateSize();
			}
		}


		protected override void OnEnable()
		{
			base.OnEnable();
			CalculateSize();
		}


		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();
			CalculateSize();
		}


		public void OnResolutionChanged()
		{
			CalculateSize();
		}


		public override void SetVerticesDirty()
		{
			base.SetVerticesDirty();
			CalculateSize();
		}


		private void CalculateSize()
		{
			if (isCalculatingSize)
			{
				return;
			}

			isCalculatingSize = true;
			switch (fitting)
			{
				case FittingMode.SizerOnly:
					resizeTextForBestFit = false;
					fontSize = Mathf.RoundToInt(FontSizer.CalculateSize(this));
					break;
				case FittingMode.StayInBounds:
				{
					resizeTextMinSize = Mathf.RoundToInt(FontSizer.MinSize);
					resizeTextMaxSize = Mathf.RoundToInt(FontSizer.MaxSize);
					resizeTextForBestFit = true;
					int num = Mathf.RoundToInt(FontSizer.CalculateSize(this));
					fontSize = num;
					base.Rebuild(CanvasUpdate.PreRender);
					int fontSizeUsedForBestFit = cachedTextGenerator.fontSizeUsedForBestFit;
					resizeTextForBestFit = false;
					fontSize = fontSizeUsedForBestFit < num ? fontSizeUsedForBestFit : num;
					FontSizer.OverrideLastCalculatedSize(fontSize);
					break;
				}
				case FittingMode.BestFit:
					resizeTextMinSize = Mathf.RoundToInt(FontSizer.MinSize);
					resizeTextMaxSize = Mathf.RoundToInt(FontSizer.MaxSize);
					resizeTextForBestFit = true;
					base.Rebuild(CanvasUpdate.PreRender);
					FontSizer.OverrideLastCalculatedSize(cachedTextGenerator.fontSizeUsedForBestFit);
					break;
			}

			isCalculatingSize = false;
		}
	}
}