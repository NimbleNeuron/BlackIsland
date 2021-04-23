namespace Werewolf.StatusIndicators.Components
{
	public class RangeIndicator : Splat
	{
		public float DefaultScale;


		public override ScalingType Scaling => ScalingType.LengthAndHeight;


		public override void OnShow()
		{
			UpdateRangeIndicatorSize();
		}


		private void UpdateRangeIndicatorSize()
		{
			Scale = DefaultScale;
		}
	}
}