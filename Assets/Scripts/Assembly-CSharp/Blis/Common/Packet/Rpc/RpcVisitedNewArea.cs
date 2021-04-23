using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcVisitedNewArea, false)]
	public class RpcVisitedNewArea : RpcPacket
	{
		[Key(0)] public int VisitedNewAreaMask;


		public override void Action(ClientService clientService)
		{
			clientService.MyPlayer.AddVisitedArea(VisitedNewAreaMask);
		}
	}
}