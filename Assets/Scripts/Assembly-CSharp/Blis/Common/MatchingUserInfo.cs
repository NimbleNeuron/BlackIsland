using Newtonsoft.Json;

namespace Blis.Common
{
	public class MatchingUserInfo
	{
		[JsonProperty("cc")] public readonly int characterCode;


		[JsonProperty("nn")] public readonly string nickname;


		[JsonProperty("sa")] public readonly int selectArea;


		[JsonProperty("un")] public readonly long userNum;
	}
}