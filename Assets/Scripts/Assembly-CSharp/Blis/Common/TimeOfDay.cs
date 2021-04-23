using Newtonsoft.Json;

namespace Blis.Common
{
	public class TimeOfDay
	{
		public readonly int code;


		public readonly DayNight dayNight;


		public readonly float sightRangeRatio;

		[JsonConstructor]
		public TimeOfDay(int code, DayNight dayNight, float sightRangeRatio)
		{
			this.code = code;
			this.dayNight = dayNight;
			this.sightRangeRatio = sightRangeRatio;
		}
	}
}