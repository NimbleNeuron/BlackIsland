using System;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UIExp : BaseUI
	{
		private Text label;


		private Action onFinish;


		private Outline outline;


		private PositionTweener positionTweener;

		protected override void Awake()
		{
			base.Awake();
			label = GameUtil.Bind<Text>(gameObject, "Text");
			outline = GameUtil.Bind<Outline>(gameObject, "Text");
			positionTweener = GameUtil.Bind<PositionTweener>(gameObject, "Text");
			positionTweener.OnAnimationFinish += FinishEvent;
		}


		private void FinishEvent()
		{
			Action action = onFinish;
			if (action == null)
			{
				return;
			}

			action();
		}


		public void SetLabel(string text, int fontSize, Color color, Color outlineColor, Action onFinish)
		{
			label.text = text;
			label.fontSize = fontSize;
			label.color = color;
			outline.effectColor = outlineColor;
			this.onFinish = onFinish;
			positionTweener.PlayAnimation();
		}
	}
}