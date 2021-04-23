using UnityEngine;

namespace Blis.Common
{
	
	public static class StateTypeExtensions
	{
		
		public static bool IsNegativelyCrowdControl(this StateType stateType)
		{
			return stateType - StateType.Airborne <= 16 || stateType == StateType.Dance;
		}

		
		public static bool IsCrowdControl(this StateType stateType)
		{
			return stateType - StateType.Airborne <= 18;
		}

		
		public static bool IsShowFloatingUi(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Unstoppable:
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Silence:
				case StateType.BlockedSight:
				case StateType.Fetter:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Disarmed:
				case StateType.Suppressed:
				case StateType.Blind:
				case StateType.Grounding:
				case StateType.Polymorph:
				case StateType.Dance:
					return true;
			}

			return false;
		}

		
		public static bool IsBlockedByPlayingState(this StateType newType, StateType playingType)
		{
			return GameDB.crowdControl.IsBlockedByPlayingState(newType, playingType);
		}

		
		public static bool IsCancelPlayingSkill(this StateType newType, StateType playingType)
		{
			return GameDB.crowdControl.IsCancelPlayingSkill(newType, playingType);
		}

		
		public static bool IsPausePlayingSkill(this StateType newType, StateType playingType)
		{
			return GameDB.crowdControl.IsPausePlayingSkill(newType, playingType);
		}

		
		public static ActionCategoryType CanNotAction(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return ActionCategoryType.All;
				case StateType.Uninteractionable:
					return ActionCategoryType.CastAction | ActionCategoryType.ItemEquipOrUnequipAction;
			}

			return ActionCategoryType.None;
		}

		
		public static bool CanControl(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grab:
					return false;
			}

			return true;
		}

		
		public static bool CanNormalAttack(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Charm:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Disarmed:
				case StateType.Suppressed:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return false;
			}

			return true;
		}

		
		public static bool CanUseSkill(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Silence:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return false;
			}

			return true;
		}

		
		public static bool CanUseAggressiveSkill(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Charm:
				case StateType.Silence:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return false;
			}

			return true;
		}

		
		public static bool CanUseMovementSkill(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Silence:
				case StateType.Fetter:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grounding:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return false;
			}

			return true;
		}

		
		public static bool CanMove(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Fetter:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grab:
					return false;
			}

			return true;
		}

		
		public static bool ApplyMoveAnimation(this StateType stateType)
		{
			return stateType != StateType.Airborne && stateType - StateType.Fetter > 3 && stateType != StateType.Grab;
		}

		
		public static bool CancelNormalAttack(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Disarmed:
				case StateType.Suppressed:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return true;
			}

			return false;
		}

		
		public static bool CancelAggressiveSkill(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Silence:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return true;
			}

			return false;
		}

		
		public static bool CancelMovementSkill(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Charm:
				case StateType.Fetter:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return true;
			}

			return false;
		}

		
		public static bool CancelMove(this StateType stateType, Vector3 position)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Fear:
				case StateType.Charm:
				case StateType.Fetter:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grab:
				{
					Vector3 vector;
					return MoveAgent.CanStandToPosition(position, 2147483640, 0.5f, out vector);
				}
				case StateType.Slow:
				case StateType.Taunt:
				case StateType.Silence:
				case StateType.BlockedSight:
				case StateType.Disarmed:
				case StateType.Blind:
				case StateType.Grounding:
				case StateType.Polymorph:
				case StateType.Uninteractionable:
				case StateType.Dance:
					return false;
				default:
					return false;
			}
		}

		
		public static bool IsReduceDurationType01(this StateType stateType, StateType targetType)
		{
			return stateType == StateType.ReduceCCType01 && (targetType - StateType.Airborne <= 1 ||
			                                                 targetType - StateType.Fetter <= 1 ||
			                                                 targetType == StateType.Suppressed);
		}

		
		public static bool IsImmune(this StateType stateType, StateType targetType)
		{
			if (stateType == StateType.Unstoppable)
			{
				switch (targetType)
				{
					case StateType.Airborne:
					case StateType.Slow:
					case StateType.Fear:
					case StateType.Taunt:
					case StateType.Charm:
					case StateType.Fetter:
					case StateType.Stun:
					case StateType.Knockback:
					case StateType.Sleep:
					case StateType.Suppressed:
					case StateType.Grab:
					case StateType.Polymorph:
					case StateType.Dance:
						return true;
				}

				return false;
			}

			return false;
		}

		
		public static bool IsNegativelyAffectMovement(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Airborne:
				case StateType.Slow:
				case StateType.Fear:
				case StateType.Taunt:
				case StateType.Charm:
				case StateType.Fetter:
				case StateType.Stun:
				case StateType.Knockback:
				case StateType.Sleep:
				case StateType.Suppressed:
				case StateType.Grounding:
				case StateType.Grab:
				case StateType.Polymorph:
				case StateType.Dance:
					return true;
			}

			return false;
		}

		
		public static int StateCode(this StateType stateType)
		{
			if (stateType == StateType.MonsterReset)
			{
				return 10000;
			}

			return 0;
		}

		
		public static bool IsUpperPriority(this StateType stateType, StateType otherStateType)
		{
			return stateType != StateType.Slow && (otherStateType == StateType.Slow ||
			                                       stateType.ToStateTypePriority() >
			                                       otherStateType.ToStateTypePriority());
		}
	}
}