using System;
using Newtonsoft.Json;

namespace Blis.Common
{
	
	public class MaintenanceScheduleResult
	{
		
		public bool maintenance;

		
		[JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime maintenanceStart;

		
		[JsonConverter(typeof(MicrosecondEpochConverter))]
		public DateTime maintenanceEnd;
	}
}
