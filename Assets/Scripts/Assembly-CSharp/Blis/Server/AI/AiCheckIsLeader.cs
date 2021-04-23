using Blis.Common;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("봇이 리더 인지 아닌지 체크. true면 LEADER false면 FOLLOWER")]
	public class AiCheckIsLeader : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.ObjectType == ObjectType.BotPlayerCharacter && ((WorldBotPlayerCharacter)base.agent).TeamRole == BotTeamRole.LEADER;
		}
	}
}
