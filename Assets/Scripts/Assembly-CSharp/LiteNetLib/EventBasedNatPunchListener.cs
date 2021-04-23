using System.Net;

namespace LiteNetLib
{
	
	public class EventBasedNatPunchListener : INatPunchListener
	{
		
		public delegate void OnNatIntroductionRequest(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint,
			string token);

		
		public delegate void OnNatIntroductionSuccess(IPEndPoint targetEndPoint, NatAddressType type, string token);

		
		void INatPunchListener.OnNatIntroductionRequest(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint,
			string token)
		{
			if (NatIntroductionRequest != null)
			{
				NatIntroductionRequest(localEndPoint, remoteEndPoint, token);
			}
		}

		
		void INatPunchListener.OnNatIntroductionSuccess(IPEndPoint targetEndPoint, NatAddressType type, string token)
		{
			if (NatIntroductionSuccess != null)
			{
				NatIntroductionSuccess(targetEndPoint, type, token);
			}
		}

		
		
		
		public event OnNatIntroductionRequest NatIntroductionRequest;

		
		
		
		public event OnNatIntroductionSuccess NatIntroductionSuccess;
	}
}