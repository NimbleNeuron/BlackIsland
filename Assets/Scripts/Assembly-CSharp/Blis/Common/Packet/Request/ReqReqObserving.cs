using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqObserving, false)]
	public class ReqReqObserving : ReqPacket
	{
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.SettingObserving();
			playerSession.BroadcastTeamMember(new RpcObserving
			{
				objectId = playerSession.Character.ObjectId
			});
		}
	}
}