using System.Collections.Generic;
using MessagePack;
using Newtonsoft.Json;

namespace Blis.Common
{
	[MessagePackObject(true)]
	public class MatchingTeamToken
	{
		[JsonProperty("b")] public bool isBotTeam;


		[JsonProperty("tms")] public Dictionary<long, MatchingTeamMemberToken> teamMembers;


		[JsonProperty("tm")] public int teamMMR;


		[JsonProperty("tn")] public int teamNo;

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}