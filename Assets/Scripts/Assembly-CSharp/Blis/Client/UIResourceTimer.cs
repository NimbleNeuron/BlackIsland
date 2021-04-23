using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIResourceTimer : BaseTrackUI
	{
		[SerializeField] private Image fill = default;


		[SerializeField] private Text timer = default;


		private int max;


		private int value;

		public void SetValue(int value, int max)
		{
			this.max = max;
			SetValue(value);
		}


		public void SetValue(int value)
		{
			this.value = value;
			if (max < value)
			{
				max = value;
			}

			UpdateTimerUI();
		}


		private void UpdateTimerUI()
		{
			timer.text = string.Format("{0}s", value);
			if (max > 0)
			{
				fill.fillAmount = value / (float) max;
				return;
			}

			fill.fillAmount = 0f;
		}
	}
}