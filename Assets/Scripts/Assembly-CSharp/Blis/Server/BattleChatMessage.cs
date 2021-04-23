using System;
using Blis.Common;
using Newtonsoft.Json;

namespace Blis.Server
{
	
	public class BattleChatMessage
	{
		
		
		
		[JsonProperty]
		[JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime dtm { get; set; }

		
		
		
		[JsonProperty]
		public long source { get; set; }

		
		
		
		[JsonProperty]
		public string targetType { get; set; }

		
		
		
		[JsonProperty]
		public long target { get; set; }

		
		
		
		[JsonProperty]
		public string message { get; set; }
	}
}
