using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class BasicSettingPage : BasePage
	{
		private readonly List<SupportLanguage> countryVoiceList = new List<SupportLanguage>
		{
			SupportLanguage.Korean,
			SupportLanguage.English,
			SupportLanguage.Japanese
		};


		private readonly List<int> graphicQualityList = new List<int>
		{
			0,
			2,
			4,
			5
		};


		private readonly List<SupportLanguage> languageList = new List<SupportLanguage>
		{
			SupportLanguage.Korean,
			SupportLanguage.English,
			SupportLanguage.Japanese,
			SupportLanguage.ChineseSimplified,
			SupportLanguage.ChineseTraditional,
			SupportLanguage.French,
			SupportLanguage.Spanish,
			SupportLanguage.SpanishLatin,
			SupportLanguage.Portuguese,
			SupportLanguage.PortugueseLatin,
			SupportLanguage.Indonesian,
			SupportLanguage.German,
			SupportLanguage.Russian,
			SupportLanguage.Thai,
			SupportLanguage.Vietnamese
		};

		private Slider cameraSlider;


		private Text cameraSpeed;


		private Dropdown countryCharacterVoice;


		private List<string> countryCharacterVoiceOptions;


		private Dropdown fullScreenMode;


		private List<string> fullScreenOptions;


		private Dropdown gameFPS;


		private List<GameFPS> gameFPSOptions;


		private Dropdown graphicMode;


		private List<string> graphicOptions;


		private Dropdown language;


		private List<string> languageOptions;


		private Dropdown resolutionDropdown;


		private List<Resolution> resolutionOptions;


		private int screenHeight;


		private FullScreenMode screenMode;


		private int screenWidth;


		private Text sensitive;


		private Toggle vSync;

		
		
		public event Action onResolution = delegate { };


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			cameraSlider = GameUtil.Bind<Slider>(gameObject, "CameraSpeed/Slider");
			cameraSpeed = GameUtil.Bind<Text>(gameObject, "CameraSpeed/Slider/Text");
			resolutionDropdown = GameUtil.Bind<Dropdown>(gameObject, "ResScreen/Resolution/Dropdown");
			fullScreenMode = GameUtil.Bind<Dropdown>(gameObject, "ResScreen/FullScreenMode/Dropdown");
			graphicMode = GameUtil.Bind<Dropdown>(gameObject, "ResScreen/Quality/Dropdown");
			language = GameUtil.Bind<Dropdown>(gameObject, "ETC/Language/Dropdown");
			countryCharacterVoice = GameUtil.Bind<Dropdown>(gameObject, "ETC/CountryCharacterVoice/Dropdown");
			gameFPS = GameUtil.Bind<Dropdown>(gameObject, "ETC/FPS/Dropdown");
			vSync = GameUtil.Bind<Toggle>(gameObject, "VSync/Toggle");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			InitDropBoxData();
		}


		private void InitDropBoxData()
		{
			resolutionOptions = new List<Resolution>();
			Resolution[] resolutions = Screen.resolutions;
			for (int i = resolutions.Length - 1; i >= 0; i--)
			{
				resolutionOptions.Add(resolutions[i]);
			}

			fullScreenOptions = new List<string>();
			fullScreenOptions.Add(Ln.Get(string.Format("FullScreenMode/{0}", FullScreenMode.MaximizedWindow)));
			fullScreenOptions.Add(Ln.Get(string.Format("FullScreenMode/{0}", FullScreenMode.ExclusiveFullScreen)));
			fullScreenOptions.Add(Ln.Get(string.Format("FullScreenMode/{0}", FullScreenMode.Windowed)));
			graphicOptions = new List<string>
			{
				Ln.Get("Graphic/VeryLow"),
				Ln.Get("Graphic/Medium"),
				Ln.Get("Graphic/High"),
				Ln.Get("Graphic/Ultra")
			};
			languageOptions = new List<string>();
			languageList.ForEach(delegate(SupportLanguage x)
			{
				languageOptions.Add(Ln.Get(string.Format("SupportLanguage/{0}", x)));
			});
			countryCharacterVoiceOptions = new List<string>();
			countryVoiceList.ForEach(delegate(SupportLanguage x)
			{
				countryCharacterVoiceOptions.Add(Ln.Get(string.Format("SupportCountryCharacterVoice/{0}", x)));
			});
			gameFPSOptions = new List<GameFPS>
			{
				GameFPS.Fps30,
				GameFPS.Fps60,
				GameFPS.Fps80,
				GameFPS.Fps120,
				GameFPS.Fps144,
				GameFPS.FpsUnLimit
			};
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			LoadSetting();
			ResetDropBox();
			RegisterEvent();
		}


		protected override void OnClosePage()
		{
			base.OnClosePage();
			UnregisterEvent();
			ResetDropBox();
		}


		public void LoadSetting()
		{
			resolutionDropdown.ClearOptions();
			resolutionDropdown.AddOptions((from x in resolutionOptions
				select x.ToString()).ToList<string>());
			fullScreenMode.ClearOptions();
			fullScreenMode.AddOptions(fullScreenOptions);
			graphicMode.ClearOptions();
			graphicMode.AddOptions(graphicOptions);
			language.ClearOptions();
			language.AddOptions(languageOptions);
			countryCharacterVoice.ClearOptions();
			countryCharacterVoice.AddOptions(countryCharacterVoiceOptions);
			gameFPS.ClearOptions();
			List<string> options = new List<string>
			{
				"30",
				"60",
				"80",
				"120",
				"144",
				Ln.Get("제한 해제")
			};
			gameFPS.AddOptions(options);
			switch (Screen.fullScreenMode)
			{
				case FullScreenMode.ExclusiveFullScreen:
					fullScreenMode.value = 1;
					break;
				case FullScreenMode.FullScreenWindow:
				case FullScreenMode.MaximizedWindow:
					fullScreenMode.value = 0;
					break;
				case FullScreenMode.Windowed:
					fullScreenMode.value = 2;
					break;
			}

			cameraSlider.value = Singleton<LocalSetting>.inst.setting.cameraSpeed;
			cameraSpeed.text = Singleton<LocalSetting>.inst.setting.cameraSpeed.ToString();
			int num = graphicQualityList.FindIndex(x => x == QualitySettings.GetQualityLevel());
			graphicMode.value = num >= 0 ? num : 0;
			language.value = languageList.FindIndex(x => x == Ln.GetCurrentLanguage());
			countryCharacterVoice.value = countryVoiceList.FindIndex(x =>
				x == Singleton<LocalSetting>.inst.setting.voiceCountryCode.ToSupportLanguage());
			gameFPS.value = gameFPSOptions.FindIndex(x => x == Singleton<LocalSetting>.inst.setting.GameFPS);
			screenMode = Screen.fullScreenMode;
			screenWidth = Singleton<LocalSetting>.inst.setting.screenWidth;
			screenHeight = Singleton<LocalSetting>.inst.setting.screenHeight;
			resolutionDropdown.value =
				resolutionOptions.FindIndex(x => x.height == screenHeight && x.width == screenWidth);
			vSync.isOn = Singleton<LocalSetting>.inst.setting.VSync;
		}


		private void UpdateSetting()
		{
			Singleton<LocalSetting>.inst.setting.cameraSpeed = cameraSlider.value;
			Singleton<LocalSetting>.inst.setting.screenHeight = screenHeight;
			Singleton<LocalSetting>.inst.setting.screenWidth = screenWidth;
			Singleton<LocalSetting>.inst.setting.fullScreenMode = screenMode;
			Singleton<LocalSetting>.inst.setting.graphicQuality = QualitySettings.GetQualityLevel();
			Singleton<LocalSetting>.inst.setting.supportLanguage = Ln.GetCurrentLanguage().GetAppLanguageCode();
			Singleton<LocalSetting>.inst.setting.voiceCountryCode =
				countryVoiceList[countryCharacterVoice.value].GetSupportVoiceLanguageCode();
			Singleton<LocalSetting>.inst.setting.GameFPS = gameFPSOptions[gameFPS.value];
			Singleton<LocalSetting>.inst.setting.VSync = vSync.isOn;
			Singleton<LocalSetting>.inst.Save();
		}


		public void UnregisterEvent()
		{
			cameraSlider.onValueChanged.RemoveAllListeners();
			resolutionDropdown.onValueChanged.RemoveAllListeners();
			fullScreenMode.onValueChanged.RemoveAllListeners();
			graphicMode.onValueChanged.RemoveAllListeners();
			language.onValueChanged.RemoveAllListeners();
			countryCharacterVoice.onValueChanged.RemoveAllListeners();
			gameFPS.onValueChanged.RemoveAllListeners();
			vSync.onValueChanged.RemoveAllListeners();
		}


		public void RegisterEvent()
		{
			cameraSlider.onValueChanged.AddListener(delegate
			{
				ApplySensitive();
				UpdateSetting();
			});
			resolutionDropdown.onValueChanged.AddListener(delegate
			{
				ApplyResolution();
				UpdateSetting();
			});
			fullScreenMode.onValueChanged.AddListener(delegate
			{
				ApplyResolution();
				UpdateSetting();
			});
			graphicMode.onValueChanged.AddListener(delegate
			{
				ApplyGraphicLevel();
				ApplyVSync();
				UpdateSetting();
			});
			language.onValueChanged.AddListener(delegate
			{
				UnregisterEvent();
				ApplyLanguage();
				UpdateSetting();
				InitDropBoxData();
				LoadSetting();
				RegisterEvent();
			});
			countryCharacterVoice.onValueChanged.AddListener(delegate { UpdateSetting(); });
			gameFPS.onValueChanged.AddListener(delegate
			{
				ApplyGameFPS();
				UpdateSetting();
			});
			vSync.onValueChanged.AddListener(delegate
			{
				ApplyVSync();
				UpdateSetting();
			});
		}


		private void ApplySensitive()
		{
			if (MonoBehaviourInstance<MobaCamera>.inst != null)
			{
				MonoBehaviourInstance<MobaCamera>.inst.SetCameraSpeed(cameraSlider.value);
			}

			cameraSpeed.text = cameraSlider.value.ToString();
		}


		private void ApplyResolution()
		{
			Resolution resolution = resolutionOptions[resolutionDropdown.value];
			FullScreenMode fullscreenMode = Screen.fullScreenMode;
			switch (fullScreenMode.value)
			{
				case 0:
					fullscreenMode = FullScreenMode.MaximizedWindow;
					break;
				case 1:
					fullscreenMode = FullScreenMode.ExclusiveFullScreen;
					break;
				case 2:
					fullscreenMode = FullScreenMode.Windowed;
					break;
			}

			Screen.SetResolution(resolution.width, resolution.height, fullscreenMode);
			screenWidth = resolution.width;
			screenHeight = resolution.height;
			screenMode = fullscreenMode;
			onResolution();
			SingletonMonoBehaviour<GameAnalytics>.inst.UpdateScreen();
		}


		private void ApplyGraphicLevel()
		{
			if (MonoBehaviourInstance<GameClient>.inst != null)
			{
				MonoBehaviourInstance<GameClient>.inst.ApplyGraphicLevel(graphicMode.value);
			}

			QualitySettings.SetQualityLevel(graphicQualityList[graphicMode.value]);
			SingletonMonoBehaviour<GameAnalytics>.inst.UpdateQualityLevel();
		}


		private void ApplyLanguage()
		{
			SupportLanguage supportLanguage = languageList[language.value];
			SingletonMonoBehaviour<LnLoader>.inst.LoadData(supportLanguage);
		}


		private void ApplyGameFPS()
		{
			Singleton<LocalSetting>.inst.setting.GameFPS = gameFPSOptions[gameFPS.value];
		}


		private void ApplyVSync()
		{
			Singleton<LocalSetting>.inst.setting.VSync = vSync.isOn;
		}


		public void ResetDropBox()
		{
			resolutionDropdown.Hide();
			fullScreenMode.Hide();
			graphicMode.Hide();
			language.Hide();
			countryCharacterVoice.Hide();
			gameFPS.Hide();
		}
	}
}