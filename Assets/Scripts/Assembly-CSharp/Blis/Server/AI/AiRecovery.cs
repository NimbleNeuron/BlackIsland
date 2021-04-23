using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("캐릭터의 HP / SP를 모두 최대치로 회복 시킨다.")]
	public class AiRecovery : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			HealInfo healInfo = HealInfo.Create(base.agent.Stat.MaxHp, base.agent.Stat.MaxSp);
			healInfo.SetHealer(base.agent);
			base.agent.Heal(healInfo);
			base.EndAction(true);
		}
	}
}
