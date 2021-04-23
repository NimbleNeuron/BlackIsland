using Newtonsoft.Json;

namespace Blis.Common
{
	public class PurchasedDLC
	{
		[JsonProperty("c")] public readonly string contents;


		[JsonProperty("p")] public readonly string productId;


		[JsonProperty("r")] public readonly string rewardId;


		[JsonProperty("t")] public readonly string title;
	}
}