using Newtonsoft.Json;

namespace Blis.Common
{
	public class MatchingParam
	{
		[JsonProperty("hne")] public bool hideNameFromEnemy;
		[JsonProperty("mm")] public MatchingMode matchingMode;
		[JsonProperty("mr")] public MatchingRegion matchingRegion;
		[JsonProperty("mt")] public MatchingTeamMode matchingTeamMode;
		[JsonProperty("si")] public string steamID;
		[JsonProperty("tk")] public string teamKey;
		[JsonProperty("tmc")] public int teamMemberCount;

		public MatchingParam(MatchingRegion matchingRegion, MatchingMode matchingMode,
			MatchingTeamMode matchingTeamMode, string teamKey, int teamMemberCount, string steamID,
			bool hideNameFromEnemy)
		{
			this.matchingRegion = matchingRegion;
			this.matchingMode = matchingMode;
			this.matchingTeamMode = matchingTeamMode;
			this.teamKey = teamKey;
			this.teamMemberCount = teamMemberCount;
			this.steamID = steamID;
			this.hideNameFromEnemy = hideNameFromEnemy;
		}
	}
}