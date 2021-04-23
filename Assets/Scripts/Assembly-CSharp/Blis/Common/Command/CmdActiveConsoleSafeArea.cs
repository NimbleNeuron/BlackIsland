using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdActiveConsoleSafeArea, false)]
	public class CmdActiveConsoleSafeArea : LocalSecurityConsoleCommandPacket
	{
		
		[Key(1)] public bool enable;

		
		public override void Action(ClientService service, LocalSecurityConsole self)
		{
			self.ActiveLastSafeConsole(enable);
		}
	}
}