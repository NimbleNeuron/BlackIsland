using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Blis.Client
{
	public class SoundControl : Singleton<SoundControl>
	{
		public const ClientService.GamePlayMode IngameFlag =
			ClientService.GamePlayMode.Play | ClientService.GamePlayMode.ObserveTeam;


		private static readonly int MAX_SE_COUNT = 128;


		private readonly List<int> ambientASoundRateList = new List<int>();


		private readonly AmbientBGMSoundSource[] ambientASources = new AmbientBGMSoundSource[2];


		private readonly List<int> ambientBSoundRateList = new List<int>();


		private readonly AmbientBGMSoundSource[] ambientBSources = new AmbientBGMSoundSource[2];


		private readonly Queue<AudioClip> announceQueue = new Queue<AudioClip>();


		private readonly BGMAudioSource[] BGMAudioSources = new BGMAudioSource[2];


		private readonly Dictionary<string, AudioClip> bgmCacheSound = new Dictionary<string, AudioClip>();


		private readonly Dictionary<string, AudioClip> dicCacheSound = new Dictionary<string, AudioClip>();


		private readonly Dictionary<string, string> fxSoundPath = new Dictionary<string, string>();


		private readonly Dictionary<string, AudioMixerGroup> newInGameMixerGroups =
			new Dictionary<string, AudioMixerGroup>();


		private readonly Dictionary<string, AudioMixerGroup> newOutGameMixerGroups =
			new Dictionary<string, AudioMixerGroup>();


		private readonly List<SfxAudioSource> sfxAudioSources = new List<SfxAudioSource>();


		private readonly List<AudioSource> uiAudioSources = new List<AudioSource>();


		private readonly Dictionary<string, AudioClip> uiCacheSound = new Dictionary<string, AudioClip>();


		private readonly List<VoiceAudioSource> voiceAudioSources = new List<VoiceAudioSource>();


		private readonly Dictionary<string, string> voiceSoundPath = new Dictionary<string, string>();


		private List<SoundGroupData> ambientASoundList = new List<SoundGroupData>();


		private List<SoundGroupData> ambientBSoundList = new List<SoundGroupData>();


		private float ambientVolume;


		private AudioSource announceAudioSource;


		private float announceVolume;


		private float bgmVolume;


		private bool inGame;


		private float masterVolume;


		private bool muteAmbientVolume;


		private bool muteAnnounceVolume;


		private bool muteBGMVolume;


		private bool muteMasterVolume;


		private bool muteSfxVolume;


		private bool muteVoiceVolume;


		private AudioMixer newInGameMixer;


		private AudioMixer newOutGameMixer;


		private int playAmbientAIndex;


		private int playAmbientBIndex;


		private int playBGMIndex;


		private GameObject resourceSoundSource;


		private GameObject resourceSoundSource3D;


		private float sfxVolume;


		public float videoVolume;


		private float voiceVolume;


		public float MasterVolume => masterVolume / 100f;


		public float BGMVolume => bgmVolume / 100f * MasterVolume;


		public float SFXVolume => sfxVolume / 100f * MasterVolume;


		public float AmbientVolume => ambientVolume / 100f * MasterVolume;


		public float AnnounceVolume => announceVolume / 100f * MasterVolume;


		public float VoiceVolume => voiceVolume / 100f * MasterVolume;


		public float VideoVolume => videoVolume / 100f * MasterVolume;


		public bool MuteMasterVolume => muteMasterVolume;


		public AudioSource AnnounceAudioSource => announceAudioSource;


		protected override void OnCreated()
		{
			base.OnCreated();
			resourceSoundSource = Resources.Load<GameObject>("Sound/SoundSource");
			resourceSoundSource3D = Resources.Load<GameObject>("Sound/SoundSource3D");
			SceneManager.sceneLoaded += OnSceneLoaded;
			foreach (AudioClip audioClip in Resources.LoadAll<AudioClip>("Sound/BGM"))
			{
				bgmCacheSound.Add(audioClip.name, audioClip);
			}
		}


		protected override void OnClear()
		{
			base.OnClear();
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}


		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (loadSceneMode == LoadSceneMode.Single)
			{
				fxSoundPath.Clear();
				voiceSoundPath.Clear();
				dicCacheSound.Clear();
			}
		}


		private AudioClip GetBgmSound(string name)
		{
			AudioClip result;
			if (!bgmCacheSound.TryGetValue(name, out result))
			{
				Log.E("[KeyNotFoundException] Bgm sound No Key : " + name);
			}

			return result;
		}


		private AudioClip GetUiSoundPath(string name)
		{
			AudioClip audioClip;
			if (!uiCacheSound.TryGetValue(name, out audioClip))
			{
				audioClip = Resources.Load<AudioClip>("Sound/UI/" + name);
				uiCacheSound.Add(name, audioClip);
			}

			return audioClip;
		}


		public void SetMasterVolume(float volume)
		{
			masterVolume = volume;
			SetBGMVolume(bgmVolume);
			SetSfxVolume(sfxVolume);
			SetAmbientVolume(ambientVolume);
			SetAnnounceVolume(announceVolume);
			SetVoiceVolume(voiceVolume);
		}


		public void SetVideoModeMute(bool isMute)
		{
			if (!Singleton<LocalSetting>.inst.setting.bgmVolumeMute)
			{
				SetMuteBGMVolume(isMute);
			}

			if (!Singleton<LocalSetting>.inst.setting.fxVolumeMute)
			{
				SetMuteSfxVolume(isMute);
			}

			if (!Singleton<LocalSetting>.inst.setting.envVolumeMute)
			{
				SetMuteAmbientVolume(isMute);
			}

			if (!Singleton<LocalSetting>.inst.setting.announceVolumeMute)
			{
				SetMuteAnnounceVolume(isMute);
			}

			if (!Singleton<LocalSetting>.inst.setting.voiceVolumeMute)
			{
				SetMuteVoiceVolume(isMute);
			}
		}


		public void SetBGMVolume(float volume)
		{
			bgmVolume = volume;
			float bgmvolume = BGMVolume;
			foreach (BGMAudioSource bgmaudioSource in BGMAudioSources)
			{
				if (!(bgmaudioSource == null))
				{
					bgmaudioSource.SetVolume(bgmvolume);
				}
			}
		}


		public void SetSfxVolume(float volume)
		{
			sfxVolume = volume;
			SetPlayingSfxVolume(SFXVolume);
		}


		private void SetPlayingSfxVolume(float volume)
		{
			foreach (SfxAudioSource sfxAudioSource in sfxAudioSources)
			{
				if (!(sfxAudioSource == null))
				{
					sfxAudioSource.SetVolume(volume);
				}
			}

			foreach (AudioSource audioSource in uiAudioSources)
			{
				if (!(audioSource == null))
				{
					audioSource.volume = volume;
				}
			}
		}


		public void SetAmbientVolume(float volume)
		{
			ambientVolume = volume;
			float volume2 = AmbientVolume;
			foreach (AmbientBGMSoundSource ambientBGMSoundSource in ambientASources)
			{
				if (!(ambientBGMSoundSource == null))
				{
					ambientBGMSoundSource.SetVolume(volume2);
				}
			}

			foreach (AmbientBGMSoundSource ambientBGMSoundSource2 in ambientBSources)
			{
				if (!(ambientBGMSoundSource2 == null))
				{
					ambientBGMSoundSource2.SetVolume(volume2);
				}
			}
		}


		public void SetAnnounceVolume(float volume)
		{
			announceVolume = volume;
			float volume2 = AnnounceVolume;
			if (announceAudioSource != null)
			{
				announceAudioSource.volume = volume2;
			}
		}


		public void SetVoiceVolume(float volume)
		{
			voiceVolume = volume;
			SetPlayingVoiceVolume(VoiceVolume);
		}


		private void SetPlayingVoiceVolume(float volume)
		{
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (!(voiceAudioSource == null))
				{
					voiceAudioSource.SetVolume(volume);
				}
			}
		}


		public void PauseSfx(string tag)
		{
			foreach (SfxAudioSource sfxAudioSource in sfxAudioSources)
			{
				if (!(sfxAudioSource == null) && (string.IsNullOrEmpty(tag) || tag.Equals(sfxAudioSource.Tag)))
				{
					sfxAudioSource.Pause();
				}
			}
		}


		public void PauseVoice(string tag)
		{
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (!(voiceAudioSource == null) && (string.IsNullOrEmpty(tag) || tag.Equals(voiceAudioSource.Tag)))
				{
					voiceAudioSource.Pause();
				}
			}
		}


		public void ResumeSfx(string tag)
		{
			foreach (SfxAudioSource sfxAudioSource in sfxAudioSources)
			{
				if (!(sfxAudioSource == null) && (string.IsNullOrEmpty(tag) || tag.Equals(sfxAudioSource.Tag)))
				{
					sfxAudioSource.Resume();
				}
			}
		}


		public void ResumeVoice(string tag)
		{
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (!(voiceAudioSource == null) && (string.IsNullOrEmpty(tag) || tag.Equals(voiceAudioSource.Tag)))
				{
					voiceAudioSource.Resume();
				}
			}
		}


		public void StopSfxAudio(string tag)
		{
			foreach (SfxAudioSource sfxAudioSource in sfxAudioSources)
			{
				if (!(sfxAudioSource == null) && (string.IsNullOrEmpty(tag) || tag.Equals(sfxAudioSource.Tag)) &&
				    sfxAudioSource.IsPlaying)
				{
					sfxAudioSource.Stop();
				}
			}
		}


		public void StopVoiceAudio(string tag)
		{
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (!(voiceAudioSource == null) && (string.IsNullOrEmpty(tag) || tag.Equals(voiceAudioSource.Tag)) &&
				    voiceAudioSource.IsPlaying)
				{
					voiceAudioSource.Stop();
				}
			}
		}


		public void SetMuteMasterVolume(bool mute)
		{
			muteMasterVolume = mute;
		}


		public void SetMuteBGMVolume(bool mute)
		{
			muteBGMVolume = mute | muteMasterVolume;
			foreach (BGMAudioSource bgmaudioSource in BGMAudioSources)
			{
				if (!(bgmaudioSource == null))
				{
					bgmaudioSource.SetMute(muteBGMVolume);
				}
			}
		}


		public void SetMuteSfxVolume(bool mute)
		{
			muteSfxVolume = mute | muteMasterVolume;
			foreach (SfxAudioSource sfxAudioSource in sfxAudioSources)
			{
				if (!(sfxAudioSource == null))
				{
					sfxAudioSource.SetMute(muteSfxVolume);
				}
			}

			foreach (AudioSource audioSource in uiAudioSources)
			{
				if (!(audioSource == null))
				{
					audioSource.mute = muteSfxVolume;
				}
			}
		}


		public void SetMuteAmbientVolume(bool mute)
		{
			muteAmbientVolume = mute | muteMasterVolume;
			foreach (AmbientBGMSoundSource ambientBGMSoundSource in ambientASources)
			{
				if (!(ambientBGMSoundSource == null))
				{
					ambientBGMSoundSource.SetMute(muteAmbientVolume);
				}
			}

			foreach (AmbientBGMSoundSource ambientBGMSoundSource2 in ambientBSources)
			{
				if (!(ambientBGMSoundSource2 == null))
				{
					ambientBGMSoundSource2.SetMute(muteAmbientVolume);
				}
			}
		}


		public void SetMuteAnnounceVolume(bool mute)
		{
			muteAnnounceVolume = mute | muteMasterVolume;
			if (announceAudioSource != null)
			{
				announceAudioSource.mute = muteAnnounceVolume;
			}
		}


		public void SetMuteVoiceVolume(bool mute)
		{
			muteVoiceVolume = mute | muteMasterVolume;
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (!(voiceAudioSource == null))
				{
					voiceAudioSource.SetMute(muteVoiceVolume);
				}
			}
		}


		public void Init()
		{
			if (newInGameMixer == null)
			{
				newInGameMixer = Resources.Load<AudioMixer>("Sound/InGame");

				if (newInGameMixer != null)
				{
					foreach (AudioMixerGroup audioMixerGroup in newInGameMixer.FindMatchingGroups("Master"))
					{
						newInGameMixerGroups.Add(audioMixerGroup.name, audioMixerGroup);
					}
				}
			}

			if (newOutGameMixer == null)
			{
				newOutGameMixer = Resources.Load<AudioMixer>("Sound/OutGame");
				if (newOutGameMixer != null)
				{
					foreach (AudioMixerGroup audioMixerGroup2 in newOutGameMixer.FindMatchingGroups("Master"))
					{
						newOutGameMixerGroups.Add(audioMixerGroup2.name, audioMixerGroup2);
					}
				}
			}

			playBGMIndex = -1;
			playAmbientAIndex = -1;
			playAmbientBIndex = -1;
			inGame = false;
			InitVolume();
		}


		public void CleanUp()
		{
			StopAllSounds();
			playBGMIndex = -1;
			for (int i = 0; i < BGMAudioSources.Length; i++)
			{
				if (BGMAudioSources[i] != null)
				{
					Object.Destroy(BGMAudioSources[i].gameObject);
					BGMAudioSources[i] = null;
				}
			}

			playAmbientAIndex = -1;
			for (int j = 0; j < ambientASources.Length; j++)
			{
				if (ambientASources[j] != null)
				{
					Object.Destroy(ambientASources[j].gameObject);
					ambientASources[j] = null;
				}
			}

			playAmbientBIndex = -1;
			for (int k = 0; k < ambientBSources.Length; k++)
			{
				if (ambientBSources[k] != null)
				{
					Object.Destroy(ambientBSources[k].gameObject);
					ambientBSources[k] = null;
				}
			}

			for (int l = 0; l < sfxAudioSources.Count; l++)
			{
				if (sfxAudioSources[l] != null)
				{
					Object.Destroy(sfxAudioSources[l].gameObject);
					sfxAudioSources[l] = null;
				}
			}

			for (int m = 0; m < uiAudioSources.Count; m++)
			{
				if (uiAudioSources[m] != null)
				{
					Object.Destroy(uiAudioSources[m].gameObject);
					uiAudioSources[m] = null;
				}
			}

			for (int n = 0; n < voiceAudioSources.Count; n++)
			{
				if (voiceAudioSources[n] != null)
				{
					Object.Destroy(voiceAudioSources[n].gameObject);
					voiceAudioSources[n] = null;
				}
			}

			sfxAudioSources.Clear();
			uiAudioSources.Clear();
			voiceAudioSources.Clear();
			Object.Destroy(announceAudioSource);
			announceQueue.Clear();
		}


		public void SetInGame(bool isInGame)
		{
			inGame = isInGame;
		}


		private void InitVolume()
		{
			masterVolume = Singleton<LocalSetting>.inst.setting.masterVolume;
			bgmVolume = Singleton<LocalSetting>.inst.setting.bgmVolume;
			sfxVolume = Singleton<LocalSetting>.inst.setting.fxVolume;
			ambientVolume = Singleton<LocalSetting>.inst.setting.envVolume;
			ambientVolume = Singleton<LocalSetting>.inst.setting.announceVolume;
			voiceVolume = Singleton<LocalSetting>.inst.setting.voiceVolume;
			muteMasterVolume = Singleton<LocalSetting>.inst.setting.masterVolumeMute;
			muteBGMVolume = Singleton<LocalSetting>.inst.setting.bgmVolumeMute;
			muteSfxVolume = Singleton<LocalSetting>.inst.setting.fxVolumeMute;
			muteAmbientVolume = Singleton<LocalSetting>.inst.setting.envVolumeMute;
			muteAnnounceVolume = Singleton<LocalSetting>.inst.setting.announceVolumeMute;
			muteVoiceVolume = Singleton<LocalSetting>.inst.setting.voiceVolumeMute;
		}


		public void StopAllSounds()
		{
			StopBGM();
			StopAmbientA();
			StopAmbientB();
			StopAllSoundEffects();
			StopAnnounce();
		}


		public void StopAnnounce()
		{
			if (announceAudioSource != null && announceAudioSource.isPlaying)
			{
				announceAudioSource.Stop();
			}
		}


		public void StopAllSoundEffects()
		{
			StopSfxAudio();
			StopUIAudio();
			StopVoiceAudio();
		}


		public void StopSfxAudio()
		{
			foreach (SfxAudioSource sfxAudioSource in sfxAudioSources)
			{
				if (sfxAudioSource.IsPlaying)
				{
					sfxAudioSource.Stop();
				}
			}
		}


		private void StopUIAudio()
		{
			foreach (AudioSource audioSource in uiAudioSources)
			{
				if (audioSource.isPlaying)
				{
					audioSource.Stop();
				}
			}
		}


		public void StopUIAudio(string clipName)
		{
			foreach (AudioSource audioSource in uiAudioSources)
			{
				if (!(audioSource == null) &&
				    (string.IsNullOrEmpty(clipName) || clipName.Equals(audioSource.clip.name)) && audioSource.isPlaying)
				{
					audioSource.Stop();
				}
			}
		}


		public void StopVoiceAudio()
		{
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (voiceAudioSource.IsPlaying)
				{
					voiceAudioSource.Stop();
				}
			}
		}


		public void Update()
		{
			if (announceQueue.Count > 0 && announceAudioSource != null && !announceAudioSource.isPlaying)
			{
				PlayAnnounceSound(announceQueue.Dequeue());
			}
		}


		private void PlayAnnounceSound(AudioClip audioClip)
		{
			if (announceAudioSource == null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource);
				Object.DontDestroyOnLoad(gameObject);
				announceAudioSource = gameObject.GetComponent<AudioSource>();
				announceAudioSource.volume = AnnounceVolume;
				announceAudioSource.mute = muteAnnounceVolume;
				announceAudioSource.outputAudioMixerGroup = GetMixerGroup("Announcer");
			}

			if (announceAudioSource != null && announceAudioSource.isPlaying)
			{
				announceQueue.Enqueue(audioClip);
				return;
			}

			announceAudioSource.clip = audioClip;
			announceAudioSource.Play();
		}


		public float GetAnnouceSoundLength()
		{
			return announceAudioSource.clip.length;
		}


		public bool CheckAnnounceSoundPlaying()
		{
			return announceAudioSource != null && announceAudioSource.isPlaying;
		}


		public void PlayAnnounceSound(AnnounceVoiceType type, int extra = 0)
		{
			AudioClip audioClip =
				SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(AnnounceVoice.GetAnnounceVoice(type, extra));
			if (audioClip == null)
			{
				audioClip = SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(
					AnnounceVoice.GetDefaultLanguageVoice(type, extra));
			}

			if (audioClip != null)
			{
				PlayAnnounceSound(audioClip);
			}
		}


		public AudioClip PlayAnnounceSoundTutorial(int extra = 0)
		{
			AudioClip audioClip =
				SingletonMonoBehaviour<ResourceManager>.inst.LoadTutorialVoice(
					AnnounceVoice.GetAnnounceVoice(AnnounceVoiceType.Tutorial, extra));
			if (audioClip == null)
			{
				audioClip = SingletonMonoBehaviour<ResourceManager>.inst.LoadTutorialVoice(
					AnnounceVoice.GetDefaultLanguageVoice(AnnounceVoiceType.Tutorial, extra));
			}

			PlayAnnounceSound(audioClip);
			return audioClip;
		}


		public void PlayAnnounceSoundDeadPlayer(int killCount, int aliveCount)
		{
			AudioClip audioClip =
				SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(
					AnnounceVoice.GetAnnounceVoice(AnnounceVoiceType.DeadPlayer, killCount));
			if (audioClip == null)
			{
				audioClip = SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(
					AnnounceVoice.GetDefaultLanguageVoice(AnnounceVoiceType.DeadPlayer, killCount));
			}

			if (audioClip != null)
			{
				PlayAnnounceSound(audioClip);
			}

			PlayAnnounceSound(AnnounceVoiceType.AlivePlayer, aliveCount);
		}


		public void PlayBGM(string name, bool loopImmediately = false, bool loop = false)
		{
			playBGMIndex = (int) Mathf.Repeat(playBGMIndex + 1, BGMAudioSources.Length);
			if (BGMAudioSources[playBGMIndex] == null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource);
				Object.DontDestroyOnLoad(gameObject);
				BGMAudioSources[playBGMIndex] = gameObject.AddComponent<BGMAudioSource>();
			}

			BGMAudioSources[playBGMIndex].SetClip(GetBgmSound(name));
			BGMAudioSources[playBGMIndex].SetVolume(BGMVolume);
			BGMAudioSources[playBGMIndex].SetMute(muteBGMVolume);
			BGMAudioSources[playBGMIndex].SetLoop(loop);
			BGMAudioSources[playBGMIndex].SetLoopImmediately(loopImmediately);
			BGMAudioSources[playBGMIndex].Play(GetMixerGroup("BGM"));
			int num = (int) Mathf.Repeat(playBGMIndex + 1, BGMAudioSources.Length);
			if (BGMAudioSources[num] != null)
			{
				BGMAudioSources[num].Stop();
			}
		}


		public void StopBGM()
		{
			foreach (BGMAudioSource bgmaudioSource in BGMAudioSources)
			{
				if (bgmaudioSource != null)
				{
					bgmaudioSource.Stop();
				}
			}
		}


		public void PlayAmbientA(List<SoundGroupData> soundList)
		{
			if (soundList.Count == 0)
			{
				return;
			}

			ambientASoundList = soundList;
			ambientASoundRateList.Clear();
			foreach (SoundGroupData soundGroupData in ambientASoundList)
			{
				ambientASoundRateList.Add(soundGroupData.rate);
			}

			PlayNextAmbientA();
		}


		private void PlayNextAmbientA()
		{
			int randomElementalIndex = GameUtil.GetRandomElementalIndex(ambientASoundRateList);
			if (randomElementalIndex == -1)
			{
				return;
			}

			SoundGroupData soundGroupData = ambientASoundList.ElementAt(randomElementalIndex);
			PlayAmbientAInternal(soundGroupData.fileName);
		}


		private void PlayAmbientAInternal(string name)
		{
			playAmbientAIndex = (int) Mathf.Repeat(playAmbientAIndex + 1, ambientASources.Length);
			if (ambientASources[playAmbientAIndex] == null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource);
				Object.DontDestroyOnLoad(gameObject);
				ambientASources[playAmbientAIndex] = gameObject.AddComponent<AmbientBGMSoundSource>();
				ambientASources[playAmbientAIndex].OnFinishPlay += PlayNextAmbientA;
			}

			ambientASources[playAmbientAIndex].SetClip(Resources.Load<AudioClip>("Sound/BGM/" + name));
			ambientASources[playAmbientAIndex].SetVolume(AmbientVolume);
			ambientASources[playAmbientAIndex].SetMute(muteAmbientVolume);
			ambientASources[playAmbientAIndex].Play(GetMixerGroup("AmbiBGM"));
			int num = (int) Mathf.Repeat(playAmbientAIndex + 1, ambientASources.Length);
			if (ambientASources[num] != null)
			{
				ambientASources[num].Stop();
			}
		}


		public void StopAmbientA()
		{
			foreach (AmbientBGMSoundSource ambientBGMSoundSource in ambientASources)
			{
				if (ambientBGMSoundSource != null)
				{
					ambientBGMSoundSource.Stop();
				}
			}
		}


		public void PlayAmbientB(List<SoundGroupData> soundList)
		{
			if (soundList.Count == 0)
			{
				return;
			}

			ambientBSoundList = soundList;
			ambientBSoundRateList.Clear();
			foreach (SoundGroupData soundGroupData in ambientBSoundList)
			{
				ambientBSoundRateList.Add(soundGroupData.rate);
			}

			PlayNextAmbientB();
		}


		private void PlayNextAmbientB()
		{
			int randomElementalIndex = GameUtil.GetRandomElementalIndex(ambientBSoundRateList);
			SoundGroupData soundGroupData = ambientBSoundList.ElementAt(randomElementalIndex);
			PlayAmbientBInternal(soundGroupData.fileName);
		}


		private void PlayAmbientBInternal(string name)
		{
			playAmbientBIndex = (int) Mathf.Repeat(playAmbientBIndex + 1, ambientBSources.Length);
			if (ambientBSources[playAmbientBIndex] == null)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource);
				Object.DontDestroyOnLoad(gameObject);
				ambientBSources[playAmbientBIndex] = gameObject.AddComponent<AmbientBGMSoundSource>();
				ambientBSources[playAmbientBIndex].OnFinishPlay += PlayNextAmbientB;
			}

			ambientBSources[playAmbientBIndex].SetClip(GetBgmSound(name));
			ambientBSources[playAmbientBIndex].SetVolume(AmbientVolume);
			ambientBSources[playAmbientBIndex].SetMute(muteAmbientVolume);
			ambientBSources[playAmbientBIndex].Play(GetMixerGroup("AmbiBGM"));
			int num = (int) Mathf.Repeat(playAmbientBIndex + 1, ambientBSources.Length);
			if (ambientBSources[num] != null)
			{
				ambientBSources[num].Stop();
			}
		}


		public void StopAmbientB()
		{
			foreach (AmbientBGMSoundSource ambientBGMSoundSource in ambientBSources)
			{
				if (ambientBGMSoundSource != null)
				{
					ambientBGMSoundSource.Stop();
				}
			}
		}


		public AudioSource Play2DSound(string soundName)
		{
			AudioClip audioClip = SingletonMonoBehaviour<ResourceManager>.inst.LoadFXSound(soundName);
			return Play2DSound(audioClip);
		}


		public AudioSource Play2DSound(int characterCode, int skinIndex, string resourceName)
		{
			string resource = GameDB.character.GetCharacterData(characterCode).resource;
			AudioClip audioClip =
				SingletonMonoBehaviour<ResourceManager>.inst.LoadVoice(characterCode, skinIndex, resource,
					resourceName);
			return Play2DSound(audioClip);
		}


		private AudioSource Play2DSound(AudioClip audioClip)
		{
			if (audioClip == null)
			{
				return null;
			}

			foreach (AudioSource audioSource in uiAudioSources)
			{
				if (!audioSource.isPlaying)
				{
					Play2DInMixer(audioClip, "SFX", audioSource);
					return audioSource;
				}
			}

			if (uiAudioSources.Count >= MAX_SE_COUNT)
			{
				return null;
			}

			GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource);
			Object.DontDestroyOnLoad(gameObject);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			Play2DInMixer(audioClip, "SFX", component);
			uiAudioSources.Add(component);
			return component;
		}


		private void Play2DInMixer(AudioClip audioClip, string mixer, AudioSource source)
		{
			source.clip = audioClip;
			source.volume = SFXVolume;
			source.mute = muteSfxVolume;
			source.outputAudioMixerGroup = GetMixerGroup(mixer);
			source.Play();
		}


		public AudioSource PlayUISound(string name,
			ClientService.GamePlayMode checkMode = ClientService.GamePlayMode.All)
		{
			if (!SingletonMonoBehaviour<Bootstrap>.inst.IsGameScene ||
			    MonoBehaviourInstance<ClientService>.inst == null)
			{
				if ((checkMode & ClientService.GamePlayMode.NotInGame) == ClientService.GamePlayMode.None)
				{
					return null;
				}
			}
			else if ((MonoBehaviourInstance<ClientService>.inst.CurGamePlayMode & checkMode) ==
			         ClientService.GamePlayMode.None)
			{
				return null;
			}

			foreach (AudioSource audioSource in uiAudioSources)
			{
				if (!audioSource.isPlaying)
				{
					PlayUIInMixer(name, "SFX", audioSource);
					return audioSource;
				}
			}

			if (uiAudioSources.Count >= MAX_SE_COUNT)
			{
				return null;
			}

			GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource);
			Object.DontDestroyOnLoad(gameObject);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			PlayUIInMixer(name, "SFX", component);
			uiAudioSources.Add(component);
			return component;
		}


		private void PlayUIInMixer(string name, string mixer, AudioSource source)
		{
			source.clip = GetUiSoundPath(name);
			source.volume = SFXVolume;
			source.mute = muteSfxVolume;
			source.outputAudioMixerGroup = GetMixerGroup(mixer);
			source.Play();
		}


		public float GetCharacterVoiceSoundLength(int ownerId)
		{
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (voiceAudioSource.OwnerId == ownerId)
				{
					AudioSource audioSource = voiceAudioSource.AudioSource();
					if (audioSource.clip != null)
					{
						return audioSource.clip.length;
					}
				}
			}

			return 0f;
		}


		public void StopCharacterVoice(int ownerId)
		{
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (voiceAudioSource.OwnerId == ownerId)
				{
					voiceAudioSource.Stop();
				}
			}
		}


		public void PlayCharacterVoiceSound(Transform parent, AudioClip audioClip, int ownerId, string tag,
			int maxDistance, Vector3 position, bool spatial3d, bool immediately)
		{
			foreach (VoiceAudioSource voiceAudioSource in voiceAudioSources)
			{
				if (voiceAudioSource.OwnerId == ownerId)
				{
					if (voiceAudioSource.IsPlaying)
					{
						if (!immediately)
						{
							return;
						}

						voiceAudioSource.Stop();
					}

					PlayCharacterVoiceSoundInMixer(parent, ownerId, tag, maxDistance, position, spatial3d, "Voice",
						voiceAudioSource, audioClip);
					return;
				}
			}

			GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource3D);
			Object.DontDestroyOnLoad(gameObject);
			VoiceAudioSource voiceAudioSource2 = gameObject.AddComponent<VoiceAudioSource>();
			PlayCharacterVoiceSoundInMixer(parent, ownerId, tag, maxDistance, position, spatial3d, "Voice",
				voiceAudioSource2, audioClip);
			voiceAudioSources.Add(voiceAudioSource2);
		}


		private void PlayCharacterVoiceSoundInMixer(Transform parent, int ownerId, string tag, int maxDistance,
			Vector3 position, bool spatial3d, string output, VoiceAudioSource source, AudioClip audioClip)
		{
			source.SetClip(audioClip);
			source.SetTag(tag);
			source.SetVolume(VoiceVolume);
			source.SetMute(muteVoiceVolume);
			source.SetMixer(GetMixerGroup(output));
			source.SetLoop(false);
			source.SetMaxDistance(maxDistance);
			source.SetPosition(position);
			source.SetSpatial3D(spatial3d);
			source.SetOwnerId(ownerId);
			if (parent != null)
			{
				source.SetParent(parent);
			}

			source.Play();
		}


		public SfxAudioSource PlayFXSound(string name, string tag, int maxDistance, Vector3 position, bool loop)
		{
			AudioClip audioClip = SingletonMonoBehaviour<ResourceManager>.inst.LoadFXSound(name);
			return PlayFXSound(audioClip, tag, maxDistance, position, loop);
		}


		public void PlayFXSoundChild(string name, string tag, int maxDistance, bool loop, Transform parent,
			bool isStopParentIsNull)
		{
			AudioClip audioClip = SingletonMonoBehaviour<ResourceManager>.inst.LoadFXSound(name);
			PlayFXSoundChild(audioClip, tag, maxDistance, loop, parent, isStopParentIsNull);
		}


		public SfxAudioSource PlayFXSound(AudioClip audioClip, string tag, int maxDistance, Vector3 position, bool loop)
		{
			foreach (SfxAudioSource sfxAudioSource in sfxAudioSources)
			{
				if (!sfxAudioSource.IsPlaying)
				{
					PlayFXSoundInMixer(audioClip, tag, maxDistance, position, "SFX", sfxAudioSource, loop);
					return sfxAudioSource;
				}
			}

			if (sfxAudioSources.Count >= MAX_SE_COUNT)
			{
				return null;
			}

			GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource3D);
			Object.DontDestroyOnLoad(gameObject);
			SfxAudioSource sfxAudioSource2 = gameObject.AddComponent<SfxAudioSource>();
			PlayFXSoundInMixer(audioClip, tag, maxDistance, position, "SFX", sfxAudioSource2, loop);
			sfxAudioSources.Add(sfxAudioSource2);
			return sfxAudioSource2;
		}


		public void PlayFXSoundChild(AudioClip audioClip, string tag, int maxDistance, bool loop, Transform parent,
			bool isStopParentIsNull)
		{
			string name = audioClip.name;
			for (int i = 0; i < sfxAudioSources.Count; i++)
			{
				if (sfxAudioSources[i].IsMatched(parent, name))
				{
					PlayFXSoundInMixer(audioClip, tag, maxDistance, parent, isStopParentIsNull, "SFX",
						sfxAudioSources[i], loop);
					return;
				}
			}

			for (int j = 0; j < sfxAudioSources.Count; j++)
			{
				if (!sfxAudioSources[j].IsPlaying)
				{
					PlayFXSoundInMixer(audioClip, tag, maxDistance, parent, isStopParentIsNull, "SFX",
						sfxAudioSources[j], loop);
					return;
				}
			}

			if (sfxAudioSources.Count >= MAX_SE_COUNT)
			{
				return;
			}

			GameObject gameObject = Object.Instantiate<GameObject>(resourceSoundSource3D);
			Object.DontDestroyOnLoad(gameObject);
			SfxAudioSource sfxAudioSource = gameObject.AddComponent<SfxAudioSource>();
			PlayFXSoundInMixer(audioClip, tag, maxDistance, parent, isStopParentIsNull, "SFX", sfxAudioSource, loop);
			sfxAudioSources.Add(sfxAudioSource);
		}


		public void StopFxSoundChild(Transform parent, string soundName)
		{
			for (int i = 0; i < sfxAudioSources.Count; i++)
			{
				if (sfxAudioSources[i].IsMatched(parent, soundName))
				{
					sfxAudioSources[i].SetParent(null);
					sfxAudioSources[i].Stop();
					return;
				}
			}
		}


		private void PlayFXSoundInMixer(AudioClip audioClip, string tag, int maxDistance, Vector3 position,
			string output, SfxAudioSource source, bool loop)
		{
			source.SetClip(audioClip);
			source.SetTag(tag);
			source.SetVolume(SFXVolume);
			source.SetMute(muteSfxVolume);
			source.SetMixer(GetMixerGroup(output));
			source.SetLoop(loop);
			source.SetMaxDistance(maxDistance);
			source.SetPosition(position);
			source.SetIsStopParentIsNull(false);
			source.Play();
		}


		private void PlayFXSoundInMixer(AudioClip audioClip, string tag, int maxDistance, Transform parent,
			bool isStopParentIsNull, string output, SfxAudioSource source, bool loop)
		{
			source.SetClip(audioClip);
			source.SetTag(tag);
			source.SetVolume(SFXVolume);
			source.SetMute(muteSfxVolume);
			source.SetMixer(GetMixerGroup(output));
			source.SetLoop(loop);
			source.SetMaxDistance(maxDistance);
			source.SetParent(parent);
			source.SetIsStopParentIsNull(isStopParentIsNull);
			source.Play();
		}


		private AudioMixerGroup GetMixerGroup(string output)
		{
			if (!inGame)
			{
				return GetOutGameMixerGroup(output);
			}

			return GetInGameMixerGroup(output);
		}


		private AudioMixerGroup GetInGameMixerGroup(string output)
		{
			AudioMixerGroup result;
			if (newInGameMixerGroups.TryGetValue(output, out result))
			{
				return result;
			}

			return null;
		}


		private AudioMixerGroup GetOutGameMixerGroup(string output)
		{
			AudioMixerGroup result;
			if (newOutGameMixerGroups.TryGetValue(output, out result))
			{
				return result;
			}

			return null;
		}
	}
}