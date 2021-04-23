using UnityEngine;
using UnityEngine.Audio;

namespace Blis.Client
{
	[RequireComponent(typeof(AudioSource))]
	public class AmbientBGMSoundSource : MonoBehaviour
	{
		public delegate void FinishPlay();


		private AudioSource audioSource;


		private float deltaTime;


		private float replayTime;


		private bool startToPlay;


		private bool startToStop;


		public bool isPlaying => audioSource.isPlaying && !startToStop;


		public AudioSource AudioSource => audioSource;


		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}


		private void Update()
		{
			if (startToPlay)
			{
				deltaTime -= Time.unscaledDeltaTime;
				audioSource.volume = Singleton<SoundControl>.inst.AmbientVolume * (1f - deltaTime);
				if (deltaTime <= 0f)
				{
					startToPlay = false;
				}

				return;
			}

			if (startToStop)
			{
				deltaTime -= Time.unscaledDeltaTime;
				audioSource.volume = Singleton<SoundControl>.inst.AmbientVolume * deltaTime;
				if (deltaTime <= 0f)
				{
					audioSource.Stop();
					startToPlay = false;
				}

				return;
			}

			audioSource.volume = Singleton<SoundControl>.inst.AmbientVolume;
			if (replayTime > 0f)
			{
				replayTime -= Time.deltaTime;
				if (replayTime <= 0f)
				{
					FinishPlay onFinishPlay = OnFinishPlay;
					if (onFinishPlay == null)
					{
						return;
					}

					onFinishPlay();
				}
			}
		}


		
		
		public event FinishPlay OnFinishPlay = delegate { };


		public void SetClip(AudioClip audioClip)
		{
			audioSource.clip = audioClip;
		}


		public void SetLoop(bool loop)
		{
			audioSource.loop = loop;
		}


		public void SetVolume(float volume)
		{
			audioSource.volume = volume;
		}


		public void SetMute(bool mute)
		{
			audioSource.mute = mute;
		}


		public void Play(AudioMixerGroup mixerGroup, float deltaTime = 1f)
		{
			startToStop = false;
			this.deltaTime = deltaTime;
			startToPlay = true;
			audioSource.outputAudioMixerGroup = mixerGroup;
			audioSource.volume = 0f;
			audioSource.Play();
			replayTime = audioSource.clip.length - 1f;
		}


		public void Stop(float deltaTime = 1f)
		{
			this.deltaTime = deltaTime;
			startToStop = true;
		}
	}
}