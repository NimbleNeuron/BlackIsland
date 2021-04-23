using System.IO;

namespace Tftp.Net
{
	
	internal class TftpStreamWriter
	{
		
		public TftpStreamWriter(Stream stream)
		{
			this.stream = stream;
		}

		
		public void WriteUInt16(ushort value)
		{
			this.stream.WriteByte((byte)(value >> 8));
			this.stream.WriteByte((byte)(value & 255));
		}

		
		public void WriteByte(byte b)
		{
			this.stream.WriteByte(b);
		}

		
		public void WriteBytes(byte[] data)
		{
			this.stream.Write(data, 0, data.Length);
		}

		
		private readonly Stream stream;
	}
}
