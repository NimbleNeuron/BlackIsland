using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("타겟을 해제 한다.")]
	public class AiClearTarget : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			base.agent.Controller.Stop();
			base.EndAction(true);
		}
	}
}
