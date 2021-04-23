namespace LiteNetLib
{
	
	public class TooBigPacketException : InvalidPacketException
	{
		
		public TooBigPacketException(string message) : base(message) { }
	}
}