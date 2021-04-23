using System.Collections.Generic;

namespace Blis.Common
{
	
	public class OneHandSwordSkillActiveData : Singleton<OneHandSwordSkillActiveData>
	{
		
		public readonly float ApCoefficient = 1f;

		
		public readonly int BuffGroup1 = 3015000;

		
		public readonly int BuffGroup2 = 3015100;

		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> BuffState_2 = new Dictionary<int, int>();

		
		public readonly int DaggerAttackDamageRatioPerHp = 10;

		
		public readonly Dictionary<int, int> DaggerAttackSkillCode = new Dictionary<int, int>();

		
		public readonly int EffectAndSoundCode = 1000000;

		
		public OneHandSwordSkillActiveData()
		{
			BuffState.Add(1, 3015001);
			BuffState.Add(2, 3015002);
			BuffState_2.Add(1, 3015101);
			BuffState_2.Add(2, 3015102);
			DaggerAttackSkillCode.Add(1, 3015101);
			DaggerAttackSkillCode.Add(2, 3015102);
		}
	}
}