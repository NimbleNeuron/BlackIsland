namespace Tftp.Net.Trace
{
	
	public static class TftpTrace
	{
		
		
		
		public static bool Enabled { get; set; } = false;

		
		internal static void Trace(string message, ITftpTransfer transfer)
		{
			if (!TftpTrace.Enabled)
			{
				return;
			}
			System.Diagnostics.Trace.WriteLine(message, transfer.ToString());
		}
	}
}
