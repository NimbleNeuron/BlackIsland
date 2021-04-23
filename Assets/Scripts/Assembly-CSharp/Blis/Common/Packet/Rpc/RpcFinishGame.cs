using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcFinishGame, false)]
	public class RpcFinishGame : RpcPacket
	{
		[Key(0)] public Dictionary<long, NicknamePair> nicknamePairDic;


		public override void Action(ClientService clientService)
		{
			clientService.NickNamePairDic(nicknamePairDic);
			clientService.StopGame();
		}
	}
}