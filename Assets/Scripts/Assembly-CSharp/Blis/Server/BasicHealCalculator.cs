using UnityEngine;

namespace Blis.Server
{
	
	public class BasicHealCalculator : HealCalculator
	{
		
		public BasicHealCalculator(int baseHp, float hpCoefficient, int addHp, int baseSp, float spCoefficient,
			int addSp) : base(baseHp, hpCoefficient, addHp, baseSp, spCoefficient, addSp) { }

		
		public override HealInfo Calculate(WorldCharacter self, WorldCharacter healer)
		{
			int hp = Mathf.FloorToInt((baseHp * hpCoefficient + addHp) *
			                          (1f + (healer != null ? new float?(healer.Stat.IncreaseModeHealRatio) : null) ??
			                           0f));
			int sp = Mathf.FloorToInt(baseSp * spCoefficient + addSp);
			return HealInfo.Create(hp, sp);
		}
	}
}