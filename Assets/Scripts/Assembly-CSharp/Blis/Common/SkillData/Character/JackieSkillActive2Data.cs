using System.Collections.Generic;

namespace Blis.Common
{
	
	public class JackieSkillActive2Data : Singleton<JackieSkillActive2Data>
	{
		
		public readonly int Active2BuffEffectAndSoundCode = 1001301;

		
		public readonly int Active4BuffGroup = 1001500;

		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> DamageApCoef = new Dictionary<int, float>();

		
		public readonly float HealApCoef = 0.1f;

		
		public readonly Dictionary<int, int> HealByLevel = new Dictionary<int, int>();

		
		public JackieSkillActive2Data()
		{
			BuffState.Add(1, 1001301);
			BuffState.Add(2, 1001302);
			BuffState.Add(3, 1001303);
			BuffState.Add(4, 1001304);
			BuffState.Add(5, 1001305);
			HealByLevel.Add(1, 12);
			HealByLevel.Add(2, 19);
			HealByLevel.Add(3, 26);
			HealByLevel.Add(4, 33);
			HealByLevel.Add(5, 40);
			DamageApCoef.Add(1, 0.1f);
			DamageApCoef.Add(2, 0.12f);
			DamageApCoef.Add(3, 0.15f);
			DamageApCoef.Add(4, 0.17f);
			DamageApCoef.Add(5, 0.2f);
		}
	}
}