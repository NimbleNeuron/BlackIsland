using System;
using LiteNetLib.Utils;

namespace LiteNetLib
{
	
	internal sealed class NetConnectAcceptPacket
	{
		
		public const int Size = 11;

		
		public readonly long ConnectionId;

		
		public readonly byte ConnectionNumber;

		
		public readonly bool IsReusedPeer;

		
		private NetConnectAcceptPacket(long connectionId, byte connectionNumber, bool isReusedPeer)
		{
			ConnectionId = connectionId;
			ConnectionNumber = connectionNumber;
			IsReusedPeer = isReusedPeer;
		}

		
		public static NetConnectAcceptPacket FromData(NetPacket packet)
		{
			if (packet.Size > 11)
			{
				return null;
			}

			long connectionId = BitConverter.ToInt64(packet.RawData, 1);
			byte b = packet.RawData[9];
			if (b >= 4)
			{
				return null;
			}

			byte b2 = packet.RawData[10];
			if (b2 > 1)
			{
				return null;
			}

			return new NetConnectAcceptPacket(connectionId, b, b2 == 1);
		}

		
		public static NetPacket Make(long connectId, byte connectNum, bool reusedPeer)
		{
			NetPacket netPacket = new NetPacket(PacketProperty.ConnectAccept, 0);
			FastBitConverter.GetBytes(netPacket.RawData, 1, connectId);
			netPacket.RawData[9] = connectNum;
			// co: dotPeek
			netPacket.RawData[10] = reusedPeer ? (byte) 1 : (byte) 0;
			return netPacket;
		}
	}
}