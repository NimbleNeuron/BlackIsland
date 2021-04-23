using System.Collections.Generic;
using Blis.Common;
using Newtonsoft.Json;

namespace Blis.Server
{
	
	public class BattleResultToken
	{
		
		[JsonProperty("btk")] public string battleTokenKey;

		
		[JsonProperty("clv")] public int characterLevel;

		
		[JsonProperty("cn")] public int characterNum;

		
		[JsonProperty("dtm")] public int damageToMonster;

		
		[JsonProperty("dtp")] public int damageToPlayer;

		
		[JsonProperty("dr")] public float duelRating;

		
		[JsonProperty("kun")] public long killerUserNum;

		
		[JsonProperty("mm")] public MatchingMode matchingMode;

		
		[JsonProperty("mt")] public MatchingTeamMode matchingTeamMode;

		
		[JsonProperty("mr")] public float matchRating;

		
		[JsonProperty("mir")] public Dictionary<int, int> missionResult;

		
		[JsonProperty("mk")] public int monsterKill;

		
		[JsonProperty("pk")] public int playerKill;

		
		[JsonProperty("rk")] public int rank;

		
		[JsonProperty("rc")] public int rewardCoin;

		
		[JsonProperty("un")] public long userNum;
	}
}