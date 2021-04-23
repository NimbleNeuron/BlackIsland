using System;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class PlayoutBufferCounter
	{
		public const int MAX_COUNT = 4;


		private const float logBase = 2.5f;


		private readonly GameEventLogger gameEventLogger;


		private int DecreaseThreshold;


		private int enoughCount;


		private bool isStarted;


		private int notEnoughCount;


		private float stopTime;

		public PlayoutBufferCounter(GameEventLogger gameEventLogger)
		{
			this.gameEventLogger = gameEventLogger;
		}


		public bool IsStarted()
		{
			return isStarted;
		}


		public void Start(float frameUpdateRate)
		{
			isStarted = true;
			DecreaseThreshold = Mathf.FloorToInt(1f / frameUpdateRate);
			stopTime = 0f;
		}


		public void Stop()
		{
			isStarted = false;
			stopTime = Time.realtimeSinceStartup;
		}


		public float TimeSinceStop()
		{
			if (isStarted)
			{
				return 0f;
			}

			return Time.realtimeSinceStartup - stopTime;
		}


		private void NotEnough()
		{
			enoughCount = Math.Max(0, enoughCount - 1);
		}


		private void NoCommand()
		{
			notEnoughCount = Math.Min(60, notEnoughCount + 1);
			enoughCount = 0;
		}


		private void Enough()
		{
			enoughCount++;
			if (enoughCount > DecreaseThreshold)
			{
				enoughCount = 0;
				notEnoughCount = Math.Max(notEnoughCount - 1, 0);
			}
		}


		public int Count(int queueCount)
		{
			if (!isStarted)
			{
				return CalcCount();
			}

			int b = CalcCount();
			if (queueCount >= Mathf.Max(2, b))
			{
				Enough();
			}
			else if (queueCount == 0)
			{
				Singleton<GameEventLogger>.inst.IncNoCommandCount();
				NoCommand();
			}
			else if (queueCount < Mathf.Max(2, b))
			{
				NotEnough();
			}

			return CalcCount();
		}


		private int CalcCount()
		{
			int num = Mathf.Clamp(Mathf.FloorToInt(Mathf.Log(notEnoughCount, 2.5f)), 0, 4);
			gameEventLogger.SetBufferCount(num);
			return num;
		}
	}
}