using System;

namespace Blis.Common
{
	public class GameException : Exception
	{
		public readonly ErrorType errorType;


		public string msg;

		public GameException(ErrorType errorType) : base(errorType.ToString())
		{
			this.errorType = errorType;
			msg = errorType.ToString();
		}


		public GameException(ErrorType errorType, string msg) : base(msg)
		{
			this.msg = msg;
			this.errorType = errorType;
		}


		public GameException(string msg) : base(msg)
		{
			this.msg = msg;
			errorType = ErrorType.Internal;
		}
	}
}