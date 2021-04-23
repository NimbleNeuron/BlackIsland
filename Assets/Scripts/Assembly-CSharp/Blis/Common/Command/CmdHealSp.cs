using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHealSp, false)]
	public class CmdHealSp : LocalCharacterCommandPacket
	{
		[Key(1)] public int addSp;


		[Key(2)] public bool showUI;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnHeal(0, addSp, 0, showUI);
		}
	}
}