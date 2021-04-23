using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcObserving, true)]
	public class RpcObserving : RpcPacket
	{
		[Key(0)] public int objectId;


		public override void Action(ClientService service)
		{
			service.World.Find<LocalPlayerCharacter>(objectId).Observing();
		}
	}
}