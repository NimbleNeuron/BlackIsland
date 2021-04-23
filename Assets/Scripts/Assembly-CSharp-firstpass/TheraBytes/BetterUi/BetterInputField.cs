using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TheraBytes.BetterUi
{
	[AddComponentMenu("Better UI/Controls/Better Input Field", 30)]
	public class BetterInputField : InputField, IBetterTransitionUiElement
	{
		[SerializeField] private List<Transitions> betterTransitions = new List<Transitions>();


		[SerializeField] private List<Graphic> additionalPlaceholders = new List<Graphic>();


		public List<Graphic> AdditionalPlaceholders => additionalPlaceholders;


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
				transitions.SetState(state.ToString(), instant);
			}
		}


		public override void OnUpdateSelected(BaseEventData eventData)
		{
			base.OnUpdateSelected(eventData);
			DisplayPlaceholders(text);
		}


		private void DisplayPlaceholders(string input)
		{
			bool enabled = string.IsNullOrEmpty(input);
			if (Application.isPlaying)
			{
				foreach (Graphic graphic in additionalPlaceholders)
				{
					graphic.enabled = enabled;
				}
			}
		}
	}
}