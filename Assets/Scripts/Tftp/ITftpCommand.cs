namespace Tftp.Net
{
	
	internal interface ITftpCommand
	{
		
		void Visit(ITftpCommandVisitor visitor);
	}
}
