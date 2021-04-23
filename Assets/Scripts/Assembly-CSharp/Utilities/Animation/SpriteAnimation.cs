using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.Animation
{
	
	public class SpriteAnimation : MonoBehaviour
	{
		
		private void Start()
		{
			if (this.targetImage == null)
			{
				this.targetImage = base.GetComponent<Image>();
			}
		}

		
		private void Update()
		{
			if (this.tempPlaying.Equals(this.nowPlaying))
			{
				this.time += Time.deltaTime;
				if (this.time >= this.nowPlay.DelayTime)
				{
					Sprite animationFrame = this.nowPlay.GetAnimationFrame(this.frame);
					if (animationFrame == null)
					{
						if (!this.loop)
						{
							return;
						}
						this.frame = 0;
					}
					else
					{
						this.frame++;
						this.targetImage.sprite = animationFrame;
						this.targetImage.SetNativeSize();
					}
					this.Reset();
				}
			}
			else
			{
				for (int i = 0; i < this.animationList.Count; i++)
				{
					if (this.animationList[i].AnimationName.Equals(this.nowPlaying))
					{
						if (this.nowPlay != null)
						{
							this.nowPlay.IsPlaying = false;
						}
						this.nowPlay = this.animationList[i];
						this.nowPlay.IsPlaying = true;
						this.Reset();
						this.tempPlaying = this.nowPlaying;
						return;
					}
				}
			}
		}

		
		public void Reset()
		{
			this.time = 0f;
			this.delayTime = this.nowPlay.DelayTime;
			this.loop = this.nowPlay.Loop;
		}

		
		public void ChangePlaying(string name)
		{
			this.beforePlaying = this.nowPlaying;
			this.nowPlaying = name;
		}

		
		public void ChangeAnimation(List<SpriteChunk> chunk)
		{
			this.animationList = chunk;
		}

		
		[SerializeField]
		private string nowPlaying;

		
		[SerializeField]
		private List<SpriteChunk> animationList;

		
		[SerializeField]
		private Image targetImage;

		
		private string beforePlaying = "beforePlaying";

		
		private string tempPlaying = "tempPlaying";

		
		private SpriteChunk nowPlay;

		
		private float time;

		
		private float delayTime;

		
		private int frame;

		
		private bool loop;
	}
}
