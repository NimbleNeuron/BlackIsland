using System;
using UnityEngine;

namespace ProceduralWorlds.RealIvy
{
	public class IvyController : MonoBehaviour
	{
		public enum State
		{
			GROWTH_NOT_STARTED,


			WAITING_FOR_DELAY,


			PAUSED,


			GROWING,


			GROWTH_FINISHED
		}


		public RTIvy rtIvy;


		public IvyContainer ivyContainer;


		public IvyParameters ivyParameters;


		public RuntimeGrowthParameters growthParameters;


		private float currentTimer;


		private State state;


		private void Awake()
		{
			rtIvy.AwakeInit();
			state = State.GROWTH_NOT_STARTED;
			if (growthParameters.startGrowthOnAwake)
			{
				StartGrowth();
			}
		}


		private void Update()
		{
			float deltaTime = Time.deltaTime;
			State state = this.state;
			if (state == State.WAITING_FOR_DELAY)
			{
				UpdateWaitingForDelayState(deltaTime);
				return;
			}

			if (state != State.GROWING)
			{
				return;
			}

			UpdateGrowingState(deltaTime);
		}


		
		
		public event Action OnGrowthStarted;


		
		
		public event Action OnGrowthPaused;


		
		
		public event Action OnGrowthFinished;


		[ContextMenu("Start Growth")]
		public void StartGrowth()
		{
			if (state == State.GROWTH_NOT_STARTED)
			{
				rtIvy.InitIvy(growthParameters, ivyContainer, ivyParameters);
				if (growthParameters.delay > 0f)
				{
					state = State.WAITING_FOR_DELAY;
				}
				else
				{
					state = State.GROWING;
				}

				if (OnGrowthStarted != null)
				{
					OnGrowthStarted();
				}
			}
		}


		[ContextMenu("Pause Growth")]
		public void PauseGrowth()
		{
			if (state == State.GROWING || state == State.PAUSED)
			{
				state = State.PAUSED;
			}

			if (OnGrowthPaused != null)
			{
				OnGrowthPaused();
			}
		}


		[ContextMenu("Resume Growth")]
		public void ResumeGrowth()
		{
			if (state == State.GROWING || state == State.PAUSED)
			{
				state = State.GROWING;
			}
		}


		public State GetState()
		{
			return state;
		}


		private void UpdateWaitingForDelayState(float deltaTime)
		{
			currentTimer += deltaTime;
			if (currentTimer > growthParameters.delay)
			{
				state = State.GROWING;
				currentTimer = 0f;
			}
		}


		private void UpdateGrowingState(float deltaTime)
		{
			if (!rtIvy.IsGrowingFinished() && !rtIvy.IsVertexLimitReached())
			{
				rtIvy.UpdateIvy(deltaTime);
				return;
			}

			state = State.GROWTH_FINISHED;
			if (OnGrowthFinished != null)
			{
				OnGrowthFinished();
			}
		}
	}
}