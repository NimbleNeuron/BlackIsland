using Blis.Common.Utils;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("매칭된 방의 평균 MMR을 mmr에 저장.")]
	public class AiGetBotMMR : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			this.mmr.value = MonoBehaviourInstance<GameService>.inst.PlayerCharacter.minMM;
			base.EndAction(true);
		}

		
		public BBParameter<int> mmr;
	}
}
