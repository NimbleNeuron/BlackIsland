using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Blis.Common
{
	
	public class PacketControllerProxy
	{
		
		private readonly Dictionary<PacketType, Type> packetClassMap =
			new Dictionary<PacketType, Type>(SingletonComparerEnum<PacketTypeComparer, PacketType>.Instance);

		
		protected Dictionary<PacketType, ProxyInfo> proxyMap;

		
		public PacketControllerProxy()
		{
			proxyMap = new Dictionary<PacketType, ProxyInfo>(SingletonComparerEnum<PacketTypeComparer, PacketType>
				.Instance);
			InitPacketClassMap();
		}

		
		public void RegisterAllControllers(string targetNamespace)
		{
			foreach (Type type in (from x in typeof(IPacketController).Assembly.GetTypes()
				where x.Namespace.Equals(targetNamespace) && !x.IsAbstract && !x.IsInterface &&
				      x.GetInterfaces().Contains(typeof(IPacketController))
				select x).ToList<Type>())
			{
				IPacketController packetController = (IPacketController) Activator.CreateInstance(type);
				AddController(packetController);
			}
		}

		
		public void AddController(IPacketController packetController)
		{
			foreach (MethodInfo methodInfo in packetController.GetType().GetMethods())
			{
				PacketHandler packetHandler = methodInfo.GetCustomAttribute(typeof(PacketHandler)) as PacketHandler;
				Type packetClassType;
				if (packetHandler != null && packetClassMap.TryGetValue(packetHandler.packetType, out packetClassType))
				{
					proxyMap.Add(packetHandler.packetType,
						new ProxyInfo(packetController, methodInfo, packetClassType));
				}
			}
		}

		
		private void InitPacketClassMap()
		{
			foreach (Type type in (from x in typeof(IPacket).Assembly.GetTypes()
				where !x.IsAbstract && !x.IsInterface && x.GetInterfaces().Contains(typeof(IPacket))
				select x).ToList<Type>())
			{
				PacketAttr packetAttr = type.GetCustomAttribute(typeof(PacketAttr)) as PacketAttr;
				if (packetAttr != null)
				{
					packetClassMap.Add(packetAttr.packetType, type);
				}
			}
		}

		
		protected class ProxyInfo
		{
			
			public readonly IPacketController controller;

			
			public readonly MethodInfo handleMethod;

			
			public readonly MethodInfo serializePacketMethod;

			
			public ProxyInfo(IPacketController controller, MethodInfo handleMethod, Type packetClassType)
			{
				this.controller = controller;
				this.handleMethod = handleMethod;
				MethodInfo method = typeof(PacketWrapper).GetMethod("GetPacket");
				serializePacketMethod = method.MakeGenericMethod(packetClassType);
			}
		}
	}
}