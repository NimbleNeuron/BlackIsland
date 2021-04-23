using System;

namespace LiteNetLib.Utils
{
	
	public class InvalidTypeException : ArgumentException
	{
		
		public InvalidTypeException(string message) : base(message)
		{
		}
	}
}
