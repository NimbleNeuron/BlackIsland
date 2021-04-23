using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIProgress : BaseControl
	{
		public Image foreground = default;


		[SerializeField] private Text label = default;


		private float value = default;

		public void SetValue(int value, int maxValue)
		{
			if (maxValue == 0)
			{
				SetValue(0f);
			}
			else
			{
				SetValue(value / (float) maxValue);
			}

			if (label != null)
			{
				label.text = string.Format("{0} / {1}", value, maxValue);
			}
		}


		public void SetLabel(string text)
		{
			if (label != null)
			{
				label.text = text;
			}
		}


		public void SetValue(float v)
		{
			value = Mathf.Clamp01(v);
			UpdateUI();
		}


		public float GetValue()
		{
			return value;
		}


		public void SetColor(Color color)
		{
			foreground.color = color;
		}


		protected virtual void UpdateUI()
		{
			foreground.fillAmount = value;
		}
	}
}