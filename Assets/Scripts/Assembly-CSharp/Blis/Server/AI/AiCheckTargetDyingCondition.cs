using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 target이 빈사 상태인지 확인. 빈사 상태면 true, 아니면 false")]
	public class AiCheckTargetDyingCondition : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			WorldCharacter worldCharacter = base.agent.Controller.GetTarget() as WorldCharacter;
			return worldCharacter != null && worldCharacter.IsDyingCondition;
		}
	}
}
