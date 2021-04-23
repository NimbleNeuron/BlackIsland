using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqMoveTo, false)]
	public class ReqMoveTo : ReqPacket
	{
		[Key(0)] public Vector3 destination;


		[Key(1)] public bool findAttackTarget;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (playerSession.Character.IsRest)
			{
				return;
			}

			if (playerSession.Character.IsRunningCastingAction())
			{
				playerSession.Character.CancelActionCasting(CastingCancelType.Action);
			}

			playerSession.Character.Controller.MoveTo(destination, findAttackTarget);
		}
	}
}