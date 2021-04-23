using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdSetExtraPoint, false)]
	public class CmdSetExtraPoint : LocalCharacterCommandPacket
	{
		[Key(1)] public int setExtraPoint;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnSetExtraPoint(setExtraPoint);
		}
	}
}