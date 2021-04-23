using Blis.Common;

namespace Blis.Server
{
	
	public class UpdateMasteryInfo
	{
		
		public UpdateMasteryInfo()
		{
			this.conditionType = MasteryConditionType.None;
			this.takeMasteryValue = 0;
			this.masteryType = MasteryType.None;
			this.itemGrade = ItemGrade.None;
			this.extraValue = 0;
			this.assistMemberCount = 0;
		}

		
		public MasteryConditionType conditionType;

		
		public int takeMasteryValue;

		
		public MasteryType masteryType;

		
		public ItemGrade itemGrade;

		
		public int extraValue;

		
		public int assistMemberCount;
	}
}
