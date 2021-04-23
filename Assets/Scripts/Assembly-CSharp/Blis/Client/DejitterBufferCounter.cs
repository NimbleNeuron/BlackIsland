using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class DejitterBufferCounter
	{
		private readonly float frameUpdateRate;
		private float accumulatedBufferSize;


		private int index;


		private int jitterBufferSize = 1;


		private float smoothDeviation;

		public DejitterBufferCounter(float frameUpdateRate)
		{
			this.frameUpdateRate = frameUpdateRate;
		}


		public int CalcJitterBufferSize(float packetDelta)
		{
			float num = Mathf.Max(packetDelta - frameUpdateRate, 0f);
			float num2 = num > smoothDeviation ? 0.2f : 0.05f;
			float num3 = 1f - num2;
			smoothDeviation = index == 0 ? num : num3 * smoothDeviation + num2 * num;
			float num4 = smoothDeviation / frameUpdateRate;
			accumulatedBufferSize = 0.99f * accumulatedBufferSize + 0.01f * num4;
			int num5 = Mathf.Min(Mathf.CeilToInt(accumulatedBufferSize), 4);
			int num6 = jitterBufferSize;
			jitterBufferSize = num5;
			index++;
			Singleton<GameEventLogger>.inst.SetBufferCount(jitterBufferSize);
			return jitterBufferSize;
		}
	}
}