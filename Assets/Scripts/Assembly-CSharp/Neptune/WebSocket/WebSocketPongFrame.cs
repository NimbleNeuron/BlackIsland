using System.Text;

namespace Neptune.WebSocket
{
	
	public class WebSocketPongFrame : WebSocketBinaryFrame
	{
		
		
		public override WebSocketOpcodes Opcode
		{
			get
			{
				return WebSocketOpcodes.Pong;
			}
		}

		
		
		public string Text
		{
			get
			{
				if (this.text != null)
				{
					return this.text;
				}
				if (!base.Header.Final)
				{
					return null;
				}
				this.text = Encoding.UTF8.GetString(base.Data, 0, (int)base.Header.PayloadLength);
				return this.text;
			}
		}

		
		public WebSocketPongFrame(WebSocketPingFrame ping) : base(ping.Data)
		{
		}

		
		internal WebSocketPongFrame(WebSocketFrameHeader header, byte[] buffer, int offset) : base(header, buffer, offset)
		{
		}

		
		internal WebSocketPongFrame(WebSocketFrameHeader header, byte[] buffer) : base(header, buffer)
		{
		}

		
		private string text;
	}
}
