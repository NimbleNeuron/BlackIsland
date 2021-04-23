using Blis.Common.Utils;
using ParadoxNotion.Design;

namespace Blis.Server
{
	
	[Category("BS:ER")]
	[Description("선제 공격이 가능한 경우 주변의 가까운 적을 타겟으로 잡는다. 비선공 상태인 경우에는 공격을 당했을 경우 공격한 캐릭터를 타겟으로 잡는다.")]
	public class AiTargetOn : ActionTaskBase
	{
		
		protected override void OnExecuteCommandFrame()
		{
			if (base.agent.StateEffector.CanCharacterControl())
			{
				if (base.agent.Controller.GetTarget() != null)
				{
					base.EndAction(true);
					return;
				}
				if (base.agent.IsAggressive() && MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - this.lastTargetOnTick > this.targetOnTick)
				{
					this.lastTargetOnTick = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
					float targetRange = 0f;
					if (this.autoRange)
					{
						targetRange = base.agent.Stat.SightRange;
						base.agent.IfTypeOf<WorldMonster>(delegate(WorldMonster monster)
						{
							targetRange = monster.MonsterData.firstAttackRange;
						});
					}
					else
					{
						targetRange = this.manualRange;
					}
					WorldCharacter worldCharacter = null;
					float num = 0f;
					foreach (WorldCharacter worldCharacter2 in base.agent.FindEnemiesForAttack(base.agent.GetPosition(), targetRange))
					{
						if (!worldCharacter2.IsDyingCondition || this.dyingConditionTarget)
						{
							if (worldCharacter == null)
							{
								worldCharacter = worldCharacter2;
								num = (worldCharacter2.GetPosition() - base.agent.GetPosition()).sqrMagnitude;
							}
							else
							{
								float sqrMagnitude = (worldCharacter2.GetPosition() - base.agent.GetPosition()).sqrMagnitude;
								if (sqrMagnitude < num)
								{
									worldCharacter = worldCharacter2;
									num = sqrMagnitude;
								}
							}
						}
					}
					if (worldCharacter != null)
					{
						if (this.monsterTarget && worldCharacter.IsTypeOf<WorldMonster>())
						{
							if (this.SetTarget(worldCharacter, base.agent))
							{
								return;
							}
						}
						else if (!this.monsterTarget)
						{
							if (this.playerTarget && !worldCharacter.IsAI)
							{
								if (this.SetTarget(worldCharacter, base.agent))
								{
									return;
								}
							}
							else if (this.aiTarget && worldCharacter.IsAI && this.SetTarget(worldCharacter, base.agent))
							{
								return;
							}
						}
						base.EndAction(false);
						return;
					}
				}
			}
			base.EndAction(false);
		}

		
		private bool SetTarget(WorldCharacter enemy, WorldMovableCharacter agent)
		{
			if (!enemy.IsDyingCondition)
			{
				agent.Controller.TargetOn(enemy);
				base.EndAction(true);
				return true;
			}
			if (this.dyingConditionTarget)
			{
				agent.Controller.TargetOn(enemy);
				base.EndAction(true);
				return true;
			}
			return false;
		}

		
		public bool monsterTarget;

		
		public bool playerTarget;

		
		public bool aiTarget;

		
		public bool dyingConditionTarget;

		
		public bool autoRange;

		
		public float manualRange;

		
		private float targetOnTick = 0.3f;

		
		private float lastTargetOnTick;
	}
}
