using System;
using System.IO;

namespace Neptune.WebSocket
{
	
	public class WebSocketBinaryFrame : WebSocketFrame
	{
		
		
		public override WebSocketOpcodes Opcode
		{
			get
			{
				return WebSocketOpcodes.Binary;
			}
		}

		
		
		
		public byte[] Data { get; private set; }

		
		public WebSocketBinaryFrame(byte[] data) : base((long)data.Length)
		{
			this.Data = data;
		}

		
		internal WebSocketBinaryFrame(WebSocketFrameHeader header, byte[] buffer, int offset) : base(header)
		{
			if (header.Final)
			{
				this.OnFinal(buffer, offset, true);
				return;
			}
			base.SetIncompletePayload(buffer, offset);
		}

		
		internal WebSocketBinaryFrame(WebSocketFrameHeader header, byte[] buffer) : base(header)
		{
			if (header.Final)
			{
				this.OnFinal(buffer, true);
				return;
			}
			base.SetIncompletePayload(buffer);
		}

		
		private void OnFinal(byte[] buffer, int offset, bool unmask)
		{
			if (base.Header.PayloadLength > 2147483647L)
			{
				throw new WebSocketException(WebSocketStatusCodes.TooLarge);
			}
			int num = (int)base.Header.PayloadLength;
			this.Data = new byte[num];
			Buffer.BlockCopy(buffer, offset, this.Data, 0, num);
			if (unmask)
			{
				base.Unmask(this.Data, 0, num);
			}
		}

		
		protected override void OnFinal(byte[] buffer, bool unmask)
		{
			base.Unmask(buffer, 0, buffer.Length);
			this.Data = buffer;
		}

		
		public override void WriteTo(Stream stream)
		{
			base.WriteTo(stream, new byte[][]
			{
				this.Data
			});
		}
	}
}
