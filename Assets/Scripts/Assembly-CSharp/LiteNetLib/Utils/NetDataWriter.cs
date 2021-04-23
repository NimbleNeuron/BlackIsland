using System;
using System.Net;
using System.Text;

namespace LiteNetLib.Utils
{
	
	public class NetDataWriter
	{
		
		
		public int Capacity
		{
			get
			{
				return this._data.Length;
			}
		}

		
		public NetDataWriter() : this(true, 64)
		{
		}

		
		public NetDataWriter(bool autoResize) : this(autoResize, 64)
		{
		}

		
		public NetDataWriter(bool autoResize, int initialSize)
		{
			this._data = new byte[initialSize];
			this._autoResize = autoResize;
		}

		
		public static NetDataWriter FromBytes(byte[] bytes, bool copy)
		{
			if (copy)
			{
				NetDataWriter netDataWriter = new NetDataWriter(true, bytes.Length);
				netDataWriter.Put(bytes);
				return netDataWriter;
			}
			return new NetDataWriter(true, 0)
			{
				_data = bytes
			};
		}

		
		public static NetDataWriter FromBytes(byte[] bytes, int offset, int length)
		{
			NetDataWriter netDataWriter = new NetDataWriter(true, bytes.Length);
			netDataWriter.Put(bytes, offset, length);
			return netDataWriter;
		}

		
		public static NetDataWriter FromString(string value)
		{
			NetDataWriter netDataWriter = new NetDataWriter();
			netDataWriter.Put(value);
			return netDataWriter;
		}

		
		public void ResizeIfNeed(int newSize)
		{
			int i = this._data.Length;
			if (i < newSize)
			{
				while (i < newSize)
				{
					i *= 2;
				}
				Array.Resize<byte>(ref this._data, i);
			}
		}

		
		public void Reset(int size)
		{
			this.ResizeIfNeed(size);
			this._position = 0;
		}

		
		public void Reset()
		{
			this._position = 0;
		}

		
		public byte[] CopyData()
		{
			byte[] array = new byte[this._position];
			Buffer.BlockCopy(this._data, 0, array, 0, this._position);
			return array;
		}

		
		
		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}

		
		
		public int Length
		{
			get
			{
				return this._position;
			}
		}

		
		public void Put(float value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 4);
			}
			FastBitConverter.GetBytes(this._data, this._position, value);
			this._position += 4;
		}

		
		public void Put(double value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 8);
			}
			FastBitConverter.GetBytes(this._data, this._position, value);
			this._position += 8;
		}

		
		public void Put(long value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 8);
			}
			FastBitConverter.GetBytes(this._data, this._position, value);
			this._position += 8;
		}

		
		public void Put(ulong value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 8);
			}
			FastBitConverter.GetBytes(this._data, this._position, value);
			this._position += 8;
		}

		
		public void Put(int value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 4);
			}
			FastBitConverter.GetBytes(this._data, this._position, value);
			this._position += 4;
		}

		
		public void Put(uint value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 4);
			}
			FastBitConverter.GetBytes(this._data, this._position, value);
			this._position += 4;
		}

		
		public void Put(char value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 2);
			}
			FastBitConverter.GetBytes(this._data, this._position, (ushort)value);
			this._position += 2;
		}

		
		public void Put(ushort value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 2);
			}
			FastBitConverter.GetBytes(this._data, this._position, value);
			this._position += 2;
		}

		
		public void Put(short value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 2);
			}
			FastBitConverter.GetBytes(this._data, this._position, value);
			this._position += 2;
		}

		
		public void Put(sbyte value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 1);
			}
			this._data[this._position] = (byte)value;
			this._position++;
		}

		
		public void Put(byte value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 1);
			}
			this._data[this._position] = value;
			this._position++;
		}

		
		public void Put(byte[] data, int offset, int length)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + length);
			}
			Buffer.BlockCopy(data, offset, this._data, this._position, length);
			this._position += length;
		}

		
		public void Put(byte[] data)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + data.Length);
			}
			Buffer.BlockCopy(data, 0, this._data, this._position, data.Length);
			this._position += data.Length;
		}

		
		public void PutSBytesWithLength(sbyte[] data, int offset, int length)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + length + 4);
			}
			FastBitConverter.GetBytes(this._data, this._position, length);
			Buffer.BlockCopy(data, offset, this._data, this._position + 4, length);
			this._position += length + 4;
		}

		
		public void PutSBytesWithLength(sbyte[] data)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + data.Length + 4);
			}
			FastBitConverter.GetBytes(this._data, this._position, data.Length);
			Buffer.BlockCopy(data, 0, this._data, this._position + 4, data.Length);
			this._position += data.Length + 4;
		}

		
		public void PutBytesWithLength(byte[] data, int offset, int length)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + length + 4);
			}
			FastBitConverter.GetBytes(this._data, this._position, length);
			Buffer.BlockCopy(data, offset, this._data, this._position + 4, length);
			this._position += length + 4;
		}

		
		public void PutBytesWithLength(byte[] data)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + data.Length + 4);
			}
			FastBitConverter.GetBytes(this._data, this._position, data.Length);
			Buffer.BlockCopy(data, 0, this._data, this._position + 4, data.Length);
			this._position += data.Length + 4;
		}

		
		public void Put(bool value)
		{
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + 1);
			}
			// co: dotPeek
			this._data[this._position] = value ? (byte) 1 : (byte) 0;
			this._position++;
		}

		
		private void PutArray(Array arr, int sz)
		{
			// co: dotPeek
			ushort num = arr == null ? (ushort) 0 : (ushort) arr.Length;
			sz *= (int)num;
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + sz + 2);
			}
			FastBitConverter.GetBytes(this._data, this._position, num);
			if (arr != null)
			{
				Buffer.BlockCopy(arr, 0, this._data, this._position + 2, sz);
			}
			this._position += sz + 2;
		}

		
		public void PutArray(float[] value)
		{
			this.PutArray(value, 4);
		}

		
		public void PutArray(double[] value)
		{
			this.PutArray(value, 8);
		}

		
		public void PutArray(long[] value)
		{
			this.PutArray(value, 8);
		}

		
		public void PutArray(ulong[] value)
		{
			this.PutArray(value, 8);
		}

		
		public void PutArray(int[] value)
		{
			this.PutArray(value, 4);
		}

		
		public void PutArray(uint[] value)
		{
			this.PutArray(value, 4);
		}

		
		public void PutArray(ushort[] value)
		{
			this.PutArray(value, 2);
		}

		
		public void PutArray(short[] value)
		{
			this.PutArray(value, 2);
		}

		
		public void PutArray(bool[] value)
		{
			this.PutArray(value, 1);
		}

		
		public void PutArray(string[] value)
		{
			// co: dotPeek
			ushort num = value == null ? (ushort) 0 : (ushort) value.Length;
			this.Put(num);
			for (int i = 0; i < (int)num; i++)
			{
				this.Put(value[i]);
			}
		}

		
		public void PutArray(string[] value, int maxLength)
		{
			// co: dotPeek
			ushort num = value == null ? (ushort) 0 : (ushort) value.Length;
			this.Put(num);
			for (int i = 0; i < (int)num; i++)
			{
				this.Put(value[i], maxLength);
			}
		}

		
		public void Put(IPEndPoint endPoint)
		{
			this.Put(endPoint.Address.ToString());
			this.Put(endPoint.Port);
		}

		
		public void Put(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.Put(0);
				return;
			}
			int byteCount = Encoding.UTF8.GetByteCount(value);
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + byteCount + 4);
			}
			this.Put(byteCount);
			Encoding.UTF8.GetBytes(value, 0, value.Length, this._data, this._position);
			this._position += byteCount;
		}

		
		public void Put(string value, int maxLength)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.Put(0);
				return;
			}
			int charCount = (value.Length > maxLength) ? maxLength : value.Length;
			int byteCount = Encoding.UTF8.GetByteCount(value);
			if (this._autoResize)
			{
				this.ResizeIfNeed(this._position + byteCount + 4);
			}
			this.Put(byteCount);
			Encoding.UTF8.GetBytes(value, 0, charCount, this._data, this._position);
			this._position += byteCount;
		}

		
		public void Put<T>(T obj) where T : INetSerializable
		{
			obj.Serialize(this);
		}

		
		protected byte[] _data;

		
		protected int _position;

		
		private const int InitialSize = 64;

		
		private readonly bool _autoResize;
	}
}
