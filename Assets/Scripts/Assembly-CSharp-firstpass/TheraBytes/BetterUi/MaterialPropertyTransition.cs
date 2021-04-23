using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TheraBytes.BetterUi
{
	[Serializable]
	public class MaterialPropertyTransition : TransitionStateCollection<float>
	{
		private static Dictionary<MaterialPropertyTransition, Coroutine> activeCoroutines =
			new Dictionary<MaterialPropertyTransition, Coroutine>();


		private static List<MaterialPropertyTransition> keysToRemove = new List<MaterialPropertyTransition>();


		[SerializeField] private BetterImage target = default;


		[SerializeField] private float fadeDuration = 0.1f;


		[SerializeField]
		private List<MaterialPropertyTransitionState> states = new List<MaterialPropertyTransitionState>();


		[SerializeField] private int propertyIndex = default;


		public MaterialPropertyTransition(params string[] stateNames) : base(stateNames) { }


		public override Object Target => target;


		
		public float FadeDurtaion {
			get => fadeDuration;
			set => fadeDuration = value;
		}


		
		public int PropertyIndex {
			get => propertyIndex;
			set => propertyIndex = value;
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

			float materialPropertyValue = target.GetMaterialPropertyValue(propertyIndex);
			CrossFadeProperty(materialPropertyValue, state.StateObject, instant ? 0f : fadeDuration);
		}


		internal override void AddStateObject(string stateName)
		{
			MaterialPropertyTransitionState item = new MaterialPropertyTransitionState(stateName, 1f);
			states.Add(item);
		}


		protected override IEnumerable<TransitionState> GetTransitionStates()
		{
			foreach (MaterialPropertyTransitionState materialPropertyTransitionState in states)
			{
				yield return materialPropertyTransitionState;
			}
		}


		private void CrossFadeProperty(float startValue, float targetValue, float duration)
		{
			foreach (MaterialPropertyTransition materialPropertyTransition in activeCoroutines.Keys)
			{
				if (materialPropertyTransition.target == target &&
				    materialPropertyTransition.propertyIndex == propertyIndex)
				{
					if (materialPropertyTransition.target != null)
					{
						materialPropertyTransition.target.StopCoroutine(activeCoroutines[materialPropertyTransition]);
					}

					keysToRemove.Add(materialPropertyTransition);
				}
			}

			foreach (MaterialPropertyTransition key in keysToRemove)
			{
				activeCoroutines.Remove(key);
			}

			keysToRemove.Clear();
			if (duration == 0f)
			{
				target.SetMaterialProperty(propertyIndex, targetValue);
				return;
			}

			Coroutine value = target.StartCoroutine(CoCrossFadeProperty(startValue, targetValue, duration));
			activeCoroutines.Add(this, value);
		}


		private IEnumerator CoCrossFadeProperty(float startValue, float targetValue, float duration)
		{
			float startTime = Time.unscaledTime;
			float endTime = startTime + duration;
			while (Time.unscaledTime < endTime)
			{
				float t = (Time.unscaledTime - startTime) / duration;
				float value = Mathf.Lerp(startValue, targetValue, t);
				target.SetMaterialProperty(propertyIndex, value);
				yield return null;
			}

			target.SetMaterialProperty(propertyIndex, targetValue);
		}


		internal override void SortStates(string[] sortedOrder)
		{
			SortStatesLogic<MaterialPropertyTransitionState>(states, sortedOrder);
		}


		[Serializable]
		public class MaterialPropertyTransitionState : TransitionState
		{
			public MaterialPropertyTransitionState(string name, float stateObject) : base(name, stateObject) { }
		}
	}
}