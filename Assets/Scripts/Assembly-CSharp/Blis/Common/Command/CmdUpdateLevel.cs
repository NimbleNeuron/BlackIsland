using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateLevel, false)]
	public class CmdUpdateLevel : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public int level;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.OnUpdateLevel(level);
		}
	}
}