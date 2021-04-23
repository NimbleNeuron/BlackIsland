using System.Collections.Generic;

namespace Blis.Common
{
	public class BatSkillActiveData : Singleton<BatSkillActiveData>
	{
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();


		public readonly float KnockBackDistance = 4f;


		public readonly Dictionary<int, float> SkillApCoef = new Dictionary<int, float>();


		public readonly float StunDuration = 0.5f;


		public BatSkillActiveData()
		{
			DamageByLevel.Add(1, 200);
			DamageByLevel.Add(2, 400);
			SkillApCoef.Add(1, 1f);
			SkillApCoef.Add(2, 1.5f);
		}
	}
}