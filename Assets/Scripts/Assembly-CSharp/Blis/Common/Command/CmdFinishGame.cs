using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdFinishGame, false)]
	public class CmdFinishGame : CommandPacket
	{
		[Key(4)] public int attackerDataCode;


		[Key(5)] public string attackerNickName;


		[Key(3)] public int attackerObjectId;


		[Key(2)] public ObjectType attackerObjectType;


		[Key(6)] public string attackerTempNickname;


		[Key(0)] public bool finishGame;


		[Key(7)] public Dictionary<long, NicknamePair> nicknamePairDic;


		[Key(1)] public int rank;


		public override void Action(ClientService clientService)
		{
			clientService.NickNamePairDic(nicknamePairDic);
			clientService.EndGame(finishGame, rank, attackerObjectType, attackerObjectId, attackerDataCode,
				attackerNickName, attackerTempNickname);
		}
	}
}