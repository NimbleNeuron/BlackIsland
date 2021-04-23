using System;
using MessagePack;

namespace Blis.Common
{
	[Union(0, typeof(ClientPacketWrapper))]
	[Union(1, typeof(ServerPacketWrapper))]
	[MessagePackObject]
	public class PacketWrapper
	{
		[IgnoreMember] protected const int LAST_KEY_IDX = 1;


		[IgnoreMember] public const string GetPacketMethodName = "GetPacket";


		[IgnoreMember] private IPacket cached;


		[Key(1)] public byte[] data;


		[Key(0)] public PacketType packetType;


		[IgnoreMember] private ISerializer serializer = Serializer.Default;

		public PacketWrapper() { }


		public PacketWrapper(IPacket packet)
		{
			data = Singleton<PacketSerializer>.inst.Serialize(packet, out packetType);
		}


		public int Size()
		{
			return data.Length;
		}


		public T GetPacket<T>()
		{
			if (cached == null)
			{
				cached = Singleton<PacketSerializer>.inst.Deserialize(packetType, data);
			}

			return (T) cached;
		}


		public byte[] Serialize<T>() where T : PacketWrapper
		{
			return Serializer.Compression.Serialize<T>((T) this);
		}


		public bool IsType(Type type)
		{
			Type type2 = GetPacket<IPacket>().GetType();
			return type2 == type || type2.IsSubclassOf(type);
		}
	}
}