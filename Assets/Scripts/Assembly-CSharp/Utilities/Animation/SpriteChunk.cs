using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.Animation
{
	
	[Serializable]
	public class SpriteChunk
	{
		
		
		public bool Loop
		{
			get
			{
				return this.loop;
			}
		}

		
		
		public float DelayTime
		{
			get
			{
				return this.delayTime;
			}
		}

		
		
		
		public bool IsPlaying
		{
			get
			{
				return this.isPlaying;
			}
			set
			{
				this.isPlaying = value;
			}
		}

		
		
		public string AnimationName
		{
			get
			{
				return this.animationName;
			}
		}

		
		public Sprite GetAnimationFrame(int frame)
		{
			if (frame >= this.animationList.Count)
			{
				return null;
			}
			return this.animationList[frame];
		}

		
		[SerializeField]
		private string animationName = default;

		
		[SerializeField]
		private bool isPlaying = default;

		
		[SerializeField]
		private float delayTime = default;

		
		[SerializeField]
		private bool loop = default;

		
		[SerializeField]
		private List<Sprite> animationList = default;
	}
}
