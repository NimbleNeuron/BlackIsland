using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdCancelConsoleAction, false)]
	public class CmdCancelConsoleAction : LocalSecurityConsoleCommandPacket
	{
		
		public override void Action(ClientService service, LocalSecurityConsole self)
		{
			self.ConsoleAnimation(SecurityConsoleEvent.ShutdownSecurityConsole);
		}
	}
}