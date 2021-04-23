using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class BeforeMatchingParam
	{
		[JsonProperty("mm")] public MatchingMode matchingMode;
		[JsonProperty("mt")] public MatchingTeamMode MatchingTeamMode;
		[JsonProperty("tms")] public List<long> teamMembers;
		
		public BeforeMatchingParam(MatchingMode matchingMode, MatchingTeamMode teamMode, List<long> teamMembers)
		{
			this.matchingMode = matchingMode;
			MatchingTeamMode = teamMode;
			this.teamMembers = teamMembers;
		}
	}
}