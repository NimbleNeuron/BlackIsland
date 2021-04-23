using System;
using System.Collections.Generic;
using System.Text;
using Blis.Common;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blis.Client
{
	public class LocalSetting : Singleton<LocalSetting>
	{
		private readonly string SETTING_KEY = "_setting";
		private readonly string SHORTCUT_SETTING_KEY = "_shortcutSetting";
		private LobbyTab lobbyTab = LobbyTab.MainTab;
		public SettingData setting { get; set; }
		public LobbyTab LobbyTab => lobbyTab;

		private void Ref()
		{
			Reference.Use(SHORTCUT_SETTING_KEY);
		}

		protected override void OnCreated()
		{
			if (PlayerPrefs.HasKey(SETTING_KEY))
			{
				try
				{
					Load(JsonConvert.DeserializeObject<SettingData>(PlayerPrefs.GetString(SETTING_KEY)));
				}
				catch
				{
					LoadDefaultSetting();
				}
			}
			else
			{
				LoadDefaultSetting();
			}
			
			setting.ForceVSyncAndFpsApply();
			if (PlayerPrefs.GetInt("KeySettingVersion", 0) < 16)
			{
				DefaultKeySetting();
				PlayerPrefs.SetInt("KeySettingVersion", 16);
			}

			QualitySettings.SetQualityLevel(setting.graphicQuality);
			QualitySettings.vSyncCount = setting.VSync ? 1 : 0;
			QualitySettings.maxQueuedFrames = setting.VSync ? 2 : 3;
			
			Screen.SetResolution(setting.screenWidth, setting.screenHeight, setting.fullScreenMode);
			
			Save();
			SceneManager.sceneLoaded += SceneLoaded;
		}

		protected override void OnClear()
		{
			base.OnClear();
			SceneManager.sceneLoaded -= SceneLoaded;
		}

		private void SceneLoaded(Scene scene, LoadSceneMode loadMode)
		{
			setting.ForceFpsApply();
		}

		public void SetLobbyTab(LobbyTab _lobbyTab)
		{
			this.lobbyTab = _lobbyTab;
		}

		public void Save()
		{
			PlayerPrefs.SetString(SETTING_KEY, JsonConvert.SerializeObject(setting));
		}

		public void SaveHoldCamera(bool hold)
		{
			setting.holdCamera = hold;
			Save();
		}

		private void Load(SettingData settingData)
		{
			setting = settingData;
			setting.screenWidth = Screen.width;
			setting.screenHeight = Screen.height;
			if (string.IsNullOrEmpty(setting.voiceCountryCode))
			{
				SetDefaultVoice();
			}

			if (setting.quickCastKeys.Count == 0 || settingData.newCastKeys.Count == 0)
			{
				DefaultKeySetting();
				return;
			}

			foreach (ShortcutInputEvent shortcutInputEvent in setting.newCastKeys)
			{
				if ((shortcutInputEvent.combinationKeys == null || shortcutInputEvent.combinationKeys.Length == 0) &&
				    GameInput.IgnoreInputKeyCode(shortcutInputEvent.key))
				{
					shortcutInputEvent.key = KeyCode.None;
				}
			}

			SetFixedKeys();
		}

		public void LoadDefaultSetting()
		{
			setting = new SettingData
			{
				screenWidth = Screen.width,
				screenHeight = Screen.height,
				fullScreenMode = FullScreenMode.MaximizedWindow,
				graphicQuality = QualitySettings.GetQualityLevel(),
				cameraSpeed = 10f,
				holdCamera = true,
				GameFPS = GameFPS.Fps60,
				VSync = true
			};
			SetDefaultVoice();
			DefaultKeySetting();
		}


		private void SetFixedKeys()
		{
			SetFixedKey(GameInputEvent.Escape, KeyCode.Escape, Array.Empty<KeyCode>());
			SetFixedKey(GameInputEvent.ThrowItem, KeyCode.Mouse1, KeyCode.LeftShift);
			SetFixedKey(GameInputEvent.PingTarget, KeyCode.Mouse0, KeyCode.LeftAlt);
			SetFixedKey(GameInputEvent.MarkTarget, KeyCode.Mouse0, KeyCode.LeftControl);
			SetFixedKey(GameInputEvent.AddGuide, KeyCode.Mouse0, KeyCode.LeftShift);
			SetFixedKey(GameInputEvent.ChatItem, KeyCode.Mouse0, KeyCode.LeftAlt);
			SetFixedKey(GameInputEvent.MoveExpandMap, KeyCode.Mouse1, KeyCode.LeftControl);
			SetFixedKey(GameInputEvent.NormalMatchingSolo, KeyCode.F5, Array.Empty<KeyCode>());
			SetFixedKey(GameInputEvent.NormalMatchingDuo, KeyCode.F6, Array.Empty<KeyCode>());
			SetFixedKey(GameInputEvent.NormalMatchingSquad, KeyCode.F7, Array.Empty<KeyCode>());
			SetFixedKey(GameInputEvent.RankMatchingSolo, KeyCode.F9, Array.Empty<KeyCode>());
			SetFixedKey(GameInputEvent.RankMatchingDuo, KeyCode.F10, Array.Empty<KeyCode>());
			SetFixedKey(GameInputEvent.RankMatchingSquad, KeyCode.F11, Array.Empty<KeyCode>());
		}

		private void SetFixedKey(GameInputEvent gameInputEvent, KeyCode key, params KeyCode[] combinationKeys)
		{
			ShortcutInputEvent shortcutInputEvent = setting.newCastKeys.Find(x => x.gameInputEvent == gameInputEvent);
			if (shortcutInputEvent != null)
			{
				shortcutInputEvent.key = key;
				shortcutInputEvent.combinationKeys = combinationKeys;
				return;
			}

			setting.newCastKeys.Add(new ShortcutInputEvent(gameInputEvent, key, combinationKeys));
		}

		public void DefaultKeySetting()
		{
			SetDefaultQuickCastKeys();
			SetDefaultCastKeys();
			SetFixedKeys();
		}

		private void SetDefaultVoice()
		{
			setting.voiceCountryCode = Application.systemLanguage.ToSupportLanguage().GetSupportVoiceLanguageCode();
		}

		private void SetDefaultQuickCastKeys()
		{
			setting.quickCastKeys = new Dictionary<GameInputEvent, bool>
			{
				{
					GameInputEvent.Active1,
					false
				},
				{
					GameInputEvent.Active2,
					false
				},
				{
					GameInputEvent.Active3,
					false
				},
				{
					GameInputEvent.Active4,
					false
				},
				{
					GameInputEvent.WeaponSkill,
					false
				},
				{
					GameInputEvent.Alpha1,
					false
				},
				{
					GameInputEvent.Alpha2,
					false
				},
				{
					GameInputEvent.Alpha3,
					false
				},
				{
					GameInputEvent.Alpha4,
					false
				},
				{
					GameInputEvent.Alpha5,
					false
				},
				{
					GameInputEvent.Alpha6,
					false
				},
				{
					GameInputEvent.Alpha7,
					false
				},
				{
					GameInputEvent.Alpha8,
					false
				},
				{
					GameInputEvent.Alpha9,
					false
				},
				{
					GameInputEvent.Alpha0,
					false
				}
			};
		}

		private void SetDefaultCastKeys()
		{
			setting.newCastKeys = new List<ShortcutInputEvent>
			{
				new ShortcutInputEvent(GameInputEvent.Active1, KeyCode.Q, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Active2, KeyCode.W, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Active3, KeyCode.E, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Active4, KeyCode.R, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.WeaponSkill, KeyCode.D, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha1, KeyCode.Alpha1, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha2, KeyCode.Alpha2, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha3, KeyCode.Alpha3, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha4, KeyCode.Alpha4, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha5, KeyCode.Alpha5, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha6, KeyCode.Alpha6, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha7, KeyCode.Alpha7, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha8, KeyCode.Alpha8, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha9, KeyCode.Alpha9, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Alpha0, KeyCode.Alpha0, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Emotion1, KeyCode.Alpha1, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.Emotion2, KeyCode.Alpha2, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.Emotion3, KeyCode.Alpha3, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.Emotion4, KeyCode.Alpha4, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.Emotion5, KeyCode.Alpha5, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.Emotion6, KeyCode.Alpha6, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.DeleteMarkTarget, KeyCode.Delete, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.EmotionPlate, KeyCode.T, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.LearnActive1, KeyCode.Q, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.LearnActive2, KeyCode.W, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.LearnActive3, KeyCode.E, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.LearnActive4, KeyCode.R, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.LearnPassive, KeyCode.T, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.LearnWeaponSkill, KeyCode.D, KeyCode.LeftControl),
				new ShortcutInputEvent(GameInputEvent.ChangeCameraMode, KeyCode.Y, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.OpenCombineWindow, KeyCode.B, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.OpenScoreboard, KeyCode.Tab, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.OpenMap, KeyCode.M, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ResetCamera, KeyCode.Space, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.OpenCharacterMastery, KeyCode.V, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.OpenCharacterStat, KeyCode.C, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ShowObjectText, KeyCode.LeftAlt, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ShowRouteList, KeyCode.P, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ObserveNextPlayer, KeyCode.PageUp, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ObservePreviousPlayer, KeyCode.PageDown, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.MaxChatWindow, KeyCode.Z, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.MinimapZoomIn, KeyCode.KeypadPlus, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.MinimapZoomOut, KeyCode.KeypadMinus, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.CameraTeam1, KeyCode.F1, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.CameraTeam2, KeyCode.F2, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.CameraTeam3, KeyCode.F3, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Attack, KeyCode.A, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Stop, KeyCode.S, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Hold, KeyCode.H, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Rest, KeyCode.X, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.Reload, KeyCode.F, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.QuickCombine, KeyCode.BackQuote, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.MoveToAttack, KeyCode.None, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ChangeEnableGameUI, KeyCode.None, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ChangeEnableTrackerName, KeyCode.None, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ChangeEnableTrackerStatusBar, KeyCode.None,
					Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ChatActive, KeyCode.Return, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.ChatActive2, KeyCode.KeypadEnter, Array.Empty<KeyCode>()),
				new ShortcutInputEvent(GameInputEvent.AllChatActive, KeyCode.Return, KeyCode.LeftShift)
			};
		}


		public ShortcutInputEvent GetShortcutInputEvent(GameInputEvent gameInputEvent)
		{
			foreach (ShortcutInputEvent shortcutInputEvent in setting.newCastKeys)
			{
				if (gameInputEvent == shortcutInputEvent.gameInputEvent)
				{
					return shortcutInputEvent;
				}
			}

			return null;
		}


		public void ClearShortcutInputEvent(GameInputEvent gameInputEvent)
		{
			if (gameInputEvent == GameInputEvent.None)
			{
				return;
			}

			ShortcutInputEvent shortcutInputEvent = GetShortcutInputEvent(gameInputEvent);
			shortcutInputEvent.key = KeyCode.None;
			Array.Resize<KeyCode>(ref shortcutInputEvent.combinationKeys, 0);
		}


		public List<KeyCode> GetKeyCodeList(GameInputEvent gameInputEvent)
		{
			ShortcutInputEvent shortcutInputEvent = GetShortcutInputEvent(gameInputEvent);
			List<KeyCode> list = new List<KeyCode>();
			foreach (KeyCode item in shortcutInputEvent.combinationKeys)
			{
				list.Add(item);
			}

			list.Add(shortcutInputEvent.key);
			return list;
		}


		public KeyCode GetKeyCode(GameInputEvent gameInputEvent)
		{
			foreach (ShortcutInputEvent shortcutInputEvent in setting.newCastKeys)
			{
				if (gameInputEvent == shortcutInputEvent.gameInputEvent)
				{
					return shortcutInputEvent.key;
				}
			}

			return KeyCode.None;
		}


		public KeyCode[] GetCombinationKeyCode(GameInputEvent gameInputEvent)
		{
			foreach (ShortcutInputEvent shortcutInputEvent in setting.newCastKeys)
			{
				if (gameInputEvent == shortcutInputEvent.gameInputEvent)
				{
					return shortcutInputEvent.combinationKeys;
				}
			}

			return null;
		}


		public string ConvertKeyCodeListToString(List<KeyCode> keyCode)
		{
			int count = keyCode.Count;
			int num = 1;
			StringBuilder stringBuilder = GameUtil.StringBuilder;
			stringBuilder.Clear();
			foreach (KeyCode keyCode2 in keyCode)
			{
				if (keyCode2 == KeyCode.LeftControl)
				{
					stringBuilder.Append("Ctrl");
				}
				else if (keyCode2 == KeyCode.LeftShift)
				{
					stringBuilder.Append("Shift");
				}
				else if (keyCode2 == KeyCode.LeftAlt)
				{
					stringBuilder.Append("Alt");
				}
				else if (keyCode2 == KeyCode.Mouse0)
				{
					stringBuilder.Append("MB1");
				}
				else if (keyCode2 == KeyCode.Mouse1)
				{
					stringBuilder.Append("MB2");
				}
				else if (keyCode2 == KeyCode.Mouse2)
				{
					stringBuilder.Append("MB3");
				}
				else if (keyCode2 == KeyCode.None)
				{
					stringBuilder.Append(Ln.Get("알 수 없음"));
				}
				else
				{
					string text = keyCode2.ToString();
					if (text.Contains("Alpha"))
					{
						stringBuilder.Append(text.Remove(0, 5));
					}
					else
					{
						string value = StringUtil.KeycodeToString(text);
						stringBuilder.Append(value);
					}
				}

				if (num < count)
				{
					stringBuilder.Append("+");
					num++;
				}
			}

			return stringBuilder.ToString();
		}


		public int ConvertGameFPSToInt()
		{
			int result = 60;
			switch (setting.GameFPS)
			{
				case GameFPS.Fps30:
					result = 30;
					break;
				case GameFPS.Fps60:
					result = 60;
					break;
				case GameFPS.Fps80:
					result = 80;
					break;
				case GameFPS.Fps120:
					result = 120;
					break;
				case GameFPS.Fps144:
					result = 144;
					break;
				case GameFPS.FpsUnLimit:
					result = -1;
					break;
			}

			return result;
		}


		public class SettingData
		{
			public bool accelerateChina;
			public float announceVolume = 75f;
			public bool announceVolumeMute;
			public float bgmVolume = 75f;
			public bool bgmVolumeMute;
			public float cameraSpeed;
			public float envVolume = 75f;
			public bool envVolumeMute;
			public bool extendMinimapMove = true;

			public FullScreenMode fullScreenMode;
			public float fxVolume = 75f;
			public bool fxVolumeMute;

			[JsonProperty] private GameFPS? gameFPS = GameFPS.Fps60;

			public int graphicQuality;
			public bool hideNameFromEnemy;
			public bool holdCamera = true;
			public bool ignoreEmotionFromEnemy;
			public float masterVolume = 75f;
			public bool masterVolumeMute;
			public bool minimapMove = true;
			public bool miniMapZoomOut;
			public bool mouseReverse;
			public bool moveExpandMap = true;

			public List<ShortcutInputEvent> newCastKeys = new List<ShortcutInputEvent>();
			public Dictionary<GameInputEvent, bool> quickCastKeys = new Dictionary<GameInputEvent, bool>();

			public int screenHeight;
			public int screenWidth;

			public string serverRegion;
			public string supportLanguage;
			
			public bool viewAllChatting;
			public bool viewTeamChatting = true;

			public string voiceCountryCode;
			public float voiceVolume = 75f;
			public bool voiceVolumeMute;

			[JsonProperty] private bool? vSync = true;

			public GameFPS GameFPS {
				get
				{
					GameFPS? _gameFPS = this.gameFPS;
					if (_gameFPS == null)
					{
						return GameFPS.Fps60;
					}

					return _gameFPS.GetValueOrDefault();
				}
				set
				{
					GameFPS? _gameFPS = this.gameFPS;
					if ((_gameFPS.GetValueOrDefault() == value) & (_gameFPS != null))
					{
						return;
					}

					this.gameFPS = value;
					ForceFpsApply();
				}
			}
			
			public bool VSync {
				get => vSync ?? true;
				set
				{
					bool? flag = vSync;
					if ((flag.GetValueOrDefault() == value) & (flag != null))
					{
						return;
					}

					vSync = value;
					ForceVSyncApply();
				}
			}

			public void ForceVSyncAndFpsApply()
			{
				ForceFpsApply();
				ForceVSyncApply();
			}


			public void ForceFpsApply()
			{
				Application.targetFrameRate = SingletonMonoBehaviour<Bootstrap>.inst.IsLobbyScene
					? 60
					: inst.ConvertGameFPSToInt();
				SingletonMonoBehaviour<GameAnalytics>.inst.UpdateTargetFrameRate();
			}


			public void ForceVSyncApply()
			{
				QualitySettings.vSyncCount = VSync ? 1 : 0;
				QualitySettings.maxQueuedFrames = VSync ? 2 : 3;
				
				SingletonMonoBehaviour<GameAnalytics>.inst.UpdateVSyncCount();
				SingletonMonoBehaviour<GameAnalytics>.inst.UpdateMaxQueuedFrames();
			}
		}
	}
}