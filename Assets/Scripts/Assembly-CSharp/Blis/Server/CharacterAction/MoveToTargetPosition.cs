using UnityEngine;

namespace Blis.Server.CharacterAction
{
	
	public class MoveToTargetPosition : ActionBase
	{
		
		private readonly Vector3? destination;

		
		private readonly WorldCharacter target;

		
		private bool moveStarted;

		
		private Vector3 pastTargetPosition;

		
		private readonly WorldPlayerCharacter selfPlayer;

		
		public MoveToTargetPosition(WorldMovableCharacter self, WorldCharacter target, Vector3? destination = null) :
			base(self, false)
		{
			this.target = target;
			moveStarted = false;
			this.destination = destination;
			if (target != null)
			{
				pastTargetPosition = target.GetPosition();
				selfPlayer = self as WorldPlayerCharacter;
			}
		}

		
		private Vector3 GetDestination()
		{
			if (destination != null)
			{
				return destination.Value;
			}

			if (!(target != null))
			{
				return self.GetPosition();
			}

			if (selfPlayer != null)
			{
				if (!selfPlayer.IsOutSight(target.ObjectId))
				{
					pastTargetPosition = target.GetPosition();
				}

				return pastTargetPosition;
			}

			return target.GetPosition();
		}

		
		public override WorldObject GetTarget()
		{
			return target;
		}

		
		public override void Start()
		{
			if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToDestination(GetDestination());
				moveStarted = true;
			}
		}

		
		protected override ActionBase Update()
		{
			if (target == null)
			{
				return new Idle(self, true);
			}

			if (moveStarted)
			{
				if (self.CanMove() && !self.IsMoving())
				{
					return new Idle(self, true);
				}

				if (self.IsAttackable(target))
				{
					return new AttackTarget(self, target);
				}
			}
			else if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack();
				self.MoveToDestination(GetDestination());
				moveStarted = true;
			}

			return null;
		}
	}
}