using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqHold, false)]
	public class ReqHold : ReqPacket
	{
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.IsRest)
			{
				return;
			}

			if (!playerSession.Character.CanMove())
			{
				return;
			}

			if (!playerSession.Character.CanStop())
			{
				return;
			}

			playerSession.Character.Controller.Hold();
		}
	}
}