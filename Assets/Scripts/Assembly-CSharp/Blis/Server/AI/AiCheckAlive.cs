using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터가 살아 있는지 확인. 살았으면 true, 죽었으면 false")]
	public class AiCheckAlive : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.IsAlive;
		}
	}
}
