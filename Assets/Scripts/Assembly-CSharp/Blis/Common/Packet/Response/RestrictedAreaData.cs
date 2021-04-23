using Newtonsoft.Json;

namespace Blis.Common
{
	public class RestrictedAreaData
	{
		public readonly int clearCount;


		public readonly int code;


		public readonly int damageOnTime;


		public readonly int durationSeconds;


		public readonly int minimumSurvivors;


		public readonly int restrictedCount;

		[JsonConstructor]
		public RestrictedAreaData(int code, int durationSeconds, int restrictedCount, int clearCount, int damageOnTime,
			int minimumSurvivors)
		{
			this.code = code;
			this.durationSeconds = durationSeconds;
			this.restrictedCount = restrictedCount;
			this.clearCount = clearCount;
			this.damageOnTime = damageOnTime;
			this.minimumSurvivors = minimumSurvivors;
		}
	}
}