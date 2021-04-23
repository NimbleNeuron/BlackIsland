using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdDestroy, false)]
	public class CmdDestroy : LocalObjectCommandPacket
	{
		public override void Action(ClientService service, LocalObject self)
		{
			self.DestroySelf();
			service.World.DestroyObject(self);
		}
	}
}