using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHealHpStateCode, false)]
	public class CmdHealHpStateCode : LocalCharacterCommandPacket
	{
		[Key(1)] public int addHp;


		[Key(3)] public bool showUI;


		[Key(2)] public int stateCode;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnRegenHeal(addHp, 0, stateCode, 0, showUI);
		}
	}
}