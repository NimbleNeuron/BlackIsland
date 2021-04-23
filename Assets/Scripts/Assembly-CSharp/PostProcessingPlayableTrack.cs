using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Timeline;


[TrackColor(0.9454092f, 0.9779412f, 0.3883002f)]
[TrackClipType(typeof(PostProcessingPlayableClip))]
[TrackBindingType(typeof(PostProcessVolume))]
public class PostProcessingPlayableTrack : TrackAsset
{
	
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		return ScriptPlayable<PostProcessingPlayableMixerBehaviour>.Create(graph, inputCount);
	}

	
	public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
	{
		base.GatherProperties(director, driver);
	}
}
