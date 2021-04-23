using System;

namespace LiteNetLib
{
	
	[Flags]
	public enum ConnectionState : byte
	{
		
		Outgoing = 2,

		
		Connected = 4,

		
		ShutdownRequested = 8,

		
		Disconnected = 16,

		
		Any = 14
	}
}