using System.Collections.Generic;
using Blis.Common;
using MessagePack;
using Newtonsoft.Json;

namespace Blis.Server
{
	
	[MessagePackObject(true)]
	public class BattleToken
	{
		
		public BattleToken()
		{
			this.matchingTeams = new Dictionary<int, MatchingTeamToken>();
			this.observers = new List<MatchingObserverToken>();
		}

		
		public MatchingTeamMemberToken GetMatchingTokenUser(int teamNo, long userNum)
		{
			return this.matchingTeams[teamNo].teamMembers[userNum];
		}

		
		public bool IsObserver(long userNum, out MatchingObserverToken observerToken)
		{
			observerToken = this.observers.Find((MatchingObserverToken x) => x.userNum == userNum);
			return observerToken != null;
		}

		
		[JsonProperty("gi")]
		public long gameId;

		
		[JsonProperty("si")]
		public int seasonId;

		
		[JsonProperty("mm")]
		public MatchingMode matchingMode;

		
		[JsonProperty("mt")]
		public MatchingTeamMode matchingTeamMode;

		
		[JsonProperty("bc")]
		public int botCount;

		
		[JsonProperty("ioa")]
		public bool isOnAcceleration;

		
		[JsonProperty("bd")]
		public BotDifficulty botDifficulty;

		
		[JsonProperty("mts")]
		public Dictionary<int, MatchingTeamToken> matchingTeams;

		
		[JsonProperty("obs")]
		public List<MatchingObserverToken> observers;

		
		[JsonProperty("h")]
		public string host;

		
		[JsonProperty("vh")]
		public string vipHost;
	}
}
