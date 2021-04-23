using System;

namespace Blis.Client
{
	public class LocalException : Exception
	{
		public string msg;

		public LocalException(string msg) : base(msg)
		{
			this.msg = msg;
		}
	}
}