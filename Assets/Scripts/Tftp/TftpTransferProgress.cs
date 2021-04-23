namespace Tftp.Net
{
	
	public class TftpTransferProgress
	{
		
		
		
		public long TransferredBytes { get; private set; }

		
		
		
		public long TotalBytes { get; private set; }

		
		public TftpTransferProgress(long transferred, long total)
		{
			this.TransferredBytes = transferred;
			this.TotalBytes = total;
		}

		
		public override string ToString()
		{
			if (this.TotalBytes > 0L)
			{
				return this.TransferredBytes * 100L / this.TotalBytes + "% completed";
			}
			return this.TransferredBytes + " bytes transferred";
		}
	}
}
