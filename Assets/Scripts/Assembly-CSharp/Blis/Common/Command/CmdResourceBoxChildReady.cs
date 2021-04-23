using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdResourceBoxChildReady, false)]
	public class CmdResourceBoxChildReady : LocalResourceItemBoxCommandPacket
	{
		
		public override void Action(ClientService service, LocalResourceItemBox self)
		{
			self.ReadyChildObject();
		}
	}
}