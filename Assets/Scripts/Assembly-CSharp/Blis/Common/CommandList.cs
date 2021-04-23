using System.Collections.Generic;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CommandList, false)]
	public class CommandList : IPacket
	{
		[IgnoreMember] private readonly int COMMAND_LIST_SIZE = 4;


		[Key(1)] public List<PacketWrapper>[] commands;


		[Key(0)] public int seq;


		public CommandList(int seq)
		{
			this.seq = seq;
			commands = new List<PacketWrapper>[3];
		}


		public CommandList(int seq, int size)
		{
			this.seq = seq;
			commands = new List<PacketWrapper>[size];
		}


		public void Add(List<PacketWrapper> commandList, int index)
		{
			commands[index] = commandList;
		}

		private void Ref()
		{
			Reference.Use(COMMAND_LIST_SIZE);
		}
	}
}