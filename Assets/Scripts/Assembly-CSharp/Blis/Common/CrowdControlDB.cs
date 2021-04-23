using System.Collections.Generic;

namespace Blis.Common
{
	public class CrowdControlDB
	{
		private readonly Dictionary<StateType, List<CrowdControlData>> crowdControlDataMap =
			new Dictionary<StateType, List<CrowdControlData>>(SingletonComparerEnum<StateTypeComparer, StateType>
				.Instance);

		public CrowdControlDB()
		{
			Add(StateType.Airborne, StateType.Airborne, false, true, false);
			Add(StateType.Airborne, StateType.Slow, false, false, false);
			Add(StateType.Airborne, StateType.Fear, false, false, true);
			Add(StateType.Airborne, StateType.Taunt, false, false, true);
			Add(StateType.Airborne, StateType.Charm, false, false, true);
			Add(StateType.Airborne, StateType.Silence, false, false, false);
			Add(StateType.Airborne, StateType.BlockedSight, false, false, false);
			Add(StateType.Airborne, StateType.Fetter, false, false, false);
			Add(StateType.Airborne, StateType.Stun, false, false, false);
			Add(StateType.Airborne, StateType.Knockback, false, true, false);
			Add(StateType.Airborne, StateType.Sleep, false, true, false);
			Add(StateType.Airborne, StateType.Disarmed, false, false, false);
			Add(StateType.Airborne, StateType.Suppressed, false, true, false);
			Add(StateType.Airborne, StateType.Blind, false, false, false);
			Add(StateType.Airborne, StateType.Grounding, false, false, false);
			Add(StateType.Airborne, StateType.Dance, false, false, false);
			Add(StateType.Airborne, StateType.Grab, false, true, false);
			Add(StateType.Airborne, StateType.Polymorph, false, false, false);
			Add(StateType.Airborne, StateType.Uninteractionable, false, false, false);
			Add(StateType.Slow, StateType.Airborne, false, false, false);
			Add(StateType.Slow, StateType.Slow, false, false, false);
			Add(StateType.Slow, StateType.Fear, false, false, false);
			Add(StateType.Slow, StateType.Taunt, false, false, false);
			Add(StateType.Slow, StateType.Charm, false, false, false);
			Add(StateType.Slow, StateType.Silence, false, false, false);
			Add(StateType.Slow, StateType.BlockedSight, false, false, false);
			Add(StateType.Slow, StateType.Fetter, false, false, false);
			Add(StateType.Slow, StateType.Stun, false, false, false);
			Add(StateType.Slow, StateType.Knockback, false, false, false);
			Add(StateType.Slow, StateType.Sleep, false, false, false);
			Add(StateType.Slow, StateType.Disarmed, false, false, false);
			Add(StateType.Slow, StateType.Suppressed, false, false, false);
			Add(StateType.Slow, StateType.Blind, false, false, false);
			Add(StateType.Slow, StateType.Grounding, false, false, false);
			Add(StateType.Slow, StateType.Dance, false, false, false);
			Add(StateType.Slow, StateType.Grab, false, false, false);
			Add(StateType.Slow, StateType.Polymorph, false, false, false);
			Add(StateType.Slow, StateType.Uninteractionable, false, false, false);
			Add(StateType.Fear, StateType.Airborne, false, false, false);
			Add(StateType.Fear, StateType.Slow, false, false, false);
			Add(StateType.Fear, StateType.Fear, false, true, false);
			Add(StateType.Fear, StateType.Taunt, false, true, false);
			Add(StateType.Fear, StateType.Charm, false, true, false);
			Add(StateType.Fear, StateType.Silence, false, false, false);
			Add(StateType.Fear, StateType.BlockedSight, false, false, false);
			Add(StateType.Fear, StateType.Fetter, false, false, false);
			Add(StateType.Fear, StateType.Stun, false, false, false);
			Add(StateType.Fear, StateType.Knockback, false, false, false);
			Add(StateType.Fear, StateType.Sleep, false, false, false);
			Add(StateType.Fear, StateType.Disarmed, false, false, false);
			Add(StateType.Fear, StateType.Suppressed, false, false, false);
			Add(StateType.Fear, StateType.Blind, false, false, false);
			Add(StateType.Fear, StateType.Grounding, false, false, false);
			Add(StateType.Fear, StateType.Dance, false, false, false);
			Add(StateType.Fear, StateType.Grab, false, false, false);
			Add(StateType.Fear, StateType.Polymorph, false, false, false);
			Add(StateType.Fear, StateType.Uninteractionable, false, false, false);
			Add(StateType.Taunt, StateType.Airborne, false, false, false);
			Add(StateType.Taunt, StateType.Slow, false, false, false);
			Add(StateType.Taunt, StateType.Fear, false, true, false);
			Add(StateType.Taunt, StateType.Taunt, false, true, false);
			Add(StateType.Taunt, StateType.Charm, false, true, false);
			Add(StateType.Taunt, StateType.Silence, false, false, false);
			Add(StateType.Taunt, StateType.BlockedSight, false, false, false);
			Add(StateType.Taunt, StateType.Fetter, false, false, false);
			Add(StateType.Taunt, StateType.Stun, false, false, false);
			Add(StateType.Taunt, StateType.Knockback, false, false, false);
			Add(StateType.Taunt, StateType.Sleep, false, false, false);
			Add(StateType.Taunt, StateType.Disarmed, false, false, false);
			Add(StateType.Taunt, StateType.Suppressed, false, false, false);
			Add(StateType.Taunt, StateType.Blind, false, false, false);
			Add(StateType.Taunt, StateType.Grounding, false, false, false);
			Add(StateType.Taunt, StateType.Dance, false, false, false);
			Add(StateType.Taunt, StateType.Grab, false, false, false);
			Add(StateType.Taunt, StateType.Polymorph, false, false, false);
			Add(StateType.Taunt, StateType.Uninteractionable, false, false, false);
			Add(StateType.Charm, StateType.Airborne, false, false, false);
			Add(StateType.Charm, StateType.Slow, false, false, false);
			Add(StateType.Charm, StateType.Fear, false, true, false);
			Add(StateType.Charm, StateType.Taunt, false, true, false);
			Add(StateType.Charm, StateType.Charm, false, true, false);
			Add(StateType.Charm, StateType.Silence, false, false, false);
			Add(StateType.Charm, StateType.BlockedSight, false, false, false);
			Add(StateType.Charm, StateType.Fetter, false, false, false);
			Add(StateType.Charm, StateType.Stun, false, false, false);
			Add(StateType.Charm, StateType.Knockback, false, false, false);
			Add(StateType.Charm, StateType.Sleep, false, false, false);
			Add(StateType.Charm, StateType.Disarmed, false, false, false);
			Add(StateType.Charm, StateType.Suppressed, false, false, false);
			Add(StateType.Charm, StateType.Blind, false, false, false);
			Add(StateType.Charm, StateType.Grounding, false, false, false);
			Add(StateType.Charm, StateType.Dance, false, false, false);
			Add(StateType.Charm, StateType.Grab, false, false, false);
			Add(StateType.Charm, StateType.Polymorph, false, false, false);
			Add(StateType.Charm, StateType.Uninteractionable, false, false, false);
			Add(StateType.Silence, StateType.Airborne, false, false, false);
			Add(StateType.Silence, StateType.Slow, false, false, false);
			Add(StateType.Silence, StateType.Fear, false, false, false);
			Add(StateType.Silence, StateType.Taunt, false, false, false);
			Add(StateType.Silence, StateType.Charm, false, false, false);
			Add(StateType.Silence, StateType.Silence, false, false, false);
			Add(StateType.Silence, StateType.BlockedSight, false, false, false);
			Add(StateType.Silence, StateType.Fetter, false, false, false);
			Add(StateType.Silence, StateType.Stun, false, false, false);
			Add(StateType.Silence, StateType.Knockback, false, false, false);
			Add(StateType.Silence, StateType.Sleep, false, false, false);
			Add(StateType.Silence, StateType.Disarmed, false, false, false);
			Add(StateType.Silence, StateType.Suppressed, false, false, false);
			Add(StateType.Silence, StateType.Blind, false, false, false);
			Add(StateType.Silence, StateType.Grounding, false, false, false);
			Add(StateType.Silence, StateType.Dance, false, false, false);
			Add(StateType.Silence, StateType.Grab, false, false, false);
			Add(StateType.Silence, StateType.Polymorph, false, false, false);
			Add(StateType.Silence, StateType.Uninteractionable, false, false, false);
			Add(StateType.BlockedSight, StateType.Airborne, false, false, false);
			Add(StateType.BlockedSight, StateType.Slow, false, false, false);
			Add(StateType.BlockedSight, StateType.Fear, false, false, false);
			Add(StateType.BlockedSight, StateType.Taunt, false, false, false);
			Add(StateType.BlockedSight, StateType.Charm, false, false, false);
			Add(StateType.BlockedSight, StateType.Silence, false, false, false);
			Add(StateType.BlockedSight, StateType.BlockedSight, false, false, false);
			Add(StateType.BlockedSight, StateType.Fetter, false, false, false);
			Add(StateType.BlockedSight, StateType.Stun, false, false, false);
			Add(StateType.BlockedSight, StateType.Knockback, false, false, false);
			Add(StateType.BlockedSight, StateType.Sleep, false, false, false);
			Add(StateType.BlockedSight, StateType.Disarmed, false, false, false);
			Add(StateType.BlockedSight, StateType.Suppressed, false, false, false);
			Add(StateType.BlockedSight, StateType.Blind, false, false, false);
			Add(StateType.BlockedSight, StateType.Grounding, false, false, false);
			Add(StateType.BlockedSight, StateType.Dance, false, false, false);
			Add(StateType.BlockedSight, StateType.Grab, false, false, false);
			Add(StateType.BlockedSight, StateType.Polymorph, false, false, false);
			Add(StateType.BlockedSight, StateType.Uninteractionable, false, false, false);
			Add(StateType.Fetter, StateType.Airborne, false, false, false);
			Add(StateType.Fetter, StateType.Slow, false, false, false);
			Add(StateType.Fetter, StateType.Fear, false, false, true);
			Add(StateType.Fetter, StateType.Taunt, false, false, true);
			Add(StateType.Fetter, StateType.Charm, false, false, true);
			Add(StateType.Fetter, StateType.Silence, false, false, false);
			Add(StateType.Fetter, StateType.BlockedSight, false, false, false);
			Add(StateType.Fetter, StateType.Fetter, false, false, false);
			Add(StateType.Fetter, StateType.Stun, false, false, false);
			Add(StateType.Fetter, StateType.Knockback, false, false, false);
			Add(StateType.Fetter, StateType.Sleep, false, false, false);
			Add(StateType.Fetter, StateType.Disarmed, false, false, false);
			Add(StateType.Fetter, StateType.Suppressed, false, false, false);
			Add(StateType.Fetter, StateType.Blind, false, false, false);
			Add(StateType.Fetter, StateType.Grounding, false, false, false);
			Add(StateType.Fetter, StateType.Dance, false, false, false);
			Add(StateType.Fetter, StateType.Grab, false, false, false);
			Add(StateType.Fetter, StateType.Polymorph, false, false, false);
			Add(StateType.Fetter, StateType.Uninteractionable, false, false, false);
			Add(StateType.Stun, StateType.Airborne, false, false, false);
			Add(StateType.Stun, StateType.Slow, false, false, false);
			Add(StateType.Stun, StateType.Fear, false, false, true);
			Add(StateType.Stun, StateType.Taunt, false, false, true);
			Add(StateType.Stun, StateType.Charm, false, false, true);
			Add(StateType.Stun, StateType.Silence, false, false, false);
			Add(StateType.Stun, StateType.BlockedSight, false, false, false);
			Add(StateType.Stun, StateType.Fetter, false, false, false);
			Add(StateType.Stun, StateType.Stun, false, false, false);
			Add(StateType.Stun, StateType.Knockback, false, false, false);
			Add(StateType.Stun, StateType.Sleep, false, true, false);
			Add(StateType.Stun, StateType.Disarmed, false, false, false);
			Add(StateType.Stun, StateType.Suppressed, false, false, false);
			Add(StateType.Stun, StateType.Blind, false, false, false);
			Add(StateType.Stun, StateType.Grounding, false, false, false);
			Add(StateType.Stun, StateType.Dance, false, false, false);
			Add(StateType.Stun, StateType.Grab, false, false, false);
			Add(StateType.Stun, StateType.Polymorph, false, false, false);
			Add(StateType.Stun, StateType.Uninteractionable, false, false, false);
			Add(StateType.Knockback, StateType.Airborne, false, true, false);
			Add(StateType.Knockback, StateType.Slow, false, false, false);
			Add(StateType.Knockback, StateType.Fear, false, false, true);
			Add(StateType.Knockback, StateType.Taunt, false, false, true);
			Add(StateType.Knockback, StateType.Charm, false, false, true);
			Add(StateType.Knockback, StateType.Silence, false, false, false);
			Add(StateType.Knockback, StateType.BlockedSight, false, false, false);
			Add(StateType.Knockback, StateType.Fetter, false, false, false);
			Add(StateType.Knockback, StateType.Stun, false, false, false);
			Add(StateType.Knockback, StateType.Knockback, false, true, false);
			Add(StateType.Knockback, StateType.Sleep, false, true, false);
			Add(StateType.Knockback, StateType.Disarmed, false, false, false);
			Add(StateType.Knockback, StateType.Suppressed, false, true, false);
			Add(StateType.Knockback, StateType.Blind, false, false, false);
			Add(StateType.Knockback, StateType.Grounding, false, false, false);
			Add(StateType.Knockback, StateType.Dance, false, false, false);
			Add(StateType.Knockback, StateType.Grab, false, true, false);
			Add(StateType.Knockback, StateType.Polymorph, false, false, false);
			Add(StateType.Knockback, StateType.Uninteractionable, false, false, false);
			Add(StateType.Sleep, StateType.Airborne, false, false, false);
			Add(StateType.Sleep, StateType.Slow, false, false, false);
			Add(StateType.Sleep, StateType.Fear, false, true, false);
			Add(StateType.Sleep, StateType.Taunt, false, true, false);
			Add(StateType.Sleep, StateType.Charm, false, true, false);
			Add(StateType.Sleep, StateType.Silence, false, false, false);
			Add(StateType.Sleep, StateType.BlockedSight, false, false, false);
			Add(StateType.Sleep, StateType.Fetter, false, false, false);
			Add(StateType.Sleep, StateType.Stun, true, false, false);
			Add(StateType.Sleep, StateType.Knockback, false, false, false);
			Add(StateType.Sleep, StateType.Sleep, false, false, false);
			Add(StateType.Sleep, StateType.Disarmed, false, false, false);
			Add(StateType.Sleep, StateType.Suppressed, false, false, false);
			Add(StateType.Sleep, StateType.Blind, false, false, false);
			Add(StateType.Sleep, StateType.Grounding, false, false, false);
			Add(StateType.Sleep, StateType.Dance, false, false, false);
			Add(StateType.Sleep, StateType.Grab, false, false, false);
			Add(StateType.Sleep, StateType.Polymorph, false, false, false);
			Add(StateType.Sleep, StateType.Uninteractionable, false, false, false);
			Add(StateType.Disarmed, StateType.Airborne, false, false, false);
			Add(StateType.Disarmed, StateType.Slow, false, false, false);
			Add(StateType.Disarmed, StateType.Fear, false, false, false);
			Add(StateType.Disarmed, StateType.Taunt, false, false, false);
			Add(StateType.Disarmed, StateType.Charm, false, false, false);
			Add(StateType.Disarmed, StateType.Silence, false, false, false);
			Add(StateType.Disarmed, StateType.BlockedSight, false, false, false);
			Add(StateType.Disarmed, StateType.Fetter, false, false, false);
			Add(StateType.Disarmed, StateType.Stun, false, false, false);
			Add(StateType.Disarmed, StateType.Knockback, false, false, false);
			Add(StateType.Disarmed, StateType.Sleep, false, false, false);
			Add(StateType.Disarmed, StateType.Disarmed, false, false, false);
			Add(StateType.Disarmed, StateType.Suppressed, false, false, false);
			Add(StateType.Disarmed, StateType.Blind, false, false, false);
			Add(StateType.Disarmed, StateType.Grounding, false, false, false);
			Add(StateType.Disarmed, StateType.Dance, false, false, false);
			Add(StateType.Disarmed, StateType.Grab, false, false, false);
			Add(StateType.Disarmed, StateType.Polymorph, false, false, false);
			Add(StateType.Disarmed, StateType.Uninteractionable, false, false, false);
			Add(StateType.Suppressed, StateType.Airborne, false, true, false);
			Add(StateType.Suppressed, StateType.Slow, false, false, false);
			Add(StateType.Suppressed, StateType.Fear, false, false, true);
			Add(StateType.Suppressed, StateType.Taunt, false, false, true);
			Add(StateType.Suppressed, StateType.Charm, false, false, true);
			Add(StateType.Suppressed, StateType.Silence, false, false, false);
			Add(StateType.Suppressed, StateType.BlockedSight, false, false, false);
			Add(StateType.Suppressed, StateType.Fetter, false, false, false);
			Add(StateType.Suppressed, StateType.Stun, false, true, false);
			Add(StateType.Suppressed, StateType.Knockback, false, true, false);
			Add(StateType.Suppressed, StateType.Sleep, false, true, false);
			Add(StateType.Suppressed, StateType.Disarmed, false, false, false);
			Add(StateType.Suppressed, StateType.Suppressed, false, true, false);
			Add(StateType.Suppressed, StateType.Blind, false, false, false);
			Add(StateType.Suppressed, StateType.Grounding, false, false, false);
			Add(StateType.Suppressed, StateType.Dance, false, false, false);
			Add(StateType.Suppressed, StateType.Grab, false, false, false);
			Add(StateType.Suppressed, StateType.Polymorph, false, false, false);
			Add(StateType.Suppressed, StateType.Uninteractionable, false, false, false);
			Add(StateType.Blind, StateType.Airborne, false, false, false);
			Add(StateType.Blind, StateType.Slow, false, false, false);
			Add(StateType.Blind, StateType.Fear, false, false, false);
			Add(StateType.Blind, StateType.Taunt, false, false, false);
			Add(StateType.Blind, StateType.Charm, false, false, false);
			Add(StateType.Blind, StateType.Silence, false, false, false);
			Add(StateType.Blind, StateType.BlockedSight, false, false, false);
			Add(StateType.Blind, StateType.Fetter, false, false, false);
			Add(StateType.Blind, StateType.Stun, false, false, false);
			Add(StateType.Blind, StateType.Knockback, false, false, false);
			Add(StateType.Blind, StateType.Sleep, false, false, false);
			Add(StateType.Blind, StateType.Disarmed, false, false, false);
			Add(StateType.Blind, StateType.Suppressed, false, false, false);
			Add(StateType.Blind, StateType.Blind, false, false, false);
			Add(StateType.Blind, StateType.Grounding, false, false, false);
			Add(StateType.Blind, StateType.Dance, false, false, false);
			Add(StateType.Blind, StateType.Grab, false, false, false);
			Add(StateType.Blind, StateType.Polymorph, false, false, false);
			Add(StateType.Blind, StateType.Uninteractionable, false, false, false);
			Add(StateType.Grounding, StateType.Airborne, false, false, false);
			Add(StateType.Grounding, StateType.Slow, false, false, false);
			Add(StateType.Grounding, StateType.Fear, false, false, false);
			Add(StateType.Grounding, StateType.Taunt, false, false, false);
			Add(StateType.Grounding, StateType.Charm, false, false, false);
			Add(StateType.Grounding, StateType.Silence, false, false, false);
			Add(StateType.Grounding, StateType.BlockedSight, false, false, false);
			Add(StateType.Grounding, StateType.Fetter, false, false, false);
			Add(StateType.Grounding, StateType.Stun, false, false, false);
			Add(StateType.Grounding, StateType.Knockback, false, false, false);
			Add(StateType.Grounding, StateType.Sleep, false, false, false);
			Add(StateType.Grounding, StateType.Disarmed, false, false, false);
			Add(StateType.Grounding, StateType.Suppressed, false, false, false);
			Add(StateType.Grounding, StateType.Blind, false, false, false);
			Add(StateType.Grounding, StateType.Grounding, false, false, false);
			Add(StateType.Grounding, StateType.Dance, false, false, false);
			Add(StateType.Grounding, StateType.Grab, false, false, false);
			Add(StateType.Grounding, StateType.Polymorph, false, false, false);
			Add(StateType.Grounding, StateType.Uninteractionable, false, false, false);
			Add(StateType.Dance, StateType.Airborne, false, false, false);
			Add(StateType.Dance, StateType.Slow, false, false, false);
			Add(StateType.Dance, StateType.Fear, false, false, false);
			Add(StateType.Dance, StateType.Taunt, false, false, false);
			Add(StateType.Dance, StateType.Charm, false, false, false);
			Add(StateType.Dance, StateType.Silence, false, false, false);
			Add(StateType.Dance, StateType.BlockedSight, false, false, false);
			Add(StateType.Dance, StateType.Fetter, false, false, false);
			Add(StateType.Dance, StateType.Stun, false, false, false);
			Add(StateType.Dance, StateType.Knockback, false, false, false);
			Add(StateType.Dance, StateType.Sleep, false, false, false);
			Add(StateType.Dance, StateType.Disarmed, false, false, false);
			Add(StateType.Dance, StateType.Suppressed, false, false, false);
			Add(StateType.Dance, StateType.Blind, false, false, false);
			Add(StateType.Dance, StateType.Grounding, false, false, false);
			Add(StateType.Dance, StateType.Dance, false, true, false);
			Add(StateType.Dance, StateType.Grab, false, false, false);
			Add(StateType.Dance, StateType.Polymorph, false, false, false);
			Add(StateType.Dance, StateType.Uninteractionable, false, false, false);
			Add(StateType.Polymorph, StateType.Airborne, false, false, false);
			Add(StateType.Polymorph, StateType.Slow, false, false, false);
			Add(StateType.Polymorph, StateType.Fear, false, false, false);
			Add(StateType.Polymorph, StateType.Taunt, false, false, false);
			Add(StateType.Polymorph, StateType.Charm, false, false, false);
			Add(StateType.Polymorph, StateType.Silence, false, false, false);
			Add(StateType.Polymorph, StateType.BlockedSight, false, false, false);
			Add(StateType.Polymorph, StateType.Fetter, false, false, false);
			Add(StateType.Polymorph, StateType.Stun, false, false, false);
			Add(StateType.Polymorph, StateType.Knockback, false, false, false);
			Add(StateType.Polymorph, StateType.Sleep, false, false, false);
			Add(StateType.Polymorph, StateType.Disarmed, false, false, false);
			Add(StateType.Polymorph, StateType.Suppressed, false, false, false);
			Add(StateType.Polymorph, StateType.Blind, false, false, false);
			Add(StateType.Polymorph, StateType.Grounding, false, false, false);
			Add(StateType.Polymorph, StateType.Dance, false, false, false);
			Add(StateType.Polymorph, StateType.Grab, false, false, false);
			Add(StateType.Polymorph, StateType.Polymorph, false, true, false);
			Add(StateType.Polymorph, StateType.Uninteractionable, false, false, false);
			Add(StateType.Grab, StateType.Airborne, false, true, false);
			Add(StateType.Grab, StateType.Slow, false, false, false);
			Add(StateType.Grab, StateType.Fear, false, false, true);
			Add(StateType.Grab, StateType.Taunt, false, false, true);
			Add(StateType.Grab, StateType.Charm, false, false, true);
			Add(StateType.Grab, StateType.Silence, false, false, false);
			Add(StateType.Grab, StateType.BlockedSight, false, false, false);
			Add(StateType.Grab, StateType.Fetter, false, false, false);
			Add(StateType.Grab, StateType.Stun, false, false, false);
			Add(StateType.Grab, StateType.Knockback, false, true, false);
			Add(StateType.Grab, StateType.Sleep, false, true, false);
			Add(StateType.Grab, StateType.Disarmed, false, false, false);
			Add(StateType.Grab, StateType.Suppressed, false, true, false);
			Add(StateType.Grab, StateType.Blind, false, false, false);
			Add(StateType.Grab, StateType.Grounding, false, false, false);
			Add(StateType.Grab, StateType.Dance, false, false, false);
			Add(StateType.Grab, StateType.Grab, false, false, false);
			Add(StateType.Grab, StateType.Polymorph, false, false, false);
			Add(StateType.Grab, StateType.Uninteractionable, false, false, false);
			Add(StateType.Uninteractionable, StateType.Airborne, false, false, false);
			Add(StateType.Uninteractionable, StateType.Slow, false, false, false);
			Add(StateType.Uninteractionable, StateType.Fear, false, false, false);
			Add(StateType.Uninteractionable, StateType.Taunt, false, false, false);
			Add(StateType.Uninteractionable, StateType.Charm, false, false, false);
			Add(StateType.Uninteractionable, StateType.Silence, false, false, false);
			Add(StateType.Uninteractionable, StateType.BlockedSight, false, false, false);
			Add(StateType.Uninteractionable, StateType.Fetter, false, false, false);
			Add(StateType.Uninteractionable, StateType.Stun, false, false, false);
			Add(StateType.Uninteractionable, StateType.Knockback, false, false, false);
			Add(StateType.Uninteractionable, StateType.Sleep, false, false, false);
			Add(StateType.Uninteractionable, StateType.Disarmed, false, false, false);
			Add(StateType.Uninteractionable, StateType.Suppressed, false, false, false);
			Add(StateType.Uninteractionable, StateType.Blind, false, false, false);
			Add(StateType.Uninteractionable, StateType.Grounding, false, false, false);
			Add(StateType.Uninteractionable, StateType.Dance, false, false, false);
			Add(StateType.Uninteractionable, StateType.Grab, false, false, false);
			Add(StateType.Uninteractionable, StateType.Polymorph, false, false, false);
			Add(StateType.Uninteractionable, StateType.Uninteractionable, false, false, false);
		}


		private void Add(StateType newType, StateType playingType, bool blockedByPlayingState, bool cancelState,
			bool pauseState)
		{
			if (!crowdControlDataMap.ContainsKey(newType))
			{
				crowdControlDataMap.Add(newType, new List<CrowdControlData>());
			}

			crowdControlDataMap[newType]
				.Add(new CrowdControlData(newType, playingType, blockedByPlayingState, cancelState, pauseState));
		}


		public bool IsBlockedByPlayingState(StateType newType, StateType playingType)
		{
			if (!newType.IsCrowdControl())
			{
				return false;
			}

			CrowdControlData crowdControlData =
				crowdControlDataMap[newType].Find(x => x.playingStateType == playingType);
			return crowdControlData != null && crowdControlData.blockedByPlayingState;
		}


		public bool IsCancelPlayingSkill(StateType newType, StateType playingType)
		{
			if (!newType.IsCrowdControl())
			{
				return false;
			}

			CrowdControlData crowdControlData =
				crowdControlDataMap[newType].Find(x => x.playingStateType == playingType);
			return crowdControlData != null && crowdControlData.cancelPlayingSkill;
		}


		public bool IsPausePlayingSkill(StateType newType, StateType playingType)
		{
			if (!newType.IsCrowdControl())
			{
				return false;
			}

			CrowdControlData crowdControlData =
				crowdControlDataMap[newType].Find(x => x.playingStateType == playingType);
			return crowdControlData != null && crowdControlData.pausePlayingSkill;
		}
	}
}