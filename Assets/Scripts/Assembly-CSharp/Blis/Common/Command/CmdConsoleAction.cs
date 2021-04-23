using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdConsoleAction, false)]
	public class CmdConsoleAction : LocalSecurityConsoleCommandPacket
	{
		[Key(1)] public SecurityConsoleEvent eventType;


		public override void Action(ClientService service, LocalSecurityConsole self)
		{
			self.ConsoleAnimation(eventType);
		}
	}
}