using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqUpdateStrategySheet, true)]
	public class ReqUpdateStrategySheet : ReqPacket
	{
		
		[Key(0)] public int areaCode;

		
		[Key(1)] public bool updateUI;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.StartingSettings.SetStartingArea(areaCode);
			if (updateUI)
			{
				gameServer.Broadcast(new RpcUpdateStrategy
				{
					userId = playerSession.userId,
					teamNumber = playerSession.TeamNumber,
					startingAreaCode = playerSession.StartingSettings.StartingAreaCode
				});
			}
		}
	}
}