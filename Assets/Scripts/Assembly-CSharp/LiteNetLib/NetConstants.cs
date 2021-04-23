namespace LiteNetLib
{
	
	public static class NetConstants
	{
		
		public const int DefaultWindowSize = 64;

		
		public const int SocketBufferSize = 1048576;

		
		public const int SocketTTL = 255;

		
		public const int HeaderSize = 1;

		
		public const int ChanneledHeaderSize = 4;

		
		public const int FragmentHeaderSize = 6;

		
		public const int FragmentedHeaderTotalSize = 10;

		
		public const ushort MaxSequence = 32768;

		
		public const ushort HalfMaxSequence = 16384;

		
		internal const int ProtocolId = 11;

		
		internal const int MaxUdpHeaderSize = 68;

		
		internal static readonly int[] PossibleMtu = new int[]
		{
			508,
			1164,
			1392,
			1404,
			1424,
			1432
		};

		
		internal static readonly int MaxPacketSize = NetConstants.PossibleMtu[NetConstants.PossibleMtu.Length - 1];

		
		public const byte MaxConnectionNumber = 4;

		
		public const int PacketPoolSize = 1000;
	}
}
