using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 캐릭터가 이동 중인지 확인. 이동 중이면 true, 아니면 false")]
	public class AiCheckIsMoving : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.IsMoving();
		}
	}
}
