using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class ServerRegionData
	{
		public readonly int code;
		public readonly string name;
		public readonly string pingIP;

		[JsonConverter(typeof(StringEnumConverter))]
		public readonly MatchingRegion region;

		[JsonConstructor]
		public ServerRegionData(int code, MatchingRegion region, string name, string pingIP)
		{
			this.code = code;
			this.region = region;
			this.name = name;
			this.pingIP = pingIP;
		}
	}
}