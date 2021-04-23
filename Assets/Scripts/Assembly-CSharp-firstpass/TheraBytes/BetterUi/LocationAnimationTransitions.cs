using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class LocationAnimationTransitions : TransitionStateCollection<string>
	{
		[SerializeField] private LocationAnimations target = default;


		[SerializeField]
		private List<LocationAnimationTransitionState> states = new List<LocationAnimationTransitionState>();


		public LocationAnimationTransitions(params string[] stateNames) : base(stateNames) { }


		public override Object Target => target;


		protected override void ApplyState(TransitionState state, bool instant)
		{
			if (Target == null)
			{
				return;
			}

			target.StartAnimation(state.StateObject);
		}


		internal override void AddStateObject(string stateName)
		{
			LocationAnimationTransitionState item = new LocationAnimationTransitionState(stateName, "");
			states.Add(item);
		}


		protected override IEnumerable<TransitionState> GetTransitionStates()
		{
			foreach (LocationAnimationTransitionState locationAnimationTransitionState in states)
			{
				yield return locationAnimationTransitionState;
			}
		}


		internal override void SortStates(string[] sortedOrder)
		{
			SortStatesLogic<LocationAnimationTransitionState>(states, sortedOrder);
		}


		[Serializable]
		public class LocationAnimationTransitionState : TransitionState
		{
			public LocationAnimationTransitionState(string name, string stateObject) : base(name, stateObject) { }
		}
	}
}