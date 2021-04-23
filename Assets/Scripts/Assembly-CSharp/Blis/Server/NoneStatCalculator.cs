using Blis.Common;

namespace Blis.Server
{
	
	public class NoneStatCalculator : StateEffectStatCalculator
	{
		
		public NoneStatCalculator(CharacterState state) : base(state)
		{
		}

		
		protected override float CalcStat(StatType statType, float value, StatType coefType, float coefValue)
		{
			return 0f;
		}
	}
}
