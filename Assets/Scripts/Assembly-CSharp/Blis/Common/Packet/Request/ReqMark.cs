using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqMark, false)]
	public class ReqMark : ReqPacket
	{
		[Key(1)] public int targetObjectId;


		[Key(0)] public Vector3 targetPosition;


		[Key(2)] public int teamSlot;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldObject targetObject = null;
			gameService.World.TryFind<WorldObject>(targetObjectId, ref targetObject);
			playerSession.Character.SendMarkTarget(targetPosition, targetObject, teamSlot);
		}
	}
}