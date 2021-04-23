using System;

namespace Blis.Common
{
	
	public class BlisWebSocketRequest : NNWebSocketRequest
	{
		public BlisWebSocketRequest(WebSocketRequest req, Type responseType) : base(req, responseType)
		{
		}
		
		public new NNWebSocketMessage Response
		{
			get
			{
				return this.response;
			}
			internal set
			{
				if (!base.State.IsFinal())
				{
					this.response = value;
					if (this.response == null)
					{
						base.Exception = new Exception("Null response");
						return;
					}
					if (this.response.code == 200)
					{
						base.State = NNWebSocketRequestStates.Done;
						return;
					}
					base.Exception = new BlisWebSocketException(this.response.code);
				}
			}
		}

		
		public int getCode()
		{
			if (this.response == null)
			{
				return -1;
			}
			return this.response.code;
		}
	}
}
