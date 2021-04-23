using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Blis.Common;
using Blis.Common.Utils;
using Newtonsoft.Json;
using Tftp.Net;
using UnityEngine;

namespace Blis.Server
{
	
	public class GameServer : MonoBehaviourInstance<GameServer>, INetServerHandler
	{
		private BuildPhase ServerPhase
		{
			get
			{
				return this.DeployProperty.buildPhase;
			}
		}
		
		public DeployProperty DeployProperty { get; private set; } = Singleton<DeployPropertyManager>.inst.Get(DeploySetting.Local);
		private string HostName { get; set; }
		public GameService GameService { get; private set; }
		public int ListenPort { get; private set; } = GameConstants.DefaultPort;
		public int Seq { get; private set; }
		public string ServerRegion { get; private set; }
		
		protected override void _Awake()
		{
			try
			{
				Log.V($"Start Server {BSERVersion.VERSION}");
				
				this.ParseArguments();
				this.InitGameService();
				this.InitNetServer();
				this.serverStartedTime = (double)Time.realtimeSinceStartup;
				
				int num = this.ListenPort + 1000;
				
				this._tftpServer = new TftpServer(num);
				this._tftpServer.OnReadRequest += this.OnTftServerReadRequest;
				this._tftpServer.OnError += this.OnTftServerError;
				this._tftpServer.Start();
				
				Log.V(string.Format("World Server : Started {0}", num));
			}
			catch (Exception ex)
			{
				Log.Exception(ex);
				this.TerminateServer(ex.ToString());
			}
		}
		
		private void InitGameService()
		{
			this.GameService = GameUtil.TryGetOrAddComponent<GameService>(base.gameObject);
			this.GameService.Init(this);
		}
		
		private void InitNetServer()
		{
			this.netServer = NetFactory.CreateServer();
			int num = 0;
			bool flag = false;
			int num2 = this.ListenPort;
			while (!flag && num < 5)
			{
				try
				{
					this.netServer.Init(this, num2);
					flag = true;
				}
				catch (Exception)
				{
					num2++;
					num++;
				}
			}
		}
		
		private void ParseArguments()
		{
		}
		
		private void Update()
		{
			try
			{
				if (this.GameService.IsGameStarted())
				{
					this.frameCount.Update();
					this.tick += Time.deltaTime;
					
					while (this.tick + 0.001f >= 0.033333335f)
					{
						try
						{
							this.frameUpdateExcuteCount++;
							this.FrameUpdate();
							this.frameCount.FrameUpdate();
						}
						catch (Exception ex)
						{
							Log.W(ex.ToString());
						}
						finally
						{
							this.GameService.Player.ClearDeadPlayerInfo();
						}
						this.tick -= 0.033333335f;
					}
				}
				
				if (!this.GameService.IsGameStarted() && 
				    this.ServerPhase >= BuildPhase.Release && 
				    this.redisService != null && this.IsServerReserved() && 
				    this.onLineSessionMap.Count == 0 && 
				    (double)(Time.realtimeSinceStartup - this.redisService.GetReservedTime()) > GameConstants.SERVER_IDLE_TIMEOUT)
				{
					this.TerminateServer("Server Idle Timeout");
				}
			}
			catch (Exception ex2)
			{
				Log.W(ex2.ToString());
			}
			finally
			{
				INetServer netServer = this.netServer;
				if (netServer != null)
				{
					netServer.Update();
				}
			}
		}

		
		private void LateUpdate()
		{
			while (this.frameUpdateExcuteCount > 0)
			{
				try
				{
					this.frameUpdateExcuteCount--;
					this.FrameLateUpdate();
				}
				catch (Exception ex)
				{
					Log.W(ex.ToString());
				}
			}
		}

		
		public void EnqueueRpcCommandWithObserver(PlayerSession playerSession, PacketWrapper packetWrapper)
		{
			playerSession.EnqueueCommandPacket(packetWrapper);
			foreach (ObserverSession observerSession in this.GameService.Player.ObserverSessions)
			{
				observerSession.EnqueueCommandPacket(packetWrapper);
			}
		}

		
		public void EnqueueCommandTeamOnly(PlayerSession playerSession, PacketWrapper packetWrapper)
		{
			playerSession.EnqueueTeamMemberCommand(packetWrapper);
		}

		
		public void EnqueueCommandTeamAndObserver(PlayerSession playerSession, PacketWrapper packetWrapper)
		{
			playerSession.EnqueueTeamMemberCommand(packetWrapper);
			foreach (ObserverSession observerSession in this.GameService.Player.ObserverSessions)
			{
				observerSession.EnqueueCommandPacket(packetWrapper);
			}
		}

		
		public void EnqueueCommand(CommandPacket commandPacket)
		{
			PacketWrapper commandPacketWrapper = new PacketWrapper(commandPacket);
			foreach (Session session in this.allSessions)
			{
				session.EnqueueCommandPacket(commandPacketWrapper);
			}
		}

		
		private void FrameUpdate()
		{
			while (0 < this.requestQueue.Count)
			{
				ClientRequest clientRequest = this.requestQueue.Dequeue();
				this.RequestHandle(clientRequest.session, clientRequest.ClientPacketWrapper);
			}
			foreach (WorldObject worldObject in this.GameService.World.AllObjectAlloc())
			{
				try
				{
					worldObject.PreFrameUpdate();
				}
				catch (Exception)
				{
				}
			}
			foreach (WorldObject worldObject2 in this.GameService.World.AllObjectAlloc())
			{
				try
				{
					worldObject2.FrameUpdate();
				}
				catch (Exception)
				{
				}
			}
			this.GameService.Player.DecisionPlayerRaking();
		}

		
		private void FrameLateUpdate()
		{
			foreach (Session session in this.allSessions)
			{
				this.Send(session, session.GetCommands(this.Seq), NetChannel.Unreliable);
			}
			int seq = this.Seq;
			this.Seq = seq + 1;
		}

		
		public void OnStartServer()
		{
			if (this.redisService == null)
			{
				// Log.E("Why redisService is null?");
				Debug.LogError("Why redisService is null?");
				return;
			}
			this.redisService.OnServerReady();
		}

		
		public void OnConnected(int connectionId)
		{
			Log.V("[GameServer.OnConnected] connectionId: {0}", connectionId);
			Log.Gauge(1.0, "Connected");
		}

		
		public void OnDisconnected(int connectionId)
		{
			Log.V("[GameServer.OnDisconnected] connectionId: {0}", connectionId);
			Log.Gauge(1.0, "Disconnected");
			Session session;
			if (!this.onLineSessionMap.TryGetValue((long)connectionId, out session))
			{
				Log.W("Failed to find playerSession: {0}", connectionId);
				return;
			}
			Log.V("[GameServer.OnDisconnected] {0}[{1}] is disconnected.", new object[]
			{
				session.nickname,
				session.userId
			});
			this.GameService.Player.UserDisconnected(session);
			this.onLineSessionMap.Remove((long)connectionId);
			this.UpdateRunningUserCount();
			this.offLineSessionMap.Add(session.userId, session);
			this.EnqueueCommand(new CmdUserDisconnected
			{
				disconnectedObjectId = session.ObjectId
			});
			if (this.onLineSessionMap.Count == 0)
			{
				base.StartCoroutine(this.SendLogAndTerminate());
			}
		}

		
		private IEnumerator SendLogAndTerminate()
		{
			Log.W("Terminate Game Because All Session out.");
			yield return new WaitForSeconds(300f);
			if (this.onLineSessionMap.Count > 0)
			{
				Log.W("Cancel Terminate Game.");
				yield break;
			}
			bool flag = false;
			foreach (PlayerSession playerSession in this.GameService.Player.PlayerSessions)
			{
				if (playerSession.Character != null && playerSession.Character.IsAlive)
				{
					playerSession.Character.Surrender();
					flag = true;
				}
			}
			if (flag)
			{
				Log.W("Wait For Kill Players For 60s");
				yield return new WaitForSeconds(60f);
			}
			yield return base.StartCoroutine(this.TerminateCoroutine("All Session Out"));
		}

		
		private byte[] CreatePacket(IPacket packet)
		{
			byte[] array = new ServerPacketWrapper(this.Seq, Mathf.FloorToInt(this.GameService.TimeSinceGameStart * 1000f), packet).Serialize<ServerPacketWrapper>();
			if (array != null && array.Length > 1000)
			{
				PacketType packetType = packet.GetType().GetCustomAttribute<PacketAttr>().packetType;
				Log.W("<!> Packet length is too long");
				Log.W("Packet: [{0}], Size: {1}", new object[]
				{
					packetType.ToString(),
					array.Length
				});
			}
			return array;
		}

		
		public void Send(Session session, IPacket packet, NetChannel netChannel = NetChannel.ReliableOrdered)
		{
			if (session == null)
			{
				return;
			}
			try
			{
				this.netServer.Send(session.ConnectionId, this.CreatePacket(packet), netChannel);
			}
			catch (Exception ex)
			{
				Log.W(ex.ToString());
			}
		}

		
		private void SendInternal(int connectionId, IPacket packet, NetChannel netChannel = NetChannel.ReliableOrdered)
		{
			try
			{
				this.netServer.Send(connectionId, this.CreatePacket(packet), netChannel);
				Log.Gauge(1.0, "SendInternal");
			}
			catch (Exception)
			{
			}
		}

		
		public void Broadcast(IPacket packet, NetChannel netChannel = NetChannel.ReliableOrdered)
		{
			try
			{
				this.netServer.Broadcast(this.CreatePacket(packet), netChannel);
				Log.Gauge(1.0, "Broadcast");
			}
			catch (Exception)
			{
			}
		}

		
		public void Broadcast(Session excludeSession, IPacket packet, NetChannel netChannel = NetChannel.ReliableOrdered)
		{
			try
			{
				this.netServer.Broadcast((excludeSession != null) ? excludeSession.ConnectionId : -1, this.CreatePacket(packet), netChannel);
				Log.Gauge(1.0, "Broadcast");
			}
			catch (Exception)
			{
			}
		}

		
		public void Broadcast(List<Session> includeSessions, IPacket packet, NetChannel netChannel = NetChannel.ReliableOrdered)
		{
			try
			{
				for (int i = 0; i < includeSessions.Count; i++)
				{
					if (includeSessions[i] != null)
					{
						this.netServer.Send(includeSessions[i].ConnectionId, this.CreatePacket(packet), netChannel);
					}
				}
				Log.Gauge(1.0, "Broadcast");
			}
			catch (Exception)
			{
			}
		}

		
		private IEnumerator Handshake(int connectionId, Handshake handshake, BattleToken battleToken, Action<Session> callback)
		{
			while (!this.initBattleToken)
			{
				yield return new WaitForSeconds(0.1f);
			}
			try
			{
				if (!this.GameService.Player.IsValidUser(handshake.userId))
				{
					throw new GameException(ErrorType.InvalidBattleToken, string.Format("[Handshake] matchingUserTeamNoMap has no userId : {0}", handshake.userId));
				}
				Session obj = this.CreateSession(connectionId, handshake.userId, battleToken);
				callback(obj);
				yield break;
			}
			catch (Exception e)
			{
				this.HandleException(connectionId, e, new uint?(handshake.reqId));
				yield break;
			}
		}

		
		private void HandleHandshake(int connectionId, Handshake handshake, Action<Session> callback)
		{
			Log.V("[Handshake] connectionId: {0}, userId: {1}, battleTokenKey: {2}", new object[]
			{
				connectionId,
				handshake.userId,
				handshake.battleTokenKey
			});
			Log.Gauge(1.0, "Handshake");
			this.battleTokenContext.GetBattleToken(handshake.battleTokenKey, handshake.isReconnect, delegate(bool first, BattleToken battleToken)
			{
				if (first && !this.initBattleToken)
				{
					try
					{
						this.GameService.SetBattleTokenKey(handshake.battleTokenKey);
						this.GameService.SetMatchingMode(battleToken.matchingMode, battleToken.matchingTeamMode);
						this.GameService.AddBattleTeam(battleToken.matchingTeams);
						this.GameService.HideNameSettingDic.SetHideName(battleToken.matchingTeams);
						this.GameService.Player.BuildMatchingUsers(battleToken.matchingTeams);
						this.GameService.Player.BuildMatchingObservers(battleToken.observers);
						this.GameService.StartCondition.SetGameStartCondition(GameStartCondition.FullCapacity, this.GameService.Player.TotalUserCount);
						RestService restService = this.restService;
						if (restService != null)
						{
							restService.InitBattleToken(handshake.battleTokenKey, battleToken);
						}
						this.initBattleToken = true;
						Log.W("[Handshake] battleToken: {0}", JsonConvert.SerializeObject(battleToken));
					}
					catch (Exception e)
					{
						this.HandleException(connectionId, e, new uint?(handshake.reqId));
					}
				}
				this.StartCoroutine(this.Handshake(connectionId, handshake, battleToken, callback));
			});
		}

		
		private Session CreateSession(int connectionId, long userNum, BattleToken battleToken)
		{
			return this.CreateSession(connectionId, null, userNum, battleToken);
		}

		
		private Session CreateSession(int connectionId, int? testTeamNumber, long userNum, BattleToken battleToken)
		{
			if (battleToken == null)
			{
				throw new GameException(ErrorType.InvalidBattleToken, string.Format("[CreateSession] UserId({0}) battleToken({1})", userNum, battleToken));
			}
			if (!this.GameService.IsJoinable())
			{
				if (!this.offLineSessionMap.ContainsKey(userNum))
				{
					throw new GameException(ErrorType.GameStartedAlready);
				}
				if (this.GameService.GameStatus == GameStatus.Finished)
				{
					throw new GameException(ErrorType.UserGameFinished);
				}
			}
			int num = 0;
			int preMade = 0;
			string text = string.Empty;
			int characterCode = 0;
			int weaponCode = 0;
			int skinCode = 0;
			int teamMmr = 0;
			int matchCount = 0;
			int privateMmr = 0;
			List<int> missionList = new List<int>();
			Dictionary<EmotionPlateType, int> emotion = new Dictionary<EmotionPlateType, int>();
			string ip = "";
			string zipCode = "";
			string country = "";
			string countryCode = "";
			string isp = "";
			MatchingObserverToken matchingObserverToken;
			bool flag;
			if (battleToken.IsObserver(userNum, out matchingObserverToken))
			{
				flag = true;
				text = matchingObserverToken.nickname;
			}
			else
			{
				flag = false;
				num = ((testTeamNumber != null) ? testTeamNumber.Value : this.GameService.Player.GetUserTeamNo(userNum));
				MatchingTeamMemberToken matchingTokenUser = battleToken.GetMatchingTokenUser(num, userNum);
				preMade = matchingTokenUser.preMade;
				text = matchingTokenUser.nickname;
				characterCode = matchingTokenUser.characterCode;
				weaponCode = matchingTokenUser.weaponCode;
				skinCode = matchingTokenUser.skinCode;
				teamMmr = matchingTokenUser.mmr;
				matchCount = matchingTokenUser.matchCount;
				privateMmr = matchingTokenUser.privateMmr;
				missionList = matchingTokenUser.missionList;
				emotion = matchingTokenUser.emotion;
				ip = matchingTokenUser.ip;
				zipCode = matchingTokenUser.zipCode;
				country = matchingTokenUser.country;
				countryCode = matchingTokenUser.countryCode;
				isp = matchingTokenUser.isp;
			}
			Log.V("[CreateSession] ConnectionId: {0}, user: {1}, nickname: {2}", new object[]
			{
				connectionId,
				userNum,
				text
			});
			if (this.GameService.GameStatus < GameStatus.FirstJoin)
			{
				this.GameService.SetGameStatus(GameStatus.FirstJoin);
			}
			if (!this.GameService.IsLevelLoaded())
			{
				if (battleToken.matchingMode.IsTutorialMode())
				{
					this.GameService.LoadTutorialLevel(GameDB.level.GetLevelData(battleToken.matchingMode.ToString()), battleToken.botCount, battleToken.botDifficulty, battleToken.matchingMode.ToString());
				}
				else
				{
					this.GameService.LoadLevel(GameDB.level.DefaultLevel, battleToken, "Default");
				}
			}
			RedisService redisService = this.redisService;
			if (redisService != null)
			{
				redisService.ReserveHost();
			}
			Session session = flag ? this.CreateObserverSession(connectionId, userNum, text) : this.CreatePlayerSession(battleToken, connectionId, userNum, text, characterCode, weaponCode, skinCode, num, teamMmr, privateMmr, matchCount, preMade, missionList, emotion, ip, zipCode, country, countryCode, isp);
			PlayerSession playerSession;
			if ((playerSession = (session as PlayerSession)) != null)
			{
				foreach (PlayerSession playerSession2 in this.GameService.Player.PlayerSessions)
				{
					if (playerSession.userId != playerSession2.userId && !playerSession2.IsObserverSession && num == playerSession2.TeamNumber)
					{
						playerSession.AddTeamMember(playerSession2);
						playerSession2.AddTeamMember(playerSession);
					}
				}
			}
			return session;
		}

		
		private Session CreateObserverSession(int connectionId, long userNum, string nickname)
		{
			ObserverSession observerSession = new ObserverSession(connectionId, userNum, nickname);
			if (!this.allSessions.Any((Session x) => x.userId == observerSession.userId))
			{
				this.allSessions.Add(observerSession);
			}
			if (this.onLineSessionMap.Values.Any((Session session) => session.userId == observerSession.userId))
			{
				Log.W("[CreateSession][Observer] Duplicated handshake from {0}", new object[]
				{
					observerSession.userId
				});
				return this.onLineSessionMap.Values.First((Session session) => session.userId == observerSession.userId);
			}
			Session session2;
			if (this.offLineSessionMap.TryGetValue(observerSession.userId, out session2))
			{
				session2.UpdateConnectionId(connectionId);
				this.offLineSessionMap.Remove(observerSession.userId);
				this.onLineSessionMap.Add((long)connectionId, session2);
				this.UpdateRunningUserCount();
				return session2;
			}
			this.onLineSessionMap.Add((long)connectionId, observerSession);
			this.UpdateRunningUserCount();
			this.GameService.Player.AddObserver(observerSession);
			Log.V("[CreateSession][Observer] connectionId: {0}, userId: {1}, nickname: {2}", new object[]
			{
				connectionId,
				userNum,
				nickname
			});
			return observerSession;
		}

		
		private Session CreatePlayerSession(BattleToken battleToken, int connectionId, long userNum, string nickname, int characterCode, int weaponCode, int skinCode, int teamNumber, int teamMmr, int privateMmr, int matchCount, int preMade, List<int> missionList, Dictionary<EmotionPlateType, int> emotion, string ip, string zipCode, string country, string countryCode, string isp)
		{
			PlayerSession playerSession = new PlayerSession(connectionId, userNum, nickname, characterCode, weaponCode, skinCode, teamNumber, teamMmr, privateMmr, matchCount, preMade, missionList, emotion, ip, zipCode, country, countryCode, isp);
			if (!this.allSessions.Any((Session x) => x.userId == playerSession.userId))
			{
				this.allSessions.Add(playerSession);
			}
			if (this.onLineSessionMap.Values.Any((Session session) => session.userId == playerSession.userId))
			{
				Log.W("[CreateSession][Player] Duplicated handshake from {0}", new object[]
				{
					playerSession.userId
				});
				return this.onLineSessionMap.Values.First((Session session) => session.userId == playerSession.userId);
			}
			Session session2;
			if (!this.offLineSessionMap.TryGetValue(playerSession.userId, out session2))
			{
				this.onLineSessionMap.Add((long)connectionId, playerSession);
				this.UpdateRunningUserCount();
				if (battleToken.matchingMode.IsTutorialMode())
				{
					this.GameService.Player.AddTutorialPlayer(battleToken.matchingMode.ConvertToTutorialType(), playerSession);
				}
				else
				{
					this.GameService.Player.AddPlayer(playerSession);
				}
				Log.V("[CreateSession][Player] connectionId: {0}, userId: {1}, nickname: {2}, characterId: {3}", new object[]
				{
					connectionId,
					userNum,
					nickname,
					characterCode
				});
				return playerSession;
			}
			if (this.GameService.Player.IsBattleResultProcessed((session2 as PlayerSession).Character))
			{
				throw new GameException(ErrorType.UserGameFinished);
			}
			session2.UpdateConnectionId(connectionId);
			this.offLineSessionMap.Remove(playerSession.userId);
			this.onLineSessionMap.Add((long)connectionId, session2);
			this.UpdateRunningUserCount();
			return session2;
		}

		
		public void OnRecv(int connectionId, byte[] data)
		{
			ClientPacketWrapper clientPacketWrapper = Serializer.Compression.Deserialize<ClientPacketWrapper>(data);
			PacketType packetType = clientPacketWrapper.packetType;
			try
			{
				if (clientPacketWrapper.packetType == PacketType.Handshake)
				{
					Log.V("[GameServer] Recv Handshake {0}[{1}]", connectionId.ToString(), clientPacketWrapper.userId.ToString());
					Handshake handshake = clientPacketWrapper.GetPacket<Handshake>();
					Log.W("[GameServer] Recv userId({0}) : Handshake({1})", clientPacketWrapper.userId.ToString(), handshake.ToReflectionString());
					this.HandleHandshake(connectionId, handshake, delegate(Session session)
					{
						ResHandshake resHandshake = new ResHandshake();
						resHandshake.reqId = handshake.reqId;
						resHandshake.hostname = this.HostName;
						this.Send(session, resHandshake, NetChannel.ReliableOrdered);
					});
					return;
				}
				if (clientPacketWrapper.packetType == PacketType.TesterHandshake)
				{
					Log.V("[GameServer] Recv TesterHandshake {0}[{1}]", connectionId.ToString(), clientPacketWrapper.userId.ToString());
					TesterHandshake packet = clientPacketWrapper.GetPacket<TesterHandshake>();
					int value = 0;
					if (0 < packet.battleToken.matchingTeams.Count)
					{
						value = packet.battleToken.matchingTeams.First<KeyValuePair<int, MatchingTeamToken>>().Key;
					}
					else
					{
						value = 100;
					}
					bool flag = false;
					foreach (Session session in allSessions)
					{
						if (session.userId == packet.userId)
						{
							flag = true;
							break;
						}
						
					}
					
					// using (List<Session>.Enumerator enumerator = this.allSessions.GetEnumerator())
					// {
					// 	while (enumerator.MoveNext())
					// 	{
					// 		if (enumerator.Current.userId == packet.userId)
					// 		{
					// 			flag = true;
					// 			break;
					// 		}
					// 	}
					// }
					
					if (!flag)
					{
						this.GameService.SetMatchingMode(packet.battleToken.matchingMode, packet.battleToken.matchingTeamMode);
						this.GameService.AddBattleTeam(packet.battleToken.matchingTeams);
						this.GameService.HideNameSettingDic.SetHideName(packet.battleToken.matchingTeams);
						this.GameService.Player.BuildMatchingUsers(packet.battleToken.matchingTeams);
						if (packet.battleToken.matchingMode.IsTutorialMode())
						{
							this.GameService.Tutorial.Init(packet.battleToken.matchingMode.ConvertToTutorialType());
						}
						this.GameService.Player.BuildMatchingObservers(packet.battleToken.observers);
						this.GameService.StartCondition.SetGameStartCondition(GameStartCondition.ManualReady, 0);
					}
					
					Log.W("[GameServer] Recv userId({0}) : TesterHandshake({1})", clientPacketWrapper.userId.ToString(), packet.ToReflectionString());
					Session session3 = this.CreateSession(connectionId, new int?(value), clientPacketWrapper.userId, packet.battleToken);
					this.Send(session3, new ResHandshake
					{
						reqId = packet.reqId,
						hostname = this.HostName
					}, NetChannel.ReliableOrdered);
					return;
				}
			}
			catch (Exception e)
			{
				this.HandleException(connectionId, e, null);
				return;
			}
			Session session2;
			if (this.onLineSessionMap.TryGetValue((long)connectionId, out session2))
			{
				if (clientPacketWrapper.userId != session2.userId)
				{
					Log.W("Suspicious action. packet user: {0}, session user: {1}", clientPacketWrapper.userId.ToString(), session2.userId.ToString());
				}
				if (clientPacketWrapper.GetPacket<IPacket>().GetType().GetCustomAttribute<PacketAttr>().immediateResponse)
				{
					this.RequestHandle(session2, clientPacketWrapper);
				}
				else
				{
					this.requestQueue.Enqueue(new ClientRequest(session2, clientPacketWrapper));
				}
				PlayerSession playerSession;
				if ((playerSession = (session2 as PlayerSession)) != null)
				{
					playerSession.UpdateLatency(this.netServer.GetRTT(connectionId));
				}
			}
			else
			{
				Log.W("Request from no exist userId: {0}", new object[]
				{
					clientPacketWrapper.userId
				});
			}
		}

		
		private void RequestHandle(Session session, ClientPacketWrapper clientPacketWrapper)
		{
			if (clientPacketWrapper.IsType(typeof(ReqPacket)))
			{
				this.RequestHandle(session, clientPacketWrapper.GetPacket<ReqPacket>());
				return;
			}
			if (clientPacketWrapper.IsType(typeof(ReqPacketForResponse)))
			{
				this.RequestHandle(session, clientPacketWrapper.GetPacket<ReqPacketForResponse>());
				return;
			}
			Log.W("Requested packet is not valid: {0}", new object[]
			{
				clientPacketWrapper.packetType
			});
		}

		
		private void RequestHandle(Session session, ReqPacket reqPacket)
		{
			try
			{
				PlayerSession playerSession;
				ObserverSession observerSession;
				if ((playerSession = (session as PlayerSession)) != null)
				{
					reqPacket.Action(this, this.GameService, playerSession);
				}
				else if ((observerSession = (session as ObserverSession)) != null)
				{
					reqPacket.Action(this, this.GameService, observerSession);
				}
			}
			catch (Exception e)
			{
				this.HandleException(session, e, null);
			}
		}

		
		private void RequestHandle(Session session, ReqPacketForResponse reqPacket)
		{
			try
			{
				ResPacket resPacket = null;
				PlayerSession playerSession;
				ObserverSession observerSession;
				if ((playerSession = (session as PlayerSession)) != null)
				{
					resPacket = (reqPacket.Action(this, this.GameService, playerSession) ?? new ResSuccess());
				}
				else if ((observerSession = (session as ObserverSession)) != null)
				{
					resPacket = reqPacket.Action(this, this.GameService, observerSession);
					if (resPacket == null)
					{
						Log.V("[GameServer][Observer] Recv userId({0}) : ReqPacketForResponse({1})", new object[]
						{
							session.userId,
							reqPacket.ToReflectionString()
						});
					}
				}
				if (resPacket != null)
				{
					resPacket.reqId = reqPacket.reqId;
					this.Send(session, resPacket, resPacket.ResponseChannel());
				}
			}
			catch (Exception e)
			{
				this.HandleException(session, e, new uint?(reqPacket.reqId));
			}
		}

		
		public void HandleException(Session session, Exception e, uint? reqId = null)
		{
			if (session == null)
			{
				return;
			}
			this.HandleException(session.ConnectionId, e, reqId);
		}

		
		public void HandleException(int connectionId, Exception e, uint? reqId = null)
		{
			ErrorType errorType;
			string msg;
			if (e is GameException)
			{
				GameException ex = e as GameException;
				errorType = ex.errorType;
				msg = ex.msg;
			}
			else
			{
				errorType = ErrorType.Internal;
				msg = e.ToString();
			}
			if (!errorType.IsGameError())
			{
				// Log.Exception(e);
				Debug.LogException(e);
			}
			if (reqId != null)
			{
				this.SendInternal(connectionId, new ResError
				{
					reqId = reqId.Value,
					errorType = errorType,
					msg = msg
				}, NetChannel.ReliableOrdered);
			}
			else
			{
				this.SendInternal(connectionId, new RpcError
				{
					errorType = errorType,
					msg = msg
				}, NetChannel.ReliableOrdered);
			}
			Log.Gauge(1.0, "Exception");
		}

		
		public bool IsServerReserved()
		{
			return !(this.redisService == null) && (this.redisService.IsReserved() || this.GameService.HasSomeoneJoined());
		}

		
		protected override void _OnDestroy()
		{
			TftpServer tftpServer = this._tftpServer;
			if (tftpServer != null)
			{
				tftpServer.Dispose();
			}
			INetServer netServer = this.netServer;
			if (netServer == null)
			{
				return;
			}
			netServer.Close();
		}

		
		private IEnumerator TerminateCoroutine(string reason)
		{
			Log.W("Invoked: reason={0}", reason);
			if (this.GameService)
			{
				yield return base.StartCoroutine(this.GameService.StoreChatLogsThroughRestService(reason));
			}
			this.TerminateServer(reason);
		}

		
		private void TerminateServer(string reason)
		{
			Log.W("Invoked: Reason=" + reason);
			INetServer netServer = this.netServer;
			if (netServer != null)
			{
				netServer.Close();
			}
			GameUtil.ExitGame();
		}

		
		public void OnFinishGame()
		{
			base.StartCoroutine(this.DelayedTerminate());
		}

		
		private IEnumerator DelayedTerminate()
		{
			if (this.restService)
			{
				yield return base.StartCoroutine(this.restService.LogBattleGameFinish());
			}
			yield return new WaitForSeconds(10f);
			if (this.restService)
			{
				yield return this.restService.WaitForComplete();
			}
			foreach (Session session in this.onLineSessionMap.Values)
			{
				this.Send(session, new RpcFinishGame
				{
					nicknamePairDic = this.GameService.TempNicknameDic
				}, NetChannel.ReliableOrdered);
			}
			yield return new WaitForSeconds(1f);
			foreach (Session session2 in this.onLineSessionMap.Values)
			{
				this.Send(session2, new RpcExitGame(), NetChannel.ReliableOrdered);
			}
			yield return new WaitForSeconds(30f);
			yield return base.StartCoroutine(this.TerminateCoroutine("Game has finished"));
		}

		
		private void UpdateRunningUserCount()
		{
			if (this.redisService)
			{
				this.redisService.SetRunningUserCount(this.onLineSessionMap.Count);
			}
		}

		
		public void LogConnectionMap()
		{
			this.netServer.LogConnectionMap();
		}

		
		public void LogSessionMap()
		{
			foreach (KeyValuePair<long, Session> keyValuePair in this.onLineSessionMap)
			{
				Log.V(string.Format("[GameServer] Session : {0}, {1}", keyValuePair.Key, keyValuePair.Value.nickname));
			}
		}

		
		public byte[] LoadWorldSnaptshot(int userId)
		{
			byte[] result;
			if (!this.worldSnapshotMap.TryGetValue(userId, out result))
			{
				Log.V(string.Format("Missing WorldSnapshot : userId({0})", userId));
			}
			return result;
		}

		
		public void SaveInitialWorldSnapshot(List<SnapshotWrapper> worldSnapshot)
		{
			this.SaveWorldSnapshot(0, 0, worldSnapshot);
		}

		
		private void SaveWorldSnapshot(int userId, int seq, List<SnapshotWrapper> worldSnapshot)
		{
			this.worldSnapshotMap.Remove(userId);
			ResWorldSnapshot obj = new ResWorldSnapshot
			{
				currentSeq = seq,
				snapshot = worldSnapshot
			};
			byte[] array = Serializer.Compression.Serialize<ResWorldSnapshot>(obj);
			this.worldSnapshotMap.Add(userId, array);
			Log.V(string.Format("Game Serialized : userId({0}), seq({1}), worldSnapshot.Count({2}), raw.Length({3})", new object[]
			{
				userId,
				seq,
				worldSnapshot.Count,
				array.Length
			}));
		}

		
		private void OnTftServerReadRequest(ITftpTransfer transfer, EndPoint client)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(transfer.Filename);
			string extension = Path.GetExtension(transfer.Filename);
			int num = 0;
			byte[] buffer;
			if (".world".Equals(extension, StringComparison.InvariantCultureIgnoreCase) && int.TryParse(fileNameWithoutExtension, out num) && (num == 0 || this.onLineSessionMap.ContainsKey((long)num)) && this.worldSnapshotMap.TryGetValue(num, out buffer))
			{
				this.OutputTransferStatus(transfer, "Accepting request from " + client);
				this.StartTransfer(transfer, new MemoryStream(buffer));
				return;
			}
			this.CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
		}

		
		private void OnTftServerError(TftpTransferError error)
		{
			Log.V("Error: {}", error.ToString());
		}

		
		private void StartTransfer(ITftpTransfer transfer, Stream stream)
		{
			transfer.OnError += this.OnTransferError;
			transfer.OnFinished += this.OnTransferFinished;
			transfer.Start(stream);
		}

		
		private void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
		{
			this.OutputTransferStatus(transfer, "Cancelling transfer: " + reason.ErrorMessage);
			transfer.Cancel(reason);
		}

		
		private void OnTransferError(ITftpTransfer transfer, TftpTransferError error)
		{
			this.OutputTransferStatus(transfer, "Error: " + error);
		}

		
		private void OnTransferFinished(ITftpTransfer transfer)
		{
			this.OutputTransferStatus(transfer, "Finished");
		}

		
		private void OutputTransferStatus(ITftpTransfer transfer, string message)
		{
			Log.V("File Transfer (" + transfer.Filename + ") : " + message);
		}

		
		public void BuildTeam()
		{
			Dictionary<int, List<WorldPlayerCharacter>> dictionary = new Dictionary<int, List<WorldPlayerCharacter>>(this.GameService.Player.TotalCharacterCount / 2);
			foreach (PlayerSession playerSession in this.GameService.Player.PlayerSessions)
			{
				if (dictionary.ContainsKey(playerSession.TeamNumber))
				{
					dictionary[playerSession.TeamNumber].Add(playerSession.Character);
				}
				else
				{
					dictionary.Add(playerSession.TeamNumber, new List<WorldPlayerCharacter>
					{
						playerSession.Character
					});
				}
			}
			this.GameService.SettingTeam(dictionary);
			foreach (PlayerSession playerSession2 in this.GameService.Player.PlayerSessions)
			{
				WorldPlayerCharacter character = playerSession2.Character;
				foreach (WorldPlayerCharacter worldPlayerCharacter in playerSession2.GetTeamCharacters())
				{
					character.SightAgent.AddAllySight(worldPlayerCharacter.SightAgent);
				}
			}
			foreach (ObserverSession observerSession in this.GameService.Player.ObserverSessions)
			{
				WorldObserver observer = observerSession.Observer;
				foreach (PlayerSession playerSession3 in this.GameService.Player.PlayerSessions)
				{
					observer.SightAgent.AddAllySight(playerSession3.Character.SightAgent);
				}
				foreach (WorldPlayerCharacter worldPlayerCharacter2 in this.GameService.Bot.Characters)
				{
					observer.SightAgent.AddAllySight(worldPlayerCharacter2.SightAgent);
				}
			}
		}

		
		public void BroadcastJoinUser(PlayerSession joinUser, SnapshotWrapper joinUserSnapshot)
		{
			byte[] array = joinUser.Character.CreatePlayerSnapshot();
			byte[] array2 = joinUser.Character.CreateAllyPlayerSnapshot();
			foreach (Session session in this.allSessions)
			{
				if (session.userId != joinUser.userId)
				{
					bool flag = false;
					string nickname = "";
					if (!session.IsObserverSession && !joinUser.IsObserverSession)
					{
						PlayerSession playerSession;
						if ((playerSession = (session as PlayerSession)) != null)
						{
							flag = (playerSession.Character.GetHostileType(joinUser.Character) == HostileType.Ally);
							nickname = this.GameService.CheckAndChangeNickname(playerSession, joinUser);
						}
					}
					else
					{
						nickname = joinUser.nickname;
						flag = true;
					}
					this.Send(session, new RpcJoinUser
					{
						userId = joinUser.userId,
						nickname = nickname,
						isObserver = false,
						startingWeaponCode = joinUser.startingWeaponCode,
						characterSnapshot = joinUserSnapshot,
						playerSnapshot = (flag ? array2 : array)
					}, NetChannel.ReliableOrdered);
				}
			}
		}

		
		public void BroadcastJoinUser(ObserverSession joinUser, SnapshotWrapper joinUserSnapshot)
		{
			foreach (Session session in this.allSessions)
			{
				if (session.userId != joinUser.userId)
				{
					this.Send(session, new RpcJoinUser
					{
						userId = joinUser.userId,
						nickname = joinUser.nickname,
						isObserver = true,
						characterSnapshot = joinUserSnapshot
					}, NetChannel.ReliableOrdered);
				}
			}
		}

		
		public bool IsDisconnected(Session session)
		{
			return this.offLineSessionMap.ContainsKey(session.userId);
		}

		
		public bool IsOnline(Session session)
		{
			return this.onLineSessionMap.ContainsKey((long)session.ConnectionId);
		}

		
		public void ChangeToObserver(PlayerSession playerSession)
		{
			this.allSessions.RemoveAll((Session x) => x.ConnectionId == playerSession.ConnectionId);
			if (this.onLineSessionMap.ContainsKey((long)playerSession.ConnectionId))
			{
				this.onLineSessionMap.Remove((long)playerSession.ConnectionId);
			}
			if (this.offLineSessionMap.ContainsKey((long)playerSession.ConnectionId))
			{
				this.offLineSessionMap.Remove((long)playerSession.ConnectionId);
			}
			ObserverSession observerSession = this.CreateObserverSession(playerSession.ConnectionId, playerSession.userId, playerSession.nickname) as ObserverSession;
			if (observerSession != null)
			{
				observerSession.EnqueueCommandPacket(new CmdChangeToObserver
				{
					userId = observerSession.userId,
					nickname = observerSession.nickname,
					userList = this.GameService.GetPlayerSimpleInfosForObserver(),
					summonList = this.GameService.GetSummonSimpleInfosForObserver(),
					monsterList = this.GameService.GetMonsterSimpleInfosForObserver(),
					characterSnapshot = this.GameService.Player.GetSnapshot(observerSession.ObjectId),
					playerSnapshot = observerSession.Observer.CreateMyPlayerSnapshot()
				});
			}
		}

		
		private static int ParsePort(string argHost)
		{
			string[] array = argHost.Split(new char[]
			{
				':'
			});
			if (array.Length < 2)
			{
				Log.W("parameter [host] is required.");
				throw new ArgumentException("parameter [host] is required.");
			}
			string param = array[0];
			string text = array[1];
			Log.V("[ParsePort] Address: {0} | Port: {1}", param, text);
			return int.Parse(text);
		}

		
		private double serverStartedTime;

		
		private readonly List<Session> allSessions = new List<Session>();

		
		private readonly Dictionary<long, Session> offLineSessionMap = new Dictionary<long, Session>();

		
		private readonly Dictionary<long, Session> onLineSessionMap = new Dictionary<long, Session>();

		
		private readonly Queue<ClientRequest> requestQueue = new Queue<ClientRequest>();

		
		private INetServer netServer;

		
		private BattleTokenContext battleTokenContext = default;

		
		private RedisService redisService { get; set; }

		
		private RestService restService = default;

		
		private readonly Dictionary<int, byte[]> worldSnapshotMap = new Dictionary<int, byte[]>();

		
		public const int TftpPortPadding = 1000;

		
		private static string ServerDirectory = Environment.CurrentDirectory;

		
		private TftpServer _tftpServer = default;

		
		private float tick = default;

		
		private int frameUpdateExcuteCount = default;

		
		private readonly FrameCount frameCount = new FrameCount();

		
		private bool initBattleToken;
	}
}
