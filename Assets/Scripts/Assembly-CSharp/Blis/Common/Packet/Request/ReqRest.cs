using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqRest, false)]
	public class ReqRest : ReqPacket
	{
		[Key(0)] public bool rest;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.IsRest == rest)
			{
				return;
			}

			if (rest && playerSession.Character.IsInCombat)
			{
				return;
			}

			if (!playerSession.Character.CanAnyAction(ActionType.ToRest))
			{
				return;
			}

			playerSession.Character.StartActionCasting(ActionCostData.GetActionCostType(rest), true, null,
				delegate { playerSession.Character.Rest(rest, !rest); });
		}
	}
}