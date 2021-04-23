using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqNicknamePair, false)]
	public class ReqNicknamePair : ReqPacketForResponse
	{
		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			return new ResNicknamePair
			{
				nicknamePairDic = gameService.TempNicknameDic
			};
		}
	}
}