using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdInBush, false)]
	public class CmdInBush : LocalCharacterCommandPacket
	{
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.InBush();
		}
	}
}