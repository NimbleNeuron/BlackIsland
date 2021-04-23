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
using UnityEngine;

namespace Blis.Common
{
	public abstract class BlisWebSocket : MonoBehaviourInstance<BlisWebSocket>
	{
		public delegate void ServerErrorWithOutOfService();

		public enum BlisWebSocketState
		{
			Unconnected,
			Connecting,
			Connected,
			Closing
		}

		private const float PING_INTERVAL = 5f;
		private static long lastRequestId;
		private readonly float connectionTimeOut = 30f;
		protected Action exitCallback;
		protected string host;
		private Action onClose;
		private Action<string> onError;
		public ServerErrorWithOutOfService OnServerErrorWithOutOfService;
		private readonly int pingTimeOut = 30000;
		private List<BlisWebSocketRequest> requests = new List<BlisWebSocketRequest>();
		private BlisWebSocketState state;
		private WebSocket ws;

		protected void Init(string matchingHost)
		{
			host = matchingHost;
			onClose = null;
		}


		protected void Connect(string url, Action onConnect = null, Action onFailed = null)
		{
			if (state != BlisWebSocketState.Unconnected)
			{
				return;
			}

			state = BlisWebSocketState.Connecting;
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
			while (state == BlisWebSocketState.Connecting)
			{
				yield return null;
				connectingTime += Time.deltaTime;
				if (connectingTime > connectionTimeOut)
				{
					break;
				}
			}

			if (onConnect != null && state == BlisWebSocketState.Connected)
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


		private IEnumerator Ping()
		{
			if (state != BlisWebSocketState.Connected)
			{
				yield break;
			}

			if (requests.Count > 0 && requests[0].Elasped.TotalMilliseconds > 5f * pingTimeOut)
			{
				foreach (BlisWebSocketRequest blisWebSocketRequest in requests) { }

				OnRequestTimeout();
				yield break;
			}

			Send<object>(Req("Ping", Array.Empty<object>()), delegate { });
			yield return new WaitForSeconds(5f);
			StartCoroutine(Ping());
		}


		private void OnRequestTimeout()
		{
			Close();
			OnError(Ln.Get("매칭서버 타임아웃"));
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


		protected WebSocketRequest Req(string method, params object[] _params)
		{
			return new WebSocketRequest(method, lastRequestId += 1L, 0L, _params);
		}


		private IEnumerator WaitingResponse<T>(BlisWebSocketRequest blisWebSocketRequest, Response<T> response)
			where T : class
		{
			yield return StartCoroutine(blisWebSocketRequest);
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

			if (webSocketParam == null)
			{
				return default;
			}

			return webSocketParam.param;
		}


		private void OnOpen(WebSocket sender)
		{
			state = BlisWebSocketState.Connected;
		}


		protected abstract void OnNotification(NNWebSocketMessage msg);


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


		private void OnResponse(NNWebSocketMessage res)
		{
			BlisWebSocketRequest blisWebSocketRequest = requests.Find(r => r.RequestId == res.id);
			if (blisWebSocketRequest != null)
			{
				requests.Remove(blisWebSocketRequest);
				blisWebSocketRequest.Response = res;
			}
		}


		protected void OnError(string msg)
		{
			Action<string> action = onError;
			if (action == null)
			{
				return;
			}

			action(msg);
		}


		private void OnException(WebSocket sender, Exception exception)
		{
			if (exception != null)
			{
				Log.Exception(exception);
			}

			if (state == BlisWebSocketState.Connecting)
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


		private void OnOutOfService()
		{
			ServerErrorWithOutOfService onServerErrorWithOutOfService = OnServerErrorWithOutOfService;
			if (onServerErrorWithOutOfService != null)
			{
				onServerErrorWithOutOfService();
			}

			Close();
		}


		private void CancelRequests(Exception ex)
		{
			requests.ForEach(delegate(BlisWebSocketRequest req) { req.Exception = ex; });
			requests = new List<BlisWebSocketRequest>();
		}


		public void Close()
		{
			if (state != BlisWebSocketState.Connected)
			{
				return;
			}

			state = BlisWebSocketState.Closing;
			if (ws != null)
			{
				CancelRequests(new Exception("WebSocket is closed"));
				ws.Close();
			}
		}


		protected abstract bool OnClose();


		private void OnClose(WebSocket sender, bool wasClean, WebSocketStatusCodes code, string reason)
		{
			if (state == BlisWebSocketState.Connected)
			{
				OnError(Ln.Get("서버와 접속이 종료되었습니다."));
				Log.E("[GameClient] 서버와 접속이 종료되었습니다.: blisWebSocket OnClose");
			}

			state = BlisWebSocketState.Unconnected;
			if (OnClose())
			{
				Action action = exitCallback;
				if (action != null)
				{
					action();
				}
			}

			Action action2 = onClose;
			if (action2 != null)
			{
				action2();
			}

			ws = null;
		}


		protected abstract IEnumerator DelayExit(Action callback);


		protected void HandleError(BlisWebSocketRequest req, string command)
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
			if (!code.CloseOnError())
			{
				return;
			}

			Close();
		}


		public void RegisterCloseCallback(Action close)
		{
			onClose = close;
		}


		public void RegisterErrorCallback(Action<string> onError)
		{
			this.onError = onError;
		}


		protected delegate void Response<T>(BlisWebSocketRequest req, T result = default) where T : class;
	}
}