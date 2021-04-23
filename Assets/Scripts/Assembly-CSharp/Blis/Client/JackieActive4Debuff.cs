using Blis.Common;

namespace Blis.Client
{
	[SkillScript(SkillId.JackieActive4Debuff)]
	public class JackieActive4Debuff : LocalDamageForSeconds
	{
		protected override string effectTarget => Singleton<JackieSkillActive4Data>.inst.DFS_Effect_Target;
	}
}