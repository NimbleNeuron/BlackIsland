using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("stateCode에 해당하는 State를 가지고 있는지 확인.")]
	public class AiCheckHaveState : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.StateEffector.AnyHaveStateByGroup(this.stateGroupCode);
		}

		
		public int stateGroupCode;
	}
}
