using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdHealSpEffectCode, false)]
	public class CmdHealSpEffectCode : LocalCharacterCommandPacket
	{
		
		[Key(1)] public int addSp;

		
		[Key(2)] public int effectCode;

		
		[Key(3)] public bool showUI;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnHeal(0, addSp, effectCode, showUI);
		}
	}
}