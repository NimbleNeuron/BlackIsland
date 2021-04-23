using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Blis.Client.UI;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class ClientService : MonoBehaviourInstance<ClientService>
	{
		[Flags]
		public enum GamePlayMode
		{
			None = 0,
			NotInGame = 1,
			Play = 2,
			ObserveTeam = 4,
			All = 63487
		}


		public bool isCrafting;
		
		private readonly List<GameObject> airSupplyPositionEffectList = new List<GameObject>();
		private readonly float areaAnnounceTick = 7f;
		private readonly Dictionary<long, PlayerContext> playerMap = new Dictionary<long, PlayerContext>();
		private readonly SortedDictionary<int, List<PlayerContext>> teamMap =
			new SortedDictionary<int, List<PlayerContext>>();

		private int areaCode;

		private Dictionary<int, AreaRestrictionState> areaStateMap;
		private ChatCommandService chatCommandService;
		private GamePlayMode curGamePlayMode = GamePlayMode.NotInGame;
		private LevelData currentLevel;
		private DayNight dayNight;
		private float frameUpdateRate = 0.033333335f;
		private GameClient gameClient;
		private string gameMode;
		private DateTime gameStartTime;
		private IgnoreTargetService ignoreTargetService;

		private bool isAlreadyEventLogging;
		private bool isGameEnd;
		private bool isGameSetup;
		private bool isGameStarted;
		private bool isMyPlayerInDoor;
		private bool isStopAreaRestriction;
		private bool isWicklineDead;
		
		private float lastAreaAnnounceTick;
		private int myObjectId;

		private MyObserverContext myObserver;
		private MyPlayerContext myPlayer { get; set; }
		private NightLights nightLights;
		private DateTime observeStartTime;

		private float remainRestrictedTime;
		private int restrictedAreaMask;

		private Favorite selectFavorite;
		private float wicklineResponRemainTime;

		private WicklineVoiceControl wicklineVoiceControl;
		private LocalWorld world;

		public GameUI UI => MonoBehaviourInstance<GameUI>.inst;

		public LocalWorld World => world;

		public int MyObjectId => myObjectId;

		public int MyTeamNumber {
			get
			{
				if (myPlayer != null)
				{
					return myPlayer.Character.TeamNumber;
				}

				if (myObserver != null)
				{
					return myObserver.Observer.TeamNumber;
				}

				return 0;
			}
		}


		public MyPlayerContext MyPlayer => myPlayer;
		public MyObserverContext MyObserver => myObserver;
		public bool IsPlayer => myPlayer != null && myPlayer.IsValid();
		public LevelData CurrentLevel => currentLevel;
		public Dictionary<int, AreaRestrictionState> AreaStateMap => areaStateMap;

		public GamePlayMode CurGamePlayMode => curGamePlayMode;
		public bool IsGameSetup => isGameSetup;
		public bool IsGameStarted => isGameStarted;
		public bool IsGameEnd => isGameEnd;

		public DayNight DayNight => dayNight;

		public float WicklineResponRemainTime => wicklineResponRemainTime;


		public bool IsWicklineDead => isWicklineDead;


		public WicklineVoiceControl WicklineVoiceControl => wicklineVoiceControl;


		public IgnoreTargetService IgnoreTargetService => ignoreTargetService;


		public ChatCommandService ChatCommandService => chatCommandService;


		public float CurrentServerFrameTime => gameClient.CurrentSeq * 0.033333335f;


		public float CurrentSkillTime => gameClient.EstimatedServerFrameTime;


		
		public bool safeAllAreaCheatOn { get; set; }


		public bool IsStopAreaRestriction => isStopAreaRestriction;


		public bool IsTeamMode => gameClient.IsTeamMode;


		public Favorite SelectFavorite => selectFavorite;


		public int RestrictedAreaMask => restrictedAreaMask;


		public float FrameUpdateRate => frameUpdateRate;


		private void Update()
		{
			if (!isGameStarted)
			{
				return;
			}

			if (!isStopAreaRestriction)
			{
				remainRestrictedTime = Mathf.Max(0f, remainRestrictedTime - Time.deltaTime);
			}

			if (myPlayer != null)
			{
				AreaData areaData = null;
				if (!myPlayer.Character.IsAlive && MonoBehaviourInstance<MobaCamera>.inst.TrackingTargetLmc != null)
				{
					areaData = MonoBehaviourInstance<MobaCamera>.inst.TrackingTargetLmc
						.GetCurrentAreaData(currentLevel);
				}

				if (areaData == null)
				{
					areaData = myPlayer.Character.GetCurrentAreaData(currentLevel);
				}

				UpdateUI(areaData);
				UpdateAreaChange(areaData);
				myPlayer.UpdateInternal();
			}

			if (myObserver != null)
			{
				UpdateAreaChange(SingletonMonoBehaviour<ObserverController>.inst.GetCurrentAreaData(currentLevel));
			}

			UpdateWicklineInfo();
			Singleton<SoundControl>.inst.Update();
		}


		private void OnApplicationQuit() { }


		
		
		public event Action<DayNight> OnUpdateRestrictedArea;


		protected override void _Awake()
		{
			GameUtil.BindOrAdd<LocalWorld>(gameObject, ref world);
			GameUtil.BindOrAdd<NightLights>(
				GameObject.Find("Envrionment").transform.FindRecursively("Light_Night").gameObject, ref nightLights);
			ignoreTargetService = new IgnoreTargetService();
			chatCommandService = new ChatCommandService();
		}


		public void Init(GameClient gameClient)
		{
			this.gameClient = gameClient;
			Cursor.lockState = CursorLockMode.Confined;
		}


		public void SetupLevel(LevelData levelData)
		{
			currentLevel = levelData;
			Singleton<ItemService>.inst.SetLevelData(levelData);
			MonoBehaviourInstance<RestrictedAreaEffect>.inst.Init(levelData);
			MonoBehaviourInstance<EnvironmentEffectManager>.inst.Init(levelData);
		}


		public void SetupGame(int standbySecond)
		{
			isGameSetup = true;
			BuildTeam();
			if (myPlayer != null)
			{
				MonoBehaviourInstance<GameUI>.inst.StatusHud.SetTeamColor(myPlayer.Character.TeamSlot);
				MonoBehaviourInstance<GameUI>.inst.TeamHud.Init(myPlayer.TeamMembers);
			}

			Log.H("[SetupGame] StandbySecond: {0}", standbySecond);
			MonoBehaviourInstance<GameUI>.inst.OnSetupGame(standbySecond);
		}


		public void StartGame(float frameUpdateRate)
		{
			Singleton<GameTime>.inst.InitLatency();
			isGameStarted = true;
			isGameEnd = false;
			isAlreadyEventLogging = false;
			Log.H("[StartGame] FrameUpdateRate: {0}", frameUpdateRate.ToString());
			this.frameUpdateRate = frameUpdateRate;
			MonoBehaviourInstance<GameUI>.inst.OnStartGame();
			if (IsPlayer)
			{
				if (gameClient.IsTutorial)
				{
					MonoBehaviourInstance<TutorialController>.inst.SetTutorial(MonoBehaviourInstance<GameUI>.inst
						.Tutorial);
					SingletonMonoBehaviour<PlayerController>.inst.ItemGuide.InitTutorialGuide(gameClient.MatchingMode
						.ConvertToTutorialType());
					MonoBehaviourInstance<GameUI>.inst.NavigationHud.SetTutorialRouteList();
				}
				else
				{
					SingletonMonoBehaviour<PlayerController>.inst.ItemGuide.InitGuide(myPlayer.Character.CharacterCode,
						myPlayer.Character.GetWeapon().WeaponTypeInfoData.type);
					MonoBehaviourInstance<GameUI>.inst.NavigationHud.SetRouteListSlots();
					MonoBehaviourInstance<GameUI>.inst.NavigationHud.ShowRouteList();
				}

				MonoBehaviourInstance<GameUI>.inst.RestrictedArea.UpdateSurvivableTime(
					myPlayer.Character.SurvivableTime);
			}

			GlobalUserData.myTeamNumber = MyTeamNumber;
			GlobalUserData.myObjectId = myObjectId;
			Singleton<SoundControl>.inst.SetInGame(true);
			gameMode = Singleton<GameEventLogger>.inst.gameMode;
			gameStartTime = DateTime.Now;
			observeStartTime = DateTime.MinValue;
			if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
			{
				MonoBehaviourInstance<TutorialController>.inst.StartAnnounce();
			}
			else
			{
				MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(Ln.Get("게임시작안내"),
					delegate { Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.StartGame); });
			}

			MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(Ln.Get("게임문화개선캠페인"));
			gameClient.OnGameStarted(frameUpdateRate);
			SetGamePlayMode(GamePlayMode.Play);
			if (IsPlayer)
			{
				RouteApi.Use(selectFavorite);
			}

			GlobalUserData.IsPlayer = IsPlayer;
		}


		public void BuildTeam()
		{
			if (IsPlayer)
			{
				myPlayer.ClearTeam();
				int num = 1;
				using (IEnumerator<PlayerContext> enumerator = (from x in playerMap.Values.ToList<PlayerContext>()
					orderby x.userId
					select x).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						PlayerContext playerContext = enumerator.Current;
						LocalPlayerCharacter character = playerContext.Character;
						if (myObjectId == character.ObjectId)
						{
							character.SetTeamSlot(num++);
							character.ShowMapIcon(MiniMapIconType.Sight);
							character.ShowMiniMapIcon(MiniMapIconType.Sight);
						}
						else
						{
							if (IsAlly(character))
							{
								myPlayer.AddTeamMember(character);
								myPlayer.Character.SightAgent.AddAllySight(character.SightAgent);
								character.SetTeamSlot(num++);
							}

							character.UpdateMapIcon();
						}
					}

					return;
				}
			}

			if (gameClient.MatchingMode != MatchingMode.Custom)
			{
				return;
			}

			List<PlayerContext> source = playerMap.Values.ToList<PlayerContext>();
			foreach (PlayerContext playerContext2 in from x in source
				orderby x.userId
				select x)
			{
				LocalPlayerCharacter character2 = playerContext2.Character;
				if (character2.TeamSlot <= 0)
				{
					int num2 = 1;
					character2.SetTeamSlot(num2++);
					foreach (PlayerContext playerContext3 in source.OrderBy(x => x.userId))
					{
						LocalPlayerCharacter character3 = playerContext3.Character;
						if (character2.ObjectId != character3.ObjectId &&
						    character2.HostileAgent.GetHostileType(character3.HostileAgent) == HostileType.Ally)
						{
							character3.SetTeamSlot(num2++);
						}
					}
				}

				myObserver.Observer.SightAgent.AddAllySight(character2.SightAgent);
				character2.ShowMapIcon(MiniMapIconType.Sight);
				character2.UpdateMapIcon();
				character2.InSight();
				character2.OnVisible();
			}
		}


		public bool IsAlly(LocalCharacter target)
		{
			if (target == null)
			{
				return false;
			}

			if (IsPlayer)
			{
				return target.HostileAgent != null && myPlayer.GetHostileType(target) == HostileType.Ally;
			}

			if (myObserver == null)
			{
				Debug.LogError("My Observer is null. somethings wrong!");
				return false;
			}

			return target.HostileAgent != null && myObserver.GetHostileType(target) == HostileType.Ally;
		}


		public bool IsInAllySight(LocalSightAgent sightAgent, Vector3 position, float radius, bool isInvisible)
		{
			SightAgent sightAgent2 = GetSightAgent();
			return sightAgent2 != null && sightAgent2.IsInAllySight(sightAgent, position, radius, isInvisible);
		}


		private SightAgent GetSightAgent()
		{
			if (!IsPlayer)
			{
				return myObserver.Observer.SightAgent;
			}

			return myPlayer.Character.SightAgent;
		}


		public int GetTeamSlot(long userId)
		{
			PlayerContext playerContext = FindPlayerContext(userId);
			if (playerContext == null)
			{
				return 0;
			}

			return playerContext.GetTeamSlot();
		}


		public int GetAliveTeamCount()
		{
			int num = 0;
			foreach (List<PlayerContext> list in teamMap.Values)
			{
				using (List<PlayerContext>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						if (enumerator2.Current.Character.IsAlive)
						{
							num++;
							break;
						}
					}
				}
			}

			return num;
		}


		public int GetCurrentTeamRank(int teamNumber)
		{
			using (List<PlayerContext>.Enumerator enumerator = teamMap[teamNumber].GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Character.IsAlive)
					{
						return -1;
					}
				}
			}

			return GetAliveTeamCount() + 1;
		}


		public void SetInBattle(int objectId, bool isCombat)
		{
			MonoBehaviourInstance<GameUI>.inst.SetInBattle(objectId, isCombat);
		}


		public void SetInBattleByPlayer(int objectId)
		{
			MonoBehaviourInstance<GameUI>.inst.SetInBattleByPlayer(objectId);
		}


		public void UpdateRestrictedArea(Dictionary<int, AreaRestrictionState> areaStateMap, float remainTime,
			DayNight dayNight, int day = 0)
		{
			remainRestrictedTime = remainTime;
			this.areaStateMap = areaStateMap;
			isStopAreaRestriction = false;
			if (safeAllAreaCheatOn)
			{
				restrictedAreaMask = 0;
			}
			else
			{
				restrictedAreaMask = currentLevel.LaboratoryArea.maskCode;
				foreach (int key in from pair in areaStateMap
					where pair.Value == AreaRestrictionState.Restricted
					select pair.Key)
				{
					restrictedAreaMask |= currentLevel.areaDataMap[key].maskCode;
				}
			}

			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateRestrictedArea(currentLevel, areaStateMap,
				remainRestrictedTime, dayNight, day);
			MonoBehaviourInstance<RestrictedAreaEffect>.inst.UpdateRestrictedArea(currentLevel, areaStateMap,
				remainRestrictedTime);
			MonoBehaviourInstance<DirectionalLightEffect>.inst.SetDayNight(dayNight);
			this.dayNight = dayNight;
			this.StartThrowingCoroutine(CoroutineUtil.DelayedAction(3f, delegate
				{
					if (dayNight == DayNight.Night)
					{
						nightLights.Enable();
						return;
					}

					nightLights.Disable();
				}),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][RestrictedArea] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			PlayAmbientA();
			PlayAmbientB();
			foreach (LocalMonster localMonster in world.FindAll<LocalMonster>())
			{
				localMonster.OnChangeDayNight(dayNight);
			}

			Action<DayNight> onUpdateRestrictedArea = OnUpdateRestrictedArea;
			if (onUpdateRestrictedArea == null)
			{
				return;
			}

			onUpdateRestrictedArea(dayNight);
		}


		private void UpdateUI(AreaData currentAreaData)
		{
			if (currentAreaData == null || currentAreaData.code == 0)
			{
				return;
			}

			if ((myPlayer.Character.GetCurrentAreaMask() & restrictedAreaMask) > 0)
			{
				if (!IsInFinalSafeZone(myPlayer.Character, currentAreaData))
				{
					MonoBehaviourInstance<GameUI>.inst.BloodFx.Play(BloodEffect.AnimationType.Restricted);
					return;
				}

				if (MonoBehaviourInstance<GameUI>.inst.BloodFx.CurrentAnimation == BloodEffect.AnimationType.Restricted)
				{
					MonoBehaviourInstance<GameUI>.inst.BloodFx.Stop();
				}
			}
			else if (MonoBehaviourInstance<GameUI>.inst.BloodFx.CurrentAnimation ==
			         BloodEffect.AnimationType.Restricted)
			{
				MonoBehaviourInstance<GameUI>.inst.BloodFx.Stop();
			}
		}


		public void UpdateAreaChange(AreaData currentAreaData)
		{
			if (currentAreaData == null || currentAreaData.code == 0)
			{
				return;
			}

			if (areaStateMap == null)
			{
				return;
			}

			AreaRestrictionState areaState;
			if (!areaStateMap.TryGetValue(currentAreaData.code, out areaState))
			{
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.Events.OnUpdateCurrentArea(currentAreaData, areaState);
			if (areaCode != currentAreaData.code)
			{
				areaCode = currentAreaData.code;
				if (Time.time - lastAreaAnnounceTick > areaAnnounceTick)
				{
					MonoBehaviourInstance<GameUI>.inst.AnnounceMessage.ShowMessage(
						LnUtil.GetAreaName(currentAreaData.code), null);
					lastAreaAnnounceTick = Time.time;
				}

				AreaSoundData areaSound = GameDB.level.GetAreaSound(areaCode);
				Singleton<SoundControl>.inst.PlayBGM(areaSound.BGMSound);
				string name = dayNight == DayNight.Day ? areaSound.AmbiSoundGroupDay : areaSound.AmbiSoundGroupNight;
				List<SoundGroupData> soundGroupData = GameDB.level.GetSoundGroupData(name);
				Singleton<SoundControl>.inst.PlayAmbientB(soundGroupData);
				if (MonoBehaviourInstance<GameClient>.inst.IsTutorial)
				{
					MonoBehaviourInstance<TutorialController>.inst.ChangeAreaTutorial(currentAreaData.code);
				}
			}
		}


		private void AddUser(PlayerContext playerContext)
		{
			if (playerMap.ContainsKey(playerContext.userId))
			{
				Log.E("[CreateCharacter] user[{0}({1})] is redundant.", playerContext.nickname, playerContext.userId);
				return;
			}

			playerMap.Add(playerContext.userId, playerContext);
			if (!teamMap.ContainsKey(playerContext.Character.TeamNumber))
			{
				teamMap[playerContext.Character.TeamNumber] = new List<PlayerContext>();
			}

			teamMap[playerContext.Character.TeamNumber].Add(playerContext);
		}


		public PlayerContext FindPlayerContext(long userId)
		{
			if (playerMap.ContainsKey(userId))
			{
				return playerMap[userId];
			}

			if (myPlayer != null && myPlayer.userId == userId)
			{
				return myPlayer;
			}

			return null;
		}


		public void ConvertMyPlayerContextToPlayerContext()
		{
			if (playerMap.ContainsKey(myPlayer.userId))
			{
				playerMap.Remove(myPlayer.userId);
			}

			if (teamMap.ContainsKey(myPlayer.Character.TeamNumber))
			{
				for (int i = 0; i < teamMap[myPlayer.Character.TeamNumber].Count; i++)
				{
					if (teamMap[myPlayer.Character.TeamNumber][i].userId == myPlayer.userId)
					{
						teamMap[myPlayer.Character.TeamNumber].RemoveAt(i);
						break;
					}
				}
			}

			PlayerContext playerContext =
				new PlayerContext(myPlayer.userId, myPlayer.nickname, myPlayer.startingWeaponCode);
			playerContext.SetPlayerCharacter(myPlayer.Character);
			playerContext.Character.SetPlayerContext(playerContext);
			AddUser(playerContext);
			myPlayer = null;
		}


		public LocalPlayerCharacter FindPlayerCharacter(int objectId)
		{
			foreach (KeyValuePair<long, PlayerContext> keyValuePair in playerMap)
			{
				if (keyValuePair.Value != null && !(keyValuePair.Value.Character == null) &&
				    keyValuePair.Value.Character.ObjectId == objectId)
				{
					return keyValuePair.Value.Character;
				}
			}

			return null;
		}


		public int GetAliveCount()
		{
			foreach (PlayerContext playerContext in playerMap.Values) { }

			return playerMap.Count(x => x.Value.Character.IsAlive);
		}


		public int GetTotalPlayerCount()
		{
			return playerMap.Count;
		}


		public int GetTotalTeamCount()
		{
			SortedDictionary<int, List<PlayerContext>> sortedDictionary = teamMap;
			if (sortedDictionary == null)
			{
				return 0;
			}

			return sortedDictionary.Count;
		}


		public List<PlayerContext> GetTeamMember(int teamNumber)
		{
			List<PlayerContext> result = null;
			SortedDictionary<int, List<PlayerContext>> sortedDictionary = teamMap;
			if (sortedDictionary != null)
			{
				sortedDictionary.TryGetValue(teamNumber, out result);
			}

			return result;
		}


		public void StopGame()
		{
			isGameStarted = false;
			isGameEnd = true;
		}


		public bool IsUserExist(int userId)
		{
			return playerMap.ContainsKey(userId);
		}


		public void SetSelectFavorite(Favorite selectFavorite)
		{
			this.selectFavorite = selectFavorite;
		}


		public void CreateLocalObjects()
		{
			CreateItemBox();
			CreateHyperloop();
			CreateSecurityConsole();
			CreateSecurityCamera();
		}


		private void CreateItemBox()
		{
			foreach (ItemSpawnPoint itemSpawnPoint in currentLevel.itemSpawnPoints)
			{
				if (!itemSpawnPoint.airSupply)
				{
					Transform transform = itemSpawnPoint.transform;
					if (itemSpawnPoint.resource)
					{
						world.SpawnResourceItemBox(transform.position, transform.rotation, itemSpawnPoint.code,
							itemSpawnPoint.resourceDataCode, itemSpawnPoint.areaCode, itemSpawnPoint.initSpawnTime);
					}
					else
					{
						world.SpawnStaticItemBox(transform.position, transform.rotation, itemSpawnPoint.code);
					}
				}
			}
		}


		private void CreateHyperloop()
		{
			foreach (HyperloopSpawnPoint hyperloopSpawnPoint in currentLevel.hyperloopSpawnPoints)
			{
				Transform transform = hyperloopSpawnPoint.transform;
				World.SpawnHyperloop(transform.position, transform.rotation);
			}
		}


		private void CreateSecurityConsole()
		{
			foreach (SecurityConsoleSpawnPoint securityConsoleSpawnPoint in currentLevel.securityConsoleSpawnPoints)
			{
				Transform transform = securityConsoleSpawnPoint.transform;
				World.SpawnSecurityConsole(transform.position, transform.rotation);
			}
		}


		private void CreateSecurityCamera()
		{
			foreach (SecurityCameraSpawnPoint securityCameraSpawnPoint in currentLevel.securityCameraSpawnPoints)
			{
				Transform transform = securityCameraSpawnPoint.transform;
				World.SpawnSecurityCamera(transform.position, transform.rotation);
			}
		}


		public void CreateMyPlayerCharacter(long userId, string nickname, int startingWeaponCode, byte[] playerSnapshot,
			SnapshotWrapper characterSnapshot)
		{
			if (FindPlayerContext(userId) != null)
			{
				Log.E("[CreateMyPlayerCharacter] user[{0}({1})] is redundant.", nickname, userId);
				return;
			}

			myObjectId = characterSnapshot.objectId;
			myPlayer = new MyPlayerContext(userId, nickname, startingWeaponCode);
			LocalPlayerCharacter localPlayerCharacter = world.SpawnSnapshot(characterSnapshot) as LocalPlayerCharacter;
			if (localPlayerCharacter == null)
			{
				Log.E("[CreateMyPlayerCharacter] user[{0}({1})] LocalPlayerCharacter is Null", nickname, userId);
				return;
			}

			myPlayer.SetPlayerCharacter(localPlayerCharacter);
			myPlayer.Character.SetMyPlayerContext(myPlayer);
			SingletonMonoBehaviour<PlayerController>.inst.Init(gameClient, this);
			myPlayer.Init(playerSnapshot);
			AddUser(myPlayer);
			
			SingletonMonoBehaviour<AnimationEventService>.inst.AnimationCollection.LoadGameCharacter(
				localPlayerCharacter.CharacterCode);
			MonoBehaviourInstance<MobaCamera>.inst.SetTrackingTarget(localPlayerCharacter);
			MonoBehaviourInstance<MobaCamera>.inst.SetAudioListener(localPlayerCharacter.transform);
			MonoBehaviourInstance<MobaCamera>.inst.SetCameraPosition(localPlayerCharacter.GetPosition(), 0f);
			PlayerCharacterSnapshot playerCharacterSnapshot =
				Serializer.Default.Deserialize<PlayerCharacterSnapshot>(characterSnapshot.snapshot);
			myPlayer.Character.InitStateEffect(playerCharacterSnapshot.initialStateEffect);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateAllSkillHud();
			SingletonMonoBehaviour<PlayerController>.inst.PingTarget.UpdateMarkTarget(playerCharacterSnapshot.mapMarks);
		}


		public void CreatePlayerCharacter(long userId, string nickname, int startingWeaponCode, byte[] playerSnapshot,
			SnapshotWrapper characterSnapshot)
		{
			if (FindPlayerContext(userId) != null)
			{
				Log.E("[CreatePlayerCharacter] user[{0}({1})] is redundant.", nickname, userId);
				return;
			}

			bool isUseTempNickname = false;
			string nickname2;
			if (nickname.Contains("TempNickname"))
			{
				string[] array = nickname.Split('/');
				nickname2 = Ln.Format(array[0], array[1]);
				isUseTempNickname = true;
			}
			else
			{
				nickname2 = nickname;
			}

			PlayerContext playerContext = new PlayerContext(userId, nickname2, startingWeaponCode);
			LocalPlayerCharacter localPlayerCharacter = world.SpawnSnapshot(characterSnapshot) as LocalPlayerCharacter;
			if (localPlayerCharacter == null)
			{
				Log.E("[CreatePlayerCharacter] user[{0}({1})] LocalPlayerCharacter is Null", nickname, userId);
				return;
			}

			playerContext.SetPlayerCharacter(localPlayerCharacter);
			playerContext.Character.SetPlayerContext(playerContext);
			playerContext.SetIsUseTempNickname(isUseTempNickname);
			playerContext.Init(playerSnapshot);
			AddUser(playerContext);
			SingletonMonoBehaviour<AnimationEventService>.inst.AnimationCollection.LoadGameCharacter(
				localPlayerCharacter.CharacterCode);
			PlayerCharacterSnapshot playerCharacterSnapshot =
				Serializer.Default.Deserialize<PlayerCharacterSnapshot>(characterSnapshot.snapshot);
			playerContext.Character.InitStateEffect(playerCharacterSnapshot.initialStateEffect);
		}


		public void CreateMyObserver(long userId, string nickname, byte[] playerSnapshot,
			SnapshotWrapper characterSnapshot)
		{
			if (myObserver != null && myObserver.userId == userId)
			{
				Log.E("[CreateMyObserver] user[{0}({1})] is redundant.", nickname, userId.ToString());
				return;
			}

			myObjectId = characterSnapshot.objectId;
			myObserver = new MyObserverContext(userId, nickname);
			LocalObserver localObserver = world.SpawnSnapshot(characterSnapshot) as LocalObserver;
			if (localObserver == null)
			{
				Log.E("[CreateMyObserver] user[{0}({1})] LocalObserver is Null.", nickname, userId.ToString());
				return;
			}

			myObserver.SetObserver(localObserver);
			myObserver.Observer.SetMyObserverContext(myObserver);
			SingletonMonoBehaviour<ObserverController>.inst.Init(gameClient, this);
			myObserver.Init(playerSnapshot);
			MonoBehaviourInstance<MobaCamera>.inst.SetCameraMode(MobaCameraMode.Traveling);
			MonoBehaviourInstance<MobaCamera>.inst.SetCameraPosition(myObserver.Observer.GetPosition(), 0f);
			MonoBehaviourInstance<MobaCamera>.inst.SetCameraFV(MonoBehaviourInstance<MobaCamera>.inst.SightToFV(15f));
			MonoBehaviourInstance<MobaCamera>.inst.SetAudioListener();
		}


		public void CreateObserver(long userId, string nickname, byte[] playerSnapshot,
			SnapshotWrapper characterSnapshot) { }


		public void UpdateMyPlayerCharacter(byte[] playerSnapshot, SnapshotWrapper characterSnapshot,
			List<EquipItem> equips, int walkableNavMask, int exp, List<InvenItem> inventoryItems, float survivalTime,
			List<BulletItem> bulletItems)
		{
			LocalPlayerCharacter localPlayerCharacter = world.Find<LocalPlayerCharacter>(myObjectId);
			PlayerCharacterSnapshot playerCharacterSnapshot =
				Serializer.Default.Deserialize<PlayerCharacterSnapshot>(characterSnapshot.snapshot);
			Item weapon = localPlayerCharacter.Equipment.GetWeapon();
			localPlayerCharacter.Init(characterSnapshot.snapshot);
			localPlayerCharacter.transform.SetPositionAndRotation(characterSnapshot.position,
				characterSnapshot.rotation);
			localPlayerCharacter.MoveAgent.SetWalkableNavMask(walkableNavMask);
			localPlayerCharacter.MergeBulletItems(bulletItems, equips, inventoryItems);
			localPlayerCharacter.OnUpdateEquipment(equips, weapon);
			localPlayerCharacter.OnUpdateInventory(inventoryItems);
			localPlayerCharacter.InitStateEffect(playerCharacterSnapshot.initialStateEffect);
			localPlayerCharacter.OnUpdateExp(exp);
			myPlayer.Init(playerSnapshot);
			myPlayer.OnUpdateSurvivableTime(survivalTime);
			MonoBehaviourInstance<MobaCamera>.inst.SetCameraPosition(localPlayerCharacter.GetPosition(), 0f);
			SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.UpdateAllSkillHud();
			SingletonMonoBehaviour<PlayerController>.inst.PingTarget.UpdateMarkTarget(playerCharacterSnapshot.mapMarks);
			int bullet = localPlayerCharacter.Status.Bullet;
			if (bullet > 0)
			{
				localPlayerCharacter.Status.UpdateBullet(bullet);
				CharacterFloatingUI floatingUi = localPlayerCharacter.FloatingUi;
				if (floatingUi != null)
				{
					floatingUi.UpdateBullet(localPlayerCharacter.Status.Bullet);
				}
			}

			foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
			{
				if (skillSlotSet != SkillSlotSet.None)
				{
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.LockSkillSlot(skillSlotSet,
						playerCharacterSnapshot.lockedSlotSetFlag.HasFlag(skillSlotSet));
				}
			}

			localPlayerCharacter.OnVisible();
		}


		public void UpdatePlayerCharacter(long userId, byte[] playerSnapshot, SnapshotWrapper characterSnapshot,
			List<EquipItem> equips, int walkableNavMask, bool outSight, bool invisible)
		{
			PlayerContext playerContext = FindPlayerContext(userId);
			if (playerContext == null)
			{
				Log.E("[UpdatePlayerCharacter] user[{0}({1})] is not exits.", userId, userId);
				return;
			}

			PlayerContext playerContext2 = playerContext;
			LocalPlayerCharacter localPlayerCharacter = world.Find<LocalPlayerCharacter>(characterSnapshot.objectId);
			PlayerCharacterSnapshot playerCharacterSnapshot =
				Serializer.Default.Deserialize<PlayerCharacterSnapshot>(characterSnapshot.snapshot);
			Item weapon = localPlayerCharacter.Equipment.GetWeapon();
			localPlayerCharacter.Init(characterSnapshot.snapshot);
			localPlayerCharacter.transform.SetPositionAndRotation(characterSnapshot.position,
				characterSnapshot.rotation);
			localPlayerCharacter.MoveAgent.SetWalkableNavMask(walkableNavMask);
			int bullet = localPlayerCharacter.Status.Bullet;
			localPlayerCharacter.OnUpdateEquipment(equips, weapon);
			localPlayerCharacter.InitStateEffect(playerCharacterSnapshot.initialStateEffect);
			playerContext2.Init(playerSnapshot);
			if (bullet > 0)
			{
				localPlayerCharacter.Status.UpdateBullet(bullet);
				if (localPlayerCharacter.FloatingUi != null)
				{
					localPlayerCharacter.FloatingUi.UpdateBullet(localPlayerCharacter.Status.Bullet);
				}
			}

			if (!IsPlayer)
			{
				localPlayerCharacter.InSight();
				localPlayerCharacter.OnVisible();
				return;
			}

			if (outSight)
			{
				localPlayerCharacter.OutSight();
			}
			else
			{
				localPlayerCharacter.InSight();
			}

			if (invisible)
			{
				localPlayerCharacter.OnInvisible();
				return;
			}

			localPlayerCharacter.OnVisible();
		}


		public void UpdateMyObserver(byte[] playerSnapshot, SnapshotWrapper characterSnapshot)
		{
			LocalObserver localObserver = world.Find<LocalObserver>(myObjectId);
			localObserver.Init(characterSnapshot.snapshot);
			localObserver.transform.SetPositionAndRotation(characterSnapshot.position, characterSnapshot.rotation);
			myObserver.Init(playerSnapshot);
		}


		public void EndTutorial(int rank, ObjectType attackerObjectType, int attackerObjectId, int attackerDataCode,
			string attackerName)
		{
			SingletonMonoBehaviour<GameAnalytics>.inst.CustomEvent("Tutorial Complete", new Dictionary<string, object>
			{
				{
					"server",
					"release"
				},
				{
					"tutorial_type",
					(int) MonoBehaviourInstance<TutorialController>.inst.TutorialType
				}
			});
			MonoBehaviourInstance<GameUI>.inst.Tutorial.HideMainQuest();
			this.StartThrowingCoroutine(
				FinalizeGame(rank, attackerObjectType, attackerObjectId, attackerDataCode, attackerName, null,
					GameFinalizeType.Tutorial),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][EndTutorial] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		public void EndGame(bool finishGame, int rank, ObjectType attackerObjectType, int attackerObjectId,
			int attackerDataCode, string attackerNickName, string attackerTempNickname)
		{
			if (IsPlayer && myPlayer.IsObserving)
			{
				if (MyPlayer.TeamMembers.Any(x => x.IsAlive))
				{
					foreach (LocalPlayerCharacter localPlayerCharacter in MyPlayer.TeamMembers)
					{
						Log.V(string.Format(
							"[OBSERVING BUG][EndGame] Team is Alive. Rank({0}), Member({1}), IsAlive({2}), IsDyingCondition({3}), HP({4})",
							rank, localPlayerCharacter.Nickname, localPlayerCharacter.IsAlive,
							localPlayerCharacter.IsDyingCondition, localPlayerCharacter.Status.Hp));
					}
				}
			}

			GameFinalizeType gameFinalizeType = finishGame ? GameFinalizeType.GameEnd : GameFinalizeType.DeadAll;
			this.StartThrowingCoroutine(
				FinalizeGame(rank, attackerObjectType, attackerObjectId, attackerDataCode, attackerNickName,
					attackerTempNickname, gameFinalizeType),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][EndGame] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		public void EndGameTeamAlive(ObjectType attackerObjectType, int attackerObjectId, int attackerDataCode,
			string attackerNickName, string attackerTempNickname)
		{
			this.StartThrowingCoroutine(
				FinalizeGame(0, attackerObjectType, attackerObjectId, attackerDataCode, attackerNickName,
					attackerTempNickname, GameFinalizeType.DeadAndTeamAlive),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][EndGameTeamAlive] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator FinalizeGame(int rank, ObjectType attackerObjectType, int attackerObjectId,
			int attackerDataCode, string attackerNickName, string attackerTempNickname,
			GameFinalizeType gameFinalizeType)
		{
			World.SightObjectPoolCountLog();
			if (IsPlayer && myPlayer.IsObserving)
			{
				if (MyPlayer.TeamMembers.Any(x => x.IsAlive))
				{
					foreach (LocalPlayerCharacter localPlayerCharacter in MyPlayer.TeamMembers)
					{
						Log.V(string.Format(
							"[OBSERVING BUG][FinalizeGame]({0}) Team is Alive. Rank({1}), Member({2}), IsAlive({3}), IsDyingCondition({4}), HP({5})",
							gameFinalizeType, rank, localPlayerCharacter.Nickname, localPlayerCharacter.IsAlive,
							localPlayerCharacter.IsDyingCondition, localPlayerCharacter.Status.Hp));
					}
				}
			}

			if (MonoBehaviourInstance<GameUI>.inst.GameResult.IsActive())
			{
				MonoBehaviourInstance<GameUI>.inst.GameResult.Deactive();
			}

			SetPlayTime();
			yield return new WaitForSeconds(3.5f);
			if (gameFinalizeType == GameFinalizeType.DeadAndTeamAlive)
			{
				ShowGameEndResultUi(rank, attackerObjectType, attackerObjectId, attackerDataCode, attackerNickName,
					attackerTempNickname, gameFinalizeType);
				observeStartTime = DateTime.Now;
				yield break;
			}

			if (gameFinalizeType == GameFinalizeType.GameEnd)
			{
				StopGame();
				ShowGameEndResultUi(rank, attackerObjectType, attackerObjectId, attackerDataCode, attackerNickName,
					attackerTempNickname, gameFinalizeType);
			}
			else if (gameFinalizeType == GameFinalizeType.DeadAll)
			{
				if (gameClient.MatchingMode != MatchingMode.Custom)
				{
					StopGame();
				}

				ShowGameEndResultUi(rank, attackerObjectType, attackerObjectId, attackerDataCode, attackerNickName,
					attackerTempNickname, gameFinalizeType);
			}
			else if (gameFinalizeType == GameFinalizeType.Tutorial)
			{
				if (rank == 1 && MonoBehaviourInstance<TutorialController>.inst.CheckTutorialClear())
				{
					int tutorialCode = (int) MonoBehaviourInstance<TutorialController>.inst.TutorialType;
					RequestDelegate.request<TutorialApi.TutorialResult>(TutorialApi.GetTutorialResult(tutorialCode),
						delegate(RequestDelegateError err, TutorialApi.TutorialResult res)
						{
							if (err != null)
							{
								MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("ServerError/" + err.message));
							}

							GlobalUserData.tutorialClearFlag = res != null && res.result;
							GlobalUserData.tutorialClearCode = tutorialCode;
							ShowGameEndResultUi(rank, attackerObjectType, attackerObjectId, attackerDataCode,
								attackerNickName, attackerTempNickname, gameFinalizeType);
						});
				}
				else
				{
					ShowGameEndResultUi(rank, attackerObjectType, attackerObjectId, attackerDataCode, attackerNickName,
						attackerTempNickname, gameFinalizeType);
				}
			}
		}


		private void ShowGameEndResultUi(int rank, ObjectType attackerObjectType, int attackerObjectId,
			int attackerDataCode, string attackerNickName, string attackerTempNickname,
			GameFinalizeType gameFinalizeType)
		{
			bool finishedGame = gameFinalizeType == GameFinalizeType.GameEnd;
			if (!string.IsNullOrEmpty(attackerTempNickname) && attackerTempNickname.Contains("TempNickname"))
			{
				string[] array = attackerTempNickname.Split('/');
				attackerTempNickname = Ln.Format(array[0], array[1]);
			}

			if (IsPlayer)
			{
				Sprite characterResultSprite =
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterResultSprite(
						myPlayer.Character.CharacterCode, myPlayer.Character.SkinIndex);
				attackerTempNickname = attackerTempNickname != attackerNickName ? attackerTempNickname : "";
				if (gameFinalizeType == GameFinalizeType.DeadAndTeamAlive)
				{
					if (attackerObjectType == ObjectType.PlayerCharacter)
					{
						Sprite characterLobbyPortraitSprite =
							SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(
								attackerDataCode);
						MonoBehaviourInstance<GameUI>.inst.GameResult.ShowResult(null, null, attackerObjectId,
							attackerNickName, attackerTempNickname, myPlayer.nickname, characterLobbyPortraitSprite,
							characterResultSprite, false, myPlayer.Character.Status.PlayerKill,
							myPlayer.Character.Status.MonsterKill, finishedGame);
						return;
					}

					if (attackerObjectType == ObjectType.Monster)
					{
						MonoBehaviourInstance<GameUI>.inst.GameResult.ShowResult(null, null, attackerObjectId,
							attackerNickName, null, myPlayer.nickname, null, characterResultSprite, true,
							myPlayer.Character.Status.PlayerKill, myPlayer.Character.Status.MonsterKill, finishedGame);
						return;
					}

					MonoBehaviourInstance<GameUI>.inst.GameResult.ShowResult(null, null, 0, attackerNickName, null,
						myPlayer.nickname, null, characterResultSprite, false, myPlayer.Character.Status.PlayerKill,
						myPlayer.Character.Status.MonsterKill, finishedGame);
				}
				else
				{
					int totalTeamCount = GetTotalTeamCount();
					if (attackerObjectType == ObjectType.PlayerCharacter)
					{
						Sprite characterLobbyPortraitSprite2 =
							SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterLobbyPortraitSprite(
								attackerDataCode);
						MonoBehaviourInstance<GameUI>.inst.GameResult.ShowResult(rank, totalTeamCount, attackerObjectId,
							attackerNickName, attackerTempNickname, myPlayer.nickname, characterLobbyPortraitSprite2,
							characterResultSprite, false, myPlayer.Character.Status.PlayerKill,
							myPlayer.Character.Status.MonsterKill, finishedGame);
					}
					else if (attackerObjectType == ObjectType.Monster)
					{
						MonoBehaviourInstance<GameUI>.inst.GameResult.ShowResult(rank, totalTeamCount, attackerObjectId,
							attackerNickName, null, myPlayer.nickname, null, characterResultSprite, true,
							myPlayer.Character.Status.PlayerKill, myPlayer.Character.Status.MonsterKill, finishedGame);
					}
					else
					{
						MonoBehaviourInstance<GameUI>.inst.GameResult.ShowResult(rank, totalTeamCount, 0,
							attackerNickName, null, myPlayer.nickname, null, characterResultSprite, false,
							myPlayer.Character.Status.PlayerKill, myPlayer.Character.Status.MonsterKill, finishedGame);
					}

					if (!gameClient.IsTutorial)
					{
						CharacterVoiceType charVoiceType = CharacterVoiceUtil.RankConvertToCharacterVoiceType(rank);
						if (rank == 1)
						{
							Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.WinGame);
							float annouceSoundLength = Singleton<SoundControl>.inst.GetAnnouceSoundLength();
							this.StartThrowingCoroutine(
								myPlayer.Character.CharacterVoiceControl.DelaySoundPlay(
									delegate
									{
										myPlayer.Character.CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15,
											myPlayer.Character.GetPosition());
									}, annouceSoundLength),
								delegate(Exception exception)
								{
									Log.E("[EXCEPTION][ShowGameEndResultUi][Player] Message:" + exception.Message +
									      ", StackTrace:" + exception.StackTrace);
								});
							return;
						}

						myPlayer.Character.CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15,
							myPlayer.Character.GetPosition());
					}
				}
			}
			else if (rank == 1)
			{
				int totalTeamCount2 = GetTotalTeamCount();
				LocalPlayerCharacter winner = FindPlayerCharacter(attackerObjectId);
				if (winner == null)
				{
					world.TryFind<LocalPlayerCharacter>(attackerObjectId, ref winner);
				}

				Sprite characterResultSprite2 =
					SingletonMonoBehaviour<ResourceManager>.inst.GetCharacterResultSprite(attackerDataCode,
						winner.SkinIndex);
				int pkCount = winner != null ? winner.Status.PlayerKill : 0;
				int mkCount = winner != null ? winner.Status.MonsterKill : 0;
				attackerTempNickname = winner != null && winner.PlayerContext.nickname != attackerNickName
					? attackerTempNickname
					: "";
				MonoBehaviourInstance<GameUI>.inst.GameResult.ShowResult(rank, totalTeamCount2, 0, null,
					attackerTempNickname, attackerNickName, null, characterResultSprite2, false, pkCount, mkCount,
					finishedGame);
				CharacterVoiceType charVoiceType = CharacterVoiceUtil.RankConvertToCharacterVoiceType(rank);
				Singleton<SoundControl>.inst.PlayAnnounceSound(AnnounceVoiceType.WinGame);
				float annouceSoundLength2 = Singleton<SoundControl>.inst.GetAnnouceSoundLength();
				if (winner != null)
				{
					this.StartThrowingCoroutine(
						winner.CharacterVoiceControl.DelaySoundPlay(
							delegate
							{
								winner.CharacterVoiceControl.PlayCharacterVoice(charVoiceType, 15,
									winner.GetPosition());
							}, annouceSoundLength2),
						delegate(Exception exception)
						{
							Log.E("[EXCEPTION][ShowGameEndResultUi][Observer] Message:" + exception.Message +
							      ", StackTrace:" + exception.StackTrace);
						});
				}
			}
			else
			{
				MonoBehaviourInstance<GameUI>.inst.GameResult.ShowObserverResult();
			}
		}


		public void ActionCasting(LocalPlayerCharacter player, CastingActionType type, Action successCallback,
			Action failCallback)
		{
			ActionCostData actionCost = GameDB.character.GetActionCost(type);
			if (actionCost == null)
			{
				if (failCallback != null)
				{
					failCallback();
				}

				return;
			}

			if (actionCost.sp > player.Status.Sp)
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("스태미너가 부족합니다!"));
				if (failCallback != null)
				{
					failCallback();
				}

				return;
			}

			if (type >= CastingActionType.CraftCommon && type <= CastingActionType.CraftLegend)
			{
				isCrafting = true;
			}

			if (successCallback != null)
			{
				successCallback();
			}
		}


		public bool CheckCollectibleBox(LocalPlayerCharacter player, LocalItemBox itemBox)
		{
			if (itemBox.ObjectType != ObjectType.ResourceItemBox)
			{
				return true;
			}

			LocalResourceItemBox localResourceItemBox = (LocalResourceItemBox) itemBox;
			CollectibleData collectibleData = localResourceItemBox.CollectibleData;
			ItemData itemData = GameDB.item.FindItemByCode(collectibleData.itemCode);
			Item item = new Item(-1, itemData.code, itemData.initialCount, 0);
			if (!MyPlayer.Inventory.CanAddItem(item))
			{
				MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("인벤토리 공간이 부족 합니다."));
				return false;
			}

			return Time.time > localResourceItemBox.CooldownUntil;
		}


		public void HandleError(ErrorType errorType, string msg)
		{
			if (errorType == ErrorType.UserGameFinished)
			{
				GlobalUserData.finishedBattleTokenKeys.Add(MonoBehaviourInstance<GameClient>.inst.BattleTokenKey);
			}

			if (errorType > ErrorType.GAME_ERROR_BOUND)
			{
				if (isGameStarted)
				{
					Log.E("[ClientService.HandleError] {0}, {1}", errorType, msg);
					if (Ln.HasKey(msg))
					{
						msg = Ln.Get(msg);
					}

					if (errorType != ErrorType.ObjectNotFound)
					{
						ToastMessageUI toastMessage = MonoBehaviourInstance<GameUI>.inst.ToastMessage;
						if (toastMessage == null)
						{
							return;
						}

						toastMessage.ShowMessage(msg);
					}

					return;
				}

				Log.E("[ClientService.HandleError] {0}, {1}", errorType, msg);
			}
			else
			{
				if (errorType == ErrorType.GameStartedAlready)
				{
					MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("접속이 지연되어 게임이 이미 시작되었습니다."), gameClient.GoToLobby);
					return;
				}

				string text = errorType.ToString();
				if (Ln.HasKey(text))
				{
					text = Ln.Get(text);
				}

				MonoBehaviourInstance<Popup>.inst.Error(text, gameClient.GoToLobby);
			}
		}


		public void AddUserRank(int objectId, int teamNumber, int rank)
		{
			foreach (PlayerContext playerContext in teamMap[teamNumber])
			{
				playerContext.Character.SetRank(rank);
			}

			foreach (PlayerInfo playerInfo in (from x in GlobalUserData.dicPlayerResults.Values
				where x.teamNumber == teamNumber
				select x).ToList<PlayerInfo>())
			{
				UISystem.Action(new UpdatePlayerInfo(playerInfo.objectId, teamNumber, playerInfo.teamSlot, rank,
					playerInfo.userId));
			}
		}


		public void SurrenderGame()
		{
			SetPlayTime();
			Singleton<SoundControl>.inst.CleanUp();
			SingletonMonoBehaviour<Bootstrap>.inst.LoadLobby();
		}


		public IEnumerable<PlayerContext> GetPlayers()
		{
			return playerMap.Values;
		}


		public SortedDictionary<int, List<PlayerContext>> GetTeams()
		{
			return teamMap;
		}


		public bool CheckCurrentAreaRestrict(int currentAreaCode)
		{
			return areaStateMap[currentAreaCode] == AreaRestrictionState.Restricted;
		}


		public void ShowAirSupplyPositionEffect(Vector3 position)
		{
			GameObject item = Instantiate<GameObject>(
				SingletonMonoBehaviour<ResourceManager>.inst.LoadEffect("FX_BI_SupplyBox_01"), position,
				Quaternion.identity, transform);
			airSupplyPositionEffectList.Add(item);
		}


		public void ClearAirSupplyPositionEffect()
		{
			for (int i = 0; i < airSupplyPositionEffectList.Count; i++)
			{
				Destroy(airSupplyPositionEffectList[i]);
			}

			airSupplyPositionEffectList.Clear();
		}


		public bool CanOpenItemBox(LocalItemBox itemBox)
		{
			return itemBox.ObjectType != ObjectType.AirSupplyItemBox ||
			       itemBox.GetComponent<LocalAirSupplyItemBox>().CanOpen;
		}


		public void OpenAirSupplyItemBox(int objectId)
		{
			LocalAirSupplyItemBox localAirSupplyItemBox = null;
			if (world.TryFind<LocalAirSupplyItemBox>(objectId, ref localAirSupplyItemBox))
			{
				localAirSupplyItemBox.PlayOpen();
			}
		}


		public void CancelOpenAirSupplyItemBox(int objectId)
		{
			LocalAirSupplyItemBox localAirSupplyItemBox = null;
			if (world.TryFind<LocalAirSupplyItemBox>(objectId, ref localAirSupplyItemBox))
			{
				localAirSupplyItemBox.CancelOpen();
			}
		}


		private void PlayAmbientA()
		{
			List<SoundGroupData> soundGroupData;
			if (isMyPlayerInDoor)
			{
				soundGroupData = GameDB.level.GetSoundGroupData("Ambie_A_Room");
			}
			else
			{
				soundGroupData = GameDB.level.GetSoundGroupData(dayNight.GetSoundGroupName());
			}

			Singleton<SoundControl>.inst.PlayAmbientA(soundGroupData);
		}


		private void PlayAmbientB()
		{
			AreaSoundData areaSound = GameDB.level.GetAreaSound(areaCode);
			if (areaSound != null)
			{
				string text = "";
				if (isMyPlayerInDoor)
				{
					text = areaSound.AbmiSoundGroupRoom;
				}
				else if (text.Equals("None") || !isMyPlayerInDoor)
				{
					text = dayNight == DayNight.Day ? areaSound.AmbiSoundGroupDay : areaSound.AmbiSoundGroupNight;
				}

				List<SoundGroupData> soundGroupData = GameDB.level.GetSoundGroupData(text);
				Singleton<SoundControl>.inst.PlayAmbientB(soundGroupData);
			}
		}


		public void UpdateMyPlayerInDoor()
		{
			isMyPlayerInDoor = true;
			PlayAmbientA();
			PlayAmbientB();
		}


		public void UpdateMyPlayerOutDoor()
		{
			isMyPlayerInDoor = false;
			PlayAmbientA();
			PlayAmbientB();
		}


		private bool gettableReward()
		{
			return gameClient.MatchingMode == MatchingMode.Normal || gameClient.MatchingMode == MatchingMode.Rank;
		}


		public int GetCurrentPlayTime()
		{
			return (int) (DateTime.Now - gameStartTime).TotalSeconds;
		}


		public void SetPlayTime()
		{
			GlobalUserData.myPlayTime = (int) (DateTime.Now - gameStartTime).TotalSeconds;
		}


		public void ToggleStopRestriction()
		{
			isStopAreaRestriction = !isStopAreaRestriction;
		}


		public void SettingWicklineRespawnTime(float respawnStartTime)
		{
			wicklineResponRemainTime = respawnStartTime - CurrentServerFrameTime;
			isWicklineDead = false;
		}


		public void WicklineMoveArea(int areaCode)
		{
			string content = string.Format(Ln.Get("위클라인이동"), LnUtil.GetAreaName(areaCode));
			MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(content, true);
		}


		public void WicklineDead(int attackerobjectId)
		{
			isWicklineDead = true;
			string text = Ln.Format("위클라인죽음어나운스", GetPlayerNickName(attackerobjectId));
			MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(text, true);
			MonoBehaviourInstance<GameUI>.inst.SpecialAnnounceUI.ShowMessage(AnnounceType.Wickeline_Dead, text);
		}


		private void UpdateWicklineInfo()
		{
			if (isWicklineDead)
			{
				return;
			}

			if (wicklineResponRemainTime <= 0f)
			{
				return;
			}

			float num = wicklineResponRemainTime;
			wicklineResponRemainTime -= Time.unscaledDeltaTime;
			float num2 = 59.5f;
			if (num > num2 && wicklineResponRemainTime <= num2)
			{
				string text = Ln.Get("위클라인대기");
				MonoBehaviourInstance<GameUI>.inst.ChattingUi.AddSystemChatting(text, true);
				MonoBehaviourInstance<GameUI>.inst.SpecialAnnounceUI.ShowMessage(AnnounceType.Wickeline_CreateExpected,
					text);
			}

			if (wicklineResponRemainTime < 0f)
			{
				wicklineResponRemainTime = 0f;
			}
		}


		public void SpawnWickline(LocalMonster wickline)
		{
			wicklineResponRemainTime = 0f;
			wicklineVoiceControl = new WicklineVoiceControl(wickline);
		}


		public void SetGamePlayMode(GamePlayMode gamePlayMode)
		{
			curGamePlayMode = gamePlayMode;
		}


		public void UpdateRankingWatchingExit()
		{
			if (MyPlayer.IsObserving)
			{
				int currentTeamRank = GetCurrentTeamRank(MyPlayer.Character.TeamNumber);
				myPlayer.Character.SetRank(currentTeamRank);
				AddUserRank(MyObjectId, MyPlayer.Character.TeamNumber, currentTeamRank);
			}
		}


		public void GoToLobby()
		{
			this.StartThrowingCoroutine(RequestNicknamePair(LoadLobby),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][FadeOut] Message:" + exception.Message + ", StackTrace:" + exception.StackTrace);
				});
		}


		private void LoadLobby()
		{
			if (!IsGameEnd)
			{
				StopGame();
			}

			SetPlayTime();
			Singleton<SoundControl>.inst.CleanUp();
			SingletonMonoBehaviour<Bootstrap>.inst.LoadLobby();
		}


		private IEnumerator RequestNicknamePair(Action callBack = null)
		{
			bool responsed = false;
			float untilTimeOut = 2f;
			MonoBehaviourInstance<GameClient>.inst.Request<ReqNicknamePair, ResNicknamePair>(new ReqNicknamePair(),
				delegate(ResNicknamePair resNicknamePair)
				{
					responsed = true;
					if (resNicknamePair != null)
					{
						NickNamePairDic(resNicknamePair.nicknamePairDic);
					}
				});
			while (!responsed)
			{
				yield return new WaitForSeconds(0.1f);
				untilTimeOut -= 0.1f;
				if (untilTimeOut <= 0f)
				{
					break;
				}
			}

			if (callBack != null)
			{
				callBack();
			}
		}


		public LocalPlayerCharacter GetFirstPlayer()
		{
			foreach (KeyValuePair<int, List<PlayerContext>> keyValuePair in teamMap)
			{
				if (0 < keyValuePair.Value.Count)
				{
					return keyValuePair.Value[0].Character;
				}
			}

			return null;
		}


		public string GetPlayerNickName(int objectId)
		{
			foreach (KeyValuePair<long, PlayerContext> keyValuePair in playerMap)
			{
				if (keyValuePair.Value.Character.ObjectId == objectId)
				{
					return keyValuePair.Value.nickname;
				}
			}

			return null;
		}


		public void NickNamePairDic(Dictionary<long, NicknamePair> nicknamePairDic)
		{
			if (nicknamePairDic == null)
			{
				return;
			}

			foreach (KeyValuePair<long, NicknamePair> keyValuePair in nicknamePairDic)
			{
				PlayerContext playerContext = FindPlayerContext(keyValuePair.Key);
				if (playerContext != null)
				{
					LocalPlayerCharacter character = playerContext.Character;
					if (!(character == null))
					{
						UISystem.Action(new UpdatePlayerInfo(character.ObjectId, character.TeamNumber,
							character.TeamSlot, character.Rank, keyValuePair.Key, playerContext.IsUseTempNickname,
							keyValuePair.Value));
					}
				}
			}
		}


		public bool IsFinalSafeZone()
		{
			int num = areaStateMap.Count(x => x.Value == AreaRestrictionState.Restricted);
			return areaStateMap.Count == num;
		}


		public bool IsInFinalSafeZone(LocalPlayerCharacter character, AreaData areaData)
		{
			if (areaData == null)
			{
				areaData = character.GetCurrentAreaData(currentLevel);
			}

			if (character == null || areaData.code == 0)
			{
				return false;
			}

			LocalSecurityConsole localSecurityConsole =
				World.FindAll<LocalSecurityConsole>().FirstOrDefault(x => x.AreaCode == areaData.code);
			return localSecurityConsole != null && localSecurityConsole.IsLastSafeConsole() &&
			       localSecurityConsole.IsNear(character.GetPosition());
		}

		private void Ref()
		{
			Reference.Use(isAlreadyEventLogging);
		}


		private enum GameFinalizeType
		{
			Invalid,

			GameEnd,

			DeadAll,

			DeadAndTeamAlive,

			Tutorial
		}
	}
}