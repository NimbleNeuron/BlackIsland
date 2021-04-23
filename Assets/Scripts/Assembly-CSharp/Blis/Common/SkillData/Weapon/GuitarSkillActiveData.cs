using System.Collections.Generic;

namespace Blis.Common
{
	
	public class GuitarSkillActiveData : Singleton<GuitarSkillActiveData>
	{
		
		public readonly float CharmDuration = 1.5f;

		
		public readonly float MoveSpeedRatio = -50f;

		
		public readonly int ProjectileCode = 100827;

		
		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>();

		
		public GuitarSkillActiveData()
		{
			SkillApCoef.Add(1, 1.5f);
			SkillApCoef.Add(2, 2.5f);
		}
	}
}