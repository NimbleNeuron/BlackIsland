using System.Net;
using System.Net.Sockets;

namespace LiteNetLib
{
	
	internal interface INetSocketListener
	{
		
		void OnMessageReceived(byte[] data, int length, SocketError errorCode, IPEndPoint remoteEndPoint);
	}
}
