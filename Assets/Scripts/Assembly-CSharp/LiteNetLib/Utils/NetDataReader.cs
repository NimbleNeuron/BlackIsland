using System;
using System.Net;
using System.Text;

namespace LiteNetLib.Utils
{
	
	public class NetDataReader
	{
		
		
		public byte[] RawData
		{
			get
			{
				return this._data;
			}
		}

		
		
		public int RawDataSize
		{
			get
			{
				return this._dataSize;
			}
		}

		
		
		public int UserDataOffset
		{
			get
			{
				return this._offset;
			}
		}

		
		
		public int UserDataSize
		{
			get
			{
				return this._dataSize - this._offset;
			}
		}

		
		
		public bool IsNull
		{
			get
			{
				return this._data == null;
			}
		}

		
		
		public int Position
		{
			get
			{
				return this._position;
			}
		}

		
		
		public bool EndOfData
		{
			get
			{
				return this._position == this._dataSize;
			}
		}

		
		
		public int AvailableBytes
		{
			get
			{
				return this._dataSize - this._position;
			}
		}

		
		public void SkipBytes(int count)
		{
			this._position += count;
		}

		
		public void SetSource(NetDataWriter dataWriter)
		{
			this._data = dataWriter.Data;
			this._position = 0;
			this._offset = 0;
			this._dataSize = dataWriter.Length;
		}

		
		public void SetSource(byte[] source)
		{
			this._data = source;
			this._position = 0;
			this._offset = 0;
			this._dataSize = source.Length;
		}

		
		public void SetSource(byte[] source, int offset)
		{
			this._data = source;
			this._position = offset;
			this._offset = offset;
			this._dataSize = source.Length;
		}

		
		public void SetSource(byte[] source, int offset, int maxSize)
		{
			this._data = source;
			this._position = offset;
			this._offset = offset;
			this._dataSize = maxSize;
		}

		
		public NetDataReader()
		{
		}

		
		public NetDataReader(NetDataWriter writer)
		{
			this.SetSource(writer);
		}

		
		public NetDataReader(byte[] source)
		{
			this.SetSource(source);
		}

		
		public NetDataReader(byte[] source, int offset)
		{
			this.SetSource(source, offset);
		}

		
		public NetDataReader(byte[] source, int offset, int maxSize)
		{
			this.SetSource(source, offset, maxSize);
		}

		
		public IPEndPoint GetNetEndPoint()
		{
			string @string = this.GetString(1000);
			int @int = this.GetInt();
			return NetUtils.MakeEndPoint(@string, @int);
		}

		
		public byte GetByte()
		{
			byte result = this._data[this._position];
			this._position++;
			return result;
		}

		
		public sbyte GetSByte()
		{
			sbyte result = (sbyte)this._data[this._position];
			this._position++;
			return result;
		}

		
		public bool[] GetBoolArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			bool[] array = new bool[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)num);
			this._position += (int)num;
			return array;
		}

		
		public ushort[] GetUShortArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			ushort[] array = new ushort[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)(num * 2));
			this._position += (int)(num * 2);
			return array;
		}

		
		public short[] GetShortArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			short[] array = new short[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)(num * 2));
			this._position += (int)(num * 2);
			return array;
		}

		
		public long[] GetLongArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			long[] array = new long[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)(num * 8));
			this._position += (int)(num * 8);
			return array;
		}

		
		public ulong[] GetULongArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			ulong[] array = new ulong[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)(num * 8));
			this._position += (int)(num * 8);
			return array;
		}

		
		public int[] GetIntArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			int[] array = new int[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)(num * 4));
			this._position += (int)(num * 4);
			return array;
		}

		
		public uint[] GetUIntArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			uint[] array = new uint[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)(num * 4));
			this._position += (int)(num * 4);
			return array;
		}

		
		public float[] GetFloatArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			float[] array = new float[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)(num * 4));
			this._position += (int)(num * 4);
			return array;
		}

		
		public double[] GetDoubleArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			double[] array = new double[(int)num];
			Buffer.BlockCopy(this._data, this._position, array, 0, (int)(num * 8));
			this._position += (int)(num * 8);
			return array;
		}

		
		public string[] GetStringArray()
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			string[] array = new string[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				array[i] = this.GetString();
			}
			return array;
		}

		
		public string[] GetStringArray(int maxStringLength)
		{
			ushort num = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			string[] array = new string[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				array[i] = this.GetString(maxStringLength);
			}
			return array;
		}

		
		public bool GetBool()
		{
			bool result = this._data[this._position] > 0;
			this._position++;
			return result;
		}

		
		public char GetChar()
		{
			char result = BitConverter.ToChar(this._data, this._position);
			this._position += 2;
			return result;
		}

		
		public ushort GetUShort()
		{
			ushort result = BitConverter.ToUInt16(this._data, this._position);
			this._position += 2;
			return result;
		}

		
		public short GetShort()
		{
			short result = BitConverter.ToInt16(this._data, this._position);
			this._position += 2;
			return result;
		}

		
		public long GetLong()
		{
			long result = BitConverter.ToInt64(this._data, this._position);
			this._position += 8;
			return result;
		}

		
		public ulong GetULong()
		{
			ulong result = BitConverter.ToUInt64(this._data, this._position);
			this._position += 8;
			return result;
		}

		
		public int GetInt()
		{
			int result = BitConverter.ToInt32(this._data, this._position);
			this._position += 4;
			return result;
		}

		
		public uint GetUInt()
		{
			uint result = BitConverter.ToUInt32(this._data, this._position);
			this._position += 4;
			return result;
		}

		
		public float GetFloat()
		{
			float result = BitConverter.ToSingle(this._data, this._position);
			this._position += 4;
			return result;
		}

		
		public double GetDouble()
		{
			double result = BitConverter.ToDouble(this._data, this._position);
			this._position += 8;
			return result;
		}

		
		public string GetString(int maxLength)
		{
			int @int = this.GetInt();
			if (@int <= 0 || @int > maxLength * 2)
			{
				return string.Empty;
			}
			if (Encoding.UTF8.GetCharCount(this._data, this._position, @int) > maxLength)
			{
				return string.Empty;
			}
			string @string = Encoding.UTF8.GetString(this._data, this._position, @int);
			this._position += @int;
			return @string;
		}

		
		public string GetString()
		{
			int @int = this.GetInt();
			if (@int <= 0)
			{
				return string.Empty;
			}
			string @string = Encoding.UTF8.GetString(this._data, this._position, @int);
			this._position += @int;
			return @string;
		}

		
		public ArraySegment<byte> GetRemainingBytesSegment()
		{
			ArraySegment<byte> result = new ArraySegment<byte>(this._data, this._position, this.AvailableBytes);
			this._position = this._data.Length;
			return result;
		}

		
		public T Get<T>() where T : INetSerializable, new()
		{
			T result = Activator.CreateInstance<T>();
			result.Deserialize(this);
			return result;
		}

		
		public byte[] GetRemainingBytes()
		{
			byte[] array = new byte[this.AvailableBytes];
			Buffer.BlockCopy(this._data, this._position, array, 0, this.AvailableBytes);
			this._position = this._data.Length;
			return array;
		}

		
		public void GetBytes(byte[] destination, int start, int count)
		{
			Buffer.BlockCopy(this._data, this._position, destination, start, count);
			this._position += count;
		}

		
		public void GetBytes(byte[] destination, int count)
		{
			Buffer.BlockCopy(this._data, this._position, destination, 0, count);
			this._position += count;
		}

		
		public sbyte[] GetSBytesWithLength()
		{
			int @int = this.GetInt();
			sbyte[] array = new sbyte[@int];
			Buffer.BlockCopy(this._data, this._position, array, 0, @int);
			this._position += @int;
			return array;
		}

		
		public byte[] GetBytesWithLength()
		{
			int @int = this.GetInt();
			byte[] array = new byte[@int];
			Buffer.BlockCopy(this._data, this._position, array, 0, @int);
			this._position += @int;
			return array;
		}

		
		public byte PeekByte()
		{
			return this._data[this._position];
		}

		
		public sbyte PeekSByte()
		{
			return (sbyte)this._data[this._position];
		}

		
		public bool PeekBool()
		{
			return this._data[this._position] > 0;
		}

		
		public char PeekChar()
		{
			return BitConverter.ToChar(this._data, this._position);
		}

		
		public ushort PeekUShort()
		{
			return BitConverter.ToUInt16(this._data, this._position);
		}

		
		public short PeekShort()
		{
			return BitConverter.ToInt16(this._data, this._position);
		}

		
		public long PeekLong()
		{
			return BitConverter.ToInt64(this._data, this._position);
		}

		
		public ulong PeekULong()
		{
			return BitConverter.ToUInt64(this._data, this._position);
		}

		
		public int PeekInt()
		{
			return BitConverter.ToInt32(this._data, this._position);
		}

		
		public uint PeekUInt()
		{
			return BitConverter.ToUInt32(this._data, this._position);
		}

		
		public float PeekFloat()
		{
			return BitConverter.ToSingle(this._data, this._position);
		}

		
		public double PeekDouble()
		{
			return BitConverter.ToDouble(this._data, this._position);
		}

		
		public string PeekString(int maxLength)
		{
			int num = BitConverter.ToInt32(this._data, this._position);
			if (num <= 0 || num > maxLength * 2)
			{
				return string.Empty;
			}
			if (Encoding.UTF8.GetCharCount(this._data, this._position + 4, num) > maxLength)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(this._data, this._position + 4, num);
		}

		
		public string PeekString()
		{
			int num = BitConverter.ToInt32(this._data, this._position);
			if (num <= 0)
			{
				return string.Empty;
			}
			return Encoding.UTF8.GetString(this._data, this._position + 4, num);
		}

		
		public bool TryGetByte(out byte result)
		{
			if (this.AvailableBytes >= 1)
			{
				result = this.GetByte();
				return true;
			}
			result = 0;
			return false;
		}

		
		public bool TryGetSByte(out sbyte result)
		{
			if (this.AvailableBytes >= 1)
			{
				result = this.GetSByte();
				return true;
			}
			result = 0;
			return false;
		}

		
		public bool TryGetBool(out bool result)
		{
			if (this.AvailableBytes >= 1)
			{
				result = this.GetBool();
				return true;
			}
			result = false;
			return false;
		}

		
		public bool TryGetChar(out char result)
		{
			if (this.AvailableBytes >= 2)
			{
				result = this.GetChar();
				return true;
			}
			result = '\0';
			return false;
		}

		
		public bool TryGetShort(out short result)
		{
			if (this.AvailableBytes >= 2)
			{
				result = this.GetShort();
				return true;
			}
			result = 0;
			return false;
		}

		
		public bool TryGetUShort(out ushort result)
		{
			if (this.AvailableBytes >= 2)
			{
				result = this.GetUShort();
				return true;
			}
			result = 0;
			return false;
		}

		
		public bool TryGetInt(out int result)
		{
			if (this.AvailableBytes >= 4)
			{
				result = this.GetInt();
				return true;
			}
			result = 0;
			return false;
		}

		
		public bool TryGetUInt(out uint result)
		{
			if (this.AvailableBytes >= 4)
			{
				result = this.GetUInt();
				return true;
			}
			result = 0U;
			return false;
		}

		
		public bool TryGetLong(out long result)
		{
			if (this.AvailableBytes >= 8)
			{
				result = this.GetLong();
				return true;
			}
			result = 0L;
			return false;
		}

		
		public bool TryGetULong(out ulong result)
		{
			if (this.AvailableBytes >= 8)
			{
				result = this.GetULong();
				return true;
			}
			result = 0UL;
			return false;
		}

		
		public bool TryGetFloat(out float result)
		{
			if (this.AvailableBytes >= 4)
			{
				result = this.GetFloat();
				return true;
			}
			result = 0f;
			return false;
		}

		
		public bool TryGetDouble(out double result)
		{
			if (this.AvailableBytes >= 8)
			{
				result = this.GetDouble();
				return true;
			}
			result = 0.0;
			return false;
		}

		
		public bool TryGetString(out string result)
		{
			if (this.AvailableBytes >= 4)
			{
				int num = this.PeekInt();
				if (this.AvailableBytes >= num + 4)
				{
					result = this.GetString();
					return true;
				}
			}
			result = null;
			return false;
		}

		
		public bool TryGetStringArray(out string[] result)
		{
			ushort num;
			if (!this.TryGetUShort(out num))
			{
				result = null;
				return false;
			}
			result = new string[(int)num];
			for (int i = 0; i < (int)num; i++)
			{
				if (!this.TryGetString(out result[i]))
				{
					result = null;
					return false;
				}
			}
			return true;
		}

		
		public bool TryGetBytesWithLength(out byte[] result)
		{
			if (this.AvailableBytes >= 4)
			{
				int num = this.PeekInt();
				if (num >= 0 && this.AvailableBytes >= num + 4)
				{
					result = this.GetBytesWithLength();
					return true;
				}
			}
			result = null;
			return false;
		}

		
		public void Clear()
		{
			this._position = 0;
			this._dataSize = 0;
			this._data = null;
		}

		
		protected byte[] _data;

		
		protected int _position;

		
		protected int _dataSize;

		
		private int _offset;
	}
}
