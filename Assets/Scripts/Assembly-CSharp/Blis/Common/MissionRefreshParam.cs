using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blis.Common
{
	
	public class MissionRefreshParam
	{
		
		public MissionRefreshParam(MissionType missionType, int missionCode)
		{
			this.missionType = missionType;
			this.missionCode = missionCode;
		}

		
		[JsonConverter(typeof(StringEnumConverter))]
		public MissionType missionType;

		
		public int missionCode;
	}
}
