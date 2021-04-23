using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class MatchingTeam
	{
		[JsonProperty("tms")] public readonly Dictionary<long, MatchingTeamMember> teamMembers;
		[JsonProperty("tn")] public readonly int teamNo;
	}
}