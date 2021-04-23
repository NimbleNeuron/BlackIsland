using System;
using System.IO;
using System.Text;

namespace Neptune.WebSocket
{
	
	public class WebSocketCloseFrame : WebSocketFrame
	{
		
		
		public override WebSocketOpcodes Opcode
		{
			get
			{
				return WebSocketOpcodes.ConnectionClose;
			}
		}

		
		
		
		public WebSocketStatusCodes StatusCode { get; private set; }

		
		
		
		public string Reason { get; private set; }

		
		public WebSocketCloseFrame() : this(WebSocketStatusCodes.Normal, "Normal Close")
		{
		}

		
		public WebSocketCloseFrame(WebSocketException ex) : this(ex.CloseCode, ex.Message)
		{
		}

		
		public WebSocketCloseFrame(WebSocketStatusCodes statusCode, string reason) : base((long)(Encoding.UTF8.GetByteCount(reason) + 2))
		{
			this.StatusCode = statusCode;
			this.Reason = reason;
		}

		
		internal WebSocketCloseFrame(WebSocketFrameHeader header, byte[] buffer, int offset) : base(header)
		{
			if (header.Final)
			{
				this.OnFinal(buffer, offset, true);
				return;
			}
			base.SetIncompletePayload(buffer, offset);
		}

		
		internal WebSocketCloseFrame(WebSocketFrameHeader header, byte[] buffer) : base(header)
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
			byte[] bytes = BitConverter.GetBytes((ushort)this.StatusCode);
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(bytes, 0, bytes.Length);
			}
			base.WriteTo(stream, new byte[][]
			{
				bytes,
				Encoding.UTF8.GetBytes(this.Reason)
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
			int num = (int)base.Header.PayloadLength;
			if (unmask)
			{
				base.Unmask(buffer, offset, num);
			}
			byte[] array = new byte[]
			{
				buffer[offset],
				buffer[offset + 1]
			};
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(array, 0, 2);
			}
			this.StatusCode = (WebSocketStatusCodes)BitConverter.ToUInt16(array, 0);
			if (num > 2)
			{
				this.Reason = Encoding.UTF8.GetString(buffer, offset + 2, num - 2);
				return;
			}
			this.Reason = string.Empty;
		}
	}
}
