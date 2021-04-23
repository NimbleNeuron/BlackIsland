using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class SpriteSwapTransitions : TransitionStateCollection<Sprite>
	{
		[SerializeField] private Image target = default;


		[SerializeField] private List<SpriteSwapTransitionState> states = new List<SpriteSwapTransitionState>();


		public SpriteSwapTransitions(params string[] stateNames) : base(stateNames) { }


		public override Object Target => target;


		protected override void ApplyState(TransitionState state, bool instant)
		{
			if (Target == null)
			{
				return;
			}

			target.overrideSprite = state.StateObject;
		}


		internal override void AddStateObject(string stateName)
		{
			SpriteSwapTransitionState item = new SpriteSwapTransitionState(stateName, null);
			states.Add(item);
		}


		protected override IEnumerable<TransitionState> GetTransitionStates()
		{
			foreach (SpriteSwapTransitionState spriteSwapTransitionState in states)
			{
				yield return spriteSwapTransitionState;
			}
		}


		internal override void SortStates(string[] sortedOrder)
		{
			SortStatesLogic<SpriteSwapTransitionState>(states, sortedOrder);
		}


		[Serializable]
		public class SpriteSwapTransitionState : TransitionState
		{
			public SpriteSwapTransitionState(string name, Sprite stateObject) : base(name, stateObject) { }
		}
	}
}