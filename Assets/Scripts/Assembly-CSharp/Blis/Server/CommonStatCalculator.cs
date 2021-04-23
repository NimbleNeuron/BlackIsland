using Blis.Common;

namespace Blis.Server
{
	
	public class CommonStatCalculator : StateEffectStatCalculator
	{
		
		public CommonStatCalculator(CharacterState state) : base(state)
		{
		}

		
		protected override float CalcStat(StatType statType, float value, StatType coefType, float coefValue)
		{
			if (statType == StatType.None)
			{
				return 0f;
			}
			float num = 0f;
			if (coefType != StatType.None)
			{
				num = base.Self.Stat.GetValue(coefType) * coefValue;
			}
			return value + num;
		}
	}
}
