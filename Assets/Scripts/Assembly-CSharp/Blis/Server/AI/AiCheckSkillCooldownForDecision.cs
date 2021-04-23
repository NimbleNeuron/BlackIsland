using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("스킬 쿨타임을 체크한다. 사용 가능하면 true, 아니면 false")]
	public class AiCheckSkillCooldownForDecision : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return false;
		}
	}
}
