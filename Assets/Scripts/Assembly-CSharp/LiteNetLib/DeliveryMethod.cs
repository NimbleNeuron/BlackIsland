namespace LiteNetLib
{
	
	public enum DeliveryMethod : byte
	{
		
		Unreliable = 4,

		
		ReliableUnordered = 0,

		
		Sequenced,

		
		ReliableOrdered,

		
		ReliableSequenced
	}
}