using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class AlphaTransitions : TransitionStateCollection<float>
	{
		[SerializeField] private Graphic target = default;


		[SerializeField] private float fadeDuration = 0.1f;


		[SerializeField] private List<AlphaTransitionState> states = new List<AlphaTransitionState>();


		public AlphaTransitions(params string[] stateNames) : base(stateNames) { }


		public override Object Target => target;


		
		public float FadeDurtaion {
			get => fadeDuration;
			set => fadeDuration = value;
		}


		protected override void ApplyState(TransitionState state, bool instant)
		{
			if (Target == null)
			{
				return;
			}

			if (!Application.isPlaying)
			{
				instant = true;
			}

			target.CrossFadeAlpha(state.StateObject, instant ? 0f : fadeDuration, true);
		}


		internal override void AddStateObject(string stateName)
		{
			AlphaTransitionState item = new AlphaTransitionState(stateName, 1f);
			states.Add(item);
		}


		protected override IEnumerable<TransitionState> GetTransitionStates()
		{
			foreach (AlphaTransitionState alphaTransitionState in states)
			{
				yield return alphaTransitionState;
			}
		}


		internal override void SortStates(string[] sortedOrder)
		{
			SortStatesLogic<AlphaTransitionState>(states, sortedOrder);
		}


		[Serializable]
		public class AlphaTransitionState : TransitionState
		{
			public AlphaTransitionState(string name, float stateObject) : base(name, stateObject) { }
		}
	}
}