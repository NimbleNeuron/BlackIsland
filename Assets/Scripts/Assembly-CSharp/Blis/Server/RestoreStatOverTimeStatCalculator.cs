using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public class RestoreStatOverTimeStatCalculator : StateEffectStatCalculator
	{
		
		public RestoreStatOverTimeStatCalculator(CharacterState state) : base(state) { }

		
		protected override float CalcStat(StatType statType, float value, StatType coefType, float coefValue)
		{
			float num = 1f;
			if (0f < CalculatorParameter)
			{
				num = (CreatedTime + CalculatorParameter -
				       MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime) / CalculatorParameter;
				if (num < 0f)
				{
					num = 0f;
				}
			}

			return num * value;
		}
	}
}