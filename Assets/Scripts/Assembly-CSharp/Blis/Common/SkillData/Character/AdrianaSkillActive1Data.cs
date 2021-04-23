using System.Collections.Generic;

namespace Blis.Common
{
	
	public class AdrianaSkillActive1Data : Singleton<AdrianaSkillActive1Data>
	{
		
		public readonly Dictionary<int, float> ApCoef = new Dictionary<int, float>();

		
		public readonly Dictionary<int, int> Damage = new Dictionary<int, int>();

		
		public readonly float DamageTerm = 0.23f;

		
		public readonly int FireFlameDamageEffectAndSoundCode = 1017201;

		
		public readonly int MaxAttackCount = 9;

		
		public readonly float SkillApCoef = 0.3f;

		
		public AdrianaSkillActive1Data()
		{
			Damage.Add(1, 12);
			Damage.Add(2, 15);
			Damage.Add(3, 18);
			Damage.Add(4, 21);
			Damage.Add(5, 24);
			ApCoef.Add(1, 0.1f);
			ApCoef.Add(2, 0.15f);
			ApCoef.Add(3, 0.2f);
			ApCoef.Add(4, 0.25f);
			ApCoef.Add(5, 0.3f);
		}
	}
}