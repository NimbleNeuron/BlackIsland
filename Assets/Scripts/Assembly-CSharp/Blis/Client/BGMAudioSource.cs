using UnityEngine;
using UnityEngine.Audio;

namespace Blis.Client
{
	[RequireComponent(typeof(AudioSource))]
	public class BGMAudioSource : MonoBehaviour
	{
		private AudioSource audioSource;


		private float deltaTime;


		private bool loopImmediately;


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
				audioSource.volume = Singleton<SoundControl>.inst.BGMVolume * (1f - deltaTime);
				if (deltaTime <= 0f)
				{
					startToPlay = false;
				}

				return;
			}

			if (startToStop)
			{
				deltaTime -= Time.unscaledDeltaTime;
				audioSource.volume = Singleton<SoundControl>.inst.BGMVolume * deltaTime;
				if (deltaTime <= 0f)
				{
					audioSource.Stop();
					startToPlay = false;
				}

				return;
			}

			audioSource.volume = Singleton<SoundControl>.inst.BGMVolume;
			if (replayTime > 0f)
			{
				replayTime -= Time.deltaTime;
				if (replayTime <= 0f)
				{
					Play(audioSource.outputAudioMixerGroup);
				}
			}
		}


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


		public void SetLoopImmediately(bool loopImmediately)
		{
			this.loopImmediately = loopImmediately;
		}


		public void Play(AudioMixerGroup mixerGroup, float deltaTime = 1f)
		{
			startToStop = false;
			this.deltaTime = deltaTime;
			startToPlay = true;
			audioSource.outputAudioMixerGroup = mixerGroup;
			audioSource.volume = 0f;
			audioSource.Play();
			float num = 0f;
			if (!loopImmediately)
			{
				num = Random.Range(30, 40);
			}

			replayTime = audioSource.clip.length + num;
		}


		public void Stop(float deltaTime = 1f)
		{
			this.deltaTime = deltaTime;
			startToStop = true;
		}
	}
}