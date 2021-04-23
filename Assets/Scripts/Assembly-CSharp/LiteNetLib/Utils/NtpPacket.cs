using System;

namespace LiteNetLib.Utils
{
	
	public class NtpPacket
	{
		
		
		
		public byte[] Bytes { get; private set; }

		
		
		public NtpLeapIndicator LeapIndicator
		{
			get
			{
				return (NtpLeapIndicator)((this.Bytes[0] & 192) >> 6);
			}
		}

		
		
		
		public int VersionNumber
		{
			get
			{
				return (this.Bytes[0] & 56) >> 3;
			}
			private set
			{
				this.Bytes[0] = (byte)(((int)this.Bytes[0] & -57) | value << 3);
			}
		}

		
		
		
		public NtpMode Mode
		{
			get
			{
				return (NtpMode)(this.Bytes[0] & 7);
			}
			private set
			{
				this.Bytes[0] = (byte)(((NtpMode)this.Bytes[0] & (NtpMode)(-8)) | value);
			}
		}

		
		
		public int Stratum
		{
			get
			{
				return (int)this.Bytes[1];
			}
		}

		
		
		public int Poll
		{
			get
			{
				return (int)this.Bytes[2];
			}
		}

		
		
		public int Precision
		{
			get
			{
				return (int)((sbyte)this.Bytes[3]);
			}
		}

		
		
		public TimeSpan RootDelay
		{
			get
			{
				return this.GetTimeSpan32(4);
			}
		}

		
		
		public TimeSpan RootDispersion
		{
			get
			{
				return this.GetTimeSpan32(8);
			}
		}

		
		
		public uint ReferenceId
		{
			get
			{
				return this.GetUInt32BE(12);
			}
		}

		
		
		public DateTime? ReferenceTimestamp
		{
			get
			{
				return this.GetDateTime64(16);
			}
		}

		
		
		public DateTime? OriginTimestamp
		{
			get
			{
				return this.GetDateTime64(24);
			}
		}

		
		
		public DateTime? ReceiveTimestamp
		{
			get
			{
				return this.GetDateTime64(32);
			}
		}

		
		
		
		public DateTime? TransmitTimestamp
		{
			get
			{
				return this.GetDateTime64(40);
			}
			private set
			{
				this.SetDateTime64(40, value);
			}
		}

		
		
		
		public DateTime? DestinationTimestamp { get; private set; }

		
		
		public TimeSpan RoundTripTime
		{
			get
			{
				this.CheckTimestamps();
				return this.ReceiveTimestamp.Value - this.OriginTimestamp.Value + (this.DestinationTimestamp.Value - this.TransmitTimestamp.Value);
			}
		}

		
		
		public TimeSpan CorrectionOffset
		{
			get
			{
				this.CheckTimestamps();
				return TimeSpan.FromTicks((this.ReceiveTimestamp.Value - this.OriginTimestamp.Value - (this.DestinationTimestamp.Value - this.TransmitTimestamp.Value)).Ticks / 2L);
			}
		}

		
		public NtpPacket() : this(new byte[48])
		{
			this.Mode = NtpMode.Client;
			this.VersionNumber = 4;
			this.TransmitTimestamp = new DateTime?(DateTime.UtcNow);
		}

		
		internal NtpPacket(byte[] bytes)
		{
			if (bytes.Length < 48)
			{
				throw new ArgumentException("SNTP reply packet must be at least 48 bytes long.", "bytes");
			}
			this.Bytes = bytes;
		}

		
		public static NtpPacket FromServerResponse(byte[] bytes, DateTime destinationTimestamp)
		{
			return new NtpPacket(bytes)
			{
				DestinationTimestamp = new DateTime?(destinationTimestamp)
			};
		}

		
		internal void ValidateRequest()
		{
			if (this.Mode != NtpMode.Client)
			{
				throw new InvalidOperationException("This is not a request SNTP packet.");
			}
			if (this.VersionNumber == 0)
			{
				throw new InvalidOperationException("Protocol version of the request is not specified.");
			}
			if (this.TransmitTimestamp == null)
			{
				throw new InvalidOperationException("TransmitTimestamp must be set in request packet.");
			}
		}

		
		internal void ValidateReply()
		{
			if (this.Mode != NtpMode.Server)
			{
				throw new InvalidOperationException("This is not a reply SNTP packet.");
			}
			if (this.VersionNumber == 0)
			{
				throw new InvalidOperationException("Protocol version of the reply is not specified.");
			}
			if (this.Stratum == 0)
			{
				throw new InvalidOperationException(string.Format("Received Kiss-o'-Death SNTP packet with code 0x{0:x}.", this.ReferenceId));
			}
			if (this.LeapIndicator == NtpLeapIndicator.AlarmCondition)
			{
				throw new InvalidOperationException("SNTP server has unsynchronized clock.");
			}
			this.CheckTimestamps();
		}

		
		private void CheckTimestamps()
		{
			if (this.OriginTimestamp == null)
			{
				throw new InvalidOperationException("Origin timestamp is missing.");
			}
			if (this.ReceiveTimestamp == null)
			{
				throw new InvalidOperationException("Receive timestamp is missing.");
			}
			if (this.TransmitTimestamp == null)
			{
				throw new InvalidOperationException("Transmit timestamp is missing.");
			}
			if (this.DestinationTimestamp == null)
			{
				throw new InvalidOperationException("Destination timestamp is missing.");
			}
		}

		
		private DateTime? GetDateTime64(int offset)
		{
			ulong uint64BE = this.GetUInt64BE(offset);
			if (uint64BE == 0UL)
			{
				return null;
			}
			return new DateTime?(new DateTime(NtpPacket.Epoch.Ticks + Convert.ToInt64(uint64BE * 0.0023283064365386963)));
		}

		
		private void SetDateTime64(int offset, DateTime? value)
		{
			this.SetUInt64BE(offset, (value == null) ? 0UL : Convert.ToUInt64((double)(value.Value.Ticks - NtpPacket.Epoch.Ticks) * 429.4967296));
		}

		
		private TimeSpan GetTimeSpan32(int offset)
		{
			return TimeSpan.FromSeconds((double)this.GetInt32BE(offset) / 65536.0);
		}

		
		private ulong GetUInt64BE(int offset)
		{
			return NtpPacket.SwapEndianness(BitConverter.ToUInt64(this.Bytes, offset));
		}

		
		private void SetUInt64BE(int offset, ulong value)
		{
			FastBitConverter.GetBytes(this.Bytes, offset, NtpPacket.SwapEndianness(value));
		}

		
		private int GetInt32BE(int offset)
		{
			return (int)this.GetUInt32BE(offset);
		}

		
		private uint GetUInt32BE(int offset)
		{
			return NtpPacket.SwapEndianness(BitConverter.ToUInt32(this.Bytes, offset));
		}

		
		private static uint SwapEndianness(uint x)
		{
			return (x & 255U) << 24 | (x & 65280U) << 8 | (x & 16711680U) >> 8 | (x & 4278190080U) >> 24;
		}

		
		private static ulong SwapEndianness(ulong x)
		{
			return (ulong)NtpPacket.SwapEndianness((uint)x) << 32 | (ulong)NtpPacket.SwapEndianness((uint)(x >> 32));
		}

		
		private static readonly DateTime Epoch = new DateTime(1900, 1, 1);
	}
}
