using System;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqAskGameSetup, false)]
	public class ReqAskGameSetup : ReqPacketForResponse
	{
		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			double totalSeconds = (DateTime.Now - gameService.StartCondition.SetupGameStartTime).TotalSeconds;
			return new ResAskGameSetup
			{
				readyToStart = gameService.GameStatus == GameStatus.ReadyToStart,
				standByTime = (int) totalSeconds
			};
		}
	}
}