namespace Blis.Server.CharacterAction
{
	
	public class InteractTarget : ActionBase
	{
		
		private readonly WorldObject target;

		
		private bool isMoveStart;

		
		public InteractTarget(WorldMovableCharacter self, WorldObject target) : base(self, false)
		{
			this.target = target;
		}

		
		public override WorldObject GetTarget()
		{
			return target;
		}

		
		public override void Start()
		{
			if (target == null)
			{
				return;
			}

			if (self.IsInInteractableDistance(target))
			{
				return;
			}

			if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToTarget(target, 0f, false);
				isMoveStart = true;
				return;
			}

			isMoveStart = false;
		}

		
		protected override ActionBase Update()
		{
			if (target == null)
			{
				return new Idle(self, true);
			}

			if (self.IsInInteractableDistance(target))
			{
				self.Interact(target);
				return new Idle(self, false);
			}

			if (!isMoveStart && self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToTarget(target, 0f, false);
				isMoveStart = true;
			}
			else if (isMoveStart && !self.CanMove())
			{
				isMoveStart = false;
			}

			return null;
		}
	}
}