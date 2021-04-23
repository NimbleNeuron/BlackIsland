using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class AnimationTransitions : TransitionStateCollection<string>
	{
		[SerializeField] private Animator target = default;


		[SerializeField] private List<AnimationTransitionState> states = new List<AnimationTransitionState>();


		public AnimationTransitions(params string[] stateNames) : base(stateNames) { }


		public override Object Target => target;


		protected override void ApplyState(TransitionState state, bool instant)
		{
			if (Target == null || !target.isActiveAndEnabled || target.runtimeAnimatorController == null ||
			    string.IsNullOrEmpty(state.StateObject))
			{
				return;
			}

			foreach (AnimationTransitionState animationTransitionState in states)
			{
				target.ResetTrigger(animationTransitionState.StateObject);
			}

			target.SetTrigger(state.StateObject);
		}


		internal override void AddStateObject(string stateName)
		{
			AnimationTransitionState item = new AnimationTransitionState(stateName, null);
			states.Add(item);
		}


		protected override IEnumerable<TransitionState> GetTransitionStates()
		{
			foreach (AnimationTransitionState animationTransitionState in states)
			{
				yield return animationTransitionState;
			}
		}


		internal override void SortStates(string[] sortedOrder)
		{
			SortStatesLogic<AnimationTransitionState>(states, sortedOrder);
		}


		[Serializable]
		public class AnimationTransitionState : TransitionState
		{
			public AnimationTransitionState(string name, string stateObject) : base(name, stateObject) { }
		}
	}
}