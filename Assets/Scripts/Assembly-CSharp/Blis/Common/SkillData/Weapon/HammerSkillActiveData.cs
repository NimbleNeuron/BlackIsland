using System.Collections.Generic;

namespace Blis.Common
{
	
	public class HammerSkillActiveData : Singleton<HammerSkillActiveData>
	{
		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DebuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> DefCoefficient = new Dictionary<int, float>();

		
		public readonly int EffectAndSoundeCode = 3013001;

		
		public HammerSkillActiveData()
		{
			DebuffState.Add(1, 3013001);
			DebuffState.Add(2, 3013002);
			DamageByLevel.Add(1, 150);
			DamageByLevel.Add(2, 300);
			DefCoefficient.Add(1, 1f);
			DefCoefficient.Add(2, 2f);
		}
	}
}