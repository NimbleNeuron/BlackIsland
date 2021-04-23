using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdEvasion, false)]
	public class CmdEvasion : LocalCharacterCommandPacket
	{
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnEvasion();
		}
	}
}