namespace LiteNetLib
{
	
	public sealed class NetStatistics
	{
		
		
		public ulong PacketLossPercent
		{
			get
			{
				if (this.PacketsSent != 0UL)
				{
					return this.PacketLoss * 100UL / this.PacketsSent;
				}
				return 0UL;
			}
		}

		
		public void Reset()
		{
			this.PacketsSent = 0UL;
			this.PacketsReceived = 0UL;
			this.BytesSent = 0UL;
			this.BytesReceived = 0UL;
			this.PacketLoss = 0UL;
		}

		
		public override string ToString()
		{
			return string.Format("BytesReceived: {0}\nPacketsReceived: {1}\nBytesSent: {2}\nPacketsSent: {3}\nPacketLoss: {4}\nPacketLossPercent: {5}\n", new object[]
			{
				this.BytesReceived,
				this.PacketsReceived,
				this.BytesSent,
				this.PacketsSent,
				this.PacketLoss,
				this.PacketLossPercent
			});
		}

		
		public ulong PacketsSent;

		
		public ulong PacketsReceived;

		
		public ulong BytesSent;

		
		public ulong BytesReceived;

		
		public ulong PacketLoss;

		
		public ulong SequencedPacketLoss;
	}
}
