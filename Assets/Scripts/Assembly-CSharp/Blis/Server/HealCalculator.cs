namespace Blis.Server
{
	
	public abstract class HealCalculator
	{
		
		public HealCalculator(int baseHp, float hpCoefficient, int addHp, int baseSp, float spCoefficient, int addSp)
		{
			this.baseHp = baseHp;
			this.addHp = addHp;
			this.hpCoefficient = hpCoefficient;
			this.baseSp = baseSp;
			this.addSp = addSp;
			this.spCoefficient = spCoefficient;
		}

		
		public abstract HealInfo Calculate(WorldCharacter self, WorldCharacter healer);

		
		protected int baseHp;

		
		protected int addHp;

		
		protected float hpCoefficient;

		
		protected int baseSp;

		
		protected int addSp;

		
		protected float spCoefficient;
	}
}
