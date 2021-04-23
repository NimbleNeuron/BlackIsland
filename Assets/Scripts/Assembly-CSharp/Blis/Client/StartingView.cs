using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Blis.Client
{
	public class StartingView : BaseUI, IMapSlicePointerEventHandler
	{
		[SerializeField] private Image character = default;


		[SerializeField] private Image weapon = default;


		[SerializeField] private PositionTweener scaner = default;


		[SerializeField] private Text notice = default;


		[SerializeField] private Text timer = default;


		[SerializeField] private UIProgress progress = default;


		[SerializeField] private Text playerName = default;


		private readonly List<StrategySheet> allySheet = new List<StrategySheet>();


		private readonly List<Vector2> areaPosition = new List<Vector2>
		{
			new Vector2(240f, 11f),
			new Vector2(366f, 286f),
			new Vector2(57f, 169f),
			new Vector2(151f, 80f),
			new Vector2(212f, 458f),
			new Vector2(74f, 270f),
			new Vector2(304f, 365f),
			new Vector2(475f, 245f),
			new Vector2(405f, 376f),
			new Vector2(97f, 382f),
			new Vector2(310f, 181f),
			new Vector2(197f, 164f),
			new Vector2(400f, 60f),
			new Vector2(284f, 95f),
			new Vector2(169f, 329f)
		};


		private readonly List<CharacterCard> characterCards = new List<CharacterCard>();


		private readonly List<StrategySheet> enemySheets = new List<StrategySheet>();


		private readonly List<int> restrictedAreaList = new List<int>();


		private readonly List<StartingViewRouteButton> routeButtons = new List<StartingViewRouteButton>();


		private readonly List<int> routeList = new List<int>();


		private CanvasGroup _canvasGroup;


		private GraphicRaycaster _graphicRaycaster;


		private int appointedStartArea = -1;


		private int characterCode;


		private ChattingUI chattingUI;


		private Image clickBlock;


		private int failCount;


		private Transform favoriteListContent;


		private GameInput input;


		private bool isLockInput;


		private bool isOpen;


		private StartingViewRouteButton myRouteSlotPrefab;


		private StrategySheet mySheet;


		private WeaponType myStartingWeaponType;


		private RouteFilterUI routefilterUI;


		private bool showFourceStartBtn;


		private List<Favorite> startingViewFavorites;


		private UILineRenderer uiLineRendererFavorite;


		private UIMap uiMap;


		private int weaponCode;


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


		public void OnPointerMapClick(int areaCode, PointerEventData.InputButton button)
		{
			if (button == PointerEventData.InputButton.Left && uiMap.GetLaboratoryAreaCode() != areaCode)
			{
				appointedStartArea = areaCode;
				OnClickMap(areaCode);
			}
		}


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			input = gameObject.AddComponent<GameInput>();
			input.OnKeyPressed += OnKeyPressed;
			GameUtil.Bind<CanvasGroup>(gameObject, ref _canvasGroup);
			GameUtil.Bind<GraphicRaycaster>(gameObject, ref _graphicRaycaster);
			uiMap = GameUtil.Bind<UIMap>(gameObject, "Contents/Right/Maps");
			uiLineRendererFavorite = GameUtil.Bind<UILineRenderer>(uiMap.gameObject, "Areas/LineRendererFavorite");
			transform.FindRecursively("PlayerList").GetComponentsInChildren<CharacterCard>(characterCards);
			clickBlock = GameUtil.Bind<Image>(gameObject, "ClickBlock");
			chattingUI = GameUtil.Bind<ChattingUI>(gameObject, "Chatting");
			chattingUI.SetIsLockInput(IsLockInput);
			chattingUI.SetSendEvent(SendChatMessage);
			chattingUI.SetWaitInactive(false);
			routefilterUI = GameUtil.Bind<RouteFilterUI>(gameObject, "RouteList/RouteFilterUI");
			routefilterUI.changeFilterType += ChangeFilterType;
			favoriteListContent = GameUtil.Bind<Transform>(gameObject, "RouteList/ItemScrollView/Viewport/Contents");
			favoriteListContent.GetComponentsInChildren<StartingViewRouteButton>(true, routeButtons);
			myRouteSlotPrefab = routeButtons[0];
			routeButtons.ForEach(delegate(StartingViewRouteButton x) { x.onClickCallback += ChangeRoute; });
			Hide();
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			routeList.Clear();
			routeButtons.ForEach(delegate(StartingViewRouteButton x) { x.InitUI(); });
			uiMap.Init(MonoBehaviourInstance<ClientService>.inst.CurrentLevel);
			uiMap.SetEventHandler(this);
			uiLineRendererFavorite.color = GameConstants.UIColor.uiLineRendererFavorite;
			restrictedAreaList.Clear();
			restrictedAreaList.Add(uiMap.GetLaboratoryAreaCode());
			UpdateMapUI();
			MyPlayerContext myPlayer = MonoBehaviourInstance<ClientService>.inst.MyPlayer;
			if (myPlayer == null)
			{
				return;
			}

			characterCode = myPlayer.Character.CharacterCode;
			weaponCode = myPlayer.startingWeaponCode;
			ItemData itemData = GameDB.item.FindItemByCode(weaponCode);
			if (itemData != null)
			{
				WeaponTypeInfoData weaponTypeInfoData =
					GameDB.mastery.GetWeaponTypeInfoData(itemData.GetSubTypeData<ItemWeaponData>().weaponType);
				myStartingWeaponType = weaponTypeInfoData.type;
			}

			character.sprite =
				SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterFullSprite(characterCode,
					myPlayer.Character.SkinIndex);
			weapon.sprite = itemData.GetSprite();
			playerName.text = myPlayer.nickname;
			showFourceStartBtn = Debug.isDebugBuild;
		}


		private void UpdateMapUI()
		{
			uiMap.SetRouteText(routeList);
			uiMap.SetHighlightAreas(routeList, restrictedAreaList);
		}


		private void ChangeFilterType(RouteFilterType routeFilterType)
		{
			MonoBehaviourInstance<GameUI>.inst.NavigationHud.SetRouteFilterType(routeFilterType);
			startingViewFavorites = RouteApi.GetFavoritesByFilterType(routeFilterType);
			DrawRouteList();
			int num = startingViewFavorites.FindIndex(
				x => x == MonoBehaviourInstance<ClientService>.inst.SelectFavorite);
			if (num != -1)
			{
				routeButtons[num].SetSelected(true);
			}
		}


		private void DrawRouteList()
		{
			int num = Mathf.Max(0, startingViewFavorites.Count - routeButtons.Count);
			for (int i = 0; i < num; i++)
			{
				StartingViewRouteButton startingViewRouteButton =
					Instantiate<StartingViewRouteButton>(myRouteSlotPrefab, favoriteListContent);
				startingViewRouteButton.InitUI();
				startingViewRouteButton.onClickCallback += ChangeRoute;
				routeButtons.Add(startingViewRouteButton);
			}

			for (int j = 0; j < routeButtons.Count; j++)
			{
				routeButtons[j].gameObject.SetActive(false);
				if (j < startingViewFavorites.Count)
				{
					routeButtons[j].SetRouteButton(j, startingViewFavorites[j]);
				}
				else
				{
					routeButtons[j].SetSelected(false);
				}
			}
		}


		private void ChangeRoute(int routeIndex)
		{
			for (int i = 0; i < routeButtons.Count; i++)
			{
				routeButtons[i].SetSelected(i == routeIndex);
			}

			myStartingWeaponType = startingViewFavorites[routeIndex].weaponType;
			Favorite favorite = startingViewFavorites[routeIndex];
			MonoBehaviourInstance<ClientService>.inst.SetSelectFavorite(favorite);
			SingletonMonoBehaviour<PlayerController>.inst.UpdateStrategySheet(GetStartingAreaCode(myStartingWeaponType,
				favorite));
			DrawFavoritePath(favorite);
		}


		private int GetStartingAreaCode(WeaponType weaponType, Favorite favorite)
		{
			if (0 < appointedStartArea)
			{
				return appointedStartArea;
			}

			int result = -1;
			List<int> list = new List<int>();
			if (favorite != null)
			{
				list = favorite.paths;
				if (list.Count > 0)
				{
					result = list[0];
				}
			}
			else
			{
				result = GameDB.recommend.FindRecommendAreaData(characterCode, weaponType.GetWeaponMasteryType())
					.area1Code;
			}

			return result;
		}


		private void DrawFavoritePath(Favorite favorite)
		{
			routeList.Clear();
			if (favorite != null)
			{
				routeList.AddRange(favorite.paths);
			}

			UpdateMapUI();
			if (routeList.Count > 1)
			{
				uiLineRendererFavorite.gameObject.SetActive(true);
				uiLineRendererFavorite.Points = (from x in routeList
					select areaPosition[x - 1]).ToArray<Vector2>();
				uiLineRendererFavorite.SetVerticesDirty();
				return;
			}

			uiLineRendererFavorite.gameObject.SetActive(false);
		}


		private void OnClickMap(int areaCode)
		{
			SingletonMonoBehaviour<PlayerController>.inst.UpdateStrategySheet(areaCode);
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
			if (MonoBehaviourInstance<ClientService>.inst.MyPlayer != null &&
			    userId == MonoBehaviourInstance<ClientService>.inst.MyPlayer.userId)
			{
				mySheet = item;
			}
			else if (MonoBehaviourInstance<ClientService>.inst.MyPlayer != null && 0 < teamNumber &&
			         teamNumber == MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.TeamNumber)
			{
				allySheet.RemoveAll(x => x.userId == userId);
				allySheet.Add(item);
			}
			else
			{
				enemySheets.RemoveAll(x => x.userId == userId);
				enemySheets.Add(item);
			}

			UpdateMap();
		}


		private void UpdateMap()
		{
			uiMap.ClearPin();
			UpdateMapUI();
			DrawMyPin();
			DrawAllyPins();
			DrawEnemyPins();
		}


		private void DrawMyPin()
		{
			if (mySheet != null)
			{
				uiMap.HighlightArea(mySheet.startingAreaCode, Color.white);
				uiMap.PinAreaAlly(mySheet.startingAreaCode, mySheet.teamSlot, true);
			}
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
			clickBlock.gameObject.SetActive(false);
			this.StartThrowingCoroutine(StartTimer(standbySecond),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][StartTimer] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			this.StartThrowingCoroutine(PlaySoundCountDown(standbySecond),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][PlaySoundCountDown] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			UpdatePlayerList(MonoBehaviourInstance<ClientService>.inst.GetPlayers().ToList<PlayerContext>(),
				MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.TeamNumber);
		}


		private IEnumerator StartTimer(float standbySecond)
		{
			notice.text = Ln.Get("실험 시작까지 남은 시간");
			int breakTime = 5;
			DateTime startTime = DateTime.Now.AddSeconds(standbySecond - breakTime);
			TimeSpan timeSpan;
			do
			{
				yield return null;
				timeSpan = startTime - DateTime.Now;
				progress.SetValue(1f - (float) timeSpan.TotalSeconds / (standbySecond - breakTime));
				timer.text = string.Format("{0}", Mathf.CeilToInt((float) timeSpan.TotalSeconds));
			} while (timeSpan.TotalSeconds > 0.0);

			timer.text = null;
			notice.text = Ln.Get("잠시 뒤 실험이 시작됩니다.");
			clickBlock.gameObject.SetActive(true);
			isLockInput = true;
		}


		private IEnumerator PlaySoundCountDown(float TotalTime)
		{
			int countDown = 11;
			float num = 5f;
			float seconds = TotalTime - num - countDown;
			yield return new WaitForSeconds(seconds);
			int time = 0;
			do
			{
				Singleton<SoundControl>.inst.PlayUISound("StrategyMapCount");
				yield return new WaitForSeconds(1f);
				time++;
			} while (time != countDown);
		}


		private void UpdatePlayerList(List<PlayerContext> playerContext, int myTeamNumber)
		{
			playerContext.Sort(delegate(PlayerContext x, PlayerContext y)
			{
				if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == x.Character.ObjectId)
				{
					return -1;
				}

				if (MonoBehaviourInstance<ClientService>.inst.MyObjectId == y.Character.ObjectId)
				{
					return 1;
				}

				if (0 >= myTeamNumber)
				{
					return x.Character.ObjectId.CompareTo(y.Character.ObjectId);
				}

				if (myTeamNumber == x.Character.TeamNumber)
				{
					return -1;
				}

				if (myTeamNumber == y.Character.TeamNumber)
				{
					return 1;
				}

				return x.Character.TeamNumber.CompareTo(y.Character.TeamNumber);
			});
			characterCards.ForEach(delegate(CharacterCard x) { x.gameObject.SetActive(false); });
			int num = 0;
			while (num < playerContext.Count && num < characterCards.Count)
			{
				characterCards[num].gameObject.SetActive(true);
				characterCards[num].ResetCard();
				characterCards[num].SetUserId(playerContext[num].userId);
				characterCards[num].SetPlayerName(playerContext[num].nickname);
				characterCards[num].SetCharacter(playerContext[num].Character.CharacterCode,
					playerContext[num].Character.SkinIndex);
				bool isMyTeam =
					MonoBehaviourInstance<ClientService>.inst.MyObjectId == playerContext[num].Character.ObjectId ||
					0 < myTeamNumber && myTeamNumber == playerContext[num].Character.TeamNumber;
				int teamSlot = 0 < myTeamNumber ? playerContext[num].Character.TeamSlot : 1;
				characterCards[num].SetMyTeam(isMyTeam, teamSlot);
				characterCards[num].SetWeaponType(isMyTeam, playerContext[num].startingWeaponCode);
				num++;
			}

			scaner.PlayAnimation();
		}


		public void ForceStartGame()
		{
			SingletonMonoBehaviour<PlayerController>.inst.ForceStartGame();
		}


		public void Open()
		{
			isOpen = true;
			Singleton<SoundControl>.inst.PlayBGM("BSER_BGM_StrategyMap");
			chattingUI.AddSystemChatting(Ln.Get("게임문화개선캠페인"));
			failCount = 0;
			if (MonoBehaviourInstance<GameClient>.inst.MatchingMode == MatchingMode.Dev)
			{
				RouteApi.userFavorites.Clear();
			}
			else
			{
				RouteApi.SetFavoritesList(characterCode, SuccessFavorite, FailFavorite);
			}

			Show();
		}


		private void FailFavorite()
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsGameStarted)
			{
				return;
			}

			if (5 <= failCount)
			{
				return;
			}

			failCount++;
			RouteApi.SetFavoritesList(characterCode, SuccessFavorite, FailFavorite);
		}


		private void SuccessFavorite()
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsGameStarted)
			{
				return;
			}

			if (!isOpen)
			{
				return;
			}

			RouteApi.userFavorites = RouteApi.userFavorites.FindAll(x => x.weaponType == myStartingWeaponType);
			Favorite favorite = RouteApi.userFavorites[0];
			MonoBehaviourInstance<ClientService>.inst.SetSelectFavorite(favorite);
			SingletonMonoBehaviour<PlayerController>.inst.UpdateStrategySheet(GetStartingAreaCode(myStartingWeaponType,
				favorite));
			startingViewFavorites = RouteApi.GetFavoritesByFilterType(RouteFilterType.ALL);
			routefilterUI.ChangeFilterType(RouteFilterType.ALL);
			DrawRouteList();
			DrawFavoritePath(favorite);
			routeButtons[0].SetSelected(true);
		}


		private void ResetStartingViewFavorites()
		{
			startingViewFavorites.Clear();
			foreach (Favorite item in RouteApi.userFavorites)
			{
				startingViewFavorites.Add(item);
			}
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


		public void AddChatting(int senderObjectId, string characterName, string chatContent, bool isAll, bool isNotice,
			bool showTime)
		{
			chattingUI.AddChatting(senderObjectId, characterName, chatContent, isAll, isNotice, showTime);
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
	}
}