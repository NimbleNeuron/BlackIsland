using System;

namespace Blis.Common
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public class PacketAttr : Attribute
	{
		public readonly bool immediateResponse;


		public readonly PacketType packetType;

		public PacketAttr(PacketType packetType, bool immediateResponse)
		{
			this.packetType = packetType;
			this.immediateResponse = immediateResponse;
		}
	}
}