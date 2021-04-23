using Blis.Common;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("몬스터의 정보를 가져와서 monsterType에 저장한다")]
	public class AiGetMonsterType : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (!base.agent.IsTypeOf<WorldMonster>())
			{
				base.EndAction(false);
				return;
			}
			WorldMonster worldMonster = (WorldMonster)base.agent;
			this.monsterType.value = worldMonster.MonsterData.monster;
			base.EndAction(true);
		}

		
		public BBParameter<MonsterType> monsterType;
	}
}
