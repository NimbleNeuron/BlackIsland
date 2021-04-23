using Blis.Common;
using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("입력한 플레이어 타입과 타겟 플레이어 타입이 동일한지 확인한다.")]
	public class AiCheckCharacterType : ConditionTaskBase
	{
		
		protected override bool OnCheckCommandFrame()
		{
			if (base.agent.IsAggressive())
			{
				if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.lastTargetOnTick > this.targetOnTick)
				{
					this.lastTargetOnTick = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
				}
				float sightRange;
				if (this.autoRange)
				{
					sightRange = base.agent.Stat.SightRange;
				}
				else
				{
					sightRange = this.manualRange;
				}
				return base.agent.FindEnemyPlayerForAttack(base.agent.GetPosition(), sightRange, this.playerType) != null;
			}
			return false;
		}

		
		public PlayerType playerType;

		
		public bool autoRange;

		
		public float manualRange;

		
		private float targetOnTick = 0.3f;

		
		private float lastTargetOnTick;
	}
}
