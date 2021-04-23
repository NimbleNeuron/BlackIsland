using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class ObjectActivenessTransitions : TransitionStateCollection<bool>
	{
		[SerializeField] private GameObject target = default;


		[SerializeField] private List<ActiveTransitionState> states = new List<ActiveTransitionState>();


		public ObjectActivenessTransitions(params string[] stateNames) : base(stateNames) { }


		public override Object Target => target;


		protected override void ApplyState(TransitionState state, bool instant)
		{
			if (Target == null)
			{
				return;
			}

			if (Application.isPlaying)
			{
				target.SetActive(state.StateObject);
			}
		}


		internal override void AddStateObject(string stateName)
		{
			ActiveTransitionState item = new ActiveTransitionState(stateName, true);
			states.Add(item);
		}


		protected override IEnumerable<TransitionState> GetTransitionStates()
		{
			foreach (ActiveTransitionState activeTransitionState in states)
			{
				yield return activeTransitionState;
			}
		}


		internal override void SortStates(string[] sortedOrder)
		{
			SortStatesLogic<ActiveTransitionState>(states, sortedOrder);
		}


		[Serializable]
		public class ActiveTransitionState : TransitionState
		{
			public ActiveTransitionState(string name, bool stateObject) : base(name, stateObject) { }
		}
	}
}