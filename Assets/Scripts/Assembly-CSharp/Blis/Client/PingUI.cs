using Blis.Common;
using UnityEngine.UI;

namespace Blis.Client
{
	public class PingUI : BaseUI
	{
		private Text ping;

		protected override void Awake()
		{
			base.Awake();
			GameUtil.Bind<Text>(gameObject, ref ping);
			ping.text = "";
		}


		public void SetPing(string host, int latency, int commandLatency)
		{
			ping.text = string.Format("<color=#bbb>{0}</color> {1}ms", host, latency);
		}


		public void SetPing(string host, int latency, int commandLatency, int fps)
		{
			ping.text = string.Format("<color=#bbb>{0}</color> {1}ms {2}fps", host, latency, fps);
		}
	}
}