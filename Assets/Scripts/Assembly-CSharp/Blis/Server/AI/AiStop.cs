using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터를 정지 시킨다.")]
	public class AiStop : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (base.agent.StateEffector.CanCharacterControl())
			{
				base.agent.Controller.Stop();
				base.EndAction(true);
				return;
			}
			base.EndAction(false);
		}
	}
}
