using UnityEngine;

namespace Blis.Client
{
	public class GameTime : Singleton<GameTime>
	{
		private bool isInitialized;


		public float lastUpdateServerTime;


		private float lastUpdateTime;


		private int maxLatency = int.MinValue;


		private int minLatency = int.MaxValue;


		private int recvCount;


		private float rtt;


		private int sumLatency;


		public float Latency => rtt * 1000f;


		public float Rtt => rtt;


		public int MinLatency => minLatency;


		public int MaxLatency => maxLatency;


		public int AvgLatency {
			get
			{
				if (recvCount <= 0)
				{
					return 0;
				}

				return sumLatency / recvCount;
			}
		}


		public void UpdateRTT(int netRtt, float serverTime)
		{
			rtt = netRtt / 1000f;
			lastUpdateTime = Time.realtimeSinceStartup;
			lastUpdateServerTime = serverTime;
			UpdateLatency();
		}


		public float EstimateServerTime()
		{
			float num = Time.realtimeSinceStartup - lastUpdateTime;
			return lastUpdateServerTime + num + rtt / 2f;
		}


		public void InitLatency()
		{
			minLatency = int.MaxValue;
			maxLatency = int.MinValue;
			recvCount = 0;
			sumLatency = 0;
		}


		private void UpdateLatency()
		{
			int num = Mathf.FloorToInt(Latency);
			if (num > 0)
			{
				if (minLatency > num)
				{
					minLatency = num;
				}

				if (maxLatency < num)
				{
					maxLatency = num;
				}
			}

			recvCount++;
			sumLatency += num;
		}
	}
}