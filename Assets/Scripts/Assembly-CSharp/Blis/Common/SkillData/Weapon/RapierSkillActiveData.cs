using System.Collections.Generic;

namespace Blis.Common
{
	
	public class RapierSkillActiveData : Singleton<RapierSkillActiveData>
	{
		
		public readonly float DashSpeed = 20f;

		
		public readonly float DistanceCollisionEnd = 5f;

		
		public readonly float DistanceCollisionStart = 1f;

		
		public readonly int EffectAndSoundCode = 1000000;

		
		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>();

		
		public RapierSkillActiveData()
		{
			SkillApCoef.Add(1, 1f);
			SkillApCoef.Add(2, 1f);
		}
	}
}