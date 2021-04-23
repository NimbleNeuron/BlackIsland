using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdHealHpEffectAndStateCode, false)]
	public class CmdHealHpEffectAndStateCode : LocalCharacterCommandPacket
	{
		
		[Key(1)] public int addHp;

		
		[Key(3)] public int effectCode;

		
		[Key(4)] public bool showUI;

		
		[Key(2)] public int stateCode;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnRegenHeal(addHp, 0, stateCode, effectCode, showUI);
		}
	}
}