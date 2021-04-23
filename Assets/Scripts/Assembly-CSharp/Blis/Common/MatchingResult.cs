using MessagePack;
using Newtonsoft.Json;

namespace Blis.Common
{
	[MessagePackObject(true)]
	public class MatchingResult
	{
		[JsonProperty("bh")] public string battleHost;
		[JsonProperty("bk")] public string battleTokenKey;
		[JsonProperty("mm")] public MatchingMode matchingMode;
		[JsonProperty("mt")] public MatchingTeamMode matchingTeamMode;
		[JsonProperty("vip")] public string vip;
	}
}