using MessagePack;
using Newtonsoft.Json;

namespace Blis.Common
{
	
	[MessagePackObject(true)]
	public class MatchingObserverToken
	{
		
		[JsonProperty("un")]
		public long userNum;

		
		[JsonProperty("nn")]
		public string nickname;
	}
}
