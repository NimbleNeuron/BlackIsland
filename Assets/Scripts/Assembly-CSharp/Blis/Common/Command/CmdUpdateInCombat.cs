using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateInCombat, false)]
	public class CmdUpdateInCombat : LocalCharacterCommandPacket
	{
		[Key(1)] public bool isCombat;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.SetIsInCombat(isCombat);
		}
	}
}