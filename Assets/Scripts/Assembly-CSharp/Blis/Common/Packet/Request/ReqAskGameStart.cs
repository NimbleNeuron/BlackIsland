using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqAskGameStart, false)]
	public class ReqAskGameStart : ReqPacketForResponse
	{
		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			return new ResAskGameStart
			{
				gameStarted = gameService.GameStatus == GameStatus.Started
			};
		}
	}
}