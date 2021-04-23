using Newtonsoft.Json;

namespace Blis.Common
{
	public class IngameServerInfo
	{
		[JsonProperty("a")] public readonly string address;


		[JsonProperty("t")] public readonly string battleSeatKey;
	}
}