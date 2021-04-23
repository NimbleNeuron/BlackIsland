using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	public class MaintenanceResult
	{
		[JsonProperty("ed")] [JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime maintenanceEnd;

		[JsonProperty("sd")] [JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime maintenanceStart;
	}
}