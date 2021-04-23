using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResOpenCorpse, false)]
	public class ResOpenCorpse : ResPacket
	{
		[Key(2)] public List<Item> items;


		[Key(1)] public bool success;
	}
}