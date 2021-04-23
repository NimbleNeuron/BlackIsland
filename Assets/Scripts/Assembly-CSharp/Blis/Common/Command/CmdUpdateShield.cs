using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateShield, false)]
	public class CmdUpdateShield : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public int shield;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.UpdateShield(shield);
		}
	}
}