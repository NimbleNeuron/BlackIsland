using UnityEngine;

namespace Blis.Server
{
	
	public class LostHpHealCalculator : HealCalculator
	{
		
		public LostHpHealCalculator(int baseHp, float hpCoefficient, int addHp, int baseSp, float spCoefficient,
			int addSp) : base(baseHp, hpCoefficient, addHp, baseSp, spCoefficient, addSp)
		{
			baseHp = 0;
		}

		
		public override HealInfo Calculate(WorldCharacter self, WorldCharacter healer)
		{
			int num = self.Stat.MaxHp - self.Status.Hp;
			if (num < 0)
			{
				num = 0;
			}

			int hp = Mathf.FloorToInt((num * hpCoefficient + addHp) *
			                          (1f + (healer != null ? new float?(healer.Stat.IncreaseModeHealRatio) : null) ??
			                           0f));
			int sp = Mathf.FloorToInt(baseSp * spCoefficient + addSp);
			return HealInfo.Create(hp, sp);
		}
	}
}