using System.Collections.Generic;

namespace Blis.Common
{
	
	public class FioraSkillActive4Data : Singleton<FioraSkillActive4Data>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly int BuffStateGroup = 1003500;

		
		public readonly Dictionary<int, int> ConsumeSp = new Dictionary<int, int>();

		
		public readonly float NormalAttackApCoef = 1f;

		
		public readonly Dictionary<int, int> SkillAttack = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> SkillAttackApCoef = new Dictionary<int, float>();

		
		public FioraSkillActive4Data()
		{
			SkillAttack.Add(1, 30);
			SkillAttack.Add(2, 35);
			SkillAttack.Add(3, 40);
			SkillAttackApCoef.Add(1, 0.06f);
			SkillAttackApCoef.Add(2, 0.18f);
			SkillAttackApCoef.Add(3, 0.3f);
			ConsumeSp.Add(1, 25);
			ConsumeSp.Add(2, 45);
			ConsumeSp.Add(3, 65);
			BuffState.Add(1, 1003501);
			BuffState.Add(2, 1003502);
			BuffState.Add(3, 1003503);
		}
	}
}