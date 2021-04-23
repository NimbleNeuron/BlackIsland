using Blis.Common;

namespace Blis.Server
{
	
	public class LostHpRateStatCalculator : StateEffectStatCalculator
	{
		
		public LostHpRateStatCalculator(CharacterState state) : base(state)
		{
		}

		
		protected override float CalcStat(StatType statType, float value, StatType coefType, float coefValue)
		{
			return this.GetCurrentLostHpRate() * value;
		}

		
		private float GetCurrentLostHpRate()
		{
			return 1f - (float)base.Self.Status.Hp / (float)base.Self.Stat.MaxHp;
		}
	}
}
