using System.Collections.Generic;

namespace Blis.Common
{
	
	public class YukiSkillPassiveData : Singleton<YukiSkillPassiveData>
	{
		
		public readonly float Active1AttackStunDuration = 1f;

		
		public readonly float CooldownReduceActive2 = 0.5f;

		
		public readonly Dictionary<int, int> Damage = new Dictionary<int, int>();

		
		public readonly float RecoverySeconds = 4f;

		
		public readonly Dictionary<int, int> ReduceDamageRatio = new Dictionary<int, int>();

		
		public YukiSkillPassiveData()
		{
			Damage.Add(1, 15);
			Damage.Add(2, 30);
			Damage.Add(3, 45);
			ReduceDamageRatio.Add(1, 10);
			ReduceDamageRatio.Add(2, 15);
			ReduceDamageRatio.Add(3, 20);
		}
	}
}