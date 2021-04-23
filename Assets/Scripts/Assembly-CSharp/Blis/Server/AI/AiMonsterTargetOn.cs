using System.Collections.Generic;
using Blis.Common;
using ParadoxNotion;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("MonsterType, MonsterLevel과 현재 캐릭터와 레벨을 비교하여 적합한 대상을 찾아서 타겟으로 지정한다.")]
	public class AiMonsterTargetOn : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			float sightRange = this.manualRange;
			if (this.autoRange)
			{
				sightRange = base.agent.Stat.SightRange;
			}
			List<WorldMonster> list = base.agent.SightAgent.FindAllInAllySights<WorldMonster>();
			for (int i = list.Count - 1; i >= 0; i--)
			{
				WorldMonster worldMonster = list[i];
				if (worldMonster.IsAlive && this.monsterType.Contains(worldMonster.MonsterData.monster) && base.agent.IsInDistance(base.agent.GetPosition(), sightRange, worldMonster.GetPosition(), worldMonster.Stat.Radius) && OperationTools.Compare(base.agent.Status.Level, worldMonster.Status.Level, this.levelCheckType))
				{
					base.agent.Controller.TargetOn(worldMonster);
					base.EndAction(true);
					return;
				}
			}
			base.EndAction(false);
		}

		
		public List<MonsterType> monsterType;

		
		public CompareMethod levelCheckType;

		
		public bool autoRange;

		
		public float manualRange;
	}
}
