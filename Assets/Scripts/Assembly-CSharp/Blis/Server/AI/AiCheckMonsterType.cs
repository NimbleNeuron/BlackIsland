using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("입력한 몬스터 타입과 타겟 몬스터 타입이 동일한지 확인한다.")]
	public class AiCheckMonsterType : ConditionTaskBase
	{
		
		protected override string OnInit()
		{
			this.monsterTypeList = new List<MonsterType>();
			return base.OnInit();
		}

		
		protected override bool OnCheck()
		{
			this.monsterTypeList.Clear();
			this.monsterTypeList.Add(this.monsterType);
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
				return base.agent.FindEnemyMonsterForAttack(base.agent.GetPosition(), sightRange, this.monsterTypeList) != null;
			}
			return false;
		}

		
		public MonsterType monsterType;

		
		public bool autoRange;

		
		public float manualRange;

		
		private float targetOnTick = 0.3f;

		
		private float lastTargetOnTick;

		
		private List<MonsterType> monsterTypeList;
	}
}
