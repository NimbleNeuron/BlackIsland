using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class SoundSettingPage : BasePage
	{
		private Toggle announceMute;


		private SliderTextGroup announceVolume;


		private Toggle bgmMute;


		private SliderTextGroup bgmVolume;


		private Toggle envMute;


		private SliderTextGroup envVolume;


		private Toggle fxMute;


		private SliderTextGroup fxVolume;


		private Toggle masterMute;


		private SliderTextGroup masterVolume;


		private List<Toggle> muteToggles;


		private float originalAnnounceVol;


		private float originalBGMVol;


		private float originalEnvVol;


		private float originalFxVol;


		private float originalMasterVol;


		private float originalVoiceVol;


		private Dictionary<Toggle, SliderTextGroup> sliderMap;


		private Toggle voiceMute;


		private SliderTextGroup voiceVolume;


		private List<SliderTextGroup> volumeSliders;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			masterVolume = new SliderTextGroup(GameUtil.Bind<Slider>(gameObject, "MasterVolume/Volume/Slider"));
			bgmVolume = new SliderTextGroup(GameUtil.Bind<Slider>(gameObject,
				"VolumeScrollRect/Contents/BGMVolume/Volume/Slider"));
			fxVolume = new SliderTextGroup(GameUtil.Bind<Slider>(gameObject,
				"VolumeScrollRect/Contents/FxVolume/Volume/Slider"));
			envVolume = new SliderTextGroup(GameUtil.Bind<Slider>(gameObject,
				"VolumeScrollRect/Contents/EnvVolume/Volume/Slider"));
			announceVolume = new SliderTextGroup(GameUtil.Bind<Slider>(gameObject,
				"VolumeScrollRect/Contents/AnnounceVolume/Volume/Slider"));
			voiceVolume =
				new SliderTextGroup(GameUtil.Bind<Slider>(gameObject,
					"VolumeScrollRect/Contents/VoiceVolume/Volume/Slider"));
			volumeSliders = new List<SliderTextGroup>
			{
				masterVolume,
				bgmVolume,
				fxVolume,
				envVolume,
				announceVolume,
				voiceVolume
			};
			masterMute = GameUtil.Bind<Toggle>(gameObject, "MasterVolume/Volume/OnOff");
			bgmMute = GameUtil.Bind<Toggle>(gameObject, "VolumeScrollRect/Contents/BGMVolume/Volume/OnOff");
			fxMute = GameUtil.Bind<Toggle>(gameObject, "VolumeScrollRect/Contents/FxVolume/Volume/OnOff");
			envMute = GameUtil.Bind<Toggle>(gameObject, "VolumeScrollRect/Contents/EnvVolume/Volume/OnOff");
			announceMute = GameUtil.Bind<Toggle>(gameObject, "VolumeScrollRect/Contents/AnnounceVolume/Volume/OnOff");
			voiceMute = GameUtil.Bind<Toggle>(gameObject, "VolumeScrollRect/Contents/VoiceVolume/Volume/OnOff");
			muteToggles = new List<Toggle>
			{
				masterMute,
				bgmMute,
				fxMute,
				envMute,
				announceMute,
				voiceMute
			};
			sliderMap = new Dictionary<Toggle, SliderTextGroup>
			{
				{
					masterMute,
					masterVolume
				},
				{
					bgmMute,
					bgmVolume
				},
				{
					fxMute,
					fxVolume
				},
				{
					envMute,
					envVolume
				},
				{
					announceMute,
					announceVolume
				},
				{
					voiceMute,
					voiceVolume
				}
			};
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			LoadSetting();
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			RegisterEvent();
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
			UnregisterEvent();
		}


		private void LoadSetting()
		{
			masterVolume.value = Singleton<LocalSetting>.inst.setting.masterVolume;
			bgmVolume.value = Singleton<LocalSetting>.inst.setting.bgmVolume;
			fxVolume.value = Singleton<LocalSetting>.inst.setting.fxVolume;
			envVolume.value = Singleton<LocalSetting>.inst.setting.envVolume;
			announceVolume.value = Singleton<LocalSetting>.inst.setting.announceVolume;
			voiceVolume.value = Singleton<LocalSetting>.inst.setting.voiceVolume;
			masterMute.isOn = Singleton<LocalSetting>.inst.setting.masterVolumeMute;
			bgmMute.isOn = Singleton<LocalSetting>.inst.setting.bgmVolumeMute;
			fxMute.isOn = Singleton<LocalSetting>.inst.setting.fxVolumeMute;
			envMute.isOn = Singleton<LocalSetting>.inst.setting.envVolumeMute;
			announceMute.isOn = Singleton<LocalSetting>.inst.setting.announceVolumeMute;
			voiceMute.isOn = Singleton<LocalSetting>.inst.setting.voiceVolumeMute;
			foreach (KeyValuePair<Toggle, SliderTextGroup> keyValuePair in sliderMap)
			{
				keyValuePair.Value.EnableSlider(!keyValuePair.Key.isOn);
			}

			ApplyVolume();
		}


		private void UpdateSetting()
		{
			ApplyVolume();
			Singleton<LocalSetting>.inst.setting.masterVolume = masterVolume.value;
			Singleton<LocalSetting>.inst.setting.bgmVolume = bgmVolume.value;
			Singleton<LocalSetting>.inst.setting.fxVolume = fxVolume.value;
			Singleton<LocalSetting>.inst.setting.envVolume = envVolume.value;
			Singleton<LocalSetting>.inst.setting.announceVolume = announceVolume.value;
			Singleton<LocalSetting>.inst.setting.voiceVolume = voiceVolume.value;
			Singleton<LocalSetting>.inst.setting.masterVolumeMute = masterMute.isOn;
			Singleton<LocalSetting>.inst.setting.bgmVolumeMute = bgmMute.isOn;
			Singleton<LocalSetting>.inst.setting.fxVolumeMute = fxMute.isOn;
			Singleton<LocalSetting>.inst.setting.envVolumeMute = envMute.isOn;
			Singleton<LocalSetting>.inst.setting.announceVolumeMute = announceMute.isOn;
			Singleton<LocalSetting>.inst.setting.voiceVolumeMute = voiceMute.isOn;
			Singleton<LocalSetting>.inst.Save();
		}


		private void ApplyVolume()
		{
			Singleton<SoundControl>.inst.SetMasterVolume(masterVolume.value);
			Singleton<SoundControl>.inst.SetBGMVolume(bgmVolume.value);
			Singleton<SoundControl>.inst.SetSfxVolume(fxVolume.value);
			Singleton<SoundControl>.inst.SetAmbientVolume(envVolume.value);
			Singleton<SoundControl>.inst.SetAnnounceVolume(announceVolume.value);
			Singleton<SoundControl>.inst.SetVoiceVolume(voiceVolume.value);
			Singleton<SoundControl>.inst.SetMuteMasterVolume(masterMute.isOn);
			Singleton<SoundControl>.inst.SetMuteBGMVolume(bgmMute.isOn);
			Singleton<SoundControl>.inst.SetMuteSfxVolume(fxMute.isOn);
			Singleton<SoundControl>.inst.SetMuteAmbientVolume(envMute.isOn);
			Singleton<SoundControl>.inst.SetMuteAnnounceVolume(announceMute.isOn);
			Singleton<SoundControl>.inst.SetMuteVoiceVolume(voiceMute.isOn);
		}


		public void UnregisterEvent()
		{
			volumeSliders.ForEach(delegate(SliderTextGroup x) { x.onValueChanged.RemoveAllListeners(); });
		}


		public void RegisterEvent()
		{
			volumeSliders.ForEach(delegate(SliderTextGroup x)
			{
				x.onValueChanged.AddListener(delegate(float v)
				{
					x.value = v;
					UpdateSetting();
				});
			});
			muteToggles.ForEach(delegate(Toggle x)
			{
				x.onValueChanged.AddListener(delegate(bool isOn)
				{
					sliderMap[x].EnableSlider(!isOn);
					UpdateSetting();
				});
			});
			masterMute.onValueChanged.AddListener(delegate(bool isOn)
			{
				muteToggles.ForEach(delegate(Toggle x) { x.isOn = isOn; });
			});
		}


		private class SliderTextGroup
		{
			private const string HEAD_RESOURCE = "Img_Option_Head";


			private const string HEAD_DISA_RESOURCE = "Img_Option_Head_Disa";


			private readonly Color disableColor = Color.gray;


			private readonly Image fill;


			private readonly Image head;


			private readonly Color oriColor;


			private readonly Text text;


			public SliderTextGroup(Slider slider)
			{
				Slider = slider;
				text = slider.GetComponentInChildren<Text>();
				fill = GameUtil.Bind<Image>(slider.gameObject, "Fill Area/Fill");
				head = GameUtil.Bind<Image>(slider.gameObject, "Handle Slide Area/Handle");
				oriColor = fill.color;
			}


			public Slider Slider { get; }


			
			public float value {
				get => Slider.value;
				set
				{
					Slider.value = value;
					text.text = value.ToString();
				}
			}


			
			public Slider.SliderEvent onValueChanged {
				get => Slider.onValueChanged;
				set => Slider.onValueChanged = value;
			}


			public void EnableSlider(bool enable)
			{
				fill.color = enable ? oriColor : disableColor;
				head.sprite = enable
					? SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Option_Head")
					: SingletonMonoBehaviour<ResourceManager>.inst.GetCommonSprite("Img_Option_Head_Disa");
			}
		}
	}
}