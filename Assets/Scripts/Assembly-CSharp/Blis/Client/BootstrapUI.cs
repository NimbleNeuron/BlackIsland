using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Blis.Common;
using Neptune.Http;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class BootstrapUI : MonoBehaviour
	{
		public enum TempCharacterCode
		{
			Jackie = 1,
			Aya,
			Fiora,
			Magnus,
			Zahir,
			Nadine,
			Hyunwoo,
			Hart,
			Isol,
			LiDailin,
			Yuki,
			Hyejin,
			Xiukai,
			Chiara,
			Sissela,
			Silvia,
			Adriana,
			Shoichi,
			Emma,
			Lenox,
			Rozzi,
			Luke,
			Adela = 24
		}

		private const string PlayerPrefKey_Nickname = "_dev_id_token";
		private const string PlayerPrefKey_Host = "_dev_host";
		private const string PlayerPrefKey_Observer = "_dev_observer";
		private const string PlayerPrefKey_Character = "_dev_character";
		private const string PlayerPrefKey_Lang = "_dev_lang";
		private const string PlayerPrefKey_TeamMode = "_dev_teamMode";
		private const string PlayerPrefKey_TeamNumber = "_dev_teamNumber";

		public bool trylogin = true;
		private readonly string DefaultHost = string.Format("localhost:{0}", GameConstants.DefaultPort);
		private Bootstrap bootstrap;
		private Toggle customRoom;
		private Dropdown dropdownBotCount;
		private Dropdown dropdownBotDifficulty;
		private Dropdown dropdownCharacter;
		private Dropdown dropdownLang;
		private Dropdown dropdownTeamMode;
		private Dropdown dropdownTeamNumber;

		private Text guidance;
		private InputField inputHost;
		private InputField inputNickname;
		private DateTime lastLoginTime;
		private GameObject loading;
		private Toggle observer;
		private Toggle toggleUpdateDB;

		private void Awake()
		{
			bootstrap = GameObject.Find("Bootstrap").GetComponent<Bootstrap>();
			toggleUpdateDB = GameUtil.Bind<Toggle>(gameObject, "Border/Configs/Line1");
			dropdownLang = GameUtil.Bind<Dropdown>(gameObject, "Border/Configs/Line1/LanguageDB");
			observer = GameUtil.Bind<Toggle>(gameObject, "Border/Configs/Line2/Observer");
			dropdownCharacter = GameUtil.Bind<Dropdown>(gameObject, "Border/Configs/Line2/DropdownCharacter");
			inputNickname = GameUtil.Bind<InputField>(gameObject, "Border/Configs/Line3/InputNickname");
			dropdownTeamMode = GameUtil.Bind<Dropdown>(gameObject, "Border/Configs/Line4/TeamMode");
			dropdownTeamNumber = GameUtil.Bind<Dropdown>(gameObject, "Border/Configs/Line4/TeamNumber");
			dropdownBotDifficulty = GameUtil.Bind<Dropdown>(gameObject, "Border/Configs/Line5/BotDifficulty");
			dropdownBotCount = GameUtil.Bind<Dropdown>(gameObject, "Border/Configs/Line5/DropdownBotCount");
			customRoom = GameUtil.Bind<Toggle>(gameObject, "Border/Configs/Line7/IsCustomRoom");
			inputHost = GameUtil.Bind<InputField>(gameObject, "Border/Configs/Line6");
			loading = this.transform.FindRecursively("Loading").gameObject;
			inputNickname.text = PlayerPrefs.HasKey("_dev_id_token")
				? PlayerPrefs.GetString("_dev_id_token")
				: Environment.MachineName;
			inputHost.text = PlayerPrefs.HasKey("_dev_host")
				? PlayerPrefs.GetString("_dev_host")
				: string.Format("127.0.0.1:{0}", GameConstants.DefaultPort);
			observer.isOn = PlayerPrefs.HasKey("_dev_observer") && 0 < PlayerPrefs.GetInt("_dev_observer");
			List<string> list = new List<string>();
			foreach (object obj in Enum.GetValues(typeof(TempCharacterCode)))
			{
				list.Add(((TempCharacterCode) obj).ToString());
			}

			dropdownCharacter.AddOptions(list);
			if (PlayerPrefs.HasKey("_dev_character"))
			{
				dropdownCharacter.value = PlayerPrefs.GetInt("_dev_character");
			}

			List<string> list2 = Enum.GetNames(typeof(BotDifficulty)).ToList<string>();
			list2.RemoveAt(0);
			dropdownBotDifficulty.AddOptions(list2);
			List<string> list3 = new List<string>();
			for (int i = 0; i <= 17; i++)
			{
				list3.Add(i.ToString());
			}

			dropdownBotCount.AddOptions(list3);
			SupportLanguage[] source = (SupportLanguage[]) Enum.GetValues(typeof(SupportLanguage));
			dropdownLang.AddOptions((from x in source
				select x.ToString()).ToList<string>());
			if (PlayerPrefs.HasKey("_dev_lang"))
			{
				dropdownLang.value = PlayerPrefs.GetInt("_dev_lang");
			}

			List<string> list4 = new List<string>();
			list4.Add(MatchingTeamMode.Solo.ToString());
			list4.Add(MatchingTeamMode.Duo.ToString());
			list4.Add(MatchingTeamMode.Squad.ToString());
			dropdownTeamMode.AddOptions(list4);
			if (PlayerPrefs.HasKey("_dev_teamMode"))
			{
				dropdownTeamMode.value = PlayerPrefs.GetInt("_dev_teamMode");
			}

			List<string> list5 = new List<string>();
			list5.Add("팀 없음");
			for (int j = 1; j <= 9; j++)
			{
				list5.Add(j.ToString());
			}

			dropdownTeamNumber.AddOptions(list5);
			if (PlayerPrefs.HasKey("_dev_teamNumber"))
			{
				dropdownTeamNumber.value = PlayerPrefs.GetInt("_dev_teamNumber");
			}

			Singleton<SoundControl>.inst.Init();
			Transform transform = this.transform.Find("Border/Buttons/BtnLiteStandlone");
			if (transform != null)
			{
				transform.gameObject.SetActive(false);
			}
		}

		private void Start()
		{
			loading.SetActive(false);
			Singleton<SoundControl>.inst.SetInGame(false);
			Singleton<SoundControl>.inst.PlayBGM("BGM_Lobby", true);
			SingletonMonoBehaviour<SwearWordManager>.inst.LoadSwearWords();
			SingletonMonoBehaviour<ResourceManager>.inst.LoadAtlas("UI/Atlas");
			Application.targetFrameRate = GameConstants.ServerTargetFrame;
		}

		public void OnClick(GameObject button)
		{
			loading.SetActive(true);
			Text componentInChildren = loading.GetComponentInChildren<Text>();
			if (componentInChildren != null)
			{
				componentInChildren.text = button.name.Substring(3);
			}

			this.StartThrowingCoroutine(Load(button.name), null);
		}

		private IEnumerator Load(string buttonName)
		{
			if (toggleUpdateDB.isOn)
			{
				yield return SingletonMonoBehaviour<GameDBLoader>.inst.Load(GameConstants.GetDataCacheFilePath());
			}
			else
			{
				yield return SingletonMonoBehaviour<GameDBLoader>.inst.LoadCache(GameConstants.GetDataCacheFilePath());
			}

			if (!SingletonMonoBehaviour<GameDBLoader>.inst.Result.success)
			{
				Log.E("Failed to load GameDB {0}, Error: ", SingletonMonoBehaviour<GameDBLoader>.inst.Result.reason);
				yield break;
			}

			SingletonMonoBehaviour<LnLoader>.inst.LoadDefaultLangaugeData();
			SupportLanguage supportLanguage;
			if (Enum.TryParse<SupportLanguage>(dropdownLang.options[dropdownLang.value].text, true,
				out supportLanguage))
			{
				SingletonMonoBehaviour<LnLoader>.inst.LoadData(supportLanguage);
			}
			else
			{
				SingletonMonoBehaviour<LnLoader>.inst.LoadData(SupportLanguage.Korean);
			}

			Singleton<SoundControl>.inst.StopBGM();
			HttpRequestFactory.SetHeader("X-BSER-Version", BSERVersion.VERSION);
			if (trylogin)
			{
				yield return StartCoroutine(Login());
			}

			if (buttonName == "BtnServer")
			{
				bootstrap.LoadServer();
				yield break;
			}

			Random.InitState(DateTime.Now.Millisecond);
			int num = Random.Range(0, int.MaxValue);
			try
			{
				foreach (IPAddress ipaddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
				{
					if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
					{
						string text = Regex.Match(ipaddress.ToString(), "^\\d+\\.\\d+\\.\\d+\\.(\\d+)$").Result("$1");
						if (!string.IsNullOrEmpty(text))
						{
							num += int.Parse(text);
							break;
						}
					}
				}
			}
			catch (Exception) { }

			string text2 = inputNickname.text;
			bool isOn = observer.isOn;
			int num2 = 1;
			TempCharacterCode tempCharacterCode;
			if (Enum.TryParse<TempCharacterCode>(dropdownCharacter.options[dropdownCharacter.value].text, true,
				out tempCharacterCode))
			{
				num2 = (int) tempCharacterCode;
			}

			BotDifficulty botDifficulty;
			Enum.TryParse<BotDifficulty>(dropdownBotDifficulty.options[dropdownBotDifficulty.value].text, true,
				out botDifficulty);
			int botCount = int.Parse(dropdownBotCount.options[dropdownBotCount.value].text);
			string host = string.IsNullOrEmpty(inputHost.text) ? DefaultHost : inputHost.text;
			MatchingTeamMode matchingTeamMode = (MatchingTeamMode) Enum.Parse(typeof(MatchingTeamMode),
				dropdownTeamMode.options[dropdownTeamMode.value].text);
			GlobalUserData.matchingTeamMode = matchingTeamMode;
			int teamNumber = -1;
			if (0 < dropdownTeamNumber.value)
			{
				teamNumber = int.Parse(dropdownTeamNumber.options[dropdownTeamNumber.value].text);
			}

			CharacterMasteryData characterMasteryData = GameDB.mastery.GetCharacterMasteryData(num2);
			int startWeapon = GameDB.recommend.FindStartingData(num2, characterMasteryData.weapon1).startWeapon;
			int matchingMode = customRoom.isOn ? 4 : trylogin ? 1 : 0;
			MatchingToken matchingToken = new MatchingToken((MatchingMode) matchingMode, matchingTeamMode,
				MatchingRegion.Dev, num.ToString(), num, 0, text2, num2, 0, startWeapon, new List<int>(), 1, isOn,
				new Dictionary<EmotionPlateType, int>());
			PlayerPrefs.SetString("_dev_id_token", text2);
			PlayerPrefs.SetString("_dev_host", inputHost.text);
			PlayerPrefs.SetInt("_dev_observer", isOn ? 1 : -1);
			PlayerPrefs.SetInt("_dev_character", dropdownCharacter.value);
			PlayerPrefs.SetInt("_dev_lang", dropdownLang.value);
			PlayerPrefs.SetInt("_dev_teamMode", dropdownTeamMode.value);
			PlayerPrefs.SetInt("_dev_teamNumber", dropdownTeamNumber.value);
			GlobalUserData.matchingMode = (MatchingMode) matchingMode;
			GlobalUserData.matchingTeamMode = matchingTeamMode;

			switch (buttonName)
			{
				case "BtnLocalhost":
					bootstrap.LoadTest(Bootstrap.Mode.Client, DefaultHost, botCount, botDifficulty, teamNumber,
						matchingToken);
					break;
				case "BtnConnect":
					bootstrap.LoadTest(Bootstrap.Mode.Client, host, botCount, botDifficulty, teamNumber,
						matchingToken);
					break;
				case "BtnHost" :
					bootstrap.LoadTest(Bootstrap.Mode.Host, DefaultHost, botCount, botDifficulty, teamNumber,
						matchingToken);
					break;
				case "BtnStandalone" :
					bootstrap.LoadTest(Bootstrap.Mode.Standalone, DefaultHost, botCount, botDifficulty, teamNumber,
						matchingToken);
					break;
				default:
					break;
			}
		}


		private IEnumerator Login()
		{
			AuthTokenAcquirer authTokenAcquirer = null;
			AuthProvider authProvider = AuthProvider.GUEST;
			Log.V("GUEST Login");
			Log.H(string.Format("AuthProvider : {0}", authProvider));
			AuthToken authToken = null;
			do
			{
				authTokenAcquirer = AuthTokenAcquirer.Create(authProvider);
				yield return authTokenAcquirer.FetchToken();
				if (authTokenAcquirer.HasError())
				{
					Log.E("토큰 발급 실패 : " + authTokenAcquirer.GetError());
				}
				else
				{
					authToken = authTokenAcquirer.GetToken();
				}
			} while (authToken == null);

			bool loginSuccess = false;
			do
			{
				WaitForTrigger loginTrigger = new WaitForTrigger();
				RequestDelegate.request<AuthResult>(LobbyApi.authenticate(GetAuthParam(authProvider, authToken)), false,
					delegate(RequestDelegateError err, AuthResult res)
					{
						if (err != null)
						{
							Log.E("[Login] Failed. {0}", err.errorType);
							return;
						}

						Log.V("[Login] SUCCESS");
						loginSuccess = true;
						lastLoginTime = DateTime.Now;
						HttpRequestFactory.SetHeader("X-BSER-SessionKey", res.sessionKey);
						loginTrigger.ActiveTrigger();
					});
				yield return loginTrigger;
			} while (!loginSuccess);
		}


		private AuthParam GetAuthParam(AuthProvider authProvider, AuthToken authToken)
		{
			return new AuthParam
			{
				authProvider = authProvider.ToString(),
				idToken = StringUtil.RandomString(20),
				machineNum = SystemInfo.deviceUniqueIdentifier,
				paramMap = authToken.GetAttributesMap(),
				appVersion = BSERVersion.VERSION,
				appLanguageCode = Ln.GetCurrentLanguage().GetAppLanguageCode(),
				deviceLanguageCode = Application.systemLanguage.ToSupportLanguage().GetAppLanguageCode(),
				geoLocationCode = "ko"
			};
		}
	}
}