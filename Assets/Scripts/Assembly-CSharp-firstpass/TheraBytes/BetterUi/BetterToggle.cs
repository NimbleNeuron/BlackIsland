using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Controls/Better Toggle", 30)]
	public class BetterToggle : Toggle, IBetterTransitionUiElement
	{
		[SerializeField] private List<Transitions> betterTransitions = new List<Transitions>();


		[SerializeField] private List<Transitions> betterToggleTransitions = new List<Transitions>();


		public List<Transitions> BetterToggleTransitions => betterToggleTransitions;


		protected override void OnEnable()
		{
			onValueChanged.AddListener(ValueChanged);
			base.OnEnable();
			ValueChanged(isOn, true);
			DoStateTransition(SelectionState.Normal, true);
		}


		protected override void OnDisable()
		{
			onValueChanged.RemoveListener(ValueChanged);
			base.OnDisable();
		}


		public List<Transitions> BetterTransitions => betterTransitions;


		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			base.DoStateTransition(state, instant);
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			using (List<Transitions>.Enumerator enumerator = betterTransitions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Transitions info = enumerator.Current;
					if (state == SelectionState.Disabled || !isOn || betterToggleTransitions.FirstOrDefault(o =>
						o.TransitionStates != null && info.TransitionStates != null &&
						o.TransitionStates.Target == info.TransitionStates.Target && o.Mode == info.Mode) == null)
					{
						info.SetState(state.ToString(), instant);
					}
				}
			}
		}


		private void ValueChanged(bool on)
		{
			ValueChanged(on, false);
		}


		private void ValueChanged(bool on, bool immediate)
		{
			foreach (Transitions transitions in betterToggleTransitions)
			{
				transitions.SetState(on ? "On" : "Off", immediate);
			}
		}


		[Serializable]
		public class ToggleGraphics
		{
			public ToggleTransition ToggleTransition = ToggleTransition.Fade;


			public Graphic Graphic;


			public float FadeDuration = 0.1f;
		}
	}
}