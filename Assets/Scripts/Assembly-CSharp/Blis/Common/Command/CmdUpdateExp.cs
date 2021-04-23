using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateExp, false)]
	public class CmdUpdateExp : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public int exp;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.OnUpdateExp(exp);
		}
	}
}