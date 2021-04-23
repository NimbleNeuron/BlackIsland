namespace Tftp.Net
{
	
	internal class Data : ITftpCommand
	{
		
		
		
		public ushort BlockNumber { get; private set; }

		
		
		
		public byte[] Bytes { get; private set; }

		
		public Data(ushort blockNumber, byte[] data)
		{
			this.BlockNumber = blockNumber;
			this.Bytes = data;
		}

		
		public void Visit(ITftpCommandVisitor visitor)
		{
			visitor.OnData(this);
		}

		
		public const ushort OpCode = 3;
	}
}
