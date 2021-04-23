using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Obsolete/Better Vertical Layout Group", 30)]
	public class BetterVerticalLayoutGroup : VerticalLayoutGroup, IBetterHorizontalOrVerticalLayoutGroup,
		IResolutionDependency
	{
		[FormerlySerializedAs("paddingSizer")] [SerializeField]
		private MarginSizeModifier paddingSizerFallback =
			new MarginSizeModifier(new Margin(), new Margin(), new Margin(1000, 1000, 1000, 1000));


		[FormerlySerializedAs("spacingSizer")] [SerializeField]
		private FloatSizeModifier spacingSizerFallback = new FloatSizeModifier(0f, 0f, 300f);


		protected override void OnEnable()
		{
			base.OnEnable();
			CalculateCellSize();
		}


		public MarginSizeModifier PaddingSizer => paddingSizerFallback;


		public FloatSizeModifier SpacingSizer => spacingSizerFallback;


		public void OnResolutionChanged()
		{
			CalculateCellSize();
		}


		public void CalculateCellSize()
		{
			Rect rect = rectTransform.rect;
			if (rect.width == float.NaN || rect.height == float.NaN)
			{
				return;
			}

			m_Spacing = SpacingSizer.CalculateSize(this);
			PaddingSizer.CalculateSize(this).CopyValuesTo(m_Padding);
		}
	}
}