using Blis.Common;

namespace Blis.Server.CharacterAction
{
	
	public class WaitForSkillFinish : ChainActionBase
	{
		
		private readonly SkillSlotSet skillSlotSet;

		
		private readonly WorldCharacter target;

		
		public WaitForSkillFinish(WorldMovableCharacter self, SkillSlotSet skillSlotSet, WorldCharacter target,
			ActionBase prevAction) : base(self, null, false)
		{
			this.skillSlotSet = skillSlotSet;
			this.target = target;
			ReplaceNextAction(prevAction);
		}

		
		public override WorldObject GetTarget()
		{
			return target;
		}

		
		public WorldCharacter GetTargetCharacter()
		{
			return target;
		}

		
		public override void Start() { }

		
		protected override ActionBase Update()
		{
			if (self.SkillController.IsPlaying(skillSlotSet))
			{
				return null;
			}

			if (this.nextAction != null)
			{
				ActionBase nextAction = this.nextAction;
				this.nextAction = null;
				return nextAction;
			}

			if (!self.IsAI && self.IsInvisible)
			{
				return new Idle(self, true);
			}

			if (!(target != null) || !target.IsAlive || self.GetHostileType(target) != HostileType.Enemy)
			{
				return new Idle(self, true);
			}

			if (!target.IsUntargetable())
			{
				return new AttackTarget(self, target);
			}

			if (self as WorldMonster == null)
			{
				return new Idle(self, true);
			}

			return new WaitForAttackTarget(self, target, GameConstants.MonsterWaitForAttackTargetTime);
		}
	}
}