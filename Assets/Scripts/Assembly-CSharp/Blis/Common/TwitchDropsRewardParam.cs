using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class TwitchDropsRewardParam
	{
		
		[JsonProperty("p")]
		public string productId;

		
		[JsonProperty("r")]
		public string rewardId;
	}
}
