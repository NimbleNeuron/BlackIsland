using System.Text;

namespace Neptune.WebSocket
{
	
	public class WebSocketPingFrame : WebSocketBinaryFrame
	{
		
		
		public override WebSocketOpcodes Opcode
		{
			get
			{
				return WebSocketOpcodes.Ping;
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

		
		public WebSocketPingFrame(byte[] data) : base(data)
		{
		}

		
		public WebSocketPingFrame(string text) : base(Encoding.UTF8.GetBytes(text))
		{
			this.text = text;
		}

		
		internal WebSocketPingFrame(WebSocketFrameHeader header, byte[] buffer, int offset) : base(header, buffer, offset)
		{
		}

		
		internal WebSocketPingFrame(WebSocketFrameHeader header, byte[] buffer) : base(header, buffer)
		{
		}

		
		private string text;
	}
}
