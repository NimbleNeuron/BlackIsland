using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터가 빈사 상태인지 확인. 빈사 상태면 true, 아니면 false")]
	public class AiCheckDyingCondition : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.IsDyingCondition;
		}
	}
}
