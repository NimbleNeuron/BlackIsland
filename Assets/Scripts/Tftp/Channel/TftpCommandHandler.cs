using System.Net;

namespace Tftp.Net.Channel
{
	
	// (Invoke) Token: 0x06000154 RID: 340
	internal delegate void TftpCommandHandler(ITftpCommand command, EndPoint endpoint);
}
