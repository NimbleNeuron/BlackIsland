using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqSurrenderGame, false)]
	public class ReqSurrenderGame : ReqPacket
	{
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession) { }


		public override void Action(GameServer gameServer, GameService gameService, ObserverSession observerSession) { }
	}
}