using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class GameUI : MonoBehaviourInstance<GameUI>
	{
		private readonly List<BaseUI> initializeList = new List<BaseUI>();
		private AnnounceMessageUI announceMessage;
		private BattleInfoHud battleInfoHud;
		private BloodEffect bloodFx;
		private CastingHud caster;
		private ChattingUI chattingUI;
		private CombineWindow combineWindow;


		private EmotionPlateHud emotionPlateHud;


		private GameUIEvent events;


		private GameResult gameResult;


		private HudButton hudButton;


		private Canvas hudCanvas;


		private HyperloopWindow hyperloopWindow;


		private InventoryHud inventoryHud;


		private List<BaseWindow> layer2Windows;


		private GameObject mainHud;


		private MapWindow mapWindow;


		private MasteryExpHud masteryExpHud;


		private MasteryWindow masteryWindow;


		private MinimapUI minimap;


		private NavigationHud navigationHud;


		private ObserverHud observerHud;


		private ObserverStartingView observerStartingView;


		private Transform overlayUI;


		private PingUI ping;


		private PingHud pingHud;


		private ProductionGoalWindow productionGoalWindow;


		private RestrictedAreaHud restrictedArea;


		private ScoreboardWindow scoreboardWindow;


		private Transform serverIndicator;


		private SettingWindow settingWindow;


		private ShortCutCraftHud shortCutCraftHud;


		private SkillHud skillHud;


		private SpecialAnnounceUI specialAnnounceUI;


		private StartingView startingView;


		private StatusHud statusHud;


		private GameObject stelthOverlayUI;


		private TargetInfoHud targetInfoHud;


		private TeamHud teamHud;


		private ToastMessageUI toastMessage;


		private Tutorial tutorial;


		private UITracker uiTracker;


		private WatchingHud watchingHud;


		public GameUIEvent Events => events;


		public InventoryHud InventoryHud => inventoryHud;


		public StatusHud StatusHud => statusHud;


		public TeamHud TeamHud => teamHud;


		public SkillHud SkillHud => skillHud;


		public BattleInfoHud BattleInfoHud => battleInfoHud;


		public RestrictedAreaHud RestrictedArea => restrictedArea;


		public NavigationHud NavigationHud => navigationHud;


		public HudButton HudButton => hudButton;


		public MinimapUI Minimap => minimap;


		public TargetInfoHud TargetInfoHud => targetInfoHud;


		public ShortCutCraftHud ShortCutCraftHud => shortCutCraftHud;


		public GameObject MainHud => mainHud;


		public WatchingHud WatchingHud => watchingHud;


		public MapWindow MapWindow => mapWindow;


		public MasteryWindow MasteryWindow => masteryWindow;


		public CombineWindow CombineWindow => combineWindow;


		public SettingWindow SettingWindow => settingWindow;


		public ScoreboardWindow ScoreboardWindow => scoreboardWindow;


		public HyperloopWindow HyperloopWindow => hyperloopWindow;


		public ProductionGoalWindow ProductionGoalWindow => productionGoalWindow;


		public StartingView StartingView => startingView;


		public ObserverStartingView ObserverStartingView => observerStartingView;


		public GameResult GameResult => gameResult;


		public ToastMessageUI ToastMessage => toastMessage;


		public AnnounceMessageUI AnnounceMessage => announceMessage;


		public CastingHud Caster => caster;


		public Tutorial Tutorial => tutorial;


		public UITracker UITracker => uiTracker;


		public Transform OverlayUI => overlayUI;


		public PingUI Ping => ping;


		public BloodEffect BloodFx => bloodFx;


		public GameObject StelthOverlayUI => stelthOverlayUI;


		public MasteryExpHud MasteryExpHud => masteryExpHud;


		public ChattingUI ChattingUi => chattingUI;


		public PingHud PingHud => pingHud;


		public EmotionPlateHud EmotionPlateHud => emotionPlateHud;


		public ObserverHud ObserverHud => observerHud;


		public Transform ServerIndicator => serverIndicator;


		public SpecialAnnounceUI SpecialAnnounceUI => specialAnnounceUI;


		protected override void _Awake()
		{
			SingletonMonoBehaviour<ResourceManager>.inst.LoadInWorld();
			hudCanvas = GameUtil.Bind<Canvas>(gameObject, "HUD");
			events = new GameUIEvent(this);
			inventoryHud = GameUtil.Bind<InventoryHud>(gameObject, "HUD/MainHud/Inven");
			statusHud = GameUtil.Bind<StatusHud>(gameObject, "HUD/MainHud/Status");
			teamHud = GameUtil.Bind<TeamHud>(gameObject, "HUD/TeamHud");
			skillHud = GameUtil.Bind<SkillHud>(gameObject, "HUD/MainHud/Skill");
			masteryExpHud = GameUtil.Bind<MasteryExpHud>(gameObject, "HUD/MainHud/MasteryExp");
			restrictedArea = GameUtil.Bind<RestrictedAreaHud>(gameObject, "HUD/RestrictedArea");
			navigationHud = GameUtil.Bind<NavigationHud>(gameObject, "HUD/NavigationHud");
			hudButton = GameUtil.Bind<HudButton>(gameObject, "HUD/HudButton");
			battleInfoHud = GameUtil.Bind<BattleInfoHud>(gameObject, "HUD/BattleInfo");
			minimap = GameUtil.Bind<MinimapUI>(gameObject, "HUD/Minimap");
			targetInfoHud = GameUtil.Bind<TargetInfoHud>(gameObject, "HUD/TargetInfoHud");
			shortCutCraftHud = GameUtil.Bind<ShortCutCraftHud>(gameObject, "HUD/MainHud/ShortCutCraft");
			chattingUI = GameUtil.Bind<ChattingUI>(gameObject, "HUD/Chatting");
			chattingUI.SetIsLockInput(IsLockInput);
			chattingUI.SetSendEvent(SendChatMessage);
			mainHud = transform.Find("HUD/MainHud").gameObject;
			watchingHud = GameUtil.Bind<WatchingHud>(gameObject, "HUD/WatchingHud");
			observerHud = GameUtil.Bind<ObserverHud>(gameObject, "HUD/ObserverHud");
			mapWindow = GameUtil.Bind<MapWindow>(gameObject, "Windows/MapWindow");
			masteryWindow = GameUtil.Bind<MasteryWindow>(gameObject, "Windows/MasteryUI");
			combineWindow = GameUtil.Bind<CombineWindow>(gameObject, "Windows/CombineUI");
			settingWindow = GameUtil.Bind<SettingWindow>(gameObject, "Windows/SettingWindow");
			scoreboardWindow = GameUtil.Bind<ScoreboardWindow>(gameObject, "Windows/ScoreboardWindow");
			hyperloopWindow = GameUtil.Bind<HyperloopWindow>(gameObject, "Windows/HyperloopWindow");
			productionGoalWindow = GameUtil.Bind<ProductionGoalWindow>(gameObject, "Windows/ProductionGoalWindow");
			overlayUI = GameUtil.Bind<Transform>(gameObject, "Overlay");
			startingView = GameUtil.Bind<StartingView>(overlayUI.gameObject, "StartingView");
			observerStartingView = GameUtil.Bind<ObserverStartingView>(overlayUI.gameObject, "ObserverStartingView");
			tutorial = GameUtil.Bind<Tutorial>(overlayUI.gameObject, "Tutorial");
			gameResult = GameUtil.Bind<GameResult>(overlayUI.gameObject, "GameResult");
			caster = GameUtil.Bind<CastingHud>(overlayUI.gameObject, "Caster");
			toastMessage = GameUtil.Bind<ToastMessageUI>(overlayUI.gameObject, "ToastMessage");
			announceMessage = GameUtil.Bind<AnnounceMessageUI>(overlayUI.gameObject, "AnnounceMessage");
			pingHud = GameUtil.Bind<PingHud>(overlayUI.gameObject, "PingHud");
			emotionPlateHud = GameUtil.Bind<EmotionPlateHud>(overlayUI.gameObject, "EmotionPlateHud");
			specialAnnounceUI = GameUtil.Bind<SpecialAnnounceUI>(overlayUI.gameObject, "SpecialAnnounceUI");
			uiTracker = GameUtil.Bind<UITracker>(gameObject, "Tracker");
			ping = GameUtil.Bind<PingUI>(gameObject, "HUD/Ping");
			stelthOverlayUI = transform.Find("StelthOverlayUI").gameObject;
			bloodFx = GameUtil.Bind<BloodEffect>(gameObject, "BloodEffect");
			serverIndicator = GameUtil.Bind<Transform>(gameObject, "ServerIndicator");
			layer2Windows = new List<BaseWindow>
			{
				mapWindow,
				masteryWindow,
				combineWindow,
				scoreboardWindow,
				hyperloopWindow
			};
			GetComponentsInChildren<BaseUI>(true, initializeList);
			foreach (BaseUI baseUI in initializeList)
			{
				baseUI.AwakeUI();
			}

			UISceneContext.currentSceneState = UISceneContext.SceneState.UIAwaked;
			stelthOverlayUI.SetActive(false);
		}


		private bool IsLockInput()
		{
			return SingletonMonoBehaviour<PlayerController>.inst.IsLockInput;
		}


		private void SendChatMessage(string chatContent)
		{
			if (string.IsNullOrEmpty(chatContent))
			{
				return;
			}

			chatContent = ArchStringUtil.CutOverSizeANSI(chatContent, 100);
			if (MonoBehaviourInstance<ClientService>.inst.ChatCommandService.IsChatCommand(chatContent))
			{
				chattingUI.ClearInput();
				return;
			}

			MonoBehaviourInstance<GameClient>.inst.Request(new ReqChat
			{
				chatContent = chatContent
			});
		}


		public void OnSetupGame(int standbyTime)
		{
			foreach (BaseUI baseUI in initializeList)
			{
				baseUI.StartUI();
			}

			UISceneContext.currentSceneState = UISceneContext.SceneState.UIStarted;
			events.Init();
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				LoadingView.inst.SetActive(false);
				HidePlayerUI();
				if (standbyTime > 0)
				{
					observerStartingView.Open();
					observerStartingView.StartStrategy(standbyTime);
				}

				return;
			}

			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				startingView.Close();
				startingView.ForceStartGame();
				return;
			}

			if (standbyTime > 0)
			{
				startingView.Open();
				startingView.StartStrategy(standbyTime);
			}

			LoadingView.inst.SetActive(false);
		}


		public void HidePlayerUI()
		{
			mainHud.gameObject.SetActive(false);
			restrictedArea.EnableSurvivableTimer(false);
			battleInfoHud.EnableBattleScore(false);
			navigationHud.gameObject.SetActive(false);
			navigationHud.ShowNaviAreaItem(false);
			teamHud.gameObject.SetActive(false);
			tutorial.gameObject.SetActive(false);
			emotionPlateHud.gameObject.SetActive(false);
			hudButton.EnableRestBtn(false);
			watchingHud.SetActive(false);
		}


		protected override void _OnDestroy()
		{
			base._OnDestroy();
			UISceneContext.currentSceneState = UISceneContext.SceneState.Loading;
		}


		public void OnStartGame()
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				startingView.Close();
				if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
				{
					LoadingView.inst.SetActive(false);
				}

				statusHud.UpdateBuffPosition();
				return;
			}

			observerStartingView.Close();
			observerHud.Open();
		}


		public void OnPressingKey(GameInputEvent inputEvent)
		{
			if (gameResult.IsActive())
			{
				return;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsGameEnd || IsSettingWindowOpen())
			{
				return;
			}

			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			if (SingletonMonoBehaviour<PlayerController>.inst.IsLockInput)
			{
				return;
			}

			OnPressingEvent(inputEvent);
		}


		public void OnPressKey(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (gameResult.IsActive())
			{
				return;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsGameEnd || IsSettingWindowOpen())
			{
				return;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				OnInputEventForPlayer(inputEvent, mousePosition);
				return;
			}

			OnInputEventForObserver(inputEvent, mousePosition);
		}


		private void OnInputEventForPlayer(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (!MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			switch (inputEvent)
			{
				case GameInputEvent.OpenMap:
					ToggleWindow(mapWindow);
					return;
				case GameInputEvent.OpenScoreboard:
					ToggleWindow(scoreboardWindow);
					return;
				case GameInputEvent.OpenCharacterMastery:
					if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsAlive)
					{
						ToggleWindow(masteryWindow);
					}

					break;
				case GameInputEvent.OpenCharacterStat:
					if (MonoBehaviourInstance<ClientService>.inst.MyPlayer.Character.IsAlive)
					{
						statusHud.ToggleStatExtension();
					}

					break;
				case GameInputEvent.OpenCombineWindow:
					ToggleWindow(combineWindow);
					return;
				case GameInputEvent.ChangeCameraMode:
					if (MonoBehaviourInstance<MobaCamera>.inst.Mode == MobaCameraMode.Tracking)
					{
						if (MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
						    MonoBehaviourInstance<GameClient>.inst.MatchingMode == MatchingMode.Tutorial1)
						{
							return;
						}

						MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Traveling);
						Singleton<LocalSetting>.inst.SaveHoldCamera(false);
					}
					else if (MonoBehaviourInstance<MobaCamera>.inst.Mode == MobaCameraMode.Traveling)
					{
						MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Tracking);
						Singleton<LocalSetting>.inst.SaveHoldCamera(true);
					}

					break;
				case GameInputEvent.ResetCamera:
					if (!pingHud.IsActive())
					{
						MonoBehaviourInstance<MobaCamera>.inst.HoldCameraInTarget(true);
					}

					break;
				case GameInputEvent.ShowObjectText:
				case GameInputEvent.Escape:
					break;
				case GameInputEvent.ShowRouteList:
					navigationHud.ShowRouteList();
					return;
				case GameInputEvent.ObserveNextPlayer:
					if (MonoBehaviourInstance<ClientService>.inst.CurGamePlayMode ==
						ClientService.GamePlayMode.ObserveTeam && !pingHud.IsActive())
					{
						MonoBehaviourInstance<MobaCamera>.inst.TrackingAliveTeamPlayer(true);
					}

					break;
				case GameInputEvent.ObservePreviousPlayer:
					if (MonoBehaviourInstance<ClientService>.inst.CurGamePlayMode ==
						ClientService.GamePlayMode.ObserveTeam && !pingHud.IsActive())
					{
						MonoBehaviourInstance<MobaCamera>.inst.TrackingAliveTeamPlayer(false);
					}

					break;
				default:
					if (inputEvent != GameInputEvent.QuickCombine)
					{
						switch (inputEvent)
						{
							case GameInputEvent.CameraTeam1:
								MonoBehaviourInstance<MobaCamera>.inst.StartTeamMoveCamera(1);
								return;
							case GameInputEvent.CameraTeam2:
								MonoBehaviourInstance<MobaCamera>.inst.StartTeamMoveCamera(2);
								return;
							case GameInputEvent.CameraTeam3:
								MonoBehaviourInstance<MobaCamera>.inst.StartTeamMoveCamera(3);
								break;
							default:
								return;
						}
					}
					else if (shortCutCraftHud.CombineableItems.Count > 0)
					{
						SingletonMonoBehaviour<PlayerController>.inst.MakeItem(shortCutCraftHud.CombineableItems[0]);
					}

					break;
			}
		}


		private void OnInputEventForObserver(GameInputEvent inputEvent, Vector3 mousePosition)
		{
			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
			{
				return;
			}

			switch (inputEvent)
			{
				case GameInputEvent.Active1:
					ObserverHud.SelectTarget(1);
					return;
				case GameInputEvent.Active2:
					ObserverHud.SelectTarget(2);
					return;
				case GameInputEvent.Active3:
					ObserverHud.SelectTarget(3);
					return;
				case GameInputEvent.Active4:
				case GameInputEvent.WeaponSkill:
				case GameInputEvent.LearnPassive:
				case GameInputEvent.LearnActive1:
				case GameInputEvent.LearnActive2:
				case GameInputEvent.LearnActive3:
				case GameInputEvent.LearnActive4:
				case GameInputEvent.LearnWeaponSkill:
				case GameInputEvent.OpenCharacterMastery:
				case GameInputEvent.OpenCharacterStat:
				case GameInputEvent.ShowObjectText:
				case GameInputEvent.Escape:
				case GameInputEvent.ShowRouteList:
				case GameInputEvent.ObserveNextPlayer:
				case GameInputEvent.ObservePreviousPlayer:
					break;
				case GameInputEvent.OpenMap:
					ToggleWindow(mapWindow);
					return;
				case GameInputEvent.OpenScoreboard:
					ToggleWindow(scoreboardWindow);
					return;
				case GameInputEvent.OpenCombineWindow:
					ToggleWindow(combineWindow);
					return;
				case GameInputEvent.ChangeCameraMode:
					if (MonoBehaviourInstance<MobaCamera>.inst.Mode == MobaCameraMode.Tracking)
					{
						if (MonoBehaviourInstance<GameClient>.inst.IsTutorial &&
						    MonoBehaviourInstance<GameClient>.inst.MatchingMode == MatchingMode.Tutorial1)
						{
							return;
						}

						MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Traveling);
					}
					else if (MonoBehaviourInstance<MobaCamera>.inst.Mode == MobaCameraMode.Traveling)
					{
						MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Tracking);
					}

					break;
				case GameInputEvent.ResetCamera:
					if (!pingHud.IsActive())
					{
						MonoBehaviourInstance<MobaCamera>.inst.HoldCameraInTarget(true);
					}

					break;
				case GameInputEvent.Alpha1:
					ObserverHud.SelectTeam(1);
					return;
				case GameInputEvent.Alpha2:
					ObserverHud.SelectTeam(2);
					return;
				case GameInputEvent.Alpha3:
					ObserverHud.SelectTeam(3);
					return;
				case GameInputEvent.Alpha4:
					ObserverHud.SelectTeam(4);
					return;
				case GameInputEvent.Alpha5:
					ObserverHud.SelectTeam(5);
					return;
				case GameInputEvent.Alpha6:
					ObserverHud.SelectTeam(6);
					return;
				case GameInputEvent.Alpha7:
					ObserverHud.SelectTeam(7);
					return;
				case GameInputEvent.Alpha8:
					ObserverHud.SelectTeam(8);
					return;
				case GameInputEvent.Alpha9:
					ObserverHud.SelectTeam(9);
					return;
				default:
					switch (inputEvent)
					{
						case GameInputEvent.CameraTeam1:
							MonoBehaviourInstance<MobaCamera>.inst.StartTeamMoveCamera(1);
							return;
						case GameInputEvent.CameraTeam2:
							MonoBehaviourInstance<MobaCamera>.inst.StartTeamMoveCamera(2);
							return;
						case GameInputEvent.CameraTeam3:
							MonoBehaviourInstance<MobaCamera>.inst.StartTeamMoveCamera(3);
							break;
						default:
							return;
					}

					break;
			}
		}


		private void OnPressingEvent(GameInputEvent inputEvent) { }


		private bool IsSettingWindowOpen()
		{
			return settingWindow.IsOpen;
		}


		public void ToggleWindow(BaseWindow window)
		{
			if (window.IsOpen)
			{
				window.Close();
				return;
			}

			OpenWindow(window);
		}


		public void OpenWindow(BaseWindow window)
		{
			for (int i = 0; i < layer2Windows.Count; i++)
			{
				if (layer2Windows[i] != window)
				{
					layer2Windows[i].Close();
				}
			}

			window.Open();
		}


		public void Escape()
		{
			if (BaseWindow.FocusedWindow == null)
			{
				settingWindow.Open();
				return;
			}

			BaseWindow focusedWindow = BaseWindow.FocusedWindow;
			if (focusedWindow == null)
			{
				return;
			}

			focusedWindow.Close();
		}


		public void OnReleaseKey(GameInputEvent gameInputEvent, Vector3 mousePos)
		{
			if (gameInputEvent != GameInputEvent.ResetCamera)
			{
				switch (gameInputEvent)
				{
					case GameInputEvent.CameraTeam1:
						if (SingletonMonoBehaviour<PlayerController>.inst.IsLockInput)
						{
							return;
						}

						MonoBehaviourInstance<MobaCamera>.inst.StopTeamMoveCamera(1);
						return;
					case GameInputEvent.CameraTeam2:
						if (SingletonMonoBehaviour<PlayerController>.inst.IsLockInput)
						{
							return;
						}

						MonoBehaviourInstance<MobaCamera>.inst.StopTeamMoveCamera(2);
						return;
					case GameInputEvent.CameraTeam3:
						if (SingletonMonoBehaviour<PlayerController>.inst.IsLockInput)
						{
							return;
						}

						MonoBehaviourInstance<MobaCamera>.inst.StopTeamMoveCamera(3);
						break;
					default:
						return;
				}
			}
			else if (!pingHud.IsActive())
			{
				MonoBehaviourInstance<MobaCamera>.inst.HoldCameraInTarget(false);
			}
		}


		public void ChangeEnableHud()
		{
			hudCanvas.enabled = !hudCanvas.enabled;
		}


		private void PopupServerErrorWithCommunity()
		{
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg = Ln.Get(BlisWebSocketError.OutOfService.ToString());
			Popup.Button[] array = new Popup.Button[2];
			array[0] = new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("확인")
			};
			int num = 1;
			Popup.Button button = new Popup.Button();
			button.type = Popup.ButtonType.Cancel;
			button.text = Ln.Get("고객문의");
			button.callback = delegate { Application.OpenURL(Ln.Get("고객지원 링크")); };
			array[num] = button;
			inst.Message(msg, array);
		}


		public void HudUpdateLevel(int objectId, int level)
		{
			teamHud.SetLevel(objectId, level);
			observerHud.SetLevel(objectId, level);
		}


		public void HudUpdateHp(int objectId, int hp, int maxHp)
		{
			teamHud.SetHp(objectId, hp, maxHp);
			observerHud.SetHp(objectId, hp, maxHp);
		}


		public void HudUpdateSp(int objectId, int sp, int maxSp)
		{
			teamHud.SetSp(objectId, sp, maxSp);
			observerHud.SetSp(objectId, sp, maxSp);
		}


		public void HudUpdateEp(int objectId, int ep) { }


		public void HudUpdateStartUltimateSkillCooldown(int objectId, float cooldown, float maxCooldown)
		{
			teamHud.StartCooldownUltimateSkill(objectId, cooldown, maxCooldown);
			observerHud.StartCooldownUltimateSkill(objectId, cooldown, maxCooldown);
		}


		public void HudUpdateTeamUltimateSkillIcon(LocalPlayerCharacter character)
		{
			teamHud.DrawSkillIcon(character);
			observerHud.DrawSkillIcon(character);
		}


		public void HudUpdateModifyUltimateSkillCooldown(int objectId, float ultimateSkillCooldown)
		{
			teamHud.ModifyCooldownUltimateSkill(objectId, ultimateSkillCooldown);
			observerHud.ModifyCooldownUltimateSkill(objectId, ultimateSkillCooldown);
		}


		public void HudUpdateHoldUltimateSkillCooldown(int objectId)
		{
			teamHud.HoldCooldownUltimateSkill(objectId);
			observerHud.HoldCooldownUltimateSkill(objectId);
		}


		public void SetInBattle(int objectId, bool isCombat)
		{
			teamHud.SetInBattle(objectId, isCombat);
			observerHud.SetInBattle(objectId, isCombat);
		}


		public void SetInBattleByPlayer(int objectId)
		{
			teamHud.SetInBattleByPlayer(objectId);
			observerHud.SetInBattleByPlayer(objectId);
		}


		public void HudUpdateDead(int objectId)
		{
			teamHud.Dead(objectId);
			observerHud.Dead(objectId);
		}


		public void HudUpdateDyingCondition(int objectId, bool isDyingCondition)
		{
			teamHud.SetDyingCondition(objectId, isDyingCondition);
			observerHud.SetDyingCondition(objectId, isDyingCondition);
		}


		public void HudUpdateKillCount(int objectId, int killCount)
		{
			observerHud.SetKillCount(objectId, killCount);
		}


		public void OnStartWaitRequest()
		{
			if (serverIndicator != null && !serverIndicator.gameObject.activeSelf)
			{
				serverIndicator.gameObject.SetActive(true);
			}
		}


		public void OnStopWaitRequest()
		{
			if (serverIndicator != null && serverIndicator.gameObject.activeSelf)
			{
				serverIndicator.gameObject.SetActive(false);
			}
		}
	}
}