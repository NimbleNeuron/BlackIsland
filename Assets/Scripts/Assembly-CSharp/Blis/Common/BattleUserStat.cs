using System.Collections.Generic;

namespace Blis.Common
{
	public class BattleUserStat
	{
		public float averageAssistants;
		public float averageHunts;
		public float averageKills;
		public float averageRank;
		public List<BattleCharacterStat> characterStats;
		public int matchingMode;
		public int matchingTeamMode;
		public int mmr;
		public int seasonId;
		public float top1;
		public float top2;
		public float top3;
		public float top5;
		public float top7;
		public long totalGames;
		public long totalTop1s;
		public long totalTop2s;
		public long totalTop3s;
		public long totalTop5s;
		public long totalTop7s;
		public long totalWins;
		public int userNum;
	}
}