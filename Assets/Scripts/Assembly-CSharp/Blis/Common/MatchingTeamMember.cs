using Newtonsoft.Json;

namespace Blis.Common
{
	public class MatchingTeamMember
	{
		[JsonProperty("cc")] public readonly int characterCode;
		[JsonProperty("nn")] public readonly string nickname;
		[JsonProperty("p")] public readonly bool pick;
		[JsonProperty("un")] public readonly long userNum;
		[JsonProperty("w")] public readonly int weaponCode;
	}
}