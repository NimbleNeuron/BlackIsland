using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public class ApplyStatOverTimeStatCalculator : StateEffectStatCalculator
	{
		
		public ApplyStatOverTimeStatCalculator(CharacterState state) : base(state) { }

		
		protected override float CalcStat(StatType statType, float value, StatType coefType, float coefValue)
		{
			float num = 0f;
			if (0f < CalculatorParameter)
			{
				num = 1f - (CreatedTime + CalculatorParameter -
				            MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime) / CalculatorParameter;
				if (1f < num)
				{
					num = 1f;
				}
			}

			return num * value;
		}
	}
}