using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;

namespace Blis.Common
{
	[MessagePackObject(true)]
	public class MatchingToken
	{
		[JsonProperty("cc")] public int characterCode;
		[JsonProperty("e")] public Dictionary<EmotionPlateType, int> emotion;
		[JsonProperty("m")] public MatchingMode matchingMode;
		[JsonProperty("mt")] public MatchingTeamMode matchingTeamMode;
		[JsonProperty("ml")] public List<int> missionList;
		[JsonProperty("mmr")] public int mmr;
		[JsonProperty("nn")] public string nickname;
		[JsonProperty("ovs")] public bool observer;
		[JsonProperty("mr")] public MatchingRegion region;
		[JsonProperty("sc")] public int skinCode;
		[JsonProperty("tk")] public string teamKey;
		[JsonProperty("tmc")] public int teamMemberCount;
		[JsonProperty("un")] public long userNum;
		[JsonProperty("wc")] public int weaponCode;

		public MatchingToken() { }

		public MatchingToken(MatchingMode matchingMode, MatchingTeamMode matchingTeamMode, MatchingRegion region,
			string teamKey, long userNum, int mmr, string nickname, int characterCode, int skinCode, int weaponCode,
			List<int> missionList, int teamMemberCount, bool observer, Dictionary<EmotionPlateType, int> emotion)
		{
			this.matchingMode = matchingMode;
			this.matchingTeamMode = matchingTeamMode;
			this.region = region;
			this.teamKey = teamKey;
			this.userNum = userNum;
			this.nickname = nickname;
			this.mmr = mmr;
			this.characterCode = characterCode;
			this.skinCode = skinCode;
			this.weaponCode = weaponCode;
			this.missionList = missionList;
			this.teamMemberCount = teamMemberCount;
			this.observer = observer;
			this.emotion = emotion;
		}
	}
}