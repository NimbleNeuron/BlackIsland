using System;

namespace LiteNetLib
{
	
	public class InvalidPacketException : ArgumentException
	{
		
		public InvalidPacketException(string message) : base(message) { }
	}
}