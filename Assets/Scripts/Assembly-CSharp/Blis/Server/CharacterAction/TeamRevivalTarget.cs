using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server.CharacterAction
{
	
	public class TeamRevivalTarget : ActionBase
	{
		
		private const float MoveToTargetTick = 0.2f;

		
		private readonly WorldPlayerCharacter selfPlayer;

		
		private readonly WorldPlayerCharacter target;

		
		private bool isRunningCastingAction;

		
		private float lastMoveToTargetTick;

		
		public TeamRevivalTarget(WorldMovableCharacter self, WorldPlayerCharacter target) : base(self, false)
		{
			this.target = target;
			selfPlayer = self as WorldPlayerCharacter;
			lastMoveToTargetTick = 0f;
			isRunningCastingAction = false;
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

			if (selfPlayer == null)
			{
				return;
			}

			if (self.IsInTeamRevivalDistance(target))
			{
				if (!target.IsRevivaling)
				{
					StartRevival();
				}
			}
			else if (self.CanMove())
			{
				self.SkillController.CancelNormalAttack(target.ObjectId);
				self.MoveToDestination(target.GetPosition());
			}
		}

		
		protected override ActionBase Update()
		{
			if (target == null || selfPlayer == null)
			{
				return new Idle(selfPlayer, true);
			}

			if (isRunningCastingAction)
			{
				if (!selfPlayer.IsRunningCastingAction())
				{
					return new Idle(self, true);
				}

				if (GameUtil.DistanceOnPlane(selfPlayer.GetPosition(), target.GetPosition()) <= 1.2f)
				{
					return null;
				}

				isRunningCastingAction = false;
				target.CancelActionCasting(CastingCancelType.Forced);
				selfPlayer.CancelActionCasting(CastingCancelType.Action);
				return new Idle(selfPlayer, true);
			}

			if (self.IsInTeamRevivalDistance(target))
			{
				if (target.IsRevivaling)
				{
					self.IfTypeOf<WorldPlayerCharacter>(delegate(WorldPlayerCharacter playerCharacter)
					{
						playerCharacter.SendToastMessage(ToastMessageType.IsRevivaling);
					});
					return new Idle(self, true);
				}

				StartRevival();
				return null;
			}

			if (selfPlayer.IsRunningCastingAction())
			{
				selfPlayer.CancelActionCasting(CastingCancelType.Action);
			}

			if (!self.CanMove())
			{
				return null;
			}

			if (MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - lastMoveToTargetTick < 0.2f)
			{
				return null;
			}

			self.SkillController.CancelNormalAttack(target.ObjectId);
			self.MoveToDestination(target.GetPosition());
			lastMoveToTargetTick = MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime;
			return null;
		}

		
		private bool StartRevival()
		{
			if (target == null || !target.IsAlive || !target.IsDyingCondition || target.TeamNumber != self.TeamNumber)
			{
				return false;
			}

			if (!self.CanAnyAction(ActionType.Resurrect))
			{
				return false;
			}

			StopMove();
			selfPlayer.LookAt(GameUtil.DirectionOnPlane(selfPlayer.GetPosition(), target.GetPosition()), 0.19f, true);
			selfPlayer.StartActionCasting(CastingActionType.Resurrect, false, delegate
			{
				target.StartRevival(selfPlayer);
				target.StartActionCasting(CastingActionType.Resurrected, false, null, null);
			}, delegate { target.OnRevivalEnd(selfPlayer, false); }, delegate
			{
				target.CancelActionCasting(CastingCancelType.Forced);
				target.OnRevivalEnd(selfPlayer, true);
			});
			isRunningCastingAction = true;
			return true;
		}
	}
}