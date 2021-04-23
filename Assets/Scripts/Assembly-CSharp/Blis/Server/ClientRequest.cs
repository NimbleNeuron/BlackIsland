using Blis.Common;

namespace Blis.Server
{
	
	public class ClientRequest
	{
		
		public ClientRequest(Session session, ClientPacketWrapper packetWrapper)
		{
			this.session = session;
			ClientPacketWrapper = packetWrapper;
		}

		
		
		public Session session { get; }

		
		
		public ClientPacketWrapper ClientPacketWrapper { get; }
	}
}