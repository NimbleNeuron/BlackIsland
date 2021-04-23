using System;
using System.Collections;
using Blis.Client;
using Blis.Common.Utils;
using Blis.Server;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Blis.Common
{
	public class Bootstrap : SingletonMonoBehaviour<Bootstrap>
	{
		public enum Mode
		{
			Standalone,
			Host,
			Client
		}

		private const string LobbySceneName = "Lobby";
		private const string LocalGameSceneName = "Game";
		private const string ServerGameSceneName = "GameServer_1";

		public static readonly string defaultHost = "localhost:" + GameConstants.DefaultPort;
		
		private string currentScene = "Lobby";
		private string host = defaultHost;

		private Mode mode;
		public bool IsLobbyScene => currentScene.Equals("Lobby");
		public bool IsGameScene => currentScene.StartsWith("Game");

		protected override void OnAwakeSingleton()
		{
			Debug.developerConsoleVisible = false;
			DontDestroyOnLoad(this);
		}

		public void LoadLobby()
		{
			SingletonMonoBehaviour<GameAnalytics>.inst.SetLoadScene("Lobby");
			SceneManager.LoadScene("Lobby");
			currentScene = "Lobby";
			SingletonMonoBehaviour<GameAnalytics>.inst.SetLoadScene("complete");
		}

		private IEnumerator LoadSceneAsync(string sceneName, Action callback)
		{
			SingletonMonoBehaviour<GameAnalytics>.inst.SetLoadScene(sceneName);
			Log.V("[LoadSceneAsync] " + sceneName);
			currentScene = sceneName;
			LoadingView.inst.SetActive(true);
			LoadingView.inst.LoadingContext = new LoadingContext();
			yield return null;
			AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
			while (!async.isDone)
			{
				LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.SceneLoad, async.progress);
				yield return null;
			}

			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.SceneLoad, async.progress);
			SingletonMonoBehaviour<GameAnalytics>.inst.SetLoadScene("complete");
			callback();
		}

		private IEnumerator LoadScene(string sceneName, Action callback)
		{
			SingletonMonoBehaviour<GameAnalytics>.inst.SetLoadScene(sceneName);
			Log.V("[LoadSceneAsync] " + sceneName);
			currentScene = sceneName;
			LoadingView.inst.SetActive(true);
			LoadingView.inst.LoadingContext = new LoadingContext();
			yield return null;
			SceneManager.LoadScene(sceneName);
			yield return null;
			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.SceneLoad, 100f);
			SingletonMonoBehaviour<GameAnalytics>.inst.SetLoadScene("complete");
			callback();
		}

		public void CreateServer()
		{
			new GameObject("Server").AddComponent<GameServer>();
		}

		public void LoadServer()
		{
			SingletonMonoBehaviour<ResourceManager>.inst.UnloadAll();
			StartCoroutine(LoadSceneAsync("GameServer_1", delegate
			{
				SingletonMonoBehaviour<ResourceManager>.inst.LoadInServer();
				Instantiate<GameObject>(Resources.Load<GameObject>("WorldObjects/Server/ServerEnviroment"));
				CreateServer();
			}));
		}

		public void LoadClient(MatchingResult matchingResult, long userNum, bool useVIP, bool isOldGame = false)
		{
			SingletonMonoBehaviour<ResourceManager>.inst.UnloadAll();
			StartCoroutine(LoadSceneAsync(GetLocalGameSceneName(), delegate
			{
				MonoBehaviourInstance<GameClient>.inst.Init(matchingResult, userNum, useVIP, isOldGame);
				MonoBehaviourInstance<GameClient>.inst.Connect();
				MatchingMode matchingMode = matchingResult.matchingMode;
				if (matchingMode == MatchingMode.Custom)
				{
					CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "InCustomGame");
					return;
				}

				CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "InGame");
			}));
		}

		public void LoadTutorial(TutorialType tutorialType, string host, int botCount, BotDifficulty botDifficulty,
			MatchingToken matchingToken)
		{
			SingletonMonoBehaviour<ResourceManager>.inst.UnloadAll();
			StartCoroutine(LoadSceneAsync(GetLocalGameSceneName(), delegate
			{
				CreateServer();
				MonoBehaviourInstance<GameClient>.inst.InitTutorial(tutorialType, true, host, botCount, botDifficulty,
					matchingToken);
			}));
		}

		public void LoadTest(Mode mode, string host, int botCount, BotDifficulty botDifficulty, int teamNumber,
			MatchingToken matchingToken, bool isLoadAsync = true)
		{
			if (isLoadAsync)
			{
				StartCoroutine(LoadSceneAsync(GetLocalGameSceneName(), SceneLoadDoneCallback));
			}
			else
			{
				StartCoroutine(LoadScene(GetLocalGameSceneName(), SceneLoadDoneCallback));
			}

			void SceneLoadDoneCallback()
			{
				CommunityService.SetRichPresence(CommunityRichPresenceType.GAME_STATUS, "InGame");
				switch (mode)
				{
					case Mode.Standalone:
						CreateServer();
						MonoBehaviourInstance<GameClient>.inst.InitTester(true, host, botCount, botDifficulty,
							teamNumber, matchingToken);
						break;
					case Mode.Host:
						CreateServer();
						MonoBehaviourInstance<GameClient>.inst.InitTester(false, host, botCount, botDifficulty,
							teamNumber, matchingToken);
						MonoBehaviourInstance<GameClient>.inst.Connect();
						break;
					case Mode.Client:
						MonoBehaviourInstance<GameClient>.inst.InitTester(false, host, botCount, botDifficulty,
							teamNumber, matchingToken);
						MonoBehaviourInstance<GameClient>.inst.Connect();
						break;
				}
			}
		}

		private string GetLocalGameSceneName()
		{
			return "Game";
		}
	}
}