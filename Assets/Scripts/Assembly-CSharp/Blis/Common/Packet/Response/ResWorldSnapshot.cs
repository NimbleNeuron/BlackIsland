using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResWorldSnapshot, false)]
	public class ResWorldSnapshot : ResPacket
	{
		[Key(1)] public int currentSeq;


		[Key(2)] public List<SnapshotWrapper> snapshot;
	}
}