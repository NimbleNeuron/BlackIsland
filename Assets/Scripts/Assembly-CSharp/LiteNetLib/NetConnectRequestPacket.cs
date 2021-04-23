using System;
using System.Net;
using LiteNetLib.Utils;

namespace LiteNetLib
{
	
	internal sealed class NetConnectRequestPacket
	{
		
		public const int HeaderSize = 14;

		
		public readonly byte ConnectionNumber;

		
		public readonly long ConnectionTime;

		
		public readonly NetDataReader Data;

		
		public readonly byte[] TargetAddress;

		
		private NetConnectRequestPacket(long connectionTime, byte connectionNumber, byte[] targetAddress,
			NetDataReader data)
		{
			ConnectionTime = connectionTime;
			ConnectionNumber = connectionNumber;
			TargetAddress = targetAddress;
			Data = data;
		}

		
		public static int GetProtocolId(NetPacket packet)
		{
			return BitConverter.ToInt32(packet.RawData, 1);
		}

		
		public static NetConnectRequestPacket FromData(NetPacket packet)
		{
			if (packet.ConnectionNumber >= 4)
			{
				return null;
			}

			long connectionTime = BitConverter.ToInt64(packet.RawData, 5);
			int num = packet.RawData[13];
			if (num != 16 && num != 28)
			{
				return null;
			}

			byte[] array = new byte[num];
			Buffer.BlockCopy(packet.RawData, 14, array, 0, num);
			NetDataReader netDataReader = new NetDataReader(null, 0, 0);
			if (packet.Size > 14 + num)
			{
				netDataReader.SetSource(packet.RawData, 14 + num, packet.Size);
			}

			return new NetConnectRequestPacket(connectionTime, packet.ConnectionNumber, array, netDataReader);
		}

		
		public static NetPacket Make(NetDataWriter connectData, SocketAddress addressBytes, long connectId)
		{
			NetPacket netPacket = new NetPacket(PacketProperty.ConnectRequest, connectData.Length + addressBytes.Size);
			FastBitConverter.GetBytes(netPacket.RawData, 1, 11);
			FastBitConverter.GetBytes(netPacket.RawData, 5, connectId);
			netPacket.RawData[13] = (byte) addressBytes.Size;
			for (int i = 0; i < addressBytes.Size; i++)
			{
				netPacket.RawData[14 + i] = addressBytes[i];
			}

			Buffer.BlockCopy(connectData.Data, 0, netPacket.RawData, 14 + addressBytes.Size, connectData.Length);
			return netPacket;
		}
	}
}