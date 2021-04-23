using ParadoxNotion;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("현재 캐릭터의 레벨과 타겟의 레벨과 비교한다.")]
	public class AICheckTargetLevel : ConditionTaskBase
	{
		
		protected override bool OnCheck()
		{
			WorldCharacter worldCharacter = base.agent.Controller.GetTarget() as WorldCharacter;
			return worldCharacter == null || OperationTools.Compare(base.agent.Status.Level, worldCharacter.Status.Level, this.checkType);
		}

		
		public CompareMethod checkType;
	}
}
