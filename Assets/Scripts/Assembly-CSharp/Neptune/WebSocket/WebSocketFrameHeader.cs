using System;

namespace Neptune.WebSocket
{
	
	public class WebSocketFrameHeader
	{
		
		
		
		public bool Final { get; private set; }

		
		
		
		public WebSocketOpcodes Opcode { get; private set; }

		
		
		
		public long PayloadLength { get; private set; }

		
		
		
		public byte[] Mask { get; private set; }

		
		
		public int Length
		{
			get
			{
				return WebSocketFrameHeader.CalcHeaderLength(this.PayloadLength, this.Mask != null);
			}
		}

		
		
		public byte[] RawHeader
		{
			get
			{
				byte[] numArray = new byte[this.Length];
				numArray[0] = this.Final ? (byte) 128 : (byte) 0;
				ref byte local = ref numArray[0];
				local = (byte) ((WebSocketOpcodes) local | this.Opcode);
				numArray[1] = this.Mask != null ? (byte) 128 : (byte) 0;
				if (this.PayloadLength >= (long) ushort.MaxValue)
				{
					numArray[1] |= (byte) 127;
					byte[] bytes = BitConverter.GetBytes((ulong) this.PayloadLength);
					if (BitConverter.IsLittleEndian)
						Array.Reverse((Array) bytes, 0, bytes.Length);
					Buffer.BlockCopy((Array) bytes, 0, (Array) numArray, 2, 8);
				}
				else if (this.PayloadLength >= 126L)
				{
					numArray[1] |= (byte) 126;
					byte[] bytes = BitConverter.GetBytes((ushort) this.PayloadLength);
					if (BitConverter.IsLittleEndian)
						Array.Reverse((Array) bytes, 0, bytes.Length);
					Buffer.BlockCopy((Array) bytes, 0, (Array) numArray, 2, 2);
				}
				else
					numArray[1] |= (byte) this.PayloadLength;
				if (this.Mask != null)
					Buffer.BlockCopy((Array) this.Mask, 0, (Array) numArray, numArray.Length - 4, 4);
				return numArray;
			}
			
			// co: dotPeek
			// get
			// {
			// 	byte[] array = new byte[this.Length];
			// 	array[0] = (this.Final ? 128 : 0);
			// 	byte[] array2 = array;
			// 	int num = 0;
			// 	array2[num] |= (byte)this.Opcode;
			// 	array[1] = ((this.Mask != null) ? 128 : 0);
			// 	if (this.PayloadLength >= 65535L)
			// 	{
			// 		byte[] array3 = array;
			// 		int num2 = 1;
			// 		array3[num2] |= 127;
			// 		byte[] bytes = BitConverter.GetBytes((ulong)this.PayloadLength);
			// 		if (BitConverter.IsLittleEndian)
			// 		{
			// 			Array.Reverse(bytes, 0, bytes.Length);
			// 		}
			// 		Buffer.BlockCopy(bytes, 0, array, 2, 8);
			// 	}
			// 	else if (this.PayloadLength >= 126L)
			// 	{
			// 		byte[] array4 = array;
			// 		int num3 = 1;
			// 		array4[num3] |= 126;
			// 		byte[] bytes2 = BitConverter.GetBytes((ushort)this.PayloadLength);
			// 		if (BitConverter.IsLittleEndian)
			// 		{
			// 			Array.Reverse(bytes2, 0, bytes2.Length);
			// 		}
			// 		Buffer.BlockCopy(bytes2, 0, array, 2, 2);
			// 	}
			// 	else
			// 	{
			// 		byte[] array5 = array;
			// 		int num4 = 1;
			// 		array5[num4] |= (byte)this.PayloadLength;
			// 	}
			// 	if (this.Mask != null)
			// 	{
			// 		Buffer.BlockCopy(this.Mask, 0, array, array.Length - 4, 4);
			// 	}
			// 	return array;
			// }
		}

		
		private WebSocketFrameHeader(bool final, WebSocketOpcodes opcode, long payloadLength, byte[] mask)
		{
			this.Final = final;
			this.Opcode = opcode;
			this.PayloadLength = payloadLength;
			this.Mask = mask;
		}

		
		public WebSocketFrameHeader(bool final, WebSocketOpcodes opcode, long payloadLength) : this(final, opcode, payloadLength, null)
		{
			this.Mask = BitConverter.GetBytes(this.GetHashCode());
		}

		
		public WebSocketFrameHeader(WebSocketOpcodes opcode, long payloadLength) : this(true, opcode, payloadLength)
		{
		}

		
		public override string ToString()
		{
			return string.Format("WebSocketFrameHeader: final={0}, opcode={1}, payload-length={2}, mask={3}", new object[]
			{
				this.Final,
				this.Opcode,
				this.PayloadLength,
				(this.Mask == null) ? "null" : BitConverter.ToString(this.Mask)
			});
		}

		
		internal void Add(WebSocketFrameHeader header)
		{
			if (this.Final)
			{
				throw new Exception("Final frame");
			}
			if (header.Opcode != WebSocketOpcodes.Continuation)
			{
				throw new Exception("Not continuation frame");
			}
			this.Final = header.Final;
			this.PayloadLength += header.PayloadLength;
		}

		
		public static int CalcHeaderLength(long payloadLength, bool hasMask)
		{
			int num = 2;
			if (payloadLength >= 65535L)
			{
				num += 8;
			}
			else if (payloadLength >= 126L)
			{
				num += 2;
			}
			if (hasMask)
			{
				num += 4;
			}
			return num;
		}

		
		private static byte[] BlockCopyReverse(byte[] src, int offset, int count)
		{
			byte[] array = new byte[count];
			Buffer.BlockCopy(src, offset, array, 0, count);
			Array.Reverse(array, 0, count);
			return array;
		}

		
		public static WebSocketFrameHeader TryGet(byte[] buffer, int offset, int count)
		{
			if (count < 2)
			{
				return null;
			}
			int num = 2;
			bool final = (buffer[offset] & 128) == 128;
			WebSocketOpcodes opcode = (WebSocketOpcodes)(buffer[offset] & 15);
			bool flag = (buffer[offset + 1] & 128) == 128;
			byte[] array = null;
			long num2 = (long)(buffer[offset + 1] & 127);
			if (count < WebSocketFrameHeader.CalcHeaderLength(num2, flag))
			{
				return null;
			}
			if (num2 != 126L)
			{
				if (num2 == 127L)
				{
					if (BitConverter.IsLittleEndian)
					{
						num2 = BitConverter.ToInt64(WebSocketFrameHeader.BlockCopyReverse(buffer, offset + num, 8), 0);
					}
					else
					{
						num2 = BitConverter.ToInt64(buffer, offset + num);
					}
					num += 8;
				}
			}
			else
			{
				if (BitConverter.IsLittleEndian)
				{
					num2 = (long)((ulong)BitConverter.ToUInt16(WebSocketFrameHeader.BlockCopyReverse(buffer, offset + num, 2), 0));
				}
				else
				{
					num2 = (long)((ulong)BitConverter.ToUInt16(buffer, offset + num));
				}
				num += 2;
			}
			if (flag)
			{
				array = new byte[4];
				Buffer.BlockCopy(buffer, offset + num, array, 0, 4);
				num += 4;
			}
			return new WebSocketFrameHeader(final, opcode, num2, array);
		}

		
		private const byte ByteZero = 0;

		
		private const byte FinalBit = 128;

		
		private const byte OpcodeMask = 15;

		
		private const byte MaskBit = 128;

		
		private const byte TwoBytesPayloadLength = 126;

		
		private const byte EightBytesPayloadLength = 127;

		
		private const int MaskLength = 4;

		
		private const byte PayloadLenthMask = 127;
	}
}
