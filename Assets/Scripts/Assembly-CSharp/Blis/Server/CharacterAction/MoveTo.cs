using UnityEngine;

namespace Blis.Server.CharacterAction
{
	
	public class MoveTo : ActionBase
	{
		

		
		private bool moveStarted;

		
		public MoveTo(WorldMovableCharacter self, Vector3 destination, bool findAttackTarget) : base(self,
			findAttackTarget)
		{
			this.Destination = destination;
			moveStarted = false;
		}

		
		
		public Vector3 Destination { get; }

		
		public override void Start()
		{
			if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToDestination(Destination);
				moveStarted = true;
			}
		}

		
		protected override ActionBase Update()
		{
			if (moveStarted)
			{
				if (self.CanMove() && !self.IsMoving())
				{
					return new Idle(self, true);
				}
			}
			else if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToDestination(Destination);
				moveStarted = true;
			}

			return null;
		}
	}
}