using Blis.Common;

namespace Blis.Client
{
	[SkillScript(SkillId.WicklineKillBuffDfs)]
	public class LocalWicklineKillBuffDfs : LocalDamageForSeconds
	{
		protected override string effectTarget => Singleton<WicklineKillSkillBuffData>.inst.DFS_Effect_Target;
	}
}