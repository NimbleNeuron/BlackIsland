using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdResourceBoxChildActive, false)]
	public class CmdResourceBoxChildActive : LocalResourceItemBoxCommandPacket
	{
		
		public override void Action(ClientService service, LocalResourceItemBox self)
		{
			self.ActiveChildObject(true);
		}
	}
}