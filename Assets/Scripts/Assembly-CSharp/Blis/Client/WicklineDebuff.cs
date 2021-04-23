using Blis.Common;

namespace Blis.Client
{
	[SkillScript(SkillId.WicklineDebuff)]
	public class WicklineDebuff : LocalDamageForSeconds
	{
		protected override string effectTarget => Singleton<WicklineSkillDebuffData>.inst.DFS_Effect_Target;
	}
}