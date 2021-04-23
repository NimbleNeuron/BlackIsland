using Blis.Common;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("특정 State Effect가 캐릭터에 적용 중인지 확인. 적용 중이면 true, 아니면 false")]
	public class AiCheckHaveStateEffect : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.StateEffector.IsHaveStateByType(this.checkStateType);
		}

		
		public StateType checkStateType;
	}
}
