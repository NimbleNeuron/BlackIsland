using System;

namespace Blis.Common
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false)]
	public class PacketHandler : Attribute
	{
		public readonly PacketType packetType;


		public PacketHandler(PacketType packetType)
		{
			this.packetType = packetType;
		}
	}
}