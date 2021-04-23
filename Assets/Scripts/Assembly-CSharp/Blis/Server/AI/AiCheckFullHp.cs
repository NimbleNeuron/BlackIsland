using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("체력이 최대치 까지 회복되었는지 확인. 최대 체력이면 true, 아니면 false")]
	public class AiCheckFullHp : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			return base.agent.Status.Hp == base.agent.Stat.MaxHp;
		}
	}
}
