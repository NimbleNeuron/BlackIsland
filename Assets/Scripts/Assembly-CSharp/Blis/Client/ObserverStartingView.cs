using System;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class ObserverStartingView : BaseUI, IMapSlicePointerEventHandler
	{
		private readonly List<StrategySheet> allySheet = new List<StrategySheet>();


		private readonly List<StrategySheet> enemySheets = new List<StrategySheet>();
		private CanvasGroup _canvasGroup;


		private GraphicRaycaster _graphicRaycaster;


		private ChattingUI chattingUI;


		private Image clickBlock;


		private GameInput input;


		private bool isLockInput;


		private bool isOpen;


		private CharacterSelectObserverView observerView;


		private bool showFourceStartBtn;


		private UIMap uiMap;


		public bool IsOpen => isOpen;


		private void OnGUI()
		{
			if (showFourceStartBtn && GUI.Button(new Rect(0f, 0f, 200f, 60f), "강제 시작"))
			{
				ForceStartGame();
			}
		}


		public void OnPointerMapEnter(int areaCode)
		{
			uiMap.SetRollOver(areaCode, true);
		}


		public void OnPointerMapExit(int areaCode)
		{
			uiMap.SetRollOver(areaCode, false);
		}


		public void OnPointerMapClick(int areaCode, PointerEventData.InputButton button) { }


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref _canvasGroup);
			GameUtil.Bind<GraphicRaycaster>(gameObject, ref _graphicRaycaster);
			observerView = GameUtil.Bind<CharacterSelectObserverView>(gameObject, "ObserverView");
			observerView.SetOnClickExit(OnClickExit);
			observerView.SetOnClickTeam(OnClickTeam);
			input = gameObject.AddComponent<GameInput>();
			input.OnKeyPressed += OnKeyPressed;
			uiMap = GameUtil.Bind<UIMap>(gameObject, "ObserverView/Content/MapWindow/Map/Maps");
			clickBlock = GameUtil.Bind<Image>(gameObject, "ClickBlock");
			chattingUI = GameUtil.Bind<ChattingUI>(gameObject, "Chatting");
			chattingUI.SetIsLockInput(IsLockInput);
			chattingUI.SetSendEvent(SendChatMessage);
			chattingUI.SetWaitInactive(false);
			Hide();
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			uiMap.Init(MonoBehaviourInstance<ClientService>.inst.CurrentLevel);
			uiMap.SetEventHandler(this);
			uiMap.SetHighlightAreas(new List<int>(), new List<int>
			{
				uiMap.GetLaboratoryAreaCode()
			});
			showFourceStartBtn = Debug.isDebugBuild;
			chattingUI.gameObject.SetActive(false);
			isLockInput = true;
		}


		public void Open()
		{
			isOpen = true;
			Singleton<SoundControl>.inst.PlayBGM("BSER_BGM_StrategyMap");
			Show();
		}


		private void Show()
		{
			_canvasGroup.alpha = 1f;
			_graphicRaycaster.enabled = true;
			gameObject.SetActive(true);
		}


		public void Close()
		{
			isOpen = false;
			chattingUI.EraseChatting();
			Hide();
			if (input != null)
			{
				Destroy(input);
			}
		}


		private void Hide()
		{
			_canvasGroup.alpha = 0f;
			_graphicRaycaster.enabled = false;
			gameObject.SetActive(false);
		}


		public void ForceStartGame()
		{
			MonoBehaviourInstance<GameClient>.inst.Request(new ReqForceStartGame());
		}


		public void OnUpdateStrategySheets(long userId, int teamNumber, int teamSlot, int startingAreaCode)
		{
			StrategySheet item = new StrategySheet
			{
				userId = userId,
				startingAreaCode = startingAreaCode,
				teamNumber = teamNumber,
				teamSlot = 0 < teamNumber ? teamSlot : 1
			};
			allySheet.RemoveAll(x => x.userId == userId);
			enemySheets.RemoveAll(x => x.userId == userId);
			if (MonoBehaviourInstance<ClientService>.inst.MyObserver != null && 0 < teamNumber &&
			    teamNumber == MonoBehaviourInstance<ClientService>.inst.MyObserver.Observer.TeamNumber)
			{
				allySheet.Add(item);
			}
			else
			{
				enemySheets.Add(item);
			}

			UpdateMap();
		}


		private void UpdateMap()
		{
			uiMap.ClearPin();
			DrawAllyPins();
			DrawEnemyPins();
		}


		private void DrawAllyPins()
		{
			Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
			for (int i = 0; i < allySheet.Count; i++)
			{
				int startingAreaCode = allySheet[i].startingAreaCode;
				if (!dictionary.ContainsKey(startingAreaCode))
				{
					dictionary.Add(startingAreaCode, new List<int>());
				}

				dictionary[startingAreaCode].Add(allySheet[i].teamSlot);
			}

			foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
			{
				foreach (int teamSlot in keyValuePair.Value)
				{
					uiMap.PinAreaAlly(keyValuePair.Key, teamSlot, true);
				}
			}
		}


		private void DrawEnemyPins()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < enemySheets.Count; i++)
			{
				int startingAreaCode = enemySheets[i].startingAreaCode;
				if (dictionary.ContainsKey(startingAreaCode))
				{
					Dictionary<int, int> dictionary2 = dictionary;
					int key = startingAreaCode;
					int num = dictionary2[key];
					dictionary2[key] = num + 1;
				}
				else
				{
					dictionary.Add(startingAreaCode, 1);
				}
			}

			foreach (KeyValuePair<int, int> keyValuePair in dictionary)
			{
				uiMap.PinAreaEnemy(keyValuePair.Key, keyValuePair.Value);
			}
		}


		public void StartStrategy(int standbySecond)
		{
			try
			{
				clickBlock.gameObject.SetActive(false);
				observerView.SetPhase(Ln.Get("실험 시작까지 남은 시간"), standbySecond, 5);
				observerView.SetFinishTimerAction(OnFinishTimer);
				GameClient inst = MonoBehaviourInstance<GameClient>.inst;
				if (inst == null)
				{
					throw new GameException("GameClient is Null");
				}

				observerView.OpenStartingView(inst.MatchingTeamMode);
				ClientService inst2 = MonoBehaviourInstance<ClientService>.inst;
				if (inst2 == null)
				{
					throw new GameException("ClientService is Null");
				}

				LocalPlayerCharacter firstPlayer = inst2.GetFirstPlayer();
				if (firstPlayer == null)
				{
					throw new GameException("FirstPlayer is Null");
				}

				OnClickTeam(firstPlayer.TeamNumber, firstPlayer.TeamSlot);
			}
			catch (Exception ex)
			{
				Log.V("[EXCEPTION] " + ex.Message + ": " + ex.StackTrace);
			}
		}


		private bool IsLockInput()
		{
			return isLockInput;
		}


		private void SendChatMessage(string chatContent)
		{
			if (IsLockInput())
			{
				return;
			}

			if (string.IsNullOrEmpty(chatContent))
			{
				return;
			}

			chatContent = ArchStringUtil.CutOverSizeANSI(chatContent, 100);
			MonoBehaviourInstance<GameClient>.inst.Request(new ReqChat
			{
				chatContent = chatContent
			});
		}


		public void AddChatting(string nickName, string characterName, string chatContent, bool isAll, bool isNotice,
			bool showTime)
		{
			chattingUI.AddChatting(nickName, characterName, chatContent, isAll, isNotice, showTime);
		}


		public void OnKeyPressed(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (inputEvent == GameInputEvent.Escape && IsOpen && chattingUI.IsFocus)
			{
				chattingUI.DeactivateInput();
				return;
			}

			if (IsLockInput())
			{
				return;
			}

			if (inputEvent - GameInputEvent.ChatActive <= 1)
			{
				chattingUI.EnterChat(false);
				return;
			}

			if (inputEvent == GameInputEvent.AllChatActive)
			{
				chattingUI.EnterChat(false);
				return;
			}

			if (inputEvent != GameInputEvent.ForceStartGame)
			{
				return;
			}

			ForceStartGame();
		}


		private void OnClickTeam(int teamNumber, int teamSlot)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			if (teamNumber == 0 || teamSlot == 0)
			{
				return;
			}

			ObserverHostileAgent hostileAgent =
				MonoBehaviourInstance<ClientService>.inst.MyObserver.Observer.HostileAgent;
			hostileAgent.SelectTeamNumber(teamNumber);
			hostileAgent.SelectTeamSlot(teamSlot);
			observerView.SelectTeam(teamNumber);
			for (int i = 0; i < allySheet.Count; i++)
			{
				if (allySheet[i].teamNumber != teamNumber)
				{
					enemySheets.Add(allySheet[i]);
					allySheet.RemoveAt(i);
					i--;
				}
			}

			for (int j = 0; j < enemySheets.Count; j++)
			{
				if (enemySheets[j].teamNumber == teamNumber)
				{
					allySheet.Add(enemySheets[j]);
					enemySheets.RemoveAt(j);
					j--;
				}
			}

			UpdateMap();
		}


		private void OnClickExit() { }


		private void OnFinishTimer()
		{
			clickBlock.gameObject.SetActive(true);
		}
	}
}