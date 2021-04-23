using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqCreateDummy, false)]
	public class ReqCreateDummy : ReqPacket
	{
		[Key(1)] public ObjectType objectType;


		[Key(0)] public Vector3 position;


		[Key(2)] public int prefabNo;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession) { }


		public override void Action(GameServer gameServer, GameService gameService, ObserverSession observerSession) { }
	}
}