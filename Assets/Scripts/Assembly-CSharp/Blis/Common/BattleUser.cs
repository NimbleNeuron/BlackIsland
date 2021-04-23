using System;

namespace Blis.Common
{
	public class BattleUser
	{
		public int batchCount;
		public bool batchMode;
		public BattleUserStat battleUserStat;
		public MatchingMode matchingMode;
		public MatchingTeamMode matchingTeamMode;
		public int mmr;
		public long rank;
		public int rankRewardId;
		public DateTime rewardDtm;
		public int seasonId;
		public RankingTierChangeType tierChangeType;
		public RankingTierGrade tierGrade;
		public RankingTierType tierType;
		public int userNum;

		public BattleUser(int seasonId, int userNum, MatchingMode matchingMode, MatchingTeamMode matchingTeamMode,
			RankingTierType tierType, RankingTierGrade tierGrade, bool batchMode, int batchCount, int mmr,
			RankingTierChangeType tierChangeType, long rank, long rewardDtm, int rankRewardId,
			BattleUserStat battleUserStat)
		{
			this.seasonId = seasonId;
			this.userNum = userNum;
			this.matchingMode = matchingMode;
			this.matchingTeamMode = matchingTeamMode;
			this.tierType = tierType;
			this.tierGrade = tierGrade;
			this.batchMode = batchMode;
			this.batchCount = batchCount;
			this.mmr = mmr;
			this.tierChangeType = tierChangeType;
			this.rank = rank;
			this.rewardDtm = GameUtil.ConvertFromUnixTimestamp(rewardDtm / 1000L);
			this.rankRewardId = rankRewardId;
			this.battleUserStat = battleUserStat;
		}
	}
}