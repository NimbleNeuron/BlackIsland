using System.Collections.Generic;
using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqMissingCommand, true)]
	public class ReqMissingCommand : ReqPacketForResponse
	{
		[Key(1)] public List<int> seqs;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			return new ResMissingCommand
			{
				commandLists = playerSession.GetMissingCommands(seqs)
			};
		}


		public override ResPacket Action(GameServer gameServer, GameService gameService,
			ObserverSession observerSession)
		{
			return new ResMissingCommand
			{
				commandLists = observerSession.GetMissingCommands(seqs)
			};
		}
	}
}