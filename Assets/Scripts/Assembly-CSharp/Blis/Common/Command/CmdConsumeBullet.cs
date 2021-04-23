using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateBullet, false)]
	public class CmdConsumeBullet : LocalPlayerCharacterCommandPacket
	{
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.ConsumeBullet();
		}
	}
}