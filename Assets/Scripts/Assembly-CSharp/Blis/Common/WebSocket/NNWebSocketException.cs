using System;

namespace Blis.Common
{
	
	public class NNWebSocketException : Exception
	{
		public int Code { get; private set; }
		
		public ErrorType ErrorType { get; private set; }
		
		public NNWebSocketException(NNWebSocketMessage msg) : base(msg.msg)
		{
			this.Code = msg.code;
			this.ErrorType = (ErrorType)this.Code;
			Log.E(string.Concat(new object[]
			{
				"[ERROR] ",
				this.Code,
				" || ",
				this.ErrorType
			}));
		}
	}
}
