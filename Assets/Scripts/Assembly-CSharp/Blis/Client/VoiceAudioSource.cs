using UnityEngine;
using UnityEngine.Audio;

namespace Blis.Client
{
	[RequireComponent(typeof(AudioSource))]
	public class VoiceAudioSource : MonoBehaviour
	{
		private const float DeltaTime = 0.1f;


		private const float ChangeDeltaTime = 0.2f;


		private AudioMixer audioMixer;


		private AudioSource audioSource;


		private float deltaTime;


		private bool isPause;


		private string myTag;


		private float newVolume;


		private float oldVolume;


		private int ownerId;


		private Transform parent;


		private bool startChangeVolume;


		private bool startToStop;


		private float volumeDeltaTime;


		public bool IsPlaying => audioSource.isPlaying;


		public int OwnerId => ownerId;


		public string Tag => myTag;


		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}


		private void Update()
		{
			if (IsPlaying)
			{
				if (parent != null)
				{
					transform.position = parent.position;
				}
			}
			else if (parent != null)
			{
				SetParent(null);
			}

			if (Singleton<SoundControl>.inst.AnnounceAudioSource != null &&
			    Singleton<SoundControl>.inst.AnnounceAudioSource.isPlaying)
			{
				audioMixer.SetFloat("VoiceDecibel", -14f);
			}
			else
			{
				audioMixer.SetFloat("VoiceDecibel", 0f);
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

				audioSource.volume = Singleton<SoundControl>.inst.VoiceVolume * (0.1f - deltaTime);
			}
		}


		public void SetParent(Transform parent)
		{
			this.parent = parent;
			if (this.parent != null)
			{
				transform.localPosition = Vector3.zero;
				return;
			}

			transform.position = Vector3.zero;
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


		public void SetMixer(AudioMixerGroup mixerGroup)
		{
			AudioSource().outputAudioMixerGroup = mixerGroup;
			audioMixer = mixerGroup.audioMixer;
		}


		public void SetLoop(bool loop)
		{
			audioSource.loop = loop;
		}


		public void SetMaxDistance(int maxDistance)
		{
			audioSource.maxDistance = maxDistance != 0 ? maxDistance : 10;
		}


		public void SetSpatial3D(bool spatial3D)
		{
			audioSource.spatialize = spatial3D;
		}


		public void SetOwnerId(int ownerId)
		{
			this.ownerId = ownerId;
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