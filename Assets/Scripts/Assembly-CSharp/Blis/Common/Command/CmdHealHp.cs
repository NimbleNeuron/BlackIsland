using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHealHp, false)]
	public class CmdHealHp : LocalCharacterCommandPacket
	{
		[Key(1)] public int addHp;


		[Key(2)] public bool showUI;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnHeal(addHp, 0, 0, showUI);
		}
	}
}