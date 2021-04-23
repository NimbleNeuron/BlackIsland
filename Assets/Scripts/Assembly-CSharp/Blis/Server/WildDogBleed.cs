using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WildDogBleed)]
	public class WildDogBleed : DamageForSeconds
	{
		
		
		protected override int intervalCount => Singleton<WildDogSkillData>.inst.DFS_IntervalCount;

		
		
		protected override float intervalTime => Singleton<WildDogSkillData>.inst.DFS_IntervalTime;

		
		
		protected override Dictionary<int, int> damageByLevel => Singleton<WildDogSkillData>.inst.DFS_DamageByLevel;

		
		
		protected override Dictionary<int, float> damageApCoefByLevel =>
			Singleton<WildDogSkillData>.inst.DFS_DamageApCoefByLevel;
	}
}