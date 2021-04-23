using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraBytes.BetterUi
{
	public abstract class TransitionStateCollection<T> : TransitionStateCollection
	{
		protected TransitionStateCollection(string[] stateNames)
		{
			foreach (string stateName in stateNames)
			{
				AddStateObject(stateName);
			}
		}


		public IEnumerable<TransitionState> GetStates()
		{
			foreach (TransitionState transitionState in GetTransitionStates())
			{
				yield return transitionState;
			}
		}


		public override void Apply(string stateName, bool instant)
		{
			TransitionState transitionState = GetTransitionStates().FirstOrDefault(o => o.Name == stateName);
			if (transitionState != null)
			{
				ApplyState(transitionState, instant);
			}
		}


		protected abstract IEnumerable<TransitionState> GetTransitionStates();


		protected abstract void ApplyState(TransitionState state, bool instant);


		internal abstract void AddStateObject(string stateName);


		[Serializable]
		public abstract class TransitionState : TransitionStateBase
		{
			public T StateObject;


			public TransitionState(string name, T stateObject) : base(name)
			{
				StateObject = stateObject;
			}
		}
	}
}