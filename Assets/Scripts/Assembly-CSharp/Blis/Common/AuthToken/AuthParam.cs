using System.Collections.Generic;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class AuthParam
	{
		[JsonProperty("alc")] public string appLanguageCode;
		[JsonProperty("ver")] public string appVersion;
		[JsonProperty("ap")] public string authProvider;
		[JsonProperty("dlc")] public string deviceLanguageCode;
		[JsonProperty("glc")] public string geoLocationCode;
		[JsonProperty("idt")] public string idToken;
		[JsonProperty("mn")] public string machineNum;
		[JsonProperty("prm")] public Dictionary<string, string> paramMap;
	}
}