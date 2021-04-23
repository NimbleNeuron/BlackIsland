using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터가 target을 가지고 있는지 확인.(Blackboard의 target이 아닌 게임 코드의 target) 있으면 true, 없으면 false")]
	public class AiCheckHaveTarget : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.Controller.GetTarget() != null;
		}
	}
}
