using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqCheat, false)]
	public class ReqCheat : ReqPacket
	{
		[Key(0)] public string cheatCommand;


		[Key(1)] public Vector3 position;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession) { }


		public override void Action(GameServer gameServer, GameService gameService, ObserverSession observerSession) { }
	}
}