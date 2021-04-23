namespace Tftp.Net
{
	
	internal class Error : ITftpCommand
	{
		
		
		
		public ushort ErrorCode { get; private set; }

		
		
		
		public string Message { get; private set; }

		
		public Error(ushort errorCode, string message)
		{
			this.ErrorCode = errorCode;
			this.Message = message;
		}

		
		public void Visit(ITftpCommandVisitor visitor)
		{
			visitor.OnError(this);
		}

		
		public const ushort OpCode = 5;
	}
}
