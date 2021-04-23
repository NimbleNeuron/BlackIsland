using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdHealEffectAndStateCode, false)]
	public class CmdHealEffectAndStateCode : LocalCharacterCommandPacket
	{
		
		[Key(1)] public int addHp;

		
		[Key(2)] public int addSp;

		
		[Key(4)] public int effectCode;

		
		[Key(5)] public bool showUI;

		
		[Key(3)] public int stateCode;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnRegenHeal(addHp, addSp, stateCode, effectCode, showUI);
		}
	}
}