using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.WicklineDebuff)]
	public class WicklineDebuff : DamageForSeconds
	{
		
		
		protected override int intervalCount => Singleton<WicklineSkillDebuffData>.inst.DFS_IntervalCount;

		
		
		protected override float intervalTime => Singleton<WicklineSkillDebuffData>.inst.DFS_IntervalTime;

		
		
		protected override Dictionary<int, int> damageByLevel =>
			Singleton<WicklineSkillDebuffData>.inst.DFS_DamageByLevel;

		
		
		protected override Dictionary<int, float> damageApCoefByLevel =>
			Singleton<WicklineSkillDebuffData>.inst.DFS_DamageApCoefByLevel;
	}
}