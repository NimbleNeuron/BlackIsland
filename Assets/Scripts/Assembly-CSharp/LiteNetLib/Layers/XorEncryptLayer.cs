using System;
using System.Text;

namespace LiteNetLib.Layers
{
	
	public class XorEncryptLayer : PacketLayerBase
	{
		
		public XorEncryptLayer() : base(0)
		{
		}

		
		public XorEncryptLayer(byte[] key) : this()
		{
			this.SetKey(key);
		}

		
		public XorEncryptLayer(string key) : this()
		{
			this.SetKey(key);
		}

		
		public void SetKey(string key)
		{
			this._byteKey = Encoding.UTF8.GetBytes(key);
		}

		
		public void SetKey(byte[] key)
		{
			if (this._byteKey == null || this._byteKey.Length != key.Length)
			{
				this._byteKey = new byte[key.Length];
			}
			Buffer.BlockCopy(key, 0, this._byteKey, 0, key.Length);
		}

		
		public override void ProcessInboundPacket(ref byte[] data, ref int length)
		{
			if (this._byteKey == null)
			{
				return;
			}
			for (int i = 0; i < length; i++)
			{
				int num = i % this._byteKey.Length;
				data[i] ^= this._byteKey[num];
			}
		}

		
		public override void ProcessOutBoundPacket(ref byte[] data, ref int offset, ref int length)
		{
			if (this._byteKey == null)
			{
				return;
			}
			int num = offset;
			int i = 0;
			while (i < length)
			{
				data[num] ^= this._byteKey[i % this._byteKey.Length];
				i++;
				num++;
			}
		}

		
		private byte[] _byteKey;
	}
}
