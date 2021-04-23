using Blis.Client;

namespace Blis.Common
{
	[PacketAttr(PacketType.CmdOnVisible, false)]
	public class CmdOnVisible : LocalCharacterCommandPacket
	{
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnVisible();
		}
	}
}