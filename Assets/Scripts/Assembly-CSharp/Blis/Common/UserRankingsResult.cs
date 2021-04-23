using System.Collections.Generic;

namespace Blis.Common
{
	
	public class UserRankingsResult
	{
		
		public BattleUser battleUser;

		
		public Dictionary<RankingTierGrade, List<RankingUser>> tierRankingUsers;
	}
}