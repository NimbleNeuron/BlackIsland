using System.Linq;
using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("플레이어를 무조건 타겟으로 잡는다. 튜토리얼에서만 사용해야 함.")]
	public class AiTutorialTargetPlayer : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (MonoBehaviourInstance<GameService>.inst.Player.PlayerCount == 0)
			{
				base.EndAction(false);
				return;
			}
			PlayerSession playerSession = MonoBehaviourInstance<GameService>.inst.Player.PlayerSessions.First<PlayerSession>();
			base.agent.Controller.ForceTargetOn(playerSession.Character);
			base.EndAction(true);
		}
	}
}
