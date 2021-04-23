using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdHealHpEffectCode, false)]
	public class CmdHealHpEffectCode : LocalCharacterCommandPacket
	{
		
		[Key(1)] public int addHp;

		
		[Key(2)] public int effectCode;

		
		[Key(3)] public bool showUI;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnHeal(addHp, 0, effectCode, showUI);
		}
	}
}