namespace LiteNetLib
{
	
	public interface IDeliveryEventListener
	{
		
		void OnMessageDelivered(NetPeer peer, object userData);
	}
}