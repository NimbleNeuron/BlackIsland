using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHealSpStateCode, false)]
	public class CmdHealSpStateCode : LocalCharacterCommandPacket
	{
		[Key(1)] public int addSp;


		[Key(3)] public bool showUI;


		[Key(2)] public int stateCode;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnRegenHeal(0, addSp, stateCode, 0, showUI);
		}
	}
}