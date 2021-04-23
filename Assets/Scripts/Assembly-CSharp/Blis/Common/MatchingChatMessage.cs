using MessagePack;
using Newtonsoft.Json;

namespace Blis.Common
{
	
	[MessagePackObject(true)]
	public class MatchingChatMessage
	{
		
		[JsonProperty("s")]
		public string sender;

		
		[JsonProperty("m")]
		public string message;
	}
}
