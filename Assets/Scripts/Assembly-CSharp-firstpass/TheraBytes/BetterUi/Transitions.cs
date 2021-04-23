using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class Transitions
	{
		public enum TransitionMode
		{
			None,


			ColorTint,


			SpriteSwap,


			Animation,


			ObjectActiveness,


			Alpha,


			MaterialProperty,


			Color32Tint,


			LocationAnimationTransition
		}


		public static readonly string[] OnOffStateNames =
		{
			"On",
			"Off"
		};


		public static readonly string[] ShowHideStateNames =
		{
			"Show",
			"Hide"
		};


		public static readonly string[] SelectionStateNames =
		{
			"Normal",
			"Highlighted",
			"Pressed",
			"Selected",
			"Disabled"
		};


		[SerializeField] private TransitionMode mode;


		[SerializeField] private string[] stateNames;


		[SerializeField] private ColorTransitions colorTransitions;


		[SerializeField] private Color32Transitions color32Transitions;


		[SerializeField] private SpriteSwapTransitions spriteSwapTransitions;


		[SerializeField] private AnimationTransitions animationTransitions;


		[SerializeField] private ObjectActivenessTransitions activenessTransitions;


		[SerializeField] private AlphaTransitions alphaTransitions;


		[SerializeField] private MaterialPropertyTransition materialPropertyTransitions;


		[SerializeField] private LocationAnimationTransitions locationAnimationTransitions;


		public Transitions(params string[] stateNames)
		{
			this.stateNames = stateNames;
		}


		public TransitionMode Mode => mode;


		public ReadOnlyCollection<string> StateNames => stateNames.ToList<string>().AsReadOnly();


		public TransitionStateCollection TransitionStates {
			get
			{
				switch (mode)
				{
					case TransitionMode.ColorTint:
						return colorTransitions;
					case TransitionMode.SpriteSwap:
						return spriteSwapTransitions;
					case TransitionMode.Animation:
						return animationTransitions;
					case TransitionMode.ObjectActiveness:
						return activenessTransitions;
					case TransitionMode.Alpha:
						return alphaTransitions;
					case TransitionMode.MaterialProperty:
						return materialPropertyTransitions;
					case TransitionMode.Color32Tint:
						return color32Transitions;
					case TransitionMode.LocationAnimationTransition:
						return locationAnimationTransitions;
					default:
						return null;
				}
			}
		}


		public void SetState(string stateName, bool instant)
		{
			if (TransitionStates == null)
			{
				return;
			}

			if (!stateNames.Contains(stateName))
			{
				return;
			}

			TransitionStates.Apply(stateName, instant);
		}


		public void SetMode(TransitionMode mode)
		{
			this.mode = mode;
			colorTransitions = null;
			color32Transitions = null;
			spriteSwapTransitions = null;
			animationTransitions = null;
			activenessTransitions = null;
			alphaTransitions = null;
			locationAnimationTransitions = null;
			switch (mode)
			{
				case TransitionMode.None:
					break;
				case TransitionMode.ColorTint:
					colorTransitions = new ColorTransitions(stateNames);
					return;
				case TransitionMode.SpriteSwap:
					spriteSwapTransitions = new SpriteSwapTransitions(stateNames);
					return;
				case TransitionMode.Animation:
					animationTransitions = new AnimationTransitions(stateNames);
					return;
				case TransitionMode.ObjectActiveness:
					activenessTransitions = new ObjectActivenessTransitions(stateNames);
					return;
				case TransitionMode.Alpha:
					alphaTransitions = new AlphaTransitions(stateNames);
					return;
				case TransitionMode.MaterialProperty:
					materialPropertyTransitions = new MaterialPropertyTransition(stateNames);
					return;
				case TransitionMode.Color32Tint:
					color32Transitions = new Color32Transitions(stateNames);
					return;
				case TransitionMode.LocationAnimationTransition:
					locationAnimationTransitions = new LocationAnimationTransitions(stateNames);
					break;
				default:
					return;
			}
		}


		public void ComplementStateNames(string[] stateNames)
		{
			foreach (string text in stateNames)
			{
				if (!this.stateNames.Contains(text))
				{
					switch (mode)
					{
						case TransitionMode.ColorTint:
							colorTransitions.AddStateObject(text);
							colorTransitions.SortStates(stateNames);
							break;
						case TransitionMode.SpriteSwap:
							spriteSwapTransitions.AddStateObject(text);
							spriteSwapTransitions.SortStates(stateNames);
							break;
						case TransitionMode.Animation:
							animationTransitions.AddStateObject(text);
							animationTransitions.SortStates(stateNames);
							break;
						case TransitionMode.ObjectActiveness:
							activenessTransitions.AddStateObject(text);
							activenessTransitions.SortStates(stateNames);
							break;
						case TransitionMode.Alpha:
							alphaTransitions.AddStateObject(text);
							alphaTransitions.SortStates(stateNames);
							break;
						case TransitionMode.MaterialProperty:
							materialPropertyTransitions.AddStateObject(text);
							materialPropertyTransitions.SortStates(stateNames);
							break;
						case TransitionMode.Color32Tint:
							color32Transitions.AddStateObject(text);
							color32Transitions.SortStates(stateNames);
							break;
						case TransitionMode.LocationAnimationTransition:
							locationAnimationTransitions.AddStateObject(text);
							locationAnimationTransitions.SortStates(stateNames);
							break;
					}
				}
			}

			this.stateNames = stateNames;
		}
	}
}