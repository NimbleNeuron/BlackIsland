using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Server.CharacterAction
{
	
	public class AttackTarget : ChainActionBase
	{
		
		private const float MoveToTargetTick = 0.2f;

		
		private readonly WorldPlayerCharacter selfPlayer;

		
		private readonly WorldCharacter target;

		
		private float lastMoveToTargetTick;

		
		private Vector3? pastTargetPos;

		
		public AttackTarget(WorldMovableCharacter self, WorldCharacter target) : base(self, null, false)
		{
			this.target = target;
			selfPlayer = self as WorldPlayerCharacter;
		}

		
		public override WorldObject GetTarget()
		{
			return target;
		}

		
		public WorldCharacter GetTargetCharacter()
		{
			return target;
		}

		
		public override void Start()
		{
			if (target == null)
			{
				return;
			}

			if (self.IsDyingCondition)
			{
				return;
			}

			if (selfPlayer != null && selfPlayer.IsOutSight(target.ObjectId))
			{
				selfPlayer.Controller.SetIdle(true);
				return;
			}

			if (self.SkillController.IsPlaying(SkillSlotIndex.Attack))
			{
				if (self.IsCanNormalAttack() && self.SkillController.CancelNormalAttack(target.ObjectId))
				{
					self.NormalAttack(target);
				}

				return;
			}

			if (self.IsInAttackableDistance(target))
			{
				StopMove();
				return;
			}

			if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack(target.ObjectId);
				self.MoveToTarget(target);
			}

			pastTargetPos = target.GetPosition();
		}

		
		protected override ActionBase Update()
		{
			if (this.nextAction != null)
			{
				ActionBase nextAction = this.nextAction;
				this.nextAction = null;
				return nextAction;
			}

			if (target == null || !target.IsAlive)
			{
				return new Idle(self, true);
			}

			if (target.IsUntargetable())
			{
				if (self as WorldMonster == null)
				{
					return new Idle(self, true);
				}

				return new WaitForAttackTarget(self, target, GameConstants.MonsterWaitForAttackTargetTime);
			}

			if (self.IsDyingCondition)
			{
				return new MoveTo(self, target.GetPosition(), false);
			}

			if (selfPlayer != null && !selfPlayer.IsEquippedWeapon())
			{
				return new Hold(self, null);
			}

			if (self.SkillController.IsPlaying(SkillSlotIndex.Attack))
			{
				return null;
			}

			if (self.IsAttackable(target))
			{
				pastTargetPos = target.GetPosition();
				if (self.IsInAttackableDistance(target))
				{
					if (self.IsCanNormalAttack() && self.NormalAttack(target))
					{
						lastMoveToTargetTick = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
					}
					else if (!self.IsMoving())
					{
						self.LookAt(GameUtil.DirectionOnPlane(self.GetPosition(), target.GetPosition()), 0.1f, true);
					}
				}
				else
				{
					if (!self.CanMove())
					{
						return null;
					}

					if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastMoveToTargetTick < 0.2f)
					{
						return null;
					}

					lastMoveToTargetTick = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
					self.SkillController.CancelNormalAttack(target.ObjectId);
					self.MoveToTarget(target);
				}

				return null;
			}

			self.SkillController.CancelNormalAttack();
			if (GameUtil.DistanceOnPlane(self.GetPosition(), target.GetPosition()) > self.Stat.SightRange * 3f)
			{
				return new MoveToTargetPosition(self, target, pastTargetPos);
			}

			return new MoveToTargetPosition(self, target);
		}
	}
}