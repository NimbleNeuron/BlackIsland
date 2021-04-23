using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHealStateCode, false)]
	public class CmdHealStateCode : LocalCharacterCommandPacket
	{
		[Key(1)] public int addHp;


		[Key(2)] public int addSp;


		[Key(4)] public bool showUI;


		[Key(3)] public int stateCode;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnRegenHeal(addHp, addSp, stateCode, 0, showUI);
		}
	}
}