using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Blis.Common;
using Neptune.Http;
using UnityEngine;

namespace Neptune.WebSocket
{
	public class WebSocket
	{
		public event WebSocket.StateChangeHandler OnStateChange;
		public event WebSocket.OpenHandler OnOpen;
		public event WebSocket.MessageHandler OnMessage;
		public event WebSocket.BinaryHandler OnBinary;
		public event WebSocket.ErrorHandler OnError;
		public event WebSocket.CloseHandler OnClose;
		public string Url { get; private set; }
		public int ConnectTimeout { get; set; }
		public int ResponseTimeout { get; set; }
		public int PingTimeout { get; set; }
		public int CloseTimeout { get; set; }
		public WebSocketStates State
		{
			get
			{
				return this.state;
			}
			private set
			{
				if (this.state != value)
				{
					this.state = value;
					if (this.OnStateChange != null)
					{
						try
						{
							this.OnStateChange(this);
						}
						catch (Exception ex)
						{
							Neptune.Log.Logger.Exception(ex);
						}
					}
				}
			}
		}

		
		public WebSocket()
		{
			this.ConnectTimeout = 5000;
			this.ResponseTimeout = 5000;
			this.PingTimeout = 3000;
			this.CloseTimeout = 5000;
		}

		
		private bool CheckIfIpv6OnlyNetwork()
		{
			IPHostEntry iphostEntry = null;
			try
			{
				iphostEntry = Dns.GetHostEntry("www.apple.com");
			}
			catch (Exception e)
			{
				Blis.Common.Log.Exception(e);
				return false;
			}
			if (iphostEntry == null)
			{
				return false;
			}
			foreach (IPAddress ipaddress in iphostEntry.AddressList)
			{
				Blis.Common.Log.H("IP LOOKUP: {0}", new object[]
				{
					ipaddress
				});
				if (ipaddress.AddressFamily != AddressFamily.InterNetworkV6)
				{
					return false;
				}
			}
			return true;
		}

		
		private string GetMappedIpv6Address(string url)
		{
			Uri uri = new Uri(url);
			if (uri == null || uri.Host.Length == 0)
			{
				Blis.Common.Log.E("Invalid URL");
				return url;
			}
			IPAddress ipaddress = null;
			try
			{
				ipaddress = IPAddress.Parse(uri.Host);
			}
			catch (Exception e)
			{
				Blis.Common.Log.Exception(e);
				return url;
			}
			if (ipaddress.AddressFamily == AddressFamily.InterNetworkV6)
			{
				return url;
			}
			string arg = "bsurvival.com";
			if (uri.Host == "133.186.134.234")
			{
				arg = "www.bsurvival.com";
			}
			return string.Format("ws://{0}:{1}", arg, uri.Port);
		}

		
		public bool Open(string url, IDictionary<string, string> requestHeaders = null)
		{
			bool result;
			try
			{
				this.Url = url;
				Uri uri = new Uri(url);
				this.requestHeaders = new Dictionary<string, string>();
				this.requestHeaders["User-Agent"] = "Neptune.WebSocket";
				HttpRequestFactory.GetHeaders(url, this.requestHeaders);
				if (requestHeaders != null)
				{
					foreach (KeyValuePair<string, string> keyValuePair in requestHeaders)
					{
						this.requestHeaders[keyValuePair.Key] = keyValuePair.Value;
					}
				}
				this.requestHeaders["Host"] = uri.Authority;
				this.requestHeaders["Upgrade"] = "websocket";
				this.requestHeaders["Connection"] = "Upgrade";
				this.requestHeaders["Sec-WebSocket-Key"] = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
				this.requestHeaders["Sec-WebSocket-Version"] = "13";
				this.requestHeaders["Origin"] = "null";
				this.BeginConnect(uri, false);
				result = true;
			}
			catch (Exception e)
			{
				this.HandleException(e);
				result = false;
			}
			return result;
		}

		
		public bool Send(WebSocketFrame frame)
		{
			bool result;
			try
			{
				if (this.State != WebSocketStates.Open)
				{
					result = false;
				}
				else
				{
					this.SendFrame(frame);
					result = true;
				}
			}
			catch (Exception e)
			{
				this.HandleException(e);
				result = false;
			}
			return result;
		}

		
		public bool Send(string data)
		{
			return this.Send(new WebSocketTextFrame(data));
		}

		
		public bool Send(byte[] data)
		{
			return this.Send(new WebSocketBinaryFrame(data));
		}

		
		public void Close()
		{
			try
			{
				WebSocketStates webSocketStates = this.State;
				if (webSocketStates > WebSocketStates.Handshaking)
				{
					if (webSocketStates == WebSocketStates.Open)
					{
						this.SendFrame(new WebSocketCloseFrame());
					}
				}
				else
				{
					this.Close(null);
				}
			}
			catch (Exception e)
			{
				this.HandleException(e);
			}
		}

		
		private IEnumerator UpdateState()
		{
			long pauseCount = SingletonMonoBehaviour<WebSocketUpdater>.Instance.PauseCount;
			while (this.asyncResult != null)
			{
				bool flag = false;
				try
				{
					if (this.asyncResult.IsCompleted)
					{
						switch (this.State)
						{
						case WebSocketStates.Connecting:
							this.EndConnect();
							break;
						case WebSocketStates.Handshaking:
							this.EndHandshake();
							break;
						case WebSocketStates.Open:
						case WebSocketStates.Closing:
							this.EndReadFrame();
							break;
						}
					}
					else
					{
						if (DateTime.UtcNow >= this.stateTimeout)
						{
							throw new WebSocketException(string.Format("{0} timed out", this.State));
						}
						if (pauseCount != SingletonMonoBehaviour<WebSocketUpdater>.Instance.PauseCount && this.State == WebSocketStates.Open)
						{
							pauseCount = SingletonMonoBehaviour<WebSocketUpdater>.Instance.PauseCount;
							this.SendFrame(new WebSocketPingFrame(""));
						}
						else
						{
							flag = true;
						}
					}
				}
				catch (Exception e)
				{
					this.HandleException(e);
				}
				if (flag)
				{
					yield return null;
				}
			}
		}

		
		private void HandleException(Exception e)
		{
			if (!(e is WebSocketException))
			{
				this.Close(e);
				return;
			}
			switch (this.State)
			{
			case WebSocketStates.Connecting:
			case WebSocketStates.Handshaking:
			case WebSocketStates.Closing:
				this.Close(e);
				return;
			case WebSocketStates.Open:
				this.SendFrame(new WebSocketCloseFrame(e as WebSocketException));
				return;
			default:
				return;
			}
		}

		
		private void SetState(WebSocketStates state, int timeout = 0)
		{
			this.State = state;
			this.stateTimeout = ((timeout == 0) ? DateTime.MaxValue : (DateTime.UtcNow + TimeSpan.FromMilliseconds((double)timeout)));
			Debug.Log(string.Format("WebSocket | SetState: {0} | Timeout: {1}", state, timeout));
		}

		
		private void SendFrame(WebSocketFrame frame)
		{
			if (this.State == WebSocketStates.Open)
			{
				if (frame is WebSocketCloseFrame)
				{
					if (this.closeFrame == null)
					{
						this.closeFrame = (frame as WebSocketCloseFrame);
					}
					this.SetState(WebSocketStates.Closing, this.CloseTimeout);
				}
				else if (frame is WebSocketPingFrame && this.state == WebSocketStates.Open)
				{
					this.stateTimeout = DateTime.UtcNow + TimeSpan.FromMilliseconds((double)this.PingTimeout);
				}
				using (MemoryStream memoryStream = new MemoryStream(frame.Header.Length + (int)frame.Header.PayloadLength))
				{
					frame.WriteTo(memoryStream);
					memoryStream.WriteTo(this.stream);
				}
			}
		}

		
		private void BeginRead()
		{
			if (this.buffer == null)
			{
				this.ResetBuffer();
			}
			this.asyncResult = this.stream.BeginRead(this.buffer, this.bufferPos, this.buffer.Length - this.bufferPos, null, null);
		}

		
		private int EndRead()
		{
			int num = this.stream.EndRead(this.asyncResult);
			this.asyncResult = null;
			this.bufferPos += num;
			if (num == 0)
			{
				this.Close(null);
			}
			return num;
		}

		
		private void ResetBuffer()
		{
			this.buffer = this.defaultBuffer;
			this.bufferPos = 0;
		}

		
		private void SetBuffer(byte[] dst, byte[] src, int offset, int count)
		{
			this.buffer = dst;
			Buffer.BlockCopy(src, offset, dst, 0, count);
			this.bufferPos = count;
		}

		
		private void Close(Exception e)
		{
			if (this.State != WebSocketStates.Closed)
			{
				try
				{
					WebSocketStates webSocketStates = this.State;
					if (webSocketStates - WebSocketStates.Handshaking <= 2 && this.client != null && this.client.Connected && this.asyncResult == null)
					{
						int num;
						for (int i = this.client.Available; i > 0; i -= num)
						{
							int count = Mathf.Min(i, this.defaultBuffer.Length);
							num = this.stream.Read(this.buffer, 0, count);
						}
					}
					if (this.stream != null)
					{
						this.stream.Close();
					}
					if (this.client != null)
					{
						this.client.Close();
					}
				}
				catch (Exception ex)
				{
					Neptune.Log.Logger.Exception(ex);
				}
				finally
				{
					this.client = null;
					this.stream = null;
					this.asyncResult = null;
				}
				this.ResetBuffer();
				this.header = null;
				bool wasClean = e == null && this.closeFrame != null;
				WebSocketStatusCodes code;
				string reason;
				if (this.closeFrame == null)
				{
					code = WebSocketStatusCodes.UncleanClose;
					reason = ((e == null) ? string.Empty : e.Message);
				}
				else
				{
					code = this.closeFrame.StatusCode;
					reason = this.closeFrame.Reason;
					this.closeFrame = null;
				}
				this.SetState(WebSocketStates.Closed, 0);
				try
				{
					if (e != null && this.OnError != null)
					{
						this.OnError(this, e);
					}
				}
				catch (Exception ex2)
				{
					Neptune.Log.Logger.Exception(ex2);
				}
				try
				{
					if (this.OnClose != null)
					{
						this.OnClose(this, wasClean, code, reason);
					}
				}
				catch (Exception ex3)
				{
					Neptune.Log.Logger.Exception(ex3);
				}
			}
		}

		
		private void BeginConnect(Uri uri, bool ipv6 = false)
		{
			this.SetState(WebSocketStates.Connecting, this.ConnectTimeout);
			if (ipv6)
			{
				this.client = new TcpClient(AddressFamily.InterNetworkV6);
			}
			else
			{
				this.client = new TcpClient();
			}
			this.client.NoDelay = true;
			this.asyncResult = this.client.BeginConnect(uri.Host, uri.Port, null, null);
			SingletonMonoBehaviour<WebSocketUpdater>.Instance.UpdateState(this, this.UpdateState());
		}

		
		private void EndConnect()
		{
			this.client.EndConnect(this.asyncResult);
			this.asyncResult = null;
			this.BeginHandshake();
		}

		
		private void BeginHandshake()
		{
			this.SetState(WebSocketStates.Handshaking, this.ResponseTimeout);
			this.stream = this.client.GetStream();
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(Encoding.ASCII.GetBytes(string.Format("GET {0} HTTP/1.1", new Uri(this.Url).PathAndQuery)));
					binaryWriter.Write(WebSocket.CrLf);
					foreach (KeyValuePair<string, string> keyValuePair in this.requestHeaders)
					{
						binaryWriter.Write(Encoding.ASCII.GetBytes(keyValuePair.Key));
						binaryWriter.Write(':');
						binaryWriter.Write(' ');
						binaryWriter.Write(Encoding.ASCII.GetBytes(keyValuePair.Value));
						binaryWriter.Write(WebSocket.CrLf);
					}
					binaryWriter.Write(WebSocket.CrLf);
					memoryStream.WriteTo(this.stream);
				}
			}
			this.BeginRead();
		}

		
		private void EndHandshake()
		{
			if (this.EndRead() == 0)
			{
				return;
			}
			int num = 0;
			int num2 = 0;
			while (num2 < this.bufferPos && num != 4)
			{
				byte b = this.buffer[num2++];
				switch (num)
				{
				case 0:
					num = ((b == 13) ? 1 : 0);
					break;
				case 1:
					num = ((b == 10) ? 2 : ((b == 13) ? 1 : 0));
					break;
				case 2:
					num = ((b == 13) ? 3 : 0);
					break;
				case 3:
					num = ((b == 10) ? 4 : ((b == 13) ? 1 : 0));
					break;
				}
			}
			if (num == 4)
			{
				using (MemoryStream memoryStream = new MemoryStream(this.buffer, 0, num2))
				{
					using (StreamReader streamReader = new StreamReader(memoryStream, Encoding.ASCII))
					{
						int num3 = 0;
						string text = streamReader.ReadLine();
						string[] array = text.Split(new char[]
						{
							' '
						});
						if (array.Length < 3 || !int.TryParse(array[1], out num3))
						{
							throw new WebSocketException("Invalid HTTP status-line : " + text);
						}
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary["upgrade"] = string.Empty;
						dictionary["connection"] = string.Empty;
						dictionary["sec-websocket-accept"] = string.Empty;
						string text2;
						while ((text2 = streamReader.ReadLine()).Length > 0)
						{
							int num4 = text2.IndexOf(':');
							if (num4 >= 0)
							{
								string key = text2.Substring(0, num4).Trim().ToLower();
								string value = text2.Substring(num4 + 1).Trim();
								dictionary[key] = value;
							}
						}
						if (num3 != 101 || dictionary["upgrade"].ToLower() != "websocket" || dictionary["connection"].ToLower() != "upgrade")
						{
							throw new WebSocketException(string.Format("Handshake failed : status-code={0},upgrade={1},connection={2}", num3, dictionary["upgrade"], dictionary["connection"]));
						}
						string str = this.requestHeaders["Sec-WebSocket-Key"];
						string b2 = dictionary["sec-websocket-accept"].ToLower();
						if (Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(str + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"))).ToLower() != b2)
						{
							throw new WebSocketException("Handshake failed : invalid Sec-WebSocket-Accept");
						}
					}
				}
				this.SetState(WebSocketStates.Open, 0);
				try
				{
					if (this.OnOpen != null)
					{
						this.OnOpen(this);
					}
				}
				catch (Exception ex)
				{
					Neptune.Log.Logger.Exception(ex);
				}
				if (num2 < this.bufferPos)
				{
					this.ProcessFrames(num2);
				}
				else
				{
					this.ResetBuffer();
				}
				this.BeginRead();
				return;
			}
			if (this.bufferPos == this.buffer.Length)
			{
				throw new WebSocketException("Too large response header");
			}
			this.BeginRead();
		}

		
		private void EndReadFrame()
		{
			if (this.EndRead() == 0)
			{
				return;
			}
			if (this.header == null)
			{
				this.ProcessFrames(0);
			}
			else if (this.bufferPos == this.buffer.Length)
			{
				this.OnReceiveFrame(WebSocketFrame.Create(this.header, this.buffer));
				this.header = null;
				this.ResetBuffer();
			}
			this.BeginRead();
		}

		
		private void ProcessFrames(int offset)
		{
			while (offset < this.bufferPos)
			{
				int num = this.bufferPos - offset;
				WebSocketFrameHeader webSocketFrameHeader = WebSocketFrameHeader.TryGet(this.buffer, offset, num);
				if (webSocketFrameHeader == null)
				{
					this.SetBuffer(this.defaultBuffer, this.buffer, offset, num);
					return;
				}
				if (webSocketFrameHeader.PayloadLength > 2147483647L)
				{
					throw new WebSocketException(WebSocketStatusCodes.TooLarge);
				}
				offset += webSocketFrameHeader.Length;
				num -= webSocketFrameHeader.Length;
				int num2 = (int)webSocketFrameHeader.PayloadLength;
				if (num < num2)
				{
					this.header = webSocketFrameHeader;
					this.SetBuffer(new byte[num2], this.buffer, offset, num);
					return;
				}
				this.OnReceiveFrame(WebSocketFrame.Create(webSocketFrameHeader, this.buffer, offset));
				offset += num2;
				if (offset >= this.bufferPos)
				{
					this.ResetBuffer();
				}
			}
		}

		
		private void OnReceiveFrame(WebSocketFrame frame)
		{
			if (frame.Header.Final)
			{
				switch (frame.Opcode)
				{
				case WebSocketOpcodes.Continuation:
					this.OnContinuationFrame(frame as WebSocketContinuationFrame);
					return;
				case WebSocketOpcodes.Text:
					this.OnTextFrame(frame as WebSocketTextFrame);
					return;
				case WebSocketOpcodes.Binary:
					this.OnBinaryFrame(frame as WebSocketBinaryFrame);
					return;
				case (WebSocketOpcodes)3:
				case (WebSocketOpcodes)4:
				case (WebSocketOpcodes)5:
				case (WebSocketOpcodes)6:
				case (WebSocketOpcodes)7:
					break;
				case WebSocketOpcodes.ConnectionClose:
					this.OnCloseFrame(frame as WebSocketCloseFrame);
					return;
				case WebSocketOpcodes.Ping:
					this.OnPingFrame(frame as WebSocketPingFrame);
					return;
				case WebSocketOpcodes.Pong:
					this.OnPongFrame(frame as WebSocketPongFrame);
					break;
				default:
					return;
				}
				return;
			}
			if (this.incompleteFrame != null)
			{
				throw new WebSocketException(WebSocketStatusCodes.ProtocolError);
			}
			this.incompleteFrame = frame;
		}

		
		private void OnBinaryFrame(WebSocketBinaryFrame frame)
		{
			try
			{
				if (this.OnBinary != null)
				{
					this.OnBinary(this, frame.Data);
				}
			}
			catch (Exception ex)
			{
				Neptune.Log.Logger.Exception(ex);
			}
		}

		
		private void OnTextFrame(WebSocketTextFrame frame)
		{
			try
			{
				if (this.OnMessage != null)
				{
					this.OnMessage(this, frame.Text);
				}
			}
			catch (Exception ex)
			{
				Neptune.Log.Logger.Exception(ex);
			}
		}

		
		private void OnCloseFrame(WebSocketCloseFrame frame)
		{
			if (this.State == WebSocketStates.Open)
			{
				this.closeFrame = frame;
				this.SendFrame(new WebSocketCloseFrame());
			}
		}

		
		private void OnContinuationFrame(WebSocketContinuationFrame frame)
		{
			if (this.incompleteFrame == null)
			{
				throw new WebSocketException(WebSocketStatusCodes.ProtocolError);
			}
			this.incompleteFrame.Append(frame);
			if (this.incompleteFrame.Header.Final)
			{
				this.OnReceiveFrame(this.incompleteFrame);
				this.incompleteFrame = null;
			}
		}

		
		private void OnPingFrame(WebSocketPingFrame frame)
		{
			if (this.State == WebSocketStates.Open)
			{
				this.SendFrame(new WebSocketPongFrame(frame));
			}
		}

		
		private void OnPongFrame(WebSocketPongFrame frame)
		{
			if (this.state == WebSocketStates.Open)
			{
				this.stateTimeout = DateTime.MaxValue;
			}
		}
		
		private const int DefaultConnectTimeout = 5000;
		private const int DefaultResponseTimeout = 5000;
		private const int DefaultPingTimeout = 3000;
		private const int DefaultCloseTimeout = 5000;
		private const int BufferSize = 8192;
		private const byte Cr = 13;
		private const byte Lf = 10;
		private static readonly byte[] CrLf = new byte[]
		{
			13,
			10
		};
		
		private WebSocketStates state = WebSocketStates.Closed;
		private Dictionary<string, string> requestHeaders;
		private byte[] defaultBuffer = new byte[8192];
		private byte[] buffer;
		private int bufferPos;
		
		private WebSocketFrameHeader header;
		private WebSocketFrame incompleteFrame;
		private TcpClient client;
		private Stream stream;
		private IAsyncResult asyncResult;
		private DateTime stateTimeout = DateTime.MaxValue;
		private WebSocketCloseFrame closeFrame;
		
		public delegate void StateChangeHandler(WebSocket sender);
		public delegate void OpenHandler(WebSocket sender);
		public delegate void MessageHandler(WebSocket sender, string message);
		public delegate void BinaryHandler(WebSocket sender, byte[] binary);
		public delegate void ErrorHandler(WebSocket sender, Exception exception);
		public delegate void CloseHandler(WebSocket sender, bool wasClean, WebSocketStatusCodes code, string reason);
	}
}
