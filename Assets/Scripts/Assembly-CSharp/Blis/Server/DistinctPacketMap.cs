using System;
using System.Collections.Generic;
using System.Linq;
using Blis.Common;

namespace Blis.Server
{
	
	public class DistinctPacketMap
	{
		
		public void Set(CommandPacket packet)
		{
			this.packetMap.Add(packet.GetType(), packet);
		}

		
		public T Get<T>() where T : CommandPacket, new()
		{
			CommandPacket commandPacket = null;
			if (!this.packetMap.TryGetValue(typeof(T), out commandPacket))
			{
				commandPacket = Activator.CreateInstance<T>();
				this.packetMap.Add(typeof(T), commandPacket);
				return commandPacket as T;
			}
			if (!(commandPacket is T))
			{
				throw new InvalidCastException();
			}
			return commandPacket as T;
		}

		
		public List<CommandPacket> Flush()
		{
			List<CommandPacket> result = this.packetMap.Values.ToList<CommandPacket>();
			this.packetMap.Clear();
			return result;
		}

		
		private readonly Dictionary<Type, CommandPacket> packetMap = new Dictionary<Type, CommandPacket>();
	}
}
