using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResNicknamePair, false)]
	public class ResNicknamePair : ResPacket
	{
		[Key(1)] public Dictionary<long, NicknamePair> nicknamePairDic;
	}
}