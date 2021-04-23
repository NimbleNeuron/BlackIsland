using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Blis.Client;
using Blis.Common.Utils;
using ICSharpCode.SharpZipLib.GZip;
using Neptune.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Blis.Common
{
	[Obsolete]
	public class MatchingSocketOld : MonoBehaviourInstance<MatchingSocketOld>
	{
		public enum MatchingSocketState
		{
			Unconnected,

			Connecting,

			Connected,

			Closing
		}


		private const float PING_INTERVAL = 5f;


		private static long lastRequestId;


		private string battleSeatKey;


		private Action<MatchingResult> completeMatching;


		private readonly float connectionTimeOut = 5f;


		private Action ExitCallback;


		private Action kickedOutCustomGameRoom;


		private string matchingHost;


		private MatchingStatus matchingStatus;


		private Action<MatchingChatMessage> onChatMessage;


		private Action onClose;


		private Action<string> onError;


		private List<BlisWebSocketRequest> requests = new List<BlisWebSocketRequest>();


		private Coroutine serverIndicatorTimer;


		private Action<MatchingResult> startCustomGame;


		private MatchingSocketState state;


		private Action<CustomGameRoom> updateCustomGameRoom;


		private WebSocket ws;


		private void OnApplicationQuit()
		{
			Close();
		}

		public void OnRequestTimeout()
		{
			if (matchingStatus == MatchingStatus.Matched)
			{
				return;
			}

			Close();
			OnError(Ln.Get("매칭서버 타임아웃"));
		}


		public void RegisterErrorCallback(Action<string> onError)
		{
			this.onError = onError;
		}


		public void RegisterCloseCallback(Action close)
		{
			onClose = close;
		}


		public void OnError(string msg)
		{
			if (matchingStatus == MatchingStatus.Matched)
			{
				return;
			}

			if (onError != null)
			{
				onError(msg);
			}
		}


		private void PopupServerErrorWithCommunity(string msg)
		{
			Popup inst = MonoBehaviourInstance<Popup>.inst;
			string msg2 = Ln.Get(msg);
			Popup.Button[] array = new Popup.Button[2];
			array[0] = new Popup.Button
			{
				type = Popup.ButtonType.Confirm,
				text = Ln.Get("확인")
			};
			int num = 1;
			Popup.Button button = new Popup.Button();
			button.type = Popup.ButtonType.Cancel;
			button.text = Ln.Get("기본 커뮤니티");
			button.callback = delegate { Application.OpenURL(Ln.Get("기본 커뮤니티 공지 링크")); };
			array[num] = button;
			inst.Message(msg2, array);
		}


		private IEnumerator Ping()
		{
			if (state != MatchingSocketState.Connected)
			{
				yield break;
			}

			if (requests.Count > 0 && requests[0].Elasped.TotalMilliseconds > 5000.0)
			{
				OnRequestTimeout();
				yield break;
			}

			Send<object>(Req("Ping", Array.Empty<object>()), delegate { });
			yield return new WaitForSeconds(5f);
			StartCoroutine(Ping());
		}


		public void Init(string matchingHost, Action<MatchingResult> completeMatching)
		{
			this.matchingHost = matchingHost;
			this.completeMatching = completeMatching;
			onClose = null;
		}


		public void InitCustomMatching(string matchingHost, Action<CustomGameRoom> updateCustomGameRoom,
			Action<MatchingResult> startCustomGame, Action<MatchingChatMessage> onChatMessage,
			Action kickedOutCustomGameRoom)
		{
			this.matchingHost = matchingHost;
			this.updateCustomGameRoom = updateCustomGameRoom;
			this.onChatMessage = onChatMessage;
			this.startCustomGame = startCustomGame;
			this.kickedOutCustomGameRoom = kickedOutCustomGameRoom;
		}


		public void CreateCustomGameRoom(string tokenKey, long userNum, Action<CustomGameRoom> callback)
		{
			Log.V("[MatchingSocket] Connect to {0}", matchingHost);
			Connect(matchingHost, () => Send<Dictionary<string, object>>(
				Req(nameof(CreateCustomGameRoom), (object) "customGameTokenKey", (object) tokenKey,
					(object) nameof(userNum), (object) userNum), (request, result) =>
				{
					if (result == null)
					{
						HandleError(request, nameof(CreateCustomGameRoom));
					}
					else
					{
						matchingStatus = MatchingStatus.InCustomRoom;
						CustomGameRoom customGameRoom = ((JToken) result["roomInfo"]).ToObject<CustomGameRoom>();
						Action<CustomGameRoom> action = callback;
						if (action == null)
						{
							return;
						}

						action(customGameRoom);
					}
				}), () => MonoBehaviourInstance<LobbyUI>.inst?.UIEvent.OnLobbyStateUpdate(LobbyState.Ready));

			// co: dotPeek
			// Log.V("[MatchingSocket] Connect to {0}", this.matchingHost);
			// MatchingSocketOld.Response<Dictionary<string, object>> <>9__2;
			// this.Connect(this.matchingHost, delegate
			// {
			// 	MatchingSocketOld <>4__this = this;
			// 	WebSocketRequest websocketRequest = this.Req("CreateCustomGameRoom", new object[]
			// 	{
			// 		"customGameTokenKey",
			// 		tokenKey,
			// 		"userNum",
			// 		userNum
			// 	});
			// 	MatchingSocketOld.Response<Dictionary<string, object>> response;
			// 	if ((response = <>9__2) == null)
			// 	{
			// 		response = (<>9__2 = delegate(BlisWebSocketRequest request, Dictionary<string, object> result)
			// 		{
			// 			if (result == null)
			// 			{
			// 				this.HandleError(request, "CreateCustomGameRoom");
			// 				return;
			// 			}
			// 			this.matchingStatus = MatchingStatus.InCustomRoom;
			// 			CustomGameRoom obj = ((JObject)result["roomInfo"]).ToObject<CustomGameRoom>();
			// 			Action<CustomGameRoom> callback2 = callback;
			// 			if (callback2 == null)
			// 			{
			// 				return;
			// 			}
			// 			callback2(obj);
			// 		});
			// 	}
			// 	<>4__this.Send<Dictionary<string, object>>(websocketRequest, response);
			// }, delegate
			// {
			// 	LobbyUI inst = MonoBehaviourInstance<LobbyUI>.inst;
			// 	if (inst == null)
			// 	{
			// 		return;
			// 	}
			// 	inst.UIEvent.OnLobbyStateUpdate(LobbyState.Ready);
			// });
		}


		public void EnterCustomGameRoom(string tokenKey, string customRoomKey, long userNum,
			Action<CustomGameRoom> callback)
		{
			Log.V("[MatchingSocket] Connect to {0}", matchingHost);
			Connect(matchingHost, () => Send<Dictionary<string, object>>(
				Req(nameof(EnterCustomGameRoom), (object) nameof(tokenKey), (object) tokenKey, (object) "roomKey",
					(object) customRoomKey, (object) nameof(userNum), (object) userNum), (request, result) =>
				{
					if (result == null)
					{
						HandleError(request, nameof(EnterCustomGameRoom));
					}
					else
					{
						matchingStatus = MatchingStatus.InCustomRoom;
						CustomGameRoom customGameRoom = ((JToken) result["roomInfo"]).ToObject<CustomGameRoom>();
						Action<CustomGameRoom> action = callback;
						if (action == null)
						{
							return;
						}

						action(customGameRoom);
					}
				}), () => MonoBehaviourInstance<LobbyUI>.inst?.UIEvent.OnLobbyStateUpdate(LobbyState.Ready));

			// co: dotPeek
			// Log.V("[MatchingSocket] Connect to {0}", this.matchingHost);
			// MatchingSocketOld.Response<Dictionary<string, object>> <>9__2;
			// this.Connect(this.matchingHost, delegate
			// {
			// 	MatchingSocketOld <>4__this = this;
			// 	WebSocketRequest websocketRequest = this.Req("EnterCustomGameRoom", new object[]
			// 	{
			// 		"tokenKey",
			// 		tokenKey,
			// 		"roomKey",
			// 		customRoomKey,
			// 		"userNum",
			// 		userNum
			// 	});
			// 	MatchingSocketOld.Response<Dictionary<string, object>> response;
			// 	if ((response = <>9__2) == null)
			// 	{
			// 		response = (<>9__2 = delegate(BlisWebSocketRequest request, Dictionary<string, object> result)
			// 		{
			// 			if (result == null)
			// 			{
			// 				this.HandleError(request, "EnterCustomGameRoom");
			// 				return;
			// 			}
			// 			this.matchingStatus = MatchingStatus.InCustomRoom;
			// 			CustomGameRoom obj = ((JObject)result["roomInfo"]).ToObject<CustomGameRoom>();
			// 			Action<CustomGameRoom> callback2 = callback;
			// 			if (callback2 == null)
			// 			{
			// 				return;
			// 			}
			// 			callback2(obj);
			// 		});
			// 	}
			// 	<>4__this.Send<Dictionary<string, object>>(websocketRequest, response);
			// }, delegate
			// {
			// 	LobbyUI inst = MonoBehaviourInstance<LobbyUI>.inst;
			// 	if (inst == null)
			// 	{
			// 		return;
			// 	}
			// 	inst.UIEvent.OnLobbyStateUpdate(LobbyState.Ready);
			// });
		}


		public void LeaveCustomGameRoom(Action callback)
		{
			if (callback == null)
			{
				throw new NullReferenceException();
			}

			if (matchingStatus != MatchingStatus.InCustomRoom)
			{
				Log.W("[MatchingSocket.LeaveCustomMatchRoom] MatchingStatus is 'InCustomRoom'");
				return;
			}

			matchingStatus = MatchingStatus.WaitingExit;
			Send<object>(Req("LeaveCustomGameRoom", Array.Empty<object>()),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "LeaveCustomGameRoom");
						return;
					}

					StartCoroutine(DelayExit(callback));
				});
		}


		public void KickUserCustomGameRoom(long kickUserNum)
		{
			if (matchingStatus != MatchingStatus.InCustomRoom)
			{
				Log.W("[MatchingSocket.KickUserCustomGameRoom] MatchingStatus is 'InCustomRoom'");
				return;
			}

			Send<object>(Req("KickCustomGameRoom", "kickUserNum", kickUserNum),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "KickCustomGameRoom");
					}
				});
		}


		public void AddBotCustomGameRoom(int slotNum, int characterCode, string name)
		{
			if (matchingStatus != MatchingStatus.InCustomRoom)
			{
				Log.W("[MatchingSocket.AddBotCustomGameRoom] MatchingStatus is 'InCustomRoom'");
				return;
			}

			Send<object>(Req("AddBotCustomGameRoom", "slotNum", slotNum, "characterCode", characterCode, "name", name),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "AddBotCustomGameRoom");
					}
				});
		}


		public void RemoveBotCustomGameRoom(int slotNum)
		{
			if (matchingStatus != MatchingStatus.InCustomRoom)
			{
				Log.W("[MatchingSocket.RemoveBotCustomGameRoom] MatchingStatus is 'InCustomRoom'");
				return;
			}

			Send<object>(Req("RemoveBotCustomGameRoom", "slotNum", slotNum),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "RemoveBotCustomGameRoom");
					}
				});
		}


		public void UpdateAcceleration(bool isOn)
		{
			if (matchingStatus != MatchingStatus.InCustomRoom)
			{
				Log.W("[MatchingSocket.UpdateAcceleration] MatchingStatus is 'InCustomRoom'");
				return;
			}

			Send<object>(Req("UpdateAccelerationCustomGameRoom", "isOn", isOn),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "UpdateAccelerationCustomGameRoom");
					}
				});
		}


		public void MoveSlot(int to)
		{
			if (matchingStatus != MatchingStatus.InCustomRoom)
			{
				Log.W("[MatchingSocket.MoveSlot] MatchingStatus is 'InCustomRoom'");
				return;
			}

			Send<object>(Req("MoveSlotCustomGameRoom", "targetSlot", to),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "MoveSlotCustomGameRoom");
					}
				});
		}


		public void SendChatMessage(string message)
		{
			if (matchingStatus != MatchingStatus.InCustomRoom)
			{
				Log.W("[MatchingSocket.SendChatMessage] MatchingStatus is 'InCustomRoom'");
				return;
			}

			Send<object>(Req("CustomGameChatMessage", "message", message),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "CustomGameChatMessage");
					}
				});
		}


		public void StartCustomGame()
		{
			if (matchingStatus != MatchingStatus.InCustomRoom)
			{
				Log.W("[MatchingSocket.StartCustomGame] MatchingStatus is 'InCustomRoom'");
				return;
			}

			Send<object>(Req("StartCustomGame", Array.Empty<object>()),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "StartCustomGame");
					}
				});
		}


		public void ServiceCheck(Action callback)
		{
			Connect(matchingHost, () => Send<object>(Req(nameof(ServiceCheck), Array.Empty<object>()),
				(request, result) =>
				{
					if (result == null)
					{
						HandleError(request, nameof(ServiceCheck));
					}
					else
					{
						callback();
					}
				}));
		}

		// co: dotPeek
		// public void ServiceCheck(Action callback)
		// {
		// 	MatchingSocketOld.Response<object> <>9__1;
		// 	this.Connect(this.matchingHost, delegate
		// 	{
		// 		MatchingSocketOld <>4__this = this;
		// 		WebSocketRequest websocketRequest = this.Req("ServiceCheck", Array.Empty<object>());
		// 		MatchingSocketOld.Response<object> response;
		// 		if ((response = <>9__1) == null)
		// 		{
		// 			response = (<>9__1 = delegate(BlisWebSocketRequest request, object result)
		// 			{
		// 				if (result == null)
		// 				{
		// 					this.HandleError(request, "ServiceCheck");
		// 					return;
		// 				}
		// 				callback();
		// 			});
		// 		}
		// 		<>4__this.Send<object>(websocketRequest, response);
		// 	}, null);
		// }


		public void EnterMatching(string matchingTokenKey, long userNum, Action callback)
		{
			if (callback == null)
			{
				throw new NullReferenceException();
			}

			Log.V("[MatchingSocket] Connect to {0}", matchingHost);
			Connect(matchingHost, () =>
				Send<object>(
					Req(nameof(EnterMatching), (object) nameof(matchingTokenKey), (object) matchingTokenKey,
						(object) nameof(userNum), (object) userNum), (request, result) =>
					{
						if (result == null)
						{
							HandleError(request, "EnterNormalMatching");
						}
						else
						{
							matchingStatus = MatchingStatus.Matching;
							callback();
						}
					}), () => MonoBehaviourInstance<LobbyUI>.inst?.UIEvent.OnLobbyStateUpdate(LobbyState.Ready));

			// co: dotPeek
			// if (callback == null)
			// {
			// 	throw new NullReferenceException();
			// }
			// Log.V("[MatchingSocket] Connect to {0}", this.matchingHost);
			// MatchingSocketOld.Response<object> <>9__2;
			// this.Connect(this.matchingHost, delegate
			// {
			// 	MatchingSocketOld <>4__this = this;
			// 	WebSocketRequest websocketRequest = this.Req("EnterMatching", new object[]
			// 	{
			// 		"matchingTokenKey",
			// 		matchingTokenKey,
			// 		"userNum",
			// 		userNum
			// 	});
			// 	MatchingSocketOld.Response<object> response;
			// 	if ((response = <>9__2) == null)
			// 	{
			// 		response = (<>9__2 = delegate(BlisWebSocketRequest request, object result)
			// 		{
			// 			if (result == null)
			// 			{
			// 				this.HandleError(request, "EnterNormalMatching");
			// 				return;
			// 			}
			// 			this.matchingStatus = MatchingStatus.Matching;
			// 			callback();
			// 		});
			// 	}
			// 	<>4__this.Send<object>(websocketRequest, response);
			// }, delegate
			// {
			// 	LobbyUI inst = MonoBehaviourInstance<LobbyUI>.inst;
			// 	if (inst == null)
			// 	{
			// 		return;
			// 	}
			// 	inst.UIEvent.OnLobbyStateUpdate(LobbyState.Ready);
			// });
		}


		public void ExitMatching(Action callback)
		{
			if (callback == null)
			{
				throw new NullReferenceException();
			}

			if (matchingStatus != MatchingStatus.Matching)
			{
				callback();
				Log.W("[MatchingSocket.ExitMatching] MatchingStatus is 'Matching'");
				return;
			}

			matchingStatus = MatchingStatus.WaitingExit;
			Send<object>(Req("ExitMatching", Array.Empty<object>()),
				delegate(BlisWebSocketRequest request, object result)
				{
					if (result == null)
					{
						HandleError(request, "ExitMatching");
						return;
					}

					StartCoroutine(DelayExit(callback));
				});
		}


		private IEnumerator DelayExit(Action callback)
		{
			yield return new WaitForSeconds(0.5f);
			if (matchingStatus != MatchingStatus.WaitingExit)
			{
				Log.V("[MatchingSocket] ExitMatching - Blocked by {0}", matchingStatus);
				yield break;
			}

			ExitCallback = callback;
			Close();
		}


		private void CheckServerExitStatus(Action callback)
		{
			callback();
		}


		public void HandleError(BlisWebSocketRequest req, string command)
		{
			if (req.getCode() == -1)
			{
				return;
			}

			BlisWebSocketError code = (BlisWebSocketError) req.getCode();
			if (code == BlisWebSocketError.OutOfService)
			{
				OnOutOfService();
				return;
			}

			OnError(Ln.Get(code.ToString()));
			if (matchingStatus == MatchingStatus.InCustomRoom && !code.CloseOnError())
			{
				return;
			}

			Close();
		}


		private void OnNotification(NNWebSocketMessage msg)
		{
			Log.V("[MatchingSocket] Notification: {0}", msg.method);
			string method = msg.method;
			if (!(method == "MatchingComplete"))
			{
				if (!(method == "UpdateCustomGameRoom"))
				{
					if (!(method == "ChatMessage"))
					{
						if (!(method == "KickedOut"))
						{
							if (!(method == "StartCustomGame"))
							{
								int num = method == "NoBattleHost" ? 1 : 0;
							}
							else
							{
								if (this.matchingStatus == MatchingStatus.Exited)
								{
									return;
								}

								int matchingStatus = (int) this.matchingStatus;
								this.matchingStatus = MatchingStatus.Matched;
								Close();
								startCustomGame(ParseNotificationParam<MatchingResult>(msg));
							}
						}
						else
						{
							kickedOutCustomGameRoom();
							Close();
						}
					}
					else
					{
						onChatMessage(ParseNotificationParam<MatchingChatMessage>(msg));
					}
				}
				else
				{
					updateCustomGameRoom(((JToken) ParseNotificationParam<Dictionary<string, object>>(msg)["roomInfo"])
						.ToObject<CustomGameRoom>());
				}
			}
			else
			{
				if (this.matchingStatus == MatchingStatus.Exited)
				{
					return;
				}

				int matchingStatus = (int) this.matchingStatus;
				MatchingResult notificationParam = ParseNotificationParam<MatchingResult>(msg);
				Log.V("[MOON][MatchingSoket:OnNotification] " + notificationParam.battleHost + ", " +
				      notificationParam.battleTokenKey);
				this.matchingStatus = MatchingStatus.Matched;
				Close();
				completeMatching(ParseNotificationParam<MatchingResult>(msg));
			}

			// co: dotPeek
			// Log.V("[MatchingSocket] Notification: {0}", msg.method);
			// string method = msg.method;
			// if (!(method == "MatchingComplete"))
			// {
			// 	if (method == "UpdateCustomGameRoom")
			// 	{
			// 		CustomGameRoom obj = ((JObject)this.ParseNotificationParam<Dictionary<string, object>>(msg)["roomInfo"]).ToObject<CustomGameRoom>();
			// 		this.updateCustomGameRoom(obj);
			// 		return;
			// 	}
			// 	if (method == "ChatMessage")
			// 	{
			// 		MatchingChatMessage obj2 = this.ParseNotificationParam<MatchingChatMessage>(msg);
			// 		this.onChatMessage(obj2);
			// 		return;
			// 	}
			// 	if (method == "KickedOut")
			// 	{
			// 		this.kickedOutCustomGameRoom();
			// 		this.Close();
			// 		return;
			// 	}
			// 	if (!(method == "StartCustomGame"))
			// 	{
			// 		method == "NoBattleHost";
			// 		return;
			// 	}
			// 	if (this.matchingStatus == MatchingStatus.Exited)
			// 	{
			// 		return;
			// 	}
			// 	MatchingStatus matchingStatus = this.matchingStatus;
			// 	this.matchingStatus = MatchingStatus.Matched;
			// 	this.Close();
			// 	this.startCustomGame(this.ParseNotificationParam<MatchingResult>(msg));
			// 	return;
			// }
			// else
			// {
			// 	if (this.matchingStatus == MatchingStatus.Exited)
			// 	{
			// 		return;
			// 	}
			// 	MatchingStatus matchingStatus2 = this.matchingStatus;
			// 	MatchingResult matchingResult = this.ParseNotificationParam<MatchingResult>(msg);
			// 	Log.V("[MOON][MatchingSoket:OnNotification] " + matchingResult.battleHost + ", " + matchingResult.battleTokenKey);
			// 	this.matchingStatus = MatchingStatus.Matched;
			// 	this.Close();
			// 	this.completeMatching(this.ParseNotificationParam<MatchingResult>(msg));
			// 	return;
			// }
		}


		protected void Connect(string url, Action onConnect = null, Action onFailed = null)
		{
			if (state != MatchingSocketState.Unconnected)
			{
				return;
			}

			state = MatchingSocketState.Connecting;
			StartCoroutine(_Connect(url, onConnect, onFailed));
		}


		private IEnumerator _Connect(string url, Action onConnect, Action onFailed)
		{
			if (ws != null)
			{
				throw new InvalidProgramException();
			}

			ws = new WebSocket();
			ws.OnOpen += OnOpen;
			ws.OnMessage += OnMessage;
			ws.OnBinary += OnBinary;
			ws.OnError += OnException;
			ws.OnClose += OnClose;
			ws.Open(url);
			yield return null;
			float connectingTime = 0f;
			while (state == MatchingSocketState.Connecting)
			{
				yield return null;
				connectingTime += Time.deltaTime;
				if (connectingTime > connectionTimeOut)
				{
					break;
				}
			}

			if (onConnect != null && state == MatchingSocketState.Connected)
			{
				onConnect();
				StartCoroutine(Ping());
			}
			else
			{
				OnOutOfService();
				if (onFailed != null)
				{
					onFailed();
				}
			}
		}


		private void OnOpen(WebSocket sender)
		{
			state = MatchingSocketState.Connected;
		}


		private void OnMessage(WebSocket sender, string message)
		{
			NNWebSocketMessage nnwebSocketMessage = JsonConvert.DeserializeObject<NNWebSocketMessage>(message);
			nnwebSocketMessage.json = message;
			if (nnwebSocketMessage.IsNotification)
			{
				OnNotification(nnwebSocketMessage);
				return;
			}

			OnResponse(nnwebSocketMessage);
		}


		private void OnBinary(WebSocket sender, byte[] binary)
		{
			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (MemoryStream memoryStream2 = new MemoryStream(binary, false))
					{
						using (GZipInputStream gzipInputStream = new GZipInputStream(memoryStream2))
						{
							byte[] array = new byte[8192];
							int count;
							while ((count = gzipInputStream.Read(array, 0, array.Length)) > 0)
							{
								memoryStream.Write(array, 0, count);
							}
						}
					}

					OnMessage(sender, Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int) memoryStream.Length));
				}
			}
			catch (Exception ex)
			{
				Log.E(ex.ToString());
			}
		}


		private void OnOutOfService()
		{
			PopupServerErrorWithCommunity(BlisWebSocketError.OutOfService.ToString());
			Close();
		}


		private void OnException(WebSocket sender, Exception exception)
		{
			if (exception != null)
			{
				Log.Exception(exception);
			}

			if (state == MatchingSocketState.Connecting)
			{
				OnOutOfService();
				return;
			}

			if (exception is SocketException &&
			    ((SocketException) exception).SocketErrorCode == SocketError.ConnectionRefused)
			{
				OnOutOfService();
				return;
			}

			OnError(Ln.Get(exception.ToString()));
		}


		protected WebSocketRequest Req(string method, params object[] _params)
		{
			return new WebSocketRequest(method, lastRequestId += 1L, 0L, _params);
		}


		protected void Send<T>(WebSocketRequest websocketRequest, Response<T> response) where T : class
		{
			BlisWebSocketRequest blisWebSocketRequest = new BlisWebSocketRequest(websocketRequest, typeof(T));
			if (ws == null || ws.State != WebSocketStates.Open)
			{
				response(blisWebSocketRequest);
				return;
			}

			blisWebSocketRequest.RequestTime = DateTime.UtcNow;
			requests.Add(blisWebSocketRequest);
			string data = JsonConvert.SerializeObject(websocketRequest);
			ws.Send(data);
			StartCoroutine(WaitingResponse<T>(blisWebSocketRequest, response));
		}


		private IEnumerator StartServerIndicatorTimer()
		{
			yield return new WaitForSeconds(1f);
			if (MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator != null &&
			    !MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.activeSelf)
			{
				MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.SetActive(true);
			}
		}


		private void StopServerIndicatorTimer()
		{
			if (serverIndicatorTimer != null)
			{
				StopCoroutine(serverIndicatorTimer);
				serverIndicatorTimer = null;
			}

			if (MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator != null &&
			    MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.activeSelf)
			{
				MonoBehaviourInstance<LobbyUI>.inst.ServerIndicator.gameObject.SetActive(false);
			}
		}


		private IEnumerator WaitingResponse<T>(BlisWebSocketRequest blisWebSocketRequest, Response<T> response)
			where T : class
		{
			if (serverIndicatorTimer == null)
			{
				serverIndicatorTimer = StartCoroutine(StartServerIndicatorTimer());
			}

			yield return StartCoroutine(blisWebSocketRequest);
			StopServerIndicatorTimer();
			if (!blisWebSocketRequest.State.IsDone())
			{
				response(blisWebSocketRequest);
				yield break;
			}

			T t = blisWebSocketRequest.Result<T>();
			if (t == null && typeof(T) == typeof(object))
			{
				t = (T) new object();
			}

			response(blisWebSocketRequest, t);
		}


		private void OnResponse(NNWebSocketMessage res)
		{
			BlisWebSocketRequest blisWebSocketRequest = requests.Find(r => r.RequestId == res.id);
			if (blisWebSocketRequest != null)
			{
				requests.Remove(blisWebSocketRequest);
				blisWebSocketRequest.Response = res;
			}
		}


		protected T ParseNotificationParam<T>(NNWebSocketMessage nnWebSocketMsg) where T : class
		{
			WebSocketParam<T> webSocketParam = null;
			try
			{
				webSocketParam = JsonConvert.DeserializeObject<WebSocketParam<T>>(nnWebSocketMsg.json);
			}
			catch (Exception e)
			{
				Log.Exception(e);
				T result = default;
				return result;
			}

			if (webSocketParam != null)
			{
				return webSocketParam.param;
			}

			return default;
		}


		private void CancelRequests(Exception ex)
		{
			requests.ForEach(delegate(BlisWebSocketRequest req) { req.Exception = ex; });
			requests = new List<BlisWebSocketRequest>();
		}


		public void Close()
		{
			if (state != MatchingSocketState.Connected)
			{
				return;
			}

			state = MatchingSocketState.Closing;
			if (ws != null)
			{
				CancelRequests(new Exception("WebSocket is closed"));
				ws.Close();
			}
		}


		private void OnClose(WebSocket sender, bool wasClean, WebSocketStatusCodes code, string reason)
		{
			Log.V("[OnClose] MatchingStatus: {0}", matchingStatus);
			if (state == MatchingSocketState.Connected)
			{
				OnError(Ln.Get("서버와 접속이 종료되었습니다."));
				Log.E("[GameClient] 서버와 접속이 종료되었습니다.: MatchingSocketOld OnClose");
			}

			state = MatchingSocketState.Unconnected;
			if (matchingStatus == MatchingStatus.WaitingExit)
			{
				matchingStatus = MatchingStatus.Exited;
				Action exitCallback = ExitCallback;
				if (exitCallback != null)
				{
					exitCallback();
				}
			}

			Action action = onClose;
			if (action != null)
			{
				action();
			}

			ws = null;
		}


		protected delegate void Response<T>(BlisWebSocketRequest req, T result = default) where T : class;
	}
}