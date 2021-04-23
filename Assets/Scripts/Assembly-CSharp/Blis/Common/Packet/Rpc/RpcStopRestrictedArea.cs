using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcStopRestrictedArea, false)]
	public class RpcStopRestrictedArea : RpcPacket
	{
		
		public override void Action(ClientService clientService)
		{
			clientService.ToggleStopRestriction();
		}
	}
}