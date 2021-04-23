using Blis.Common;

namespace Blis.Client
{
	public class LocalMastery
	{
		private int exp;


		private int level;


		private float maxExp;


		private int weaponSkillPoint;


		public LocalMastery(MasteryType masteryType, int level, int exp, int weaponSkillPoint)
		{
			MasteryType = masteryType;
			SetExp(exp);
			SetLevel(level);
			SetWeaponSkillPoint(weaponSkillPoint);
		}


		public MasteryType MasteryType { get; }


		public int Exp => exp;


		public int Level => level;


		public float MaxExp => maxExp;


		public int WeaponSkillPoint => weaponSkillPoint;


		public void SetExp(int exp)
		{
			this.exp = exp;
		}


		public void SetLevel(int level)
		{
			MasteryLevelData masteryLevelData = GameDB.mastery.GetMasteryLevelData(MasteryType, level);
			maxExp = masteryLevelData != null ? masteryLevelData.nextMasteryExp : 0;
			this.level = level;
		}


		public void SetWeaponSkillPoint(int weaponSkillPoint)
		{
			this.weaponSkillPoint = weaponSkillPoint;
		}
	}
}