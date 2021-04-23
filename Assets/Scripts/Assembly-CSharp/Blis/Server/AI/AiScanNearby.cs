using Blis.Common;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("Range 범위 안에 유저 or 봇이 있는지 검사")]
	public class AiScanNearby : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			float num = (float)this.range;
			if (this.useAutoRange)
			{
				num = base.agent.Stat.SightRange;
			}
			if (this.scanUser && this.scanBot)
			{
				return base.agent.FindEnemyPlayerForAttack(base.agent.GetPosition(), num) != null;
			}
			if (this.scanUser)
			{
				return base.agent.FindEnemyPlayerForAttack(base.agent.GetPosition(), num, PlayerType.UserPlayer) != null;
			}
			return this.scanBot && base.agent.FindEnemyPlayerForAttack(base.agent.GetPosition(), num, PlayerType.BotPlayer) == null;
		}

		
		public bool scanUser;

		
		public bool scanBot;

		
		public bool useAutoRange;

		
		public int range;
	}
}
