using System.Collections;
using Blis.Common;
using Blis.Common.Utils;

namespace Blis.Server
{
	
	public abstract class CrowdControlScript : SkillScript
	{
		
		protected float beginTime;

		
		protected CharacterState characterState;

		
		protected bool isBegin;

		
		protected override void Start()
		{
			base.Start();
			isBegin = false;
		}

		
		public override IEnumerator Play(object extraData = null)
		{
			characterState = extraData as CharacterState;
			Start();
			float stateDuration = characterState.Duration;
			while (stateDuration <= 0f || MonoBehaviourInstance<GameService>.inst.CurrentServerFrameTime - startTime <=
				stateDuration)
			{
				if (0 < characterState.IsSkillPause)
				{
					yield return WaitForFrame();
				}
				else
				{
					UpdateCrowdControl();
					if (IsFinishedCrowdControl())
					{
						Finish();
						yield break;
					}

					yield return WaitForFrame();
				}
			}

			Finish();
		}

		
		protected override void Finish(bool cancel = false)
		{
			base.Finish(cancel);
			characterState = null;
		}

		
		public StateType GetStateType()
		{
			return characterState.StateGroupData.stateType;
		}

		
		public float GetRemainTime()
		{
			return characterState.RemainTime();
		}

		
		protected virtual void UpdateCrowdControl() { }

		
		protected virtual bool IsFinishedCrowdControl()
		{
			return false;
		}
	}
}