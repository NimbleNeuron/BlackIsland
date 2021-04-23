using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdBlock, false)]
	public class CmdBlock : LocalCharacterCommandPacket
	{
		[Key(1)] public int damage;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnBlock(damage);
		}
	}
}