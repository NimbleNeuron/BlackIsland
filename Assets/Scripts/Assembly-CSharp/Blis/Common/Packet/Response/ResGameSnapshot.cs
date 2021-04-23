using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResGameSnapshot, false)]
	public class ResGameSnapshot : ResPacket
	{
		[Key(2)] public int currentSeq;


		[Key(1)] public float frameUpdateRate;


		[Key(3)] public GameSnapshot gameSnapshot;


		[Key(6)] public HashSet<int> invisibleCharacterIds;


		[Key(5)] public HashSet<int> outSightCharacterIds;


		[Key(4)] public UserSnapshot userSnapshot;
	}
}