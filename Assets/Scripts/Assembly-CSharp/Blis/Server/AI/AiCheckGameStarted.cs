using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("게임이 시작되었는지 확인. 시작되었으면 true, 아니면 false")]
	public class AiCheckGameStarted : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return MonoBehaviourInstance<GameService>.inst.IsGameStarted();
		}
	}
}
