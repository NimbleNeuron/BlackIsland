using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("스킬 포인트가 있는지 확인한다. 있으면 true, 없으면 false")]
	public class AiCheckSkillPoint : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.IsTypeOf<WorldPlayerCharacter>() && ((WorldPlayerCharacter)base.agent).AnyHaveSkillUpgradePoint();
		}
	}
}
