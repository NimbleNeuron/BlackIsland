using System.Collections.Generic;

namespace Blis.Common
{
	public class BowSkillActiveData : Singleton<BowSkillActiveData>
	{
		public readonly Dictionary<int, int> DamageByLevel_IN = new Dictionary<int, int>();


		public readonly Dictionary<int, int> DamageByLevel_OUT = new Dictionary<int, int>();


		public readonly float DamageInRangeRadius = 2f;


		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();


		public readonly int EffectAndSoundWeaponType = 1000000;


		public readonly int ProjectileCode = 65;


		public readonly Dictionary<int, float> SkillApCoef_IN = new Dictionary<int, float>();


		public readonly Dictionary<int, float> SkillApCoef_OUT = new Dictionary<int, float>();


		public readonly float SkillDamageDelay = 1.5f;


		public BowSkillActiveData()
		{
			DamageByLevel_IN.Add(1, 300);
			DamageByLevel_IN.Add(2, 500);
			SkillApCoef_IN.Add(1, 2f);
			SkillApCoef_IN.Add(2, 2f);
			DamageByLevel_OUT.Add(1, 150);
			DamageByLevel_OUT.Add(2, 250);
			SkillApCoef_OUT.Add(1, 1f);
			SkillApCoef_OUT.Add(2, 1f);
			DebuffState.Add(1, 3018001);
			DebuffState.Add(2, 3018002);
		}
	}
}