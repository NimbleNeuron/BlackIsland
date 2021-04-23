using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;


public class PostProcessingPlayableMixerBehaviour : PlayableBehaviour
{
	
	public override void ProcessFrame(Playable playable, FrameData info, object playerData)
	{
		this.m_TrackBinding = (playerData as PostProcessVolume);
		if (this.m_TrackBinding == null)
		{
			return;
		}
		this.m_dofTrackBinding = this.m_TrackBinding.profile.GetSetting<DepthOfField>();
		if (this.m_dofTrackBinding == null)
		{
			return;
		}
		if (!this.m_FirstFrameHappened)
		{
			this.m_defaultFocusDistance = this.m_dofTrackBinding.focusDistance;
			this.m_defaultFocalLength = this.m_dofTrackBinding.focalLength;
			this.m_FirstFrameHappened = true;
		}
		int inputCount = playable.GetInputCount<Playable>();
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		int num5 = 0;
		for (int i = 0; i < inputCount; i++)
		{
			float inputWeight = playable.GetInputWeight(i);
			PostProcessingPlayableBehaviour behaviour = ((ScriptPlayable<PostProcessingPlayableBehaviour>) playable.GetInput<Playable>(i)).GetBehaviour();
			// co: dotPeek
			// PostProcessingPlayableBehaviour behaviour = ((ScriptPlayable<T>)playable.GetInput(i)).GetBehaviour();
			num += behaviour.focusDistance * inputWeight;
			num2 += behaviour.focalLength * inputWeight;
			num3 += inputWeight;
			if (inputWeight > num4)
			{
				num4 = inputWeight;
			}
			if (!Mathf.Approximately(inputWeight, 0f))
			{
				num5++;
			}
		}
		this.m_dofTrackBinding.focusDistance.value = num + this.m_defaultFocusDistance * (1f - num3);
		this.m_dofTrackBinding.focalLength.value = num2 + this.m_defaultFocalLength * (1f - num3);
	}

	
	public override void OnPlayableDestroy(Playable playable)
	{
		this.m_FirstFrameHappened = false;
		if (this.m_TrackBinding == null)
		{
			return;
		}
		this.m_dofTrackBinding.focusDistance.value = this.m_defaultFocusDistance;
		this.m_dofTrackBinding.focalLength.value = this.m_defaultFocalLength;
	}

	
	private float m_defaultFocusDistance;

	
	private float m_defaultFocalLength;

	
	private PostProcessVolume m_TrackBinding;

	
	private DepthOfField m_dofTrackBinding;

	
	private bool m_FirstFrameHappened;
}
