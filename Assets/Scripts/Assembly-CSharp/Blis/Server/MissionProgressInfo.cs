using Blis.Common;

namespace Blis.Server
{
	
	public class MissionProgressInfo
	{
		
		public MissionProgressInfo(MissionConditionType conditionType, MissionCheck check, int conditionCode, int value)
		{
			this.conditionType = conditionType;
			this.check = check;
			this.conditionCode = conditionCode;
			this.value = value;
		}

		
		public MissionConditionType conditionType;

		
		public MissionCheck check;

		
		public int conditionCode;

		
		public int value;
	}
}
