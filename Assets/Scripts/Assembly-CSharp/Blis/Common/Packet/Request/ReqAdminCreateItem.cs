using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqAdminCreateItem, false)]
	public class ReqAdminCreateItem : ReqPacket
	{
		[Key(0)] public int itemCode;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession) { }
	}
}