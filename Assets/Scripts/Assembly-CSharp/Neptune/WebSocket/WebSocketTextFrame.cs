using System.IO;
using System.Text;

namespace Neptune.WebSocket
{
	
	public class WebSocketTextFrame : WebSocketFrame
	{
		
		
		public override WebSocketOpcodes Opcode
		{
			get
			{
				return WebSocketOpcodes.Text;
			}
		}

		
		
		
		public string Text { get; private set; }

		
		public WebSocketTextFrame(string text) : base((long)Encoding.UTF8.GetByteCount(text))
		{
			this.Text = text;
		}

		
		internal WebSocketTextFrame(WebSocketFrameHeader header, byte[] buffer, int offset) : base(header)
		{
			if (header.Final)
			{
				this.OnFinal(buffer, offset, true);
				return;
			}
			base.SetIncompletePayload(buffer, offset);
		}

		
		internal WebSocketTextFrame(WebSocketFrameHeader header, byte[] buffer) : base(header)
		{
			if (header.Final)
			{
				this.OnFinal(buffer, 0, true);
				return;
			}
			base.SetIncompletePayload(buffer);
		}

		
		public override void WriteTo(Stream stream)
		{
			base.WriteTo(stream, new byte[][]
			{
				Encoding.UTF8.GetBytes(this.Text)
			});
		}

		
		protected override void OnFinal(byte[] buffer, bool unmask)
		{
			this.OnFinal(buffer, 0, unmask);
		}

		
		private void OnFinal(byte[] buffer, int offset, bool unmask)
		{
			if (base.Header.PayloadLength > 2147483647L)
			{
				throw new WebSocketException(WebSocketStatusCodes.TooLarge);
			}
			int count = (int)base.Header.PayloadLength;
			if (unmask)
			{
				base.Unmask(buffer, offset, count);
			}
			this.Text = Encoding.UTF8.GetString(buffer, offset, count);
		}
	}
}
