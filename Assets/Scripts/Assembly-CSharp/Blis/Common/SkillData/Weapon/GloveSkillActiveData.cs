using System.Collections.Generic;

namespace Blis.Common
{
	
	public class GloveSkillActiveData : Singleton<GloveSkillActiveData>
	{
		
		public readonly Dictionary<int, int> BuffState = new Dictionary<int, int>();

		
		public readonly Dictionary<int, int> DamageByLevel = new Dictionary<int, int>();

		
		public readonly float DashDistance = 0.4f;

		
		public readonly float DashDuration = 0.17f;

		
		public readonly float GloveUppercutAttackApCoef = 1f;

		
		public readonly Dictionary<int, int> GloveUppercutAttackSkillCode = new Dictionary<int, int>();

		
		public readonly Dictionary<int, float> UppercutFinalMoreDamage = new Dictionary<int, float>();

		
		public readonly Dictionary<int, float> UppercutIncreaseAttackRange = new Dictionary<int, float>();

		
		public GloveSkillActiveData()
		{
			BuffState.Add(1, 3001001);
			BuffState.Add(2, 3001002);
			UppercutIncreaseAttackRange.Add(1, 0.7f);
			UppercutIncreaseAttackRange.Add(2, 0.7f);
			UppercutFinalMoreDamage.Add(1, 1f);
			UppercutFinalMoreDamage.Add(2, 2f);
			GloveUppercutAttackSkillCode.Add(1, 3001101);
			GloveUppercutAttackSkillCode.Add(2, 3001102);
			DamageByLevel.Add(1, 50);
			DamageByLevel.Add(2, 100);
		}
	}
}