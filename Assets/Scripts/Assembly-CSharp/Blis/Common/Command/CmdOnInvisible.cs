using Blis.Client;

namespace Blis.Common
{
	[PacketAttr(PacketType.CmdOnInvisible, false)]
	public class CmdOnInvisible : LocalCharacterCommandPacket
	{
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnInvisible();
		}
	}
}