using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class UISurvivableTimer : BaseTrackUI
	{
		private Text timer;

		protected override void Awake()
		{
			timer = GameUtil.Bind<Text>(gameObject, "Time");
		}


		public void UpdateTimer(float remainTime)
		{
			timer.text = GameUtil.IntToString(Mathf.FloorToInt(remainTime), GameUtil.NumberOfDigits.Two);
		}
	}
}