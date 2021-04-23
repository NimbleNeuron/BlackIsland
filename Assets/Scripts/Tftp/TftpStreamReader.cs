using System;
using System.IO;

namespace Tftp.Net
{
	
	internal class TftpStreamReader
	{
		
		public TftpStreamReader(Stream stream)
		{
			this.stream = stream;
		}

		
		public ushort ReadUInt16()
		{
			byte b = (byte)this.stream.ReadByte();
			int num = this.stream.ReadByte();
			return (ushort)((int)b << 8 | (int)((byte)num));
		}

		
		public byte ReadByte()
		{
			int num = this.stream.ReadByte();
			if (num == -1)
			{
				throw new IOException();
			}
			return (byte)num;
		}

		
		public byte[] ReadBytes(int maxBytes)
		{
			byte[] array = new byte[maxBytes];
			int num = this.stream.Read(array, 0, array.Length);
			if (num == -1)
			{
				throw new IOException();
			}
			Array.Resize<byte>(ref array, num);
			return array;
		}

		
		private readonly Stream stream;
	}
}
