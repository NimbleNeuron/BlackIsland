using System.Collections.Generic;
using Blis.Common;

namespace Blis.Server
{
	
	public class CommandPack
	{
		
		public CommandPack(int seq, List<PacketWrapper> commands)
		{
			this.seq = seq;
			this.commands = new List<PacketWrapper>(commands);
		}

		
		public readonly int seq;

		
		public readonly List<PacketWrapper> commands;
	}
}
