using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	[SkillScript(SkillId.JackieActive1Debuff)]
	public class JackieActive1Debuff : DamageForSeconds
	{
		
		
		protected override int intervalCount => Singleton<JackieSkillActive1Data>.inst.DFS_IntervalCount;

		
		
		protected override float intervalTime => Singleton<JackieSkillActive1Data>.inst.DFS_IntervalTime;

		
		
		protected override Dictionary<int, int> damageByLevel =>
			Singleton<JackieSkillActive1Data>.inst.DFS_DamageByLevel;

		
		
		protected override Dictionary<int, float> damageApCoefByLevel =>
			Singleton<JackieSkillActive1Data>.inst.DFS_DamageApCoefByLevel;
	}
}