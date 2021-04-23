using System;

namespace Blis.Common
{
	
	public static class StateTypePriorityEx
	{
		
		public static StateType ToStateType(this StateTypePriority stateTypePriority)
		{
			switch (stateTypePriority)
			{
				case StateTypePriority.Airborne:
					return StateType.Airborne;
				case StateTypePriority.KnockBack:
					return StateType.Knockback;
				case StateTypePriority.Stun:
					return StateType.Stun;
				case StateTypePriority.Suppressed:
					return StateType.Suppressed;
				case StateTypePriority.Fetter:
					return StateType.Fetter;
				case StateTypePriority.Sleep:
					return StateType.Sleep;
				case StateTypePriority.Fear:
					return StateType.Fear;
				case StateTypePriority.Taunt:
					return StateType.Taunt;
				case StateTypePriority.Charm:
					return StateType.Charm;
				case StateTypePriority.Silence:
					return StateType.Silence;
				case StateTypePriority.BlockedSight:
					return StateType.BlockedSight;
				case StateTypePriority.Disarmed:
					return StateType.Disarmed;
				case StateTypePriority.Blind:
					return StateType.Blind;
				case StateTypePriority.Grounding:
					return StateType.Grounding;
				case StateTypePriority.Dance:
					return StateType.Dance;
				case StateTypePriority.Grab:
					return StateType.Grab;
				case StateTypePriority.Polymorph:
					return StateType.Polymorph;
				default:
					throw new ArgumentOutOfRangeException("stateTypePriority", stateTypePriority, null);
			}
		}

		
		public static StateTypePriority ToStateTypePriority(this StateType stateType)
		{
			switch (stateType)
			{
				case StateType.Common:
					return StateTypePriority.Common;
				case StateType.Airborne:
					return StateTypePriority.Airborne;
				case StateType.Fear:
					return StateTypePriority.Fear;
				case StateType.Taunt:
					return StateTypePriority.Taunt;
				case StateType.Charm:
					return StateTypePriority.Charm;
				case StateType.Silence:
					return StateTypePriority.Silence;
				case StateType.BlockedSight:
					return StateTypePriority.BlockedSight;
				case StateType.Fetter:
					return StateTypePriority.Fetter;
				case StateType.Stun:
					return StateTypePriority.Stun;
				case StateType.Knockback:
					return StateTypePriority.KnockBack;
				case StateType.Sleep:
					return StateTypePriority.Sleep;
				case StateType.Disarmed:
					return StateTypePriority.Disarmed;
				case StateType.Suppressed:
					return StateTypePriority.Suppressed;
				case StateType.Blind:
					return StateTypePriority.Blind;
				case StateType.Grounding:
					return StateTypePriority.Grounding;
				case StateType.Grab:
					return StateTypePriority.Grab;
				case StateType.Polymorph:
					return StateTypePriority.Polymorph;
				case StateType.Dance:
					return StateTypePriority.Dance;
			}

			throw new ArgumentOutOfRangeException("stateType", stateType, null);
		}
	}
}