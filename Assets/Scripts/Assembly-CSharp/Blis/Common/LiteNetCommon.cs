using LiteNetLib;

namespace Blis.Common
{
	public class LiteNetCommon
	{
		public static DeliveryMethod NetChannelToDeliveryMethod(NetChannel netChannel)
		{
			switch (netChannel)
			{
				default:
					return DeliveryMethod.Unreliable;
				case NetChannel.ReliableUnordered:
					return DeliveryMethod.ReliableUnordered;
				case NetChannel.ReliableOrdered:
					return DeliveryMethod.ReliableOrdered;
			}
		}
	}
}