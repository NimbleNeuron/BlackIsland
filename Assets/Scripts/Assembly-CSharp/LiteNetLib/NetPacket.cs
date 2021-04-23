using System;
using LiteNetLib.Utils;

namespace LiteNetLib
{
	
	internal sealed class NetPacket
	{
		
		
		
		public PacketProperty Property
		{
			get
			{
				return (PacketProperty)(this.RawData[0] & 31);
			}
			set
			{
				// co: dotPeek
				this.RawData[0] = (byte) ((PacketProperty) ((int) this.RawData[0] & 224) | value);
			}
		}

		
		
		
		public byte ConnectionNumber
		{
			get
			{
				return (byte)((this.RawData[0] & 96) >> 5);
			}
			set
			{
				this.RawData[0] = (byte)((int)(this.RawData[0] & 159) | (int)value << 5);
			}
		}

		
		
		
		public ushort Sequence
		{
			get
			{
				return BitConverter.ToUInt16(this.RawData, 1);
			}
			set
			{
				FastBitConverter.GetBytes(this.RawData, 1, value);
			}
		}

		
		
		public bool IsFragmented
		{
			get
			{
				return (this.RawData[0] & 128) > 0;
			}
		}

		
		public void MarkFragmented()
		{
			byte[] rawData = this.RawData;
			int num = 0;
			rawData[num] |= 128;
		}

		
		
		
		public byte ChannelId
		{
			get
			{
				return this.RawData[3];
			}
			set
			{
				this.RawData[3] = value;
			}
		}

		
		
		
		public ushort FragmentId
		{
			get
			{
				return BitConverter.ToUInt16(this.RawData, 4);
			}
			set
			{
				FastBitConverter.GetBytes(this.RawData, 4, value);
			}
		}

		
		
		
		public ushort FragmentPart
		{
			get
			{
				return BitConverter.ToUInt16(this.RawData, 6);
			}
			set
			{
				FastBitConverter.GetBytes(this.RawData, 6, value);
			}
		}

		
		
		
		public ushort FragmentsTotal
		{
			get
			{
				return BitConverter.ToUInt16(this.RawData, 8);
			}
			set
			{
				FastBitConverter.GetBytes(this.RawData, 8, value);
			}
		}

		
		public NetPacket(int size)
		{
			this.RawData = new byte[size];
			this.Size = size;
		}

		
		public NetPacket(PacketProperty property, int size)
		{
			size += NetPacket.GetHeaderSize(property);
			this.RawData = new byte[size];
			this.Property = property;
			this.Size = size;
		}

		
		public static int GetHeaderSize(PacketProperty property)
		{
			switch (property)
			{
			case PacketProperty.Channeled:
			case PacketProperty.Ack:
				return 4;
			case PacketProperty.Ping:
				return 3;
			case PacketProperty.Pong:
				return 11;
			case PacketProperty.ConnectRequest:
				return 14;
			case PacketProperty.ConnectAccept:
				return 11;
			case PacketProperty.Disconnect:
				return 9;
			default:
				return 1;
			}
		}

		
		public int GetHeaderSize()
		{
			return NetPacket.GetHeaderSize(this.Property);
		}

		
		public bool FromBytes(byte[] data, int start, int packetSize)
		{
			int num = (int) (byte) ((uint) data[start] & 31U);
			bool flag = ((uint) data[start] & 128U) > 0U;
			int headerSize = NetPacket.GetHeaderSize((PacketProperty) num);
			if (num > NetPacket.LastProperty || packetSize < headerSize || flag && packetSize < headerSize + 6 || data.Length < start + packetSize)
				return false;
			Buffer.BlockCopy((Array) data, start, (Array) this.RawData, 0, packetSize);
			this.Size = (int) (ushort) packetSize;
			return true;
			
			// co: dotPeek
			// byte b = data[start] & 31;
			// bool flag = (data[start] & 128) > 0;
			// int headerSize = NetPacket.GetHeaderSize((PacketProperty)b);
			// if ((int)b > NetPacket.LastProperty || packetSize < headerSize || (flag && packetSize < headerSize + 6) || data.Length < start + packetSize)
			// {
			// 	return false;
			// }
			// Buffer.BlockCopy(data, start, this.RawData, 0, packetSize);
			// this.Size = (int)((ushort)packetSize);
			// return true;
		}

		
		private static readonly int LastProperty = Enum.GetValues(typeof(PacketProperty)).Length;

		
		public byte[] RawData;

		
		public int Size;

		
		public object UserData;
	}
}
