using System;

namespace Blis.Common
{
	
	public class BlisWebSocketException : Exception
	{
		
		public int code;

		
		public BlisWebSocketException(int code)
		{
			this.code = code;
		}

		
		public override string ToString()
		{
			BlisWebSocketError blisWebSocketError = (BlisWebSocketError) code;
			return blisWebSocketError.ToString();
		}
	}
}