using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class MatchingRegionSchedule
	{
		
		[JsonConstructor]
		public MatchingRegionSchedule(MatchingMode matchingMode, MatchingRegion matchingRegion, MatchingTeamMode matchingTeamMode, string message, bool mon, string monStartTime, string monEndTime, bool tue, string tueStartTime, string tueEndTime, bool wed, string wedStartTime, string wedEndTime, bool thu, string thuStartTime, string thuEndTime, bool fri, string friStartTime, string friEndTime, bool sat, string satStartTime, string satEndTime, bool sun, string sunStartTime, string sunEndTime)
		{
			this.matchingMode = matchingMode;
			this.matchingRegion = matchingRegion;
			this.matchingTeamMode = matchingTeamMode;
			this.message = message;
			this.mon = mon;
			TimeSpan.TryParse(monStartTime, out this.monStartTime);
			TimeSpan.TryParse(monEndTime, out this.monEndTime);
			this.tue = tue;
			TimeSpan.TryParse(tueStartTime, out this.tueStartTime);
			TimeSpan.TryParse(tueEndTime, out this.tueEndTime);
			this.wed = wed;
			TimeSpan.TryParse(wedStartTime, out this.wedStartTime);
			TimeSpan.TryParse(wedEndTime, out this.wedEndTime);
			this.thu = thu;
			TimeSpan.TryParse(thuStartTime, out this.thuStartTime);
			TimeSpan.TryParse(thuEndTime, out this.thuEndTime);
			this.fri = fri;
			TimeSpan.TryParse(friStartTime, out this.friStartTime);
			TimeSpan.TryParse(friEndTime, out this.friEndTime);
			this.sat = sat;
			TimeSpan.TryParse(satStartTime, out this.satStartTime);
			TimeSpan.TryParse(satEndTime, out this.satEndTime);
			this.sun = sun;
			TimeSpan.TryParse(sunStartTime, out this.sunStartTime);
			TimeSpan.TryParse(sunEndTime, out this.sunEndTime);
		}

		
		public readonly MatchingMode matchingMode;
		public readonly MatchingRegion matchingRegion;
		public readonly MatchingTeamMode matchingTeamMode;
		public readonly string message;
		public readonly bool mon;
		public readonly TimeSpan monStartTime;
		public readonly TimeSpan monEndTime;
		public readonly bool tue;
		public readonly TimeSpan tueStartTime;
		public readonly TimeSpan tueEndTime;
		public readonly bool wed;
		public readonly TimeSpan wedStartTime;
		public readonly TimeSpan wedEndTime;
		public readonly bool thu;
		public readonly TimeSpan thuStartTime;
		public readonly TimeSpan thuEndTime;
		public readonly bool fri;
		public readonly TimeSpan friStartTime;
		public readonly TimeSpan friEndTime;
		public readonly bool sat;
		public readonly TimeSpan satStartTime;
		public readonly TimeSpan satEndTime;
		public readonly bool sun;
		public readonly TimeSpan sunStartTime;
		public readonly TimeSpan sunEndTime;
	}
}
