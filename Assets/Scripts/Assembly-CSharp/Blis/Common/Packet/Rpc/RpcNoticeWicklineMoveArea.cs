using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcNoticeWicklineMoveArea, false)]
	public class RpcNoticeWicklineMoveArea : RpcPacket
	{
		
		[Key(0)] public int areaCode;

		
		public override void Action(ClientService clientService)
		{
			clientService.WicklineMoveArea(areaCode);
		}
	}
}