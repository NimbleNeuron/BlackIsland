using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdCancelActionCasting, false)]
	public class CmdCancelActionCasting : LocalPlayerCharacterCommandPacket
	{
		
		[Key(1)] public int extraParam;

		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.OnCancelActionCasting(extraParam);
		}
	}
}