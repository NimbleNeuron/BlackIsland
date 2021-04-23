using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[Serializable]
public class PostProcessingPlayableClip : PlayableAsset, ITimelineClipAsset
{
	
	
	public ClipCaps clipCaps
	{
		get
		{
			return ClipCaps.Blending;
		}
	}

	
	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		return ScriptPlayable<PostProcessingPlayableBehaviour>.Create(graph, this.template, 0);
	}

	
	public PostProcessingPlayableBehaviour template = new PostProcessingPlayableBehaviour();
}
