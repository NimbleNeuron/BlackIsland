using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WicklineKillBuffDfs)]
	public class WicklineKillBuffDfs : DamageForSeconds
	{
		
		
		protected override int intervalCount => Singleton<WicklineKillSkillBuffData>.inst.DFS_IntervalCount;

		
		
		protected override float intervalTime => Singleton<WicklineKillSkillBuffData>.inst.DFS_IntervalTime;

		
		
		protected override Dictionary<int, int> damageByLevel =>
			Singleton<WicklineKillSkillBuffData>.inst.DFS_DamageByLevel;

		
		
		protected override Dictionary<int, float> damageApCoefByLevel =>
			Singleton<WicklineKillSkillBuffData>.inst.DFS_DamageApCoefByLevel;
	}
}