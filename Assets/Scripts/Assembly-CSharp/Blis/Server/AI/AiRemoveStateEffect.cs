using Blis.Common;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("지정한 State Effect를 캐릭터에서 제거한다.")]
	public class AiRemoveStateEffect : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			base.agent.RemoveAllStateByType(this.removeStateType);
			base.EndAction(true);
		}

		
		public StateType removeStateType;
	}
}
