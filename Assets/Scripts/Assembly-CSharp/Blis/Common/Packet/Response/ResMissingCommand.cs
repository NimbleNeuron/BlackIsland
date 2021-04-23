using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ResMissingCommand, false)]
	public class ResMissingCommand : ResPacket
	{
		[Key(1)] public List<CommandList> commandLists;


		public override NetChannel ResponseChannel()
		{
			return NetChannel.ReliableUnordered;
		}
	}
}