using System.Collections.Generic;

namespace Blis.Common
{
	public class NoiseHistory
	{
		private readonly Dictionary<int, List<RecentNoise>> noiseHistory = new Dictionary<int, List<RecentNoise>>();

		public float GetRecentNoiseHistory(int creatorId, NoiseType noiseType)
		{
			if (!noiseHistory.ContainsKey(creatorId))
			{
				return 0f;
			}

			return GetRecentNoise(noiseHistory[creatorId], noiseType);
		}


		public void UpdateNoiseHistory(int creatorId, NoiseType noiseType, float currentTime)
		{
			if (!noiseHistory.ContainsKey(creatorId))
			{
				noiseHistory[creatorId] = new List<RecentNoise>();
			}

			UpdateRecentNoise(noiseHistory[creatorId], noiseType, currentTime);
		}


		private float GetRecentNoise(List<RecentNoise> noises, NoiseType noiseType)
		{
			int num = 0;
			if (num >= noises.Count)
			{
				return 0f;
			}

			return noises[num].time;
		}


		private void UpdateRecentNoise(List<RecentNoise> noises, NoiseType noiseType, float currentTime)
		{
			for (int i = 0; i < noises.Count; i++)
			{
				if (noises[i].noiseType == noiseType)
				{
					noises[i].time = currentTime;
					return;
				}
			}

			noises.Add(new RecentNoise
			{
				noiseType = noiseType,
				time = currentTime
			});
		}


		private class RecentNoise
		{
			public NoiseType noiseType;


			public float time;
		}
	}
}