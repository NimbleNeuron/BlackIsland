using Blis.Common;
using Blis.Common.Utils;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 봇 난이도를 BotDifficulty로 가져온다.")]
	public class AiGetBotDifficulty : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			this.botDifficulty.value = MonoBehaviourInstance<GameService>.inst.Bot.Difficulty;
			base.EndAction(true);
		}

		
		public BBParameter<BotDifficulty> botDifficulty;
	}
}
