using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Controls/Better Dropdown", 30)]
	public class BetterDropdown : Dropdown, IBetterTransitionUiElement
	{
		[SerializeField] private List<Transitions> betterTransitions = new List<Transitions>();


		public List<Transitions> BetterTransitions => betterTransitions;


		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			foreach (Transitions transitions in betterTransitions)
			{
				transitions.SetState(state.ToString(), true);
			}
		}
	}
}