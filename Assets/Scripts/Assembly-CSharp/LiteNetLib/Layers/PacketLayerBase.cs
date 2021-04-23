namespace LiteNetLib.Layers
{
	
	public abstract class PacketLayerBase
	{
		
		protected PacketLayerBase(int extraPacketSizeForLayer)
		{
			this.ExtraPacketSizeForLayer = extraPacketSizeForLayer;
		}

		
		public abstract void ProcessInboundPacket(ref byte[] data, ref int length);

		
		public abstract void ProcessOutBoundPacket(ref byte[] data, ref int offset, ref int length);

		
		public readonly int ExtraPacketSizeForLayer;
	}
}
