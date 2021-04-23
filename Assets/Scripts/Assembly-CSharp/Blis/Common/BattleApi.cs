using System;
using Neptune.Http;

namespace Blis.Common
{
	public class BattleApi
	{
		public static Func<HttpRequest> GetBattleResult(string battleResultTokenKey)
		{
			if (string.IsNullOrEmpty(battleResultTokenKey))
			{
				throw new GameException(ErrorType.InvalidKey);
			}

			return HttpRequestFactory.Post(
				ApiConstants.Url("/battle/result/" + battleResultTokenKey, Array.Empty<object>()), null);
		}


		public class BattleResult
		{
			public RankingTierChangeType afterTierChangeType;


			public RankingTierGrade afterTierGrade;


			public RankingTierType afterTierType;


			public bool batchMode;


			public RankingTierChangeType beforeTierChangeType;


			public RankingTierGrade beforeTierGrade;


			public RankingTierType beforeTierType;


			public bool benefitByKakaoPcCafe;


			public int gainExp;


			public bool lastBatchMode;

			public int mmr;


			public int mmrGain;


			public int rewardCoin;


			public bool upgradeLevel;


			public int userLevel;


			public int userNeedExp;
		}
	}
}