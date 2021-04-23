using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive4Debuff)]
	public class JackieActive4Debuff : DamageForSeconds
	{
		
		
		protected override int intervalCount => Singleton<JackieSkillActive4Data>.inst.DFS_IntervalCount;

		
		
		protected override float intervalTime => Singleton<JackieSkillActive4Data>.inst.DFS_IntervalTime;

		
		
		protected override Dictionary<int, int> damageByLevel =>
			Singleton<JackieSkillActive4Data>.inst.DFS_DamageByLevel;

		
		
		protected override Dictionary<int, float> damageApCoefByLevel =>
			Singleton<JackieSkillActive4Data>.inst.DFS_DamageApCoefByLevel;
	}
}