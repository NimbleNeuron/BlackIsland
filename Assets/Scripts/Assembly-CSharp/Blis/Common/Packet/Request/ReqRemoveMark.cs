using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqRemoveMark, false)]
	public class ReqRemoveMark : ReqPacket
	{
		[Key(2)] public int teamSlot;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.Character.SendRemoveMarkTarget(teamSlot);
		}
	}
}