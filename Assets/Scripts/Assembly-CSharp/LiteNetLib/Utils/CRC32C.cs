using System;

namespace LiteNetLib.Utils
{
	
	public static class CRC32C
	{
		
		static CRC32C()
		{
			for (uint num = 0U; num < 256U; num += 1U)
			{
				uint num2 = num;
				for (int i = 0; i < 16; i++)
				{
					for (int j = 0; j < 8; j++)
					{
						num2 = (((num2 & 1U) == 1U) ? (2197175160U ^ num2 >> 1) : (num2 >> 1));
					}
					CRC32C.Table[(int)(checked((IntPtr)(unchecked((long)(i * 256) + (long)((ulong)num)))))] = num2;
				}
			}
		}

		
		public static uint Compute(byte[] input, int offset, int length)
		{
			uint num = uint.MaxValue;
			while (length >= 16)
			{
				uint num2 = CRC32C.Table[768 + (int)input[offset + 12]] ^ CRC32C.Table[512 + (int)input[offset + 13]] ^ CRC32C.Table[256 + (int)input[offset + 14]] ^ CRC32C.Table[(int)input[offset + 15]];
				uint num3 = CRC32C.Table[1792 + (int)input[offset + 8]] ^ CRC32C.Table[1536 + (int)input[offset + 9]] ^ CRC32C.Table[1280 + (int)input[offset + 10]] ^ CRC32C.Table[1024 + (int)input[offset + 11]];
				uint num4 = CRC32C.Table[2816 + (int)input[offset + 4]] ^ CRC32C.Table[2560 + (int)input[offset + 5]] ^ CRC32C.Table[2304 + (int)input[offset + 6]] ^ CRC32C.Table[2048 + (int)input[offset + 7]];
				num = (CRC32C.Table[3840 + (int)((byte)num ^ input[offset])] ^ CRC32C.Table[3584 + (int)((byte)(num >> 8) ^ input[offset + 1])] ^ CRC32C.Table[3328 + (int)((byte)(num >> 16) ^ input[offset + 2])] ^ CRC32C.Table[(int)(3072U + (num >> 24 ^ (uint)input[offset + 3]))] ^ num4 ^ num3 ^ num2);
				offset += 16;
				length -= 16;
			}
			while (--length >= 0)
			{
				num = (CRC32C.Table[(int)((byte)(num ^ (uint)input[offset++]))] ^ num >> 8);
			}
			return num ^ uint.MaxValue;
		}

		
		public const int ChecksumSize = 4;

		
		private const uint Poly = 2197175160U;

		
		private static readonly uint[] Table = new uint[4096];
	}
}
