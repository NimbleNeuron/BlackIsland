using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqGunReload, false)]
	public class ReqGunReload : ReqPacket
	{
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.Character.GunReload(true);
		}
	}
}