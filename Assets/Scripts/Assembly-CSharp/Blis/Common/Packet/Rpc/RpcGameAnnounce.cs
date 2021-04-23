using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcGameAnnounce, false)]
	public class RpcGameAnnounce : RpcPacket
	{
		[Key(1)] public byte[] announceInfo;


		[Key(0)] public GameAnnounceType announceType;


		public override void Action(ClientService clientService)
		{
			GameAnnounce.Create(announceType, announceInfo).ShowAnnounce();
		}
	}
}