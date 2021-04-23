using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHeal, false)]
	public class CmdHeal : LocalCharacterCommandPacket
	{
		[Key(1)] public int addHp;


		[Key(2)] public int addSp;


		[Key(3)] public bool showUI;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnHeal(addHp, addSp, 0, showUI);
		}
	}
}