using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class Color32Transitions : TransitionStateCollection<Color32>
	{
		[SerializeField] private Graphic target = default;


		[SerializeField] private float fadeDuration = 0.1f;


		[SerializeField] private List<Color32TransitionState> states = new List<Color32TransitionState>();


		public Color32Transitions(params string[] stateNames) : base(stateNames) { }


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

			target.CrossFadeColor(state.StateObject, instant ? 0f : fadeDuration, true, true);
		}


		internal override void AddStateObject(string stateName)
		{
			Color32TransitionState item = new Color32TransitionState(stateName,
				new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue));
			states.Add(item);
		}


		protected override IEnumerable<TransitionState> GetTransitionStates()
		{
			foreach (Color32TransitionState color32TransitionState in states)
			{
				yield return color32TransitionState;
			}
		}


		internal override void SortStates(string[] sortedOrder)
		{
			SortStatesLogic<Color32TransitionState>(states, sortedOrder);
		}


		[Serializable]
		public class Color32TransitionState : TransitionState
		{
			public Color32TransitionState(string name, Color32 stateObject) : base(name, stateObject) { }
		}
	}
}