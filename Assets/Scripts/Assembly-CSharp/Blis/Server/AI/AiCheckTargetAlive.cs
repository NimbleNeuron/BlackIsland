using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 target이 살아 있는지 확인. 살아있으면 true, target이 없거나 죽었으면 false")]
	public class AiCheckTargetAlive : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			WorldCharacter worldCharacter = base.agent.Controller.GetTarget() as WorldCharacter;
			return worldCharacter != null && worldCharacter.IsAlive;
		}
	}
}
