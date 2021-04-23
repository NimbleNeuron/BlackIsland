using System;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.Handshake, true)]
	public class Handshake : ReqPacketForResponse
	{
		[Key(2)] public string battleTokenKey;


		[Key(3)] public bool isReconnect;


		[Key(1)] public long userId;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			throw new NotImplementedException();
		}


		public override ResPacket Action(GameServer gameServer, GameService gameService,
			ObserverSession observerSession)
		{
			throw new NotImplementedException();
		}
	}
}