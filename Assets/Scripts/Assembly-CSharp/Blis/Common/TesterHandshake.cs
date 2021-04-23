using System;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.TesterHandshake, true)]
	public class TesterHandshake : ReqPacketForResponse
	{
		[Key(2)] public BattleToken battleToken;


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