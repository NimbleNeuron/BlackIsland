using Blis.Common;

namespace Blis.Server
{
	
	public struct MasteryInfo
	{
		
		public MasteryInfo(MasteryType masteryType, int masteryLevel, int masteryExp, int masteryMaxExp)
		{
			this.masteryType = masteryType;
			this.masteryLevel = masteryLevel;
			this.masteryExp = masteryExp;
			this.masteryMaxExp = masteryMaxExp;
			weaponSkillPoint = 0;
		}

		
		public MasteryType masteryType;

		
		public int masteryLevel;

		
		public int masteryExp;

		
		public int masteryMaxExp;

		
		public int weaponSkillPoint;
	}
}