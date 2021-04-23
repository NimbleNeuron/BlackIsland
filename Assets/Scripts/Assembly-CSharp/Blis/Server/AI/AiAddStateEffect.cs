using Blis.Common;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터에 State Effect를 추가.(현재 제대로 동작하지 않음. MonsterResetState만 추가 됨)")]
	public class AiAddStateEffect : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			int num = this.addStateType.StateCode();
			if (num > 0)
			{
				MonsterResetState state = new MonsterResetState(num, base.agent, base.agent);
				base.agent.AddState(state, base.agent.ObjectId);
			}
			base.EndAction(true);
		}

		
		public StateType addStateType;
	}
}
