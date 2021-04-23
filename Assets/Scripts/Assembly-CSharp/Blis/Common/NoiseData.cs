using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	public class NoiseData
	{
		public readonly int code;


		[JsonConverter(typeof(StringEnumConverter))]
		public readonly NoiseType noiseType;


		public readonly float pingChanceRate;


		public readonly float pingRange;

		[JsonConstructor]
		public NoiseData(int code, NoiseType noiseType, float pingRange, float pingChanceRate, float sfxRange)
		{
			this.code = code;
			this.noiseType = noiseType;
			this.pingRange = pingRange;
			this.pingChanceRate = pingChanceRate;
		}
	}
}