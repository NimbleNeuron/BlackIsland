using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blis.Common
{
	internal class PacketSerializer : Singleton<PacketSerializer>
	{
		private readonly Dictionary<Type, PacketAttr> attributeMap = new Dictionary<Type, PacketAttr>();


		private readonly Dictionary<PacketType, IHandler> handlerMap;


		private readonly ISerializer serializer = Serializer.Default;


		public PacketSerializer()
		{
			handlerMap =
				new Dictionary<PacketType, IHandler>(SingletonComparerEnum<PacketTypeComparer, PacketType>.Instance);
			foreach (Type key in Assembly.GetAssembly(typeof(IPacket)).GetTypes()
				.Where<Type>(x => x.GetInterfaces().Contains<Type>(typeof(IPacket))))
			{
				IEnumerable<PacketAttr> customAttributes = key.GetCustomAttributes<PacketAttr>(true);
				if (customAttributes.Any<PacketAttr>())
				{
					foreach (PacketAttr packetAttr in customAttributes)
					{
						attributeMap.Add(key, packetAttr);
					}

					IHandler instance = (IHandler) Activator.CreateInstance(typeof(Handler<>).MakeGenericType(key));
					handlerMap.Add(customAttributes.First<PacketAttr>().packetType, instance);
				}
			}

			// co: dotPeek
			// this.handlerMap = new Dictionary<PacketType, PacketSerializer.IHandler>(SingletonComparerEnum<PacketTypeComparer, PacketType>.Instance);
			// foreach (Type type in from x in Assembly.GetAssembly(typeof(IPacket)).GetTypes()
			// where x.GetInterfaces().Contains(typeof(IPacket))
			// select x)
			// {
			// 	IEnumerable<PacketAttr> customAttributes = type.GetCustomAttributes(true);
			// 	if (customAttributes.Any<PacketAttr>())
			// 	{
			// 		foreach (PacketAttr value in customAttributes)
			// 		{
			// 			this.attributeMap.Add(type, value);
			// 		}
			// 		PacketSerializer.IHandler value2 = (PacketSerializer.IHandler)Activator.CreateInstance(typeof(PacketSerializer.Handler<>).MakeGenericType(new Type[]
			// 		{
			// 			type
			// 		}));
			// 		this.handlerMap.Add(customAttributes.First<PacketAttr>().packetType, value2);
			// 	}
			// }
		}


		public byte[] Serialize(IPacket packet, out PacketType packetType)
		{
			packetType = attributeMap[packet.GetType()].packetType;
			IHandler handler;
			if (handlerMap.TryGetValue(packetType, out handler))
			{
				return handler.Serialize(serializer, packet);
			}

			throw new Exception();
		}


		public IPacket Deserialize(PacketType packetType, byte[] data)
		{
			IHandler handler = null;
			if (handlerMap.TryGetValue(packetType, out handler))
			{
				return handler.Deserialize(serializer, data);
			}

			throw new Exception();
		}


		private interface IHandler
		{
			byte[] Serialize(ISerializer serializer, IPacket packet);


			IPacket Deserialize(ISerializer serializer, byte[] data);
		}


		private class Handler<T> : IHandler where T : IPacket
		{
			public byte[] Serialize(ISerializer serializer, IPacket packet)
			{
				return serializer.Serialize<T>((T) packet);
			}


			public IPacket Deserialize(ISerializer serializer, byte[] data)
			{
				return serializer.Deserialize<T>(data);
			}
		}
	}
}