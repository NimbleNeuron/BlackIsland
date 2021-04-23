using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqPing, false)]
	public class ReqPingTarget : ReqPacket
	{
		[Key(2)] public PingType pingType;


		[Key(1)] public int targetObjectId;


		[Key(0)] public Vector3 targetPosition;


		[Key(3)] public int teamSlot;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldObject targetObject = null;
			gameService.World.TryFind<WorldObject>(targetObjectId, ref targetObject);
			playerSession.Character.SendPingTarget(pingType, targetPosition, targetObject, teamSlot);
		}
	}
}