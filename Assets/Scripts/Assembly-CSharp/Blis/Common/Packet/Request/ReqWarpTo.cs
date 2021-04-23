using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqWarpTo, false)]
	public class ReqWarpTo : ReqPacket
	{
		[Key(0)] public Vector3 destination;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession) { }
	}
}