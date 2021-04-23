using System;

namespace Neptune.WebSocket
{
	
	public class WebSocketException : Exception
	{
		
		
		
		public WebSocketStatusCodes CloseCode { get; private set; }

		
		public WebSocketException() : this(WebSocketStatusCodes.ClientError)
		{
		}

		
		public WebSocketException(WebSocketStatusCodes closeCode) : base(closeCode.ToString())
		{
			this.CloseCode = closeCode;
		}

		
		public WebSocketException(string message) : this(WebSocketStatusCodes.ClientError, message)
		{
		}

		
		public WebSocketException(WebSocketStatusCodes closeCode, string message) : base(message)
		{
		}
	}
}
