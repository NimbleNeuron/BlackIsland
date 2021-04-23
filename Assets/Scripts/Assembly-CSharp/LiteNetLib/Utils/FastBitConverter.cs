using System.Runtime.InteropServices;

namespace LiteNetLib.Utils
{
	
	public static class FastBitConverter
	{
		
		private static void WriteLittleEndian(byte[] buffer, int offset, ulong data)
		{
			buffer[offset] = (byte)data;
			buffer[offset + 1] = (byte)(data >> 8);
			buffer[offset + 2] = (byte)(data >> 16);
			buffer[offset + 3] = (byte)(data >> 24);
			buffer[offset + 4] = (byte)(data >> 32);
			buffer[offset + 5] = (byte)(data >> 40);
			buffer[offset + 6] = (byte)(data >> 48);
			buffer[offset + 7] = (byte)(data >> 56);
		}

		
		private static void WriteLittleEndian(byte[] buffer, int offset, int data)
		{
			buffer[offset] = (byte)data;
			buffer[offset + 1] = (byte)(data >> 8);
			buffer[offset + 2] = (byte)(data >> 16);
			buffer[offset + 3] = (byte)(data >> 24);
		}

		
		public static void WriteLittleEndian(byte[] buffer, int offset, short data)
		{
			buffer[offset] = (byte)data;
			buffer[offset + 1] = (byte)(data >> 8);
		}

		
		public static void GetBytes(byte[] bytes, int startIndex, double value)
		{
			FastBitConverter.ConverterHelperDouble converterHelperDouble = new FastBitConverter.ConverterHelperDouble
			{
				Adouble = value
			};
			FastBitConverter.WriteLittleEndian(bytes, startIndex, converterHelperDouble.Along);
		}

		
		public static void GetBytes(byte[] bytes, int startIndex, float value)
		{
			FastBitConverter.ConverterHelperFloat converterHelperFloat = new FastBitConverter.ConverterHelperFloat
			{
				Afloat = value
			};
			FastBitConverter.WriteLittleEndian(bytes, startIndex, converterHelperFloat.Aint);
		}

		
		public static void GetBytes(byte[] bytes, int startIndex, short value)
		{
			FastBitConverter.WriteLittleEndian(bytes, startIndex, value);
		}

		
		public static void GetBytes(byte[] bytes, int startIndex, ushort value)
		{
			FastBitConverter.WriteLittleEndian(bytes, startIndex, (short)value);
		}

		
		public static void GetBytes(byte[] bytes, int startIndex, int value)
		{
			FastBitConverter.WriteLittleEndian(bytes, startIndex, value);
		}

		
		public static void GetBytes(byte[] bytes, int startIndex, uint value)
		{
			FastBitConverter.WriteLittleEndian(bytes, startIndex, (int)value);
		}

		
		public static void GetBytes(byte[] bytes, int startIndex, long value)
		{
			FastBitConverter.WriteLittleEndian(bytes, startIndex, (ulong)value);
		}

		
		public static void GetBytes(byte[] bytes, int startIndex, ulong value)
		{
			FastBitConverter.WriteLittleEndian(bytes, startIndex, value);
		}

		
		[StructLayout(LayoutKind.Explicit)]
		private struct ConverterHelperDouble
		{
			
			[FieldOffset(0)]
			public ulong Along;

			
			[FieldOffset(0)]
			public double Adouble;
		}

		
		[StructLayout(LayoutKind.Explicit)]
		private struct ConverterHelperFloat
		{
			
			[FieldOffset(0)]
			public int Aint;

			
			[FieldOffset(0)]
			public float Afloat;
		}
	}
}
