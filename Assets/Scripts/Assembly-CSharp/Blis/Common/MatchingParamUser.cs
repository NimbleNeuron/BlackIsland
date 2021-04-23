using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	[Obsolete]
	public class MatchingParamUser
	{
		[JsonProperty("cc")] public int characterCode;

		[JsonProperty("un")] public int userNum;
	}
}