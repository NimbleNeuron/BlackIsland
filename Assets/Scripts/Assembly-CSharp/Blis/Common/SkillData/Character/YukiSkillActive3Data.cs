using System.Collections.Generic;

namespace Blis.Common
{
	
	public class YukiSkillActive3Data : Singleton<YukiSkillActive3Data>
	{
		
		public readonly float AfterHitDistance = 2f;

		
		public readonly float AfterHitDuration = 0.1f;

		
		public readonly int DebuffState = 1011401;

		
		public readonly float Distance = 5f;

		
		public readonly float Duration = 0.43f;

		
		public readonly int EffectAndSound = 1011401;

		
		public readonly int PassiveEffectAndSound = 1011402;

		
		public readonly float SkillApCoef = 0.4f;

		
		public readonly Dictionary<int, int> SkillDamage = new Dictionary<int, int>();

		
		public YukiSkillActive3Data()
		{
			SkillDamage.Add(1, 70);
			SkillDamage.Add(2, 125);
			SkillDamage.Add(3, 180);
			SkillDamage.Add(4, 235);
			SkillDamage.Add(5, 290);
		}
	}
}