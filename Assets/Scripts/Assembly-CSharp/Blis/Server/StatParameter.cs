using Blis.Common;

namespace Blis.Server
{
	
	public class StatParameter
	{
		
		public StatType coefStatType;

		
		public float coefStatValue;

		
		public StatType statType;

		
		public float statValue;

		
		public StatParameter(StatType statType, float statValue, StatType coefStatType, float coefStatValue)
		{
			this.statType = statType;
			this.statValue = statValue;
			this.coefStatType = coefStatType;
			this.coefStatValue = coefStatValue;
		}
	}
}