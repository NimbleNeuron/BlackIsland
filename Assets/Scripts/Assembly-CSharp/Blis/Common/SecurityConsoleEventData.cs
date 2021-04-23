using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class SecurityConsoleEventData
	{
		
		[JsonConstructor]
		public SecurityConsoleEventData(int code, SecurityConsoleEvent securityConsoleEvent, string objectAnimation, float successRate, float detectionRate)
		{
			this.code = code;
			this.securityConsoleEvent = securityConsoleEvent;
			this.objectAnimation = objectAnimation;
			this.successRate = successRate;
			this.detectionRate = detectionRate;
		}

		
		public readonly int code;

		
		[JsonConverter(typeof(StringEnumConverter))]
		public readonly SecurityConsoleEvent securityConsoleEvent;

		
		public readonly string objectAnimation;

		
		public readonly float successRate;

		
		public readonly float detectionRate;
	}
}
