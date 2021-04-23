using System;
using System.Collections.Generic;

namespace LiteNetLib.Utils
{
	
	public class NetPacketProcessor
	{
		
		public NetPacketProcessor()
		{
			this._netSerializer = new NetSerializer();
		}

		
		public NetPacketProcessor(int maxStringLength)
		{
			this._netSerializer = new NetSerializer(maxStringLength);
		}

		
		protected virtual ulong GetHash<T>()
		{
			if (NetPacketProcessor.HashCache<T>.Initialized)
			{
				return NetPacketProcessor.HashCache<T>.Id;
			}
			ulong num = 14695981039346656037UL;
			string fullName = typeof(T).FullName;
			for (int i = 0; i < fullName.Length; i++)
			{
				num ^= (ulong)fullName[i];
				num *= 1099511628211UL;
			}
			NetPacketProcessor.HashCache<T>.Initialized = true;
			NetPacketProcessor.HashCache<T>.Id = num;
			return num;
		}

		
		protected virtual NetPacketProcessor.SubscribeDelegate GetCallbackFromData(NetDataReader reader)
		{
			ulong @ulong = reader.GetULong();
			NetPacketProcessor.SubscribeDelegate result;
			if (!this._callbacks.TryGetValue(@ulong, out result))
			{
				throw new ParseException("Undefined packet in NetDataReader");
			}
			return result;
		}

		
		protected virtual void WriteHash<T>(NetDataWriter writer)
		{
			writer.Put(this.GetHash<T>());
		}

		
		public void RegisterNestedType<T>() where T : struct, INetSerializable
		{
			this._netSerializer.RegisterNestedType<T>();
		}

		
		public void RegisterNestedType<T>(Action<NetDataWriter, T> writeDelegate, Func<NetDataReader, T> readDelegate)
		{
			this._netSerializer.RegisterNestedType<T>(writeDelegate, readDelegate);
		}

		
		public void RegisterNestedType<T>(Func<T> constructor) where T : class, INetSerializable
		{
			this._netSerializer.RegisterNestedType<T>(constructor);
		}

		
		public void ReadAllPackets(NetDataReader reader)
		{
			while (reader.AvailableBytes > 0)
			{
				this.ReadPacket(reader);
			}
		}

		
		public void ReadAllPackets(NetDataReader reader, object userData)
		{
			while (reader.AvailableBytes > 0)
			{
				this.ReadPacket(reader, userData);
			}
		}

		
		public void ReadPacket(NetDataReader reader)
		{
			this.ReadPacket(reader, null);
		}

		
		public void Send<T>(NetPeer peer, T packet, DeliveryMethod options) where T : class, new()
		{
			this._netDataWriter.Reset();
			this.Write<T>(this._netDataWriter, packet);
			peer.Send(this._netDataWriter, options);
		}

		
		public void SendNetSerializable<T>(NetPeer peer, T packet, DeliveryMethod options) where T : INetSerializable
		{
			this._netDataWriter.Reset();
			this.WriteNetSerializable<T>(this._netDataWriter, packet);
			peer.Send(this._netDataWriter, options);
		}

		
		public void Send<T>(NetManager manager, T packet, DeliveryMethod options) where T : class, new()
		{
			this._netDataWriter.Reset();
			this.Write<T>(this._netDataWriter, packet);
			manager.SendToAll(this._netDataWriter, options);
		}

		
		public void SendNetSerializable<T>(NetManager manager, T packet, DeliveryMethod options) where T : INetSerializable
		{
			this._netDataWriter.Reset();
			this.WriteNetSerializable<T>(this._netDataWriter, packet);
			manager.SendToAll(this._netDataWriter, options);
		}

		
		public void Write<T>(NetDataWriter writer, T packet) where T : class, new()
		{
			this.WriteHash<T>(writer);
			this._netSerializer.Serialize<T>(writer, packet);
		}

		
		public void WriteNetSerializable<T>(NetDataWriter writer, T packet) where T : INetSerializable
		{
			this.WriteHash<T>(writer);
			packet.Serialize(writer);
		}

		
		public byte[] Write<T>(T packet) where T : class, new()
		{
			this._netDataWriter.Reset();
			this.WriteHash<T>(this._netDataWriter);
			this._netSerializer.Serialize<T>(this._netDataWriter, packet);
			return this._netDataWriter.CopyData();
		}

		
		public byte[] WriteNetSerializable<T>(T packet) where T : INetSerializable
		{
			this._netDataWriter.Reset();
			this.WriteHash<T>(this._netDataWriter);
			packet.Serialize(this._netDataWriter);
			return this._netDataWriter.CopyData();
		}

		
		public void ReadPacket(NetDataReader reader, object userData)
		{
			this.GetCallbackFromData(reader)(reader, userData);
		}

		
		public void Subscribe<T>(Action<T> onReceive, Func<T> packetConstructor) where T : class, new()
		{
			this._netSerializer.Register<T>();
			this._callbacks[this.GetHash<T>()] = delegate(NetDataReader reader, object userData)
			{
				T t = packetConstructor();
				this._netSerializer.Deserialize<T>(reader, t);
				onReceive(t);
			};
		}

		
		public void Subscribe<T, TUserData>(Action<T, TUserData> onReceive, Func<T> packetConstructor) where T : class, new()
		{
			this._netSerializer.Register<T>();
			this._callbacks[this.GetHash<T>()] = delegate(NetDataReader reader, object userData)
			{
				T t = packetConstructor();
				this._netSerializer.Deserialize<T>(reader, t);
				onReceive(t, (TUserData)((object)userData));
			};
		}

		
		public void SubscribeReusable<T>(Action<T> onReceive) where T : class, new()
		{
			this._netSerializer.Register<T>();
			T reference = Activator.CreateInstance<T>();
			this._callbacks[this.GetHash<T>()] = delegate(NetDataReader reader, object userData)
			{
				this._netSerializer.Deserialize<T>(reader, reference);
				onReceive(reference);
			};
		}

		
		public void SubscribeReusable<T, TUserData>(Action<T, TUserData> onReceive) where T : class, new()
		{
			this._netSerializer.Register<T>();
			T reference = Activator.CreateInstance<T>();
			this._callbacks[this.GetHash<T>()] = delegate(NetDataReader reader, object userData)
			{
				this._netSerializer.Deserialize<T>(reader, reference);
				onReceive(reference, (TUserData)((object)userData));
			};
		}

		
		public void SubscribeNetSerializable<T, TUserData>(Action<T, TUserData> onReceive, Func<T> packetConstructor) where T : INetSerializable
		{
			this._callbacks[this.GetHash<T>()] = delegate(NetDataReader reader, object userData)
			{
				T arg = packetConstructor();
				arg.Deserialize(reader);
				onReceive(arg, (TUserData)((object)userData));
			};
		}

		
		public void SubscribeNetSerializable<T>(Action<T> onReceive, Func<T> packetConstructor) where T : INetSerializable
		{
			this._callbacks[this.GetHash<T>()] = delegate(NetDataReader reader, object userData)
			{
				T obj = packetConstructor();
				obj.Deserialize(reader);
				onReceive(obj);
			};
		}

		
		public void SubscribeNetSerializable<T, TUserData>(Action<T, TUserData> onReceive) where T : INetSerializable, new()
		{
			T reference = Activator.CreateInstance<T>();
			this._callbacks[this.GetHash<T>()] = delegate(NetDataReader reader, object userData)
			{
				reference.Deserialize(reader);
				onReceive(reference, (TUserData)((object)userData));
			};
		}

		
		public void SubscribeNetSerializable<T>(Action<T> onReceive) where T : INetSerializable, new()
		{
			T reference = Activator.CreateInstance<T>();
			this._callbacks[this.GetHash<T>()] = delegate(NetDataReader reader, object userData)
			{
				reference.Deserialize(reader);
				onReceive(reference);
			};
		}

		
		public bool RemoveSubscription<T>()
		{
			return this._callbacks.Remove(this.GetHash<T>());
		}

		
		private readonly NetSerializer _netSerializer;

		
		private readonly Dictionary<ulong, NetPacketProcessor.SubscribeDelegate> _callbacks = new Dictionary<ulong, NetPacketProcessor.SubscribeDelegate>();

		
		private readonly NetDataWriter _netDataWriter = new NetDataWriter();

		
		private static class HashCache<T>
		{
			
			public static bool Initialized;

			
			public static ulong Id;
		}

		
		protected delegate void SubscribeDelegate(NetDataReader reader, object userData);
	}
}
