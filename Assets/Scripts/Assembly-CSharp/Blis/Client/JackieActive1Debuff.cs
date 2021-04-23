using Blis.Common;

namespace Blis.Client
{
	[SkillScript(SkillId.JackieActive1Debuff)]
	public class JackieActive1Debuff : LocalDamageForSeconds
	{
		protected override string effectTarget => Singleton<JackieSkillActive1Data>.inst.DFS_Effect_Target;
	}
}