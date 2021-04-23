using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class ColorTransitions : TransitionStateCollection<Color>
	{
		[SerializeField] private Graphic target = default;


		[Range(1f, 5f)] [SerializeField] private float colorMultiplier = 1f;


		[SerializeField] private float fadeDuration = 0.1f;


		[SerializeField] private List<ColorTransitionState> states = new List<ColorTransitionState>();


		public ColorTransitions(params string[] stateNames) : base(stateNames) { }


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

			if (colorMultiplier <= 1E-45f)
			{
				colorMultiplier = 1f;
			}

			target.CrossFadeColor(state.StateObject * colorMultiplier, instant ? 0f : fadeDuration, true, true);
		}


		internal override void AddStateObject(string stateName)
		{
			ColorTransitionState item = new ColorTransitionState(stateName, Color.white);
			states.Add(item);
		}


		protected override IEnumerable<TransitionState> GetTransitionStates()
		{
			foreach (ColorTransitionState colorTransitionState in states)
			{
				yield return colorTransitionState;
			}
		}


		internal override void SortStates(string[] sortedOrder)
		{
			SortStatesLogic<ColorTransitionState>(states, sortedOrder);
		}


		[Serializable]
		public class ColorTransitionState : TransitionState
		{
			public ColorTransitionState(string name, Color stateObject) : base(name, stateObject) { }
		}
	}
}