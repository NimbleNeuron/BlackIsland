using UnityEngine;
using Werewolf.StatusIndicators.Services;

namespace Werewolf.StatusIndicators.Components
{
	public class StatusIndicator : Splat
	{
		public int ProgressSteps;


		public override ScalingType Scaling => ScalingType.LengthAndHeight;


		public override void OnValueChanged()
		{
			ProjectorScaler.Resize(Projectors, Scaling, scale, width);
			if (ProgressSteps == 0)
			{
				UpdateProgress(progress);
				return;
			}

			UpdateProgress(StepProgress());
		}


		private float StepProgress()
		{
			float num = 1f / ProgressSteps;
			return Mathf.RoundToInt(progress / num) * num - num / 15f;
		}
	}
}