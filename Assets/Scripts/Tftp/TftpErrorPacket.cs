using System;

namespace Tftp.Net
{
	
	public class TftpErrorPacket : TftpTransferError
	{
		
		
		
		public ushort ErrorCode { get; private set; }

		
		
		
		public string ErrorMessage { get; private set; }

		
		public TftpErrorPacket(ushort errorCode, string errorMessage)
		{
			if (string.IsNullOrEmpty(errorMessage))
			{
				throw new ArgumentException("You must provide an errorMessage.");
			}
			this.ErrorCode = errorCode;
			this.ErrorMessage = errorMessage;
		}

		
		public override string ToString()
		{
			return this.ErrorCode + " - " + this.ErrorMessage;
		}

		
		public static readonly TftpErrorPacket FileNotFound = new TftpErrorPacket(1, "File not found");

		
		public static readonly TftpErrorPacket AccessViolation = new TftpErrorPacket(2, "Access violation");

		
		public static readonly TftpErrorPacket DiskFull = new TftpErrorPacket(3, "Disk full or allocation exceeded");

		
		public static readonly TftpErrorPacket IllegalOperation = new TftpErrorPacket(4, "Illegal TFTP operation");

		
		public static readonly TftpErrorPacket UnknownTransferId = new TftpErrorPacket(5, "Unknown transfer ID");

		
		public static readonly TftpErrorPacket FileAlreadyExists = new TftpErrorPacket(6, "File already exists");

		
		public static readonly TftpErrorPacket NoSuchUser = new TftpErrorPacket(7, "No such user");
	}
}
