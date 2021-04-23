using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqCloseBox, true)]
	public class ReqCloseBox : ReqPacketForResponse
	{
		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.OpenBoxId != 0)
			{
				WorldObject worldObject = null;
				if (gameService.World.TryFind<WorldObject>(playerSession.OpenBoxId, ref worldObject))
				{
					worldObject.ItemBox.Close(playerSession);
					Singleton<ItemRoutingRecoder>.inst.CloseBox(playerSession.userId, worldObject.ObjectId);
				}
				else
				{
					playerSession.SetOpenBoxId(0);
				}
			}

			return new ResSuccess();
		}
	}
}