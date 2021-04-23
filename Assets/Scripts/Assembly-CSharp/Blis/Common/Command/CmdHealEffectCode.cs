using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHealEffectCode, false)]
	public class CmdHealEffectCode : LocalCharacterCommandPacket
	{
		[Key(1)] public int addHp;


		[Key(2)] public int addSp;


		[Key(3)] public int effectCode;


		[Key(4)] public bool showUI;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnHeal(addHp, addSp, effectCode, showUI);
		}
	}
}