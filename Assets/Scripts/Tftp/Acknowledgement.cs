namespace Tftp.Net
{
	
	internal class Acknowledgement : ITftpCommand
	{
		
		
		
		public ushort BlockNumber { get; private set; }

		
		public Acknowledgement(ushort blockNumber)
		{
			this.BlockNumber = blockNumber;
		}

		
		public void Visit(ITftpCommandVisitor visitor)
		{
			visitor.OnAcknowledgement(this);
		}

		
		public const ushort OpCode = 4;
	}
}
