using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("BotPlayer의 AI 모드를 확인한다. 1이면 true, 아니면 false")]
	public class AiCheckBotAiModel : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return false;
		}
	}
}
