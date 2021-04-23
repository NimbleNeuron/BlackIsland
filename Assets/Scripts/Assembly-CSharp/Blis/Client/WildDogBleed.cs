using Blis.Common;

namespace Blis.Client
{
	[SkillScript(SkillId.WildDogBleed)]
	public class WildDogBleed : LocalDamageForSeconds
	{
		protected override string effectTarget => Singleton<WildDogSkillData>.inst.DFS_Effect_Target;
	}
}