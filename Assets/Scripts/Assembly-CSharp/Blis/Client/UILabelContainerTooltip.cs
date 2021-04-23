using System;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UILabelContainerTooltip : UITooltipComponent
	{
		[SerializeField] private RectTransform labelContainer = default;


		[SerializeField] private Text label = default;

		public void SetLabel(string text, float width = 140f)
		{
			if (Math.Abs(width - labelContainer.sizeDelta.x) > Mathf.Epsilon)
			{
				labelContainer.sizeDelta = new Vector2(width, 0f);
				MonoBehaviourInstance<BlisLayoutBuilder>.inst.Rebuild(rectTransform);
			}

			label.text = text;
			transform.localScale = Vector3.one;
		}


		public override void Clear()
		{
			base.Clear();
			transform.localScale = Vector3.zero;
		}
	}
}