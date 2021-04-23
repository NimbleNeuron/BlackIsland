using UnityEngine;
using UnityEngine.Audio;

namespace Blis.Client
{
	[RequireComponent(typeof(AudioSource))]
	public class SfxAudioSource : MonoBehaviour
	{
		private const float DeltaTime = 0.1f;
		private const float ChangeDeltaTime = 0.2f;
		private AudioSource audioSource;
		private float deltaTime;
		private bool isPause;
		private bool isStopParentIsNull;
		private string myTag;
		private float newVolume;
		private float oldVolume;
		private Transform parent;
		private bool startChangeVolume;
		private bool startToStop;
		private float volumeDeltaTime;
		public string Tag => myTag;
		public bool IsPlaying => audioSource != null && audioSource.isPlaying;
		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (IsPlaying)
			{
				if (parent == null)
				{
					if (isStopParentIsNull)
					{
						Stop();
					}
				}
				else
				{
					transform.position = parent.position;
				}
			}
			else if (parent != null)
			{
				SetParent(null);
			}

			if (startChangeVolume)
			{
				volumeDeltaTime += Time.unscaledDeltaTime;
				if (0.2f <= volumeDeltaTime)
				{
					audioSource.volume = newVolume;
					startChangeVolume = false;
				}
				else
				{
					audioSource.volume = oldVolume + (newVolume - oldVolume) * (volumeDeltaTime / 0.2f);
				}
			}

			if (startToStop)
			{
				deltaTime += Time.unscaledDeltaTime;
				if (0.1f <= deltaTime)
				{
					audioSource.Stop();
					startToStop = false;
					return;
				}

				audioSource.volume = Singleton<SoundControl>.inst.SFXVolume * (0.1f - deltaTime);
			}
		}


		public void SetParent(Transform parent)
		{
			this.parent = parent;
			transform.position = this.parent != null ? this.parent.position : Vector3.zero;
		}


		public void SetIsStopParentIsNull(bool isStopParentIsNull)
		{
			this.isStopParentIsNull = isStopParentIsNull;
		}


		public void SetTag(string tag)
		{
			myTag = tag;
		}


		public void SetPosition(Vector3 position)
		{
			transform.position = position;
			parent = null;
		}


		public void SetClip(AudioClip clip)
		{
			AudioSource().clip = clip;
		}


		public void SetVolume(float effectVolume)
		{
			if (IsPlaying)
			{
				startChangeVolume = true;
				volumeDeltaTime = 0f;
				newVolume = effectVolume;
				oldVolume = AudioSource().volume;
				return;
			}

			AudioSource().volume = effectVolume;
		}


		public void SetMute(bool mute)
		{
			AudioSource().mute = mute;
		}


		public void SetMixer(AudioMixerGroup mixter)
		{
			AudioSource().outputAudioMixerGroup = mixter;
		}


		public void SetLoop(bool loop)
		{
			audioSource.loop = loop;
		}


		public void SetMaxDistance(int maxDistance)
		{
			audioSource.maxDistance = maxDistance != 0 ? maxDistance : 10;
		}


		public void Play()
		{
			startToStop = false;
			isPause = false;
			audioSource.Play();
		}


		public void Stop()
		{
			startToStop = true;
			isPause = false;
			isStopParentIsNull = false;
			deltaTime = 0f;
		}


		public AudioSource AudioSource()
		{
			if (audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
			}

			return audioSource;
		}


		public bool IsMatched(Transform parent, string soundName)
		{
			return !(parent == null) && parent.Equals(this.parent) && !(audioSource.clip == null) &&
			       soundName.Equals(audioSource.clip.name);
		}


		public void Pause()
		{
			if (IsPlaying)
			{
				isPause = true;
				AudioSource().Pause();
			}
		}


		public void Resume()
		{
			if (isPause)
			{
				isPause = false;
				AudioSource().UnPause();
			}
		}
	}
}