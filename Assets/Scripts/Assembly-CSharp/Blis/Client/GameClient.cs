using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blis.Common;
using Blis.Common.Utils;
using Blis.Server;
using UltimateHierarchy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class GameClient : MonoBehaviourInstance<GameClient>, INetClientHandler
	{
		private const float RECONNECT_POPUP_TERM = 5f;

		public float lastHandleCommandTime;
		public float extrapolateBeginTime;
		public bool isExtrapolated;
		public float lastPingUpdateTick;

		private readonly CommandQueue commandQueue = new CommandQueue();
		private readonly List<byte[]> pendingPackets = new List<byte[]>();
		private readonly List<RequestHolder> reqQueue = new List<RequestHolder>();

		private AudioListener audioListener;
		private string battleTokenKey;
		private int botCount;
		private BotDifficulty botDifficulty;
		private ClientService clientService;
		private DejitterBufferCounter dejitterBufferCounter;
		private DialogPopup dialogPopup;

		private bool extrapolationStarted;
		private float gameSnapshotTime;

		private UltimateHierarchyPro highGraphicQuality;
		private UltimateHierarchyPro mediumGraphicQuality;
		private UltimateHierarchyPro lowGraphicQuality;

		private string host = "localhost";
		private float initialFrameTime = float.MaxValue;
		private bool isConnected;
		private bool isDisconnecting;
		private bool isInitialized;
		private bool isLoadCompleted;
		private bool isOldGame;
		private bool isPressStart;
		private bool isReconnecting;
		private bool isStandalone = true;
		private bool isTestMode;
		private bool isWaitingSnapshot;
		private float lastCommandRecvTime;
		private int lastRecvSeq;
		private float lastShowReconnectPopupTimeDelta;

		private MatchingMode matchingMode;
		private MatchingTeamMode matchingTeamMode;

		public Dictionary<int, MissingCommand> missingCommands = new Dictionary<int, MissingCommand>();

		private INetClient netClient;

		private int port = GameConstants.DefaultPort;
		private uint reqIdx;

		private ISerializer serializer = Serializer.Default;
		private string serverName = "";
		private BattleToken testBattleToken;
		private MatchingToken testMatchingToken;
		private int testTeamNumber;
		private float tick;
		private TutorialController tutorialController;
		private long userId;

		public string BattleTokenKey => battleTokenKey;
		public MatchingMode MatchingMode => matchingMode;
		public MatchingTeamMode MatchingTeamMode => matchingTeamMode;
		public bool IsTeamMode => matchingTeamMode.IsTeamMode();
		public bool IsStandalone => isStandalone;
		public bool IsTutorial => matchingMode.IsTutorialMode();
		private TutorialController TutorialController {
			get
			{
				if (tutorialController == null)
				{
					GameUtil.BindOrAdd<TutorialController>(gameObject, ref tutorialController);
				}

				return tutorialController;
			}
		}

		public float CurrentSeq => commandQueue.CurrentSeq;
		public float EstimatedServerFrameTime => Mathf.Max(Time.time - initialFrameTime, 0f);
		public bool IsConnected => isConnected;
		
		private void Update()
		{
			INetClient iNetClient = this.netClient;
			iNetClient?.Update();

			if (clientService.IsGameStarted || clientService.IsGameEnd)
			{
				if (commandQueue.CurrentSeq >= 0)
				{
					tick += Time.deltaTime;
					while (tick >= clientService.FrameUpdateRate)
					{
						if (!FrameUpdate())
						{
							tick = clientService.FrameUpdateRate;
							return;
						}

						tick -= clientService.FrameUpdateRate;
					}

					return;
				}

				if (commandQueue.IsQueueFilled())
				{
					FrameUpdate();
				}
			}
		}


		public void OnGUI()
		{
			if (clientService.IsGameStarted)
			{
				return;
			}

			if (isTestMode && isLoadCompleted && !isStandalone && !isPressStart)
			{
				float num = Screen.width / 2f;
				float num2 = Screen.height / 2f;
				float num3 = 300f;
				float num4 = 120f;
				if (GUI.Button(new Rect(num - num3 / 2f, num2 - num4 / 2f, num3, num4), "READY!"))
				{
					isPressStart = true;
					Ready();
				}

				if (GUI.Button(new Rect(num - num3 / 2f, 100f, num3, num4), "DUMMY"))
				{
					new GameObject().AddComponent<DummyClient>().Connect(host, port, clientService.MyObjectId);
				}
			}
		}


		public void OnConnected()
		{
			isConnected = true;
			
			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.Handshake, 0f);
			BeginWaitingSnapshot();
			
			if (!isReconnecting && isTestMode)
			{
				BattleToken battleToken = new BattleToken
				{
					matchingMode = testMatchingToken.matchingMode,
					matchingTeamMode = testMatchingToken.matchingTeamMode,
					botCount = botCount,
					botDifficulty = botDifficulty
				};
				
				if (testMatchingToken.matchingTeamMode == MatchingTeamMode.Solo || testTeamNumber < 0)
				{
					testTeamNumber = 1;
				}

				Dictionary<long, MatchingTeamMemberToken> teamMembers = new Dictionary<long, MatchingTeamMemberToken>
				{
					{
						testMatchingToken.userNum,
						new MatchingTeamMemberToken
						{
							userNum = testMatchingToken.userNum,
							nickname = testMatchingToken.nickname,
							mmr = testMatchingToken.mmr,
							characterCode = testMatchingToken.characterCode,
							skinCode = testMatchingToken.skinCode,
							weaponCode = testMatchingToken.weaponCode,
							emotion = testMatchingToken.emotion
						}
					}
				};
				
				if (testMatchingToken.observer)
				{
					battleToken.observers = new List<MatchingObserverToken>();
					battleToken.observers.Add(new MatchingObserverToken
					{
						userNum = testMatchingToken.userNum,
						nickname = testMatchingToken.nickname
					});
				}
				else
				{
					battleToken.matchingTeams = new Dictionary<int, MatchingTeamToken>
					{
						{
							testTeamNumber,
							new MatchingTeamToken
							{
								teamNo = testTeamNumber,
								teamMMR = 0,
								teamMembers = teamMembers
							}
						}
					};
				}

				testBattleToken = battleToken;
				Request<TesterHandshake, ResHandshake>(new TesterHandshake
				{
					userId = testMatchingToken.userNum,
					battleToken = battleToken
				}, OnHandshake);
				
				return;
			}

			if (isReconnecting && isTestMode)
			{
				Request<TesterHandshake, ResHandshake>(new TesterHandshake
				{
					userId = testMatchingToken.userNum,
					battleToken = testBattleToken
				}, OnHandshake);
				return;
			}

			Request<Handshake, ResHandshake>(new Handshake
			{
				userId = userId,
				battleTokenKey = battleTokenKey,
				isReconnect = isOldGame || isReconnecting
			}, OnHandshake);
		}

		public void OnDisconnected()
		{
			Log.E("Disconnected!!");
			
			isConnected = false;
			isReconnecting = false;
			
			if (!isDisconnecting && clientService.IsGameStarted && !clientService.IsGameEnd)
			{
				OnShowReconnectPopup();
				return;
			}

			if (!isDisconnecting)
			{
				MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("서버와 접속이 종료되어 로비로 돌아갑니다."), GoToLobby);
				Log.E("[GameClient] 서버와 접속이 종료되어 로비로 돌아갑니다: OnDisconnected");
			}

			clientService.StopGame();
		}

		public void OnRecv(byte[] data)
		{
			ServerPacketWrapper serverPacketWrapper = Serializer.Compression.Deserialize<ServerPacketWrapper>(data);
			Singleton<GameTime>.inst.UpdateRTT(netClient.GetLatency() * 2, serverPacketWrapper.serverTime / 1000f);
			if (isWaitingSnapshot)
			{
				if (!serverPacketWrapper.IsType(typeof(ResPacket)) &&
				    serverPacketWrapper.packetType != PacketType.RpcError)
				{
					pendingPackets.Add(data);
					return;
				}

				if (serverPacketWrapper.packetType == PacketType.ResGameSnapshot)
				{
					gameSnapshotTime = serverPacketWrapper.serverTime;
				}
			}

			if (gameSnapshotTime > 0f && serverPacketWrapper.serverTime < gameSnapshotTime)
			{
				return;
			}

			PacketType packetType = serverPacketWrapper.packetType;
			if (packetType == PacketType.CommandList)
			{
				CommandList packet = serverPacketWrapper.GetPacket<CommandList>();
				if (Debug.isDebugBuild && data.Length > 100)
				{
					Log.W("CommandPacket Length exceeds 100. Size: {0}", data.Length);
					for (int i = 0; i < 3; i++)
					{
						if (packet.commands[i] != null)
						{
							foreach (PacketWrapper packetWrapper in packet.commands[i]) { }
						}
					}
				}

				initialFrameTime = Mathf.Min(Time.time - packet.seq * 0.033333335f, initialFrameTime);
				try
				{
					commandQueue.Enqueue(packet);
					RequestMissingSeqs(commandQueue.GetMissingSeqs());
				}
				catch (CommandQueueOverflowException)
				{
					OnError(2);
				}
				catch (Exception e)
				{
					Log.Exception(e);
				}

				if (packet.seq > lastRecvSeq)
				{
					if (lastCommandRecvTime > 0f && dejitterBufferCounter != null)
					{
						int playoutBufferCount =
							dejitterBufferCounter.CalcJitterBufferSize(Time.time - lastCommandRecvTime);
						commandQueue.SetPlayoutBufferCount(playoutBufferCount);
					}

					lastCommandRecvTime = Time.time;
				}

				lastRecvSeq = packet.seq;
				return;
			}

			if (serverPacketWrapper.IsType(typeof(ResPacket)))
			{
				HandleResponse(serverPacketWrapper);
				return;
			}

			if (serverPacketWrapper.IsType(typeof(RpcPacket)))
			{
				serverPacketWrapper.GetPacket<RpcPacket>().Action(clientService);
				return;
			}

			Log.E("Recv Invalid PacketType: [{0}]", serverPacketWrapper.packetType.ToString());
		}

		public void OnError(int errorCode)
		{
			Log.E(string.Format("[GameClient] 서버와 접속이 종료되었습니다.: {0}", errorCode));
			if (!isDisconnecting && clientService.IsGameStarted && !clientService.IsGameEnd)
			{
				OnShowReconnectPopup();
				return;
			}

			MonoBehaviourInstance<Popup>.inst.Error(
				Ln.Get("서버와 접속이 종료되어 로비로 돌아갑니다.") + string.Format(" ({0})", errorCode), GoToLobby);
		}
		
		private uint IncrementAndGetReqId()
		{
			uint result = reqIdx + 1U;
			reqIdx = result;
			return result;
		}

		private static string RandomString(int length)
		{
			return new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
				select s[Random.Range(0, s.Length)]).ToArray<char>());
		}

		protected override void _Awake()
		{
			GameUtil.BindOrAdd<ClientService>(gameObject, ref clientService);
			clientService.Init(this);
			
			GameObject parent = GameObject.Find("GraphicQualitySettings");
			
			lowGraphicQuality = GameUtil.Bind<UltimateHierarchyPro>(parent, "GraphicQualityLow");
			mediumGraphicQuality = GameUtil.Bind<UltimateHierarchyPro>(parent, "GraphicQualityMedium");
			highGraphicQuality = GameUtil.Bind<UltimateHierarchyPro>(parent, "GraphicQualityHigh");
			
			ApplyGraphicLevel(Singleton<LocalSetting>.inst.setting.graphicQuality);
		}

		private void SetGameServerHost(string url)
		{
			try
			{
				string[] array = url.Split(':');
				host = array[0];
				port = int.Parse(array[1]);
			}
			catch (Exception ex)
			{
				Log.E("Failed to parse host. {0}", url);
				throw ex;
			}
		}

		private void InitNetwork(string url, long userId)
		{
			SetGameServerHost(url);
			this.userId = userId;
			netClient = NetFactory.CreateClient();
			netClient.Init(this, userId);
			SingletonMonoBehaviour<AnimationEventService>.inst.AnimationCollection.LoadGameCommon();
		}

		public void Init(MatchingResult matchingResult, long userNum, bool useVIP, bool isOldGame)
		{
			isInitialized = true;
			isTestMode = false;
			isStandalone = false;
			isLoadCompleted = false;
			matchingMode = matchingResult.matchingMode;
			matchingTeamMode = matchingResult.matchingTeamMode;
			battleTokenKey = matchingResult.battleTokenKey;
			this.isOldGame = isOldGame;
			string url = matchingResult.battleHost;
			if (useVIP && !string.IsNullOrEmpty(matchingResult.vip))
			{
				url = matchingResult.vip;
			}

			InitNetwork(url, userNum);
			Log.H("[GameClient] battleTokenKey: {0}, host: {1}, userId: {2}, useVIP: {3}", battleTokenKey, host,
				userNum, useVIP);
		}

		public void InitTutorial(TutorialType tutorialType, bool isStandalone, string host, int botCount,
			BotDifficulty botDifficulty, MatchingToken matchingToken)
		{
			TutorialController.Init(tutorialType);
			InitTester(isStandalone, host, botCount, botDifficulty, 0, matchingToken);
		}

		public void InitTester(bool isStandalone, string host, int botCount, BotDifficulty botDifficulty,
			int teamNumber, MatchingToken matchingToken)
		{
			isInitialized = true;
			this.isStandalone = isStandalone;
			isTestMode = true;
			testMatchingToken = matchingToken;
			this.botCount = botCount;
			this.botDifficulty = botDifficulty;
			matchingMode = matchingToken.matchingMode;
			matchingTeamMode = matchingToken.matchingTeamMode;
			testTeamNumber = teamNumber;
			InitNetwork(host, matchingToken.userNum);
			if (isStandalone)
			{
				Connect();
			}

			Log.H("[GameClient] standalone: {0}, host: {1}, userId: {2}, nickname: {3}", this.isStandalone, host,
				matchingToken.userNum, matchingToken.nickname);
		}

		public void Connect()
		{
			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.Connect, 0f);
			netClient.Open(host, port);
		}
		
		private byte[] CreatePacket(IPacket packet)
		{
			if (Debug.isDebugBuild)
			{
				PacketType packetType = packet.GetType().GetCustomAttribute<PacketAttr>().packetType;
			}

			return new ClientPacketWrapper(userId, packet).Serialize<ClientPacketWrapper>();
		}
		
		private void Send<T>(T packet, NetChannel netChannel = NetChannel.ReliableOrdered) where T : IPacket
		{
			byte[] array = CreatePacket(packet);
			if (Debug.isDebugBuild && array != null && array.Length > 500)
			{
				Log.W("Packet length is too long: {0}, {1}byte", packet.GetType().ToString(), array.Length);
			}

			netClient.Send(array, netChannel);
		}
		
		public void Request(ReqPacket packet, NetChannel netChannel = NetChannel.ReliableOrdered)
		{
			Send<ReqPacket>(packet, netChannel);
		}
		
		public void Request<Tpacket, Tcallback>(Tpacket packet, Action<Tcallback> callback)
			where Tpacket : ReqPacketForResponse where Tcallback : ResPacket
		{
			uint reqId = IncrementAndGetReqId();
			packet.reqId = reqId;
			netClient.Send(CreatePacket(packet), NetChannel.ReliableOrdered);
			reqQueue.Add(new RequestHolder(packet,
				delegate(ServerPacketWrapper res) { callback(res.GetPacket<Tcallback>()); }));
		}
		
		private void OnHandshake(ResHandshake res)
		{
			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.Join, 0f);
			serverName = res.hostname;
			
			if (string.IsNullOrEmpty(serverName))
			{
				serverName = string.Format("{0}:{1}", host, port);
			}

			if (serverName.Contains("release-"))
			{
				serverName = serverName.Replace("release-", "");
			}

			Singleton<GameEventLogger>.inst.SetServerName(res.hostname);
			if (isReconnecting)
			{
				Request<ReqGameSnapshot, ResGameSnapshot>(new ReqGameSnapshot(),
					delegate(ResGameSnapshot resGameSnapshot)
					{
						this.StartThrowingCoroutine(OnGameSnapshot(resGameSnapshot),
							delegate(Exception exception)
							{
								Log.E("[EXCEPTION][OnGameSnapshot] Message:" + exception.Message + ", StackTrace:" +
								      exception.StackTrace);
							});
					});
				
				return;
			}

			Request<ReqJoin, ResJoin>(new ReqJoin
				{
					identifier = SystemInfo.deviceUniqueIdentifier,
					language = Ln.GetCurrentLanguage().ToString()
				},callback);

			void callback(ResJoin resJoin)
			{
				this.StartThrowingCoroutine(OnJoin(resJoin),
					delegate(Exception exception)
					{
						Log.E("[EXCEPTION][OnJoin] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
			}
		}
		
		private IEnumerator OnJoin(ResJoin res)
		{
			GlobalUserData.lastGameId = res.gameId;
			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.LoadCharacter, 0f);
			
			yield return null;
			
			int totalCharacterCount = res.userList.Count + 1;
			int loadedCharacterCount = 0;
			
			clientService.SetupLevel(IsTutorial
				? GameDB.level.GetLevelData(matchingMode.ToString())
				: GameDB.level.DefaultLevel);
			clientService.CreateLocalObjects();
			
			if (res.isObserver)
			{
				clientService.CreateMyObserver(res.userId, res.nickname, res.playerSnapshot, res.character);
			}
			else
			{
				Debug.LogError($"MyUserIdRes : {userId}");
				clientService.CreateMyPlayerCharacter(res.userId, res.nickname, res.startingWeaponCode,
					res.playerSnapshot, res.character);
				
				if (isOldGame)
				{
					if (inst.MatchingMode == MatchingMode.Dev)
					{
						RouteApi.userFavorites.Clear();
					}
					else
					{
						RouteApi.SetFavoritesList(clientService.MyPlayer.Character.CharacterCode, delegate { }, null);
					}
				}
			}

			LoadingContext loadingContext = LoadingView.inst.LoadingContext;
			const LoadingContext.Phase phase = LoadingContext.Phase.LoadCharacter;
			
			loadedCharacterCount++;
			loadingContext.SetProgress(phase, (float)loadedCharacterCount / totalCharacterCount);
			
			yield return null;
			
			foreach (UserInfo userInfo in res.userList)
			{
				clientService.CreatePlayerCharacter(userInfo.userId, userInfo.nickname, userInfo.startingWeaponCode,
					userInfo.playerSnapshot, userInfo.characterSnapshot);
				
				LoadingContext loadingContext2 = LoadingView.inst.LoadingContext;
				const LoadingContext.Phase phase2 = LoadingContext.Phase.LoadCharacter;
				
				loadedCharacterCount++;
				loadingContext2.SetProgress(phase2, (float)loadedCharacterCount / totalCharacterCount);
				
				yield return null;
			}

			yield return this.StartThrowingCoroutine(LoadEffect(),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][LoadEffect] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
			
			this.StartThrowingCoroutine(LoadLevel(res.snapshot),
				delegate(Exception exception)
				{
					Log.E("[EXCEPTION][LoadLevel] Message:" + exception.Message + ", StackTrace:" +
					      exception.StackTrace);
				});
		}


		private IEnumerator LoadEffect()
		{
			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.LoadEffect, 0f);
			yield return null;
			
			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.LoadEffect, 1f);
			yield return null;
		}


		private IEnumerator LoadLevel(List<SnapshotWrapper> levelSnapshot)
		{
			GameClient gameClient = this;
			
			float beginTime = Time.realtimeSinceStartup;
			float tick = 0.0f;
			
			int loadingCount = 0;
			int totalCount = levelSnapshot.Count;
			int takeFrame = 0;
			
			foreach (SnapshotWrapper snapshotWrapper in levelSnapshot)
			{
				SnapshotWrapper snapshot = snapshotWrapper;
				
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				
				// 월드에 오브젝트 데이터가 있는 경우
				if (gameClient.clientService.World.HasObjectId(snapshot.objectId))
				{
					// 초기화 및 위치 지정
					LocalObject localObject = gameClient.clientService.World.Find<LocalObject>(snapshot.objectId);
					localObject.Init(snapshot.snapshot);
					localObject.transform.SetPositionAndRotation(snapshot.position, snapshot.rotation);
				}
				else
				{
					// 없는 경우 생성
					LocalObject localObject = gameClient.clientService.World.SpawnSnapshot(snapshot);
					if (localObject != null)
					{
						// 로컬 캐릭터인 경우
						localObject.IfTypeOf<LocalCharacter>(localCharacter =>
						{
							CharacterSnapshot characterSnapshot =
								Serializer.Default.Deserialize<CharacterSnapshot>(snapshot.snapshot);
							
							// 스테이트 업데이트
							localCharacter.InitStateEffect(characterSnapshot.initialStateEffect);
						});
					}
				}

				++loadingCount;
				tick += Time.realtimeSinceStartup - realtimeSinceStartup;
				
				if (tick > 0.0199999995529652)
				{
					tick = 0.0f;
					++takeFrame;
					
					LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.LoadLevel,
						loadingCount / (float) totalCount);
					
					yield return new WaitForEndOfFrame();
				}
			}

			Log.H("[GameClient] Load level is done. {0}s, frame: {1}",
				(object) (float) ((double) Time.realtimeSinceStartup - (double) beginTime), (object) takeFrame);
			
			gameClient.Request<ReqGameSnapshot, ResGameSnapshot>(new ReqGameSnapshot(),
				resGameSnapshot =>
				{
					this.StartThrowingCoroutine(gameClient.OnGameSnapshot(resGameSnapshot), exception =>
					{
						Log.E("[EXCEPTION][OnGameSnapshot] Message:" + exception.Message + ", StackTrace:" +
						      exception.StackTrace);
					});
				});
		}
		
		private IEnumerator OnGameSnapshot(ResGameSnapshot resGameSnapshot)
		{
			bool isJoin = !isReconnecting;
			bool hasSnapshot = LoadGameSnapshot(resGameSnapshot);
			
			isReconnecting = false;
			
			MonoBehaviourInstance<GameUI>.inst.OnStopWaitRequest();
			EndWaitingSnapshot(hasSnapshot);
			
			yield return new WaitForEndOfFrame();
			
			if (isJoin)
			{
				isLoadCompleted = true;
				if (hasSnapshot)
				{
					LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.Done, 1f);
					LoadingView.inst.LoadingContext = null;
					
					yield return null;
					
					clientService.SetupGame(0);
					clientService.StartGame(resGameSnapshot.frameUpdateRate);
				}
				else if (!isTestMode || isStandalone)
				{
					Ready();
				}
			}
			else
			{
				// DialogPopup dialogPopup = this.dialogPopup;
				if (dialogPopup != null)
				{
					dialogPopup.Close();
				}
			}
		}

		public void Ready()
		{
			SendReady();
			
			Log.V("[GameClient] Send Ready.");
			
			LoadingView.inst.LoadingContext.SetProgress(LoadingContext.Phase.Done, 1f);
			LoadingView.inst.LoadingContext = null;
			MonoBehaviourInstance<GameUI>.inst.Events.OnGameSceneLoaded();
		}

		public void SendReady()
		{
			if (isLoadCompleted)
			{
				Send<Ready>(new Ready());
			}
		}

		private void OnShowReconnectPopup()
		{
			if (dialogPopup != null && dialogPopup.CanvasGroup.alpha == 1f)
			{
				return;
			}

			MonoBehaviourInstance<GameUI>.inst.OnStopWaitRequest();
			dialogPopup = (DialogPopup) MonoBehaviourInstance<Popup>.inst.Message(Ln.Get("서버와 접속이 종료되었습니다."),
				new Popup.Button
				{
					type = Popup.ButtonType.Confirm,
					text = Ln.Get("재접속"),
					callback = Reconnect
				}, new Popup.Button
				{
					text = Ln.Get("로비로 이동"),
					callback = delegate
					{
						clientService.StopGame();
						GoToLobby();
					}
				});
		}

		public void RequestMissingSeqs(List<int> missingSeqs)
		{
			List<int> list = new List<int>();
			foreach (int num in missingSeqs)
			{
				if (!missingCommands.ContainsKey(num))
				{
					list.Add(num);
					missingCommands.Add(num, new MissingCommand
					{
						seq = num,
						reqTime = Time.realtimeSinceStartup
					});
				}
			}

			if (list.Count == 0)
			{
				return;
			}

			Request<ReqMissingCommand, ResMissingCommand>(new ReqMissingCommand
			{
				seqs = list
			}, delegate(ResMissingCommand res)
			{
				if (res.commandLists == null)
				{
					Log.E("ResMissingCommand CommandLists is null");
					return;
				}

				foreach (CommandList commandList in res.commandLists)
				{
					if (commandList == null)
					{
						Log.E("ResMissingCommand CommandList is null");
					}
					else
					{
						MissingCommand missingCommand;
						if (missingCommands.TryGetValue(commandList.seq, out missingCommand))
						{
							missingCommands.Remove(commandList.seq);
						}

						try
						{
							commandQueue.Enqueue(commandList);
						}
						catch (CommandQueueOverflowException)
						{
							OnError(2);
						}
					}
				}
			});
		}

		public void OnGameStarted(float frameUpdateRate)
		{
			dejitterBufferCounter = new DejitterBufferCounter(frameUpdateRate);
			if (IsTutorial)
			{
				TutorialController.OnGameStarted();
			}
		}

		private bool FrameUpdate()
		{
			CommandEntry commandEntry = commandQueue.Dequeue();
			if (commandEntry == null)
			{
				if (!isExtrapolated)
				{
					extrapolateBeginTime = Time.time;
					isExtrapolated = true;
					Log.W("NO COMMAND FOR SEQ {0}", commandQueue.CurrentSeq + 1);
					Singleton<GameEventLogger>.inst.noCommandCount++;
				}

				if (lastHandleCommandTime > 0f)
				{
					float num = Time.time - lastHandleCommandTime;
					if (num > 10f)
					{
						Log.E("[GameClient] 서버와 접속이 종료되었습니다.: HANDLE_COMMAND_TIME_OUT");
						if (!isDisconnecting && clientService.IsGameStarted && !clientService.IsGameEnd)
						{
							if (num - lastShowReconnectPopupTimeDelta > 5f)
							{
								lastShowReconnectPopupTimeDelta = num;
								OnShowReconnectPopup();
							}
						}
						else
						{
							clientService.StopGame();
							MonoBehaviourInstance<Popup>.inst.Error(Ln.Get("서버와 접속이 종료되어 로비로 돌아갑니다."), GoToLobby);
						}
					}
					else if (num > 5f)
					{
						if (!MonoBehaviourInstance<GameUI>.inst.ToastMessage.isShowing())
						{
							MonoBehaviourInstance<GameUI>.inst.ToastMessage.ShowMessage(Ln.Get("서버로부터 패킷이 지연되고 있습니다."));
						}

						Singleton<GameEventLogger>.inst.gamePauseCount++;
					}
				}

				return false;
			}

			float num2 = 0f;
			if (isExtrapolated)
			{
				num2 = Time.time - extrapolateBeginTime;
			}

			float frameDeltaTime = Time.time - lastHandleCommandTime;
			HandleCommand(commandEntry.seq, commandQueue.FastForwardRequired() ? num2 + 0.033333335f : num2,
				frameDeltaTime, commandEntry.commands);
			int num3 = 0;
			if (commandQueue.FastForwardRequired())
			{
				commandEntry = commandQueue.Dequeue();
				HandleCommand(commandEntry.seq, num2, frameDeltaTime, commandEntry.commands);
				num3++;
			}

			lastHandleCommandTime = Time.time;
			isExtrapolated = false;
			return true;
		}

		private void HandleCommand(int seq, float extrapolatedTime, float frameDeltaTime, List<PacketWrapper> commands)
		{
			if (commands == null)
			{
				return;
			}

			if (MonoBehaviourInstance<ClientService>.inst.IsPlayer && Time.time - lastPingUpdateTick > 1f)
			{
				MonoBehaviourInstance<GameUI>.inst.Ping.SetPing(serverName,
					Mathf.FloorToInt(Singleton<GameTime>.inst.Latency), 0);
				lastPingUpdateTick = Time.time;
			}

			for (int i = 0; i < commands.Count; i++)
			{
				CommandPacket packet = commands[i].GetPacket<CommandPacket>();
				try
				{
					if (packet is MoveCommandPacket)
					{
						(packet as MoveCommandPacket).SetExpolatedTime(extrapolatedTime);
					}

					packet.Action(clientService);
				}
				catch (Exception e)
				{
					Log.Exception(e);
				}
			}
		}

		private void HandleResponse(ServerPacketWrapper wrap)
		{
			ResPacket response = wrap.GetPacket<ResPacket>();
			int num = reqQueue.FindIndex(x => x.reqId == response.reqId);
			if (num >= 0)
			{
				if (response is ResError)
				{
					OnErrorResponse(response as ResError);
				}
				else
				{
					reqQueue[num].OnResponse(wrap);
				}

				reqQueue.RemoveAt(num);
			}
		}

		private void OnErrorResponse(ResError resError)
		{
			Log.H(resError.errorType.ToString());
			Log.H(resError.msg);
			if (resError.errorType == ErrorType.UserGameFinished)
			{
				GlobalUserData.finishedBattleTokenKeys.Add(battleTokenKey);
			}

			clientService.HandleError(resError.errorType, resError.msg);
		}

		public void Disconnect()
		{
			isDisconnecting = true;
			INetClient netClient = this.netClient;
			if (netClient == null)
			{
				return;
			}

			netClient.Close();
		}

		public void GoToLobby()
		{
			Disconnect();
			Singleton<SoundControl>.inst.CleanUp();
			SingletonMonoBehaviour<Bootstrap>.inst.LoadLobby();
		}

		protected override void _OnDestroy()
		{
			base._OnDestroy();
			Disconnect();
		}

		public void SetSimulation(int minLatency, int maxLatency, int packetLoss)
		{
			netClient.SetSimulation(minLatency, maxLatency, packetLoss);
		}

		public AudioListener GetAudioListener()
		{
			if (audioListener == null)
			{
				audioListener = GetComponentInChildren<AudioListener>();
			}

			return audioListener;
		}


		public void ApplyGraphicLevel(int level)
		{
			switch (level)
			{
				case 0:
					lowGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, false);
					lowGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, false);
					mediumGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, false);
					mediumGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, false);
					highGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, false);
					highGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, false);
					break;
				case 1:
					lowGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, true);
					lowGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, true);
					mediumGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, false);
					mediumGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, false);
					highGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, false);
					highGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, false);
					break;
				case 2:
					lowGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, true);
					lowGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, true);
					mediumGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, true);
					mediumGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, true);
					highGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, false);
					highGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, false);
					break;
				default:
					lowGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, true);
					lowGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, true);
					mediumGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, true);
					mediumGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, true);
					highGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Gameobject, true);
					highGraphicQuality.setActiveUpdateAll(UltimateHierarchy.Slot.ObjectType.Script, true);
					break;
			}
		}


		private void BeginWaitingSnapshot()
		{
			isWaitingSnapshot = true;
		}


		private void EndWaitingSnapshot(bool hasSnapshot)
		{
			isWaitingSnapshot = false;
			if (!hasSnapshot)
			{
				gameSnapshotTime = 0f;
			}

			pendingPackets.ForEach(OnRecv);
			pendingPackets.Clear();
			gameSnapshotTime = 0f;
		}


		private void Reconnect()
		{
			MonoBehaviourInstance<GameUI>.inst.OnStartWaitRequest();
			if (!isReconnecting)
			{
				reqQueue.Clear();
				isReconnecting = true;
				INetClient netClient = this.netClient;
				if (netClient != null)
				{
					netClient.Close();
				}

				this.netClient = NetFactory.CreateClient();
				this.netClient.Init(this, userId);
				LoadingView.inst.LoadingContext = new LoadingContext();
				commandQueue.DequeueAll();
				Connect();
			}
		}
		
		private bool LoadGameSnapshot(ResGameSnapshot resGameSnapshot)
		{
			if (resGameSnapshot.gameSnapshot == null)
			{
				return false;
			}

			UserSnapshot userSnapshot = resGameSnapshot.userSnapshot;
			
			HashSet<int> outSightCharacterIds = resGameSnapshot.outSightCharacterIds;
			HashSet<int> invisibleCharacterIds = resGameSnapshot.invisibleCharacterIds;
			
			GameSnapshot gameSnapshot = resGameSnapshot.gameSnapshot;
			List<SnapshotWrapper> worldSnapshot = gameSnapshot.worldSnapshot;
			
			commandQueue.SkipToSeq(resGameSnapshot.currentSeq - 1);
			initialFrameTime = Mathf.Min(Time.time - resGameSnapshot.currentSeq * 0.033333335f, initialFrameTime);
			
			// 내 플레이어가 있음
			if (clientService.IsPlayer)
			{
				clientService.UpdateMyPlayerCharacter(userSnapshot.playerSnapshot, userSnapshot.characterSnapshot,
					userSnapshot.equips, userSnapshot.walkableNavMask, userSnapshot.exp, userSnapshot.inventoryItems,
					userSnapshot.survivalTime, userSnapshot.bulletItems);
				if (isOldGame && !isReconnecting && RouteApi.userFavorites != null)
				{
					int startingWeaponCode = clientService.MyPlayer.startingWeaponCode;
					ItemData itemData = GameDB.item.FindItemByCode(startingWeaponCode);
					if (itemData != null)
					{
						WeaponTypeInfoData weaponTypeInfoData =
							GameDB.mastery.GetWeaponTypeInfoData(itemData.GetSubTypeData<ItemWeaponData>().weaponType);
						RouteApi.userFavorites =
							RouteApi.userFavorites.FindAll(x => x.weaponType == weaponTypeInfoData.type);
					}

					if (RouteApi.userFavorites.Count > 0)
					{
						Favorite selectFavorite = RouteApi.userFavorites[0];
						MonoBehaviourInstance<ClientService>.inst.SetSelectFavorite(selectFavorite);
					}
				}
			}
			else
			{
				// 옵저버 모드
				clientService.UpdateMyObserver(userSnapshot.playerSnapshot, userSnapshot.characterSnapshot);
			}

			// 유저 리스트
			foreach (UserSnapshot user in gameSnapshot.userList)
			{
				// 내 플레이어가 아닐 때만
				if (user.userId != userSnapshot.userId)
				{
					// 시야 밖 체크
					bool outSight = outSightCharacterIds != null &&
					                outSightCharacterIds.Any(x => x.Equals(user.characterSnapshot.objectId));
					
					// 은신 체크
					bool invisible = invisibleCharacterIds != null &&
					                 invisibleCharacterIds.Any(x => x.Equals(user.characterSnapshot.objectId));
						
					// 플레이어 캐릭터 업데이트
					clientService.UpdatePlayerCharacter(user.userId, user.playerSnapshot, user.characterSnapshot,
						user.equips, user.walkableNavMask, outSight, invisible);
				}
			}

			// 월드 스냅샷
			foreach (SnapshotWrapper snapshot in worldSnapshot)
			{
				LocalSummonBase localSummonBase = null;
				
				// 월드에 있는 오브젝트면
				if (clientService.World.HasObjectId(snapshot.objectId))
				{
					LocalObject localObject4 = clientService.World.Find<LocalObject>(snapshot.objectId);
					
					// 초기화 및 데이터 업데이트
					localObject4.Init(snapshot.snapshot);
					localObject4.transform.SetPositionAndRotation(snapshot.position, snapshot.rotation);
					
					// 소환체면 캐시
					localObject4.IfTypeOf<LocalSummonBase>(summonBase => localSummonBase = summonBase);
				}
				else
				{
					// 없으면 새로 생성
					LocalObject localObject2 = clientService.World.SpawnSnapshot(snapshot);
					if (localObject2 != null)
					{
						// 로컬 캐릭터면
						localObject2.IfTypeOf<LocalCharacter>(localCharacter =>
						{
							CharacterSnapshot characterSnapshot =
								Serializer.Default.Deserialize<CharacterSnapshot>(snapshot.snapshot);

							localCharacter.InitStateEffect(characterSnapshot.initialStateEffect);
						});
						
						// 로컬 소환 오브젝트면
						localObject2.IfTypeOf<LocalSummonBase>(summon => localSummonBase = summon);
					}
				}

				// 소환 오브젝트가 있고
				if (localSummonBase != null)
				{
					// 게임 중이면
					if (clientService.IsPlayer)
					{
						bool isOutSight = outSightCharacterIds != null &&
						            outSightCharacterIds.Any(x => x.Equals(localSummonBase.ObjectId));
						
						bool isInivisible = invisibleCharacterIds != null &&
						             invisibleCharacterIds.Any(x => x.Equals(localSummonBase.ObjectId));
						
						if (isOutSight)
						{
							localSummonBase.OutSight();
						}
						else
						{
							localSummonBase.InSight();
						}

						if (isInivisible)
						{
							localSummonBase.OnInvisible();
						}
						else
						{
							localSummonBase.OnVisible();
						}
					}
					else
					{
						localSummonBase.InSight();
						localSummonBase.OnVisible();
					}
				}
			}

			clientService.World.FindAllDoAction<LocalMovableCharacter>(x => x.InitSkillController());

			// 사라진 오브젝트 삭제
			foreach (LocalObject localObject in clientService.World.AllObjectAlloc())
			{
				ObjectType objectType = localObject.ObjectType;
				if (objectType != ObjectType.PlayerCharacter && 
				    objectType != ObjectType.StaticItemBox &&
				    objectType - ObjectType.BotPlayerCharacter > 3 &&
				    worldSnapshot.FindIndex(x => x.objectId == localObject.ObjectId) < 0)
				{
					clientService.World.DestroyObject(localObject);
				}
			}
			
			foreach (LocalObject localObject in clientService.World.AllObjectAlloc())
			{
				// 시야 정보
				List<SightSnapshot> attachedSights =
					gameSnapshot.sights.FindAll(x => x.targetId == localObject.ObjectId);
				
				List<SightSnapshot> attachedSights2 = attachedSights;
				
				// 만료된 시야 오브젝트 파괴
				if (localObject.attachedSights != null)
				{
					List<LocalSightAgent> list = localObject.attachedSights.FindAll(x =>
						attachedSights.FindIndex(y => y.attachSightId == x.AttachSightId) < 0);
					
					attachedSights2.RemoveAll(x =>
						localObject.attachedSights.FindIndex(y => x.attachSightId == y.AttachSightId) >= 0);
					
					list.ForEach(Destroy);
				}

				// 유효한 시야 오브젝트 리프레시
				foreach (SightSnapshot sightSnapshot in attachedSights2)
				{
					if (clientService.World.HasObjectId(sightSnapshot.targetId) &&
					    clientService.World.HasObjectId(sightSnapshot.ownerId))
					{
						LocalObject localObject3 = clientService.World.Find<LocalObject>(sightSnapshot.targetId);
						LocalSightAgent localSightAgent = localObject3.gameObject.AddComponent<LocalSightAgent>();
						localSightAgent.InitAttachSight(localObject3, sightSnapshot.attachSightId);
						localSightAgent.SetOwner(clientService.World.Find<LocalCharacter>(sightSnapshot.ownerId)
							.SightAgent);
						localSightAgent.UpdateSightRange(sightSnapshot.sightRange.Value);
						localSightAgent.UpdateSightAngle(360);
					}
				}
			}

			// 제한 구역 업데이트
			clientService.UpdateRestrictedArea(gameSnapshot.areaStateMap, gameSnapshot.areaRestrictionRemainTime.Value,
				gameSnapshot.dayNight, gameSnapshot.day);
			
			int totalPlayTime = GameDB.level.GetTotalPlayTime(
				Mathf.FloorToInt(gameSnapshot.areaRestrictionRemainTime.Value), gameSnapshot.dayNight,
				gameSnapshot.day);
			
			MonoBehaviourInstance<GameUI>.inst.BattleInfoHud.SetReconnectPlayTime(totalPlayTime);
			if (gameSnapshot.isStopAreaRestriction)
			{
				clientService.ToggleStopRestriction();
			}

			if (gameSnapshot.wicklineRespawnRemainTime != null)
			{
				clientService.SettingWicklineRespawnTime(gameSnapshot.wicklineRespawnRemainTime.Value);
			}

			foreach (KeyValuePair<int, MoveAgentSnapshot> keyValuePair in gameSnapshot.moveAgentSnapshots)
			{
				(clientService.World.Find<LocalObject>(keyValuePair.Key) as ILocalMoveAgentOwner).MoveAgent
					.ApplySnapshot(keyValuePair.Value, MonoBehaviourInstance<ClientService>.inst.World);
			}

			if (isReconnecting)
			{
				MonoBehaviourInstance<GameUI>.inst.BattleInfoHud.SetAliveCount(clientService.GetAliveCount());
			}

			return true;
		}

		private void Ref()
		{
			Reference.Use(isInitialized);
		}

		public class MissingCommand
		{
			public float reqTime;
			public int seq;
		}
	}
}