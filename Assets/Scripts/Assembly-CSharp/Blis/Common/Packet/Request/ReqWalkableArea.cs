using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqWalkableArea, false)]
	public class ReqWalkableArea : ReqPacketForResponse
	{
		[Key(1)] public int[] areaCodes;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			playerSession.Character.MoveAgent.AddWalkableAreas(areaCodes);
			return new ResWalkableArea
			{
				walkableNavMask = playerSession.Character.WalkableNavMask
			};
		}
	}
}