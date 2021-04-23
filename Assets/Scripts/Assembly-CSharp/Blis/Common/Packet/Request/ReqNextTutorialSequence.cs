using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqNextTutorialSequence, false)]
	public class ReqNextTutorialSequence : ReqPacket
	{
		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			gameService.Tutorial.Next();
		}
	}
}