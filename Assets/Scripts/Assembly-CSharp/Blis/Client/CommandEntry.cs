using System.Collections.Generic;
using Blis.Common;
using UnityEngine;

namespace Blis.Client
{
	public class CommandEntry
	{
		public List<PacketWrapper> commands;


		public float recvTime;


		public int seq;


		public CommandEntry(int seq, List<PacketWrapper> commands)
		{
			this.seq = seq;
			recvTime = Time.realtimeSinceStartup;
			this.commands = commands;
		}
	}
}