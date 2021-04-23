using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Blis.Common;
using Blis.Server;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blis.Client
{
	public class DummyClient : MonoBehaviour, INetClientHandler
	{
		public int targetCharacter = -1;
		private readonly List<RequestHolder> reqQueue = new List<RequestHolder>();
		private int defaultPort = GameConstants.DefaultPort;
		private string host = "localhost";
		private uint idx;
		private MatchingToken matchingToken;
		private int myCharacterId;
		private INetClient netClient;
		private ISerializer serializer = Serializer.Default;

		private void Update()
		{
			netClient.Update();
		}

		private void OnApplicationQuit()
		{
			netClient.Close();
		}

		public void OnConnected()
		{
			Dictionary<long, MatchingTeamMemberToken> teamMembers = new Dictionary<long, MatchingTeamMemberToken>
			{
				{
					matchingToken.userNum,
					new MatchingTeamMemberToken
					{
						userNum = matchingToken.userNum,
						nickname = matchingToken.nickname,
						mmr = matchingToken.mmr,
						characterCode = matchingToken.characterCode,
						skinCode = matchingToken.skinCode
					}
				}
			};
			
			BattleToken battleToken = new BattleToken
			{
				matchingMode = matchingToken.matchingMode,
				matchingTeamMode = matchingToken.matchingTeamMode,
				botDifficulty = BotDifficulty.NONE,
				matchingTeams = new Dictionary<int, MatchingTeamToken>
				{
					{
						0,
						new MatchingTeamToken
						{
							teamNo = 0,
							teamMMR = 0,
							teamMembers = teamMembers
						}
					}
				}
			};
			
			Send<TesterHandshake>(new TesterHandshake
			{
				userId = matchingToken.userNum,
				battleToken = battleToken
			});
			
			Request<ReqJoin, ResJoin>(new ReqJoin(), delegate(ResJoin res)
			{
				foreach (UserInfo userInfo in res.userList)
				{
					myCharacterId = res.character.objectId;
					if (userInfo.characterSnapshot.objectId != res.character.objectId)
					{
						targetCharacter = userInfo.characterSnapshot.objectId;
						break;
					}
				}

				Send<Ready>(new Ready());
			});
		}


		public void OnDisconnected() { }


		public void OnError(int errorCode) { }


		public void OnRecv(byte[] data)
		{
			ServerPacketWrapper serverPacketWrapper = Serializer.Compression.Deserialize<ServerPacketWrapper>(data);
			if (serverPacketWrapper.IsType(typeof(ResPacket)))
			{
				HandleResponse(serverPacketWrapper);
				return;
			}

			if (serverPacketWrapper.IsType(typeof(RpcPacket)))
			{
				if (serverPacketWrapper.packetType == PacketType.RpcSetupGame)
				{
					SetupDummy();
					return;
				}

				if (serverPacketWrapper.packetType == PacketType.RpcStartGame)
				{
					this.StartThrowingCoroutine(StartDummyAction(), delegate { });
				}
			}
		}

		private static string RandomString(int length)
		{
			return new string((from s in Enumerable.Repeat<string>("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
				select s[Random.Range(0, s.Length)]).ToArray<char>());
		}


		private byte[] CreatePacket(IPacket packet)
		{
			if (BSERVersion.isDebugBuild)
			{
				PacketType packetType = packet.GetType().GetCustomAttribute<PacketAttr>().packetType;
			}

			return new ClientPacketWrapper(matchingToken.userNum, packet).Serialize<ClientPacketWrapper>();
		}


		private void Send<T>(T packet) where T : IPacket
		{
			netClient.Send(CreatePacket(packet), NetChannel.ReliableOrdered);
		}

		public void Request(ReqPacket packet)
		{
			Send<ReqPacket>(packet);
		}


		public void Request<Tpacket, Tcallback>(Tpacket packet, Action<Tcallback> callback)
			where Tpacket : ReqPacketForResponse where Tcallback : ResPacket
		{
			uint num = idx;
			idx = num + 1U;
			uint reqId = num;
			packet.reqId = reqId;
			netClient.Send(CreatePacket(packet), NetChannel.ReliableOrdered);
			reqQueue.Add(new RequestHolder(packet,
				delegate(ServerPacketWrapper res) { callback(res.GetPacket<Tcallback>()); }));
		}


		private void SetupDummy()
		{
			Request(new ReqUpdateStrategySheet
			{
				areaCode = 1,
				updateUI = true
			});
		}


		private IEnumerator StartDummyAction()
		{
			for (;;)
			{
				Send<ReqMoveTo>(new ReqMoveTo
				{
					destination = new Vector3(Random.Range(100, 300), 0f, Random.Range(100, 300)),
					findAttackTarget = false
				});
				yield return new WaitForSeconds(0.033333335f);
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
		}


		public void Connect(string host, int port, int targetCharacter)
		{
			Log.H("[ADD DUMMY] host: {0}, targetCharacter: {1}", host, targetCharacter);
			
			this.host = host;
			this.targetCharacter = targetCharacter;
			netClient = NetFactory.CreateClient();
			int num = Random.Range(0, int.MaxValue);
			netClient.Init(this, num);
			
			matchingToken = new MatchingToken(MatchingMode.Test, MatchingTeamMode.Solo, MatchingRegion.None,
				num.ToString(), num, 0, RandomString(8), GameConstants.STANDALONE_DEFAULT_CHARACTER, 0,
				GameConstants.STANDALONE_DEFAULT_WEAPON, new List<int>(), 1, false,
				new Dictionary<EmotionPlateType, int>());
			netClient.Open(host, port);
		}
	}
}