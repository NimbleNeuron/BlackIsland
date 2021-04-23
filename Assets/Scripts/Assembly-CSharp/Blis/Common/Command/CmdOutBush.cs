using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdOutBush, false)]
	public class CmdOutBush : LocalCharacterCommandPacket
	{
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OutBush();
		}
	}
}