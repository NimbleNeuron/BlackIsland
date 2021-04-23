using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;

namespace Blis.Common
{
	
	[MessagePackObject(true)]
	public class MatchingTeamMemberToken
	{
		
		public MatchingTeamMemberToken()
		{
			this.missionList = new List<int>();
			this.emotion = new Dictionary<EmotionPlateType, int>();
		}

		
		[JsonProperty("un")]
		public long userNum;

		
		[JsonProperty("nn")]
		public string nickname;

		
		[JsonProperty("mmr")]
		public int mmr;

		
		[JsonProperty("cc")]
		public int characterCode;

		
		[JsonProperty("w")]
		public int weaponCode;

		
		[JsonProperty("sc")]
		public int skinCode;

		
		[JsonProperty("ml")]
		public List<int> missionList;

		
		[JsonProperty("mc")]
		public int matchCount;

		
		[JsonProperty("pm")]
		public int preMade;

		
		[JsonProperty("hne")]
		public bool hideNameFromEnemy;

		
		[JsonProperty("e")]
		public Dictionary<EmotionPlateType, int> emotion;

		
		[JsonProperty("ip")]
		public string ip;

		
		[JsonProperty("z")]
		public string zipCode;

		
		[JsonProperty("c")]
		public string country;

		
		[JsonProperty("ccd")]
		public string countryCode;

		
		[JsonProperty("is")]
		public string isp;

		
		[JsonIgnore]
		public int privateMmr;
	}
}
