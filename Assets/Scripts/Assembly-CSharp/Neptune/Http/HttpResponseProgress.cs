using System;

namespace Neptune.Http
{
	public class HttpResponseProgress
	{
		private int timeout;
		private DateTime lastDateTime = DateTime.UtcNow;
		
		public bool Loading { get; private set; }
		public bool Loaded { get; private set; }
		public long ContentLength { get; private set; }
		public long ExpectedLength { get; private set; }
		public long TotalRead { get; private set; }
		public long TotalWrite { get; private set; }
		public bool IsTimedOut
		{
			get
			{
				return this.timeout > 0 && this.Loading && !(DateTime.UtcNow - this.lastDateTime < TimeSpan.FromMilliseconds((double)this.timeout));
			}
		}
		
		internal HttpResponseProgress()
		{
			this.Loading = false;
			this.Loaded = false;
			this.ContentLength = -1L;
			this.ExpectedLength = -1L;
			this.TotalRead = 0L;
			this.TotalWrite = 0L;
		}
		
		internal void OnBegin(int timeout)
		{
			this.timeout = timeout;
			this.Loading = true;
			this.UpdateDateTime();
		}

		internal void OnSent()
		{
			this.UpdateDateTime();
		}
		
		internal void OnHeader(long contentLength, long expectedLength, string contentEncoding)
		{
			contentEncoding = (contentEncoding ?? "identity").ToLower();
			if ("gzip" == contentEncoding || "deflate" == contentEncoding)
			{
				if (contentLength > 0L && expectedLength <= contentLength)
				{
					expectedLength = -1L;
				}
			}
			else
			{
				expectedLength = contentLength;
			}
			
			this.ContentLength = contentLength;
			this.ExpectedLength = expectedLength;
			this.TotalRead = 0L;
			this.TotalWrite = 0L;
			this.UpdateDateTime();
		}

		internal void OnRead(int length)
		{
			this.TotalRead += (long)length;
			this.UpdateDateTime();
		}
		
		internal void OnWrite(int length)
		{
			this.TotalWrite += (long)length;
			this.UpdateDateTime();
		}

		internal void OnReadTotal(long length)
		{
			if (length > this.TotalRead)
			{
				this.TotalRead = length;
				this.UpdateDateTime();
			}
		}
		
		internal void OnWriteTotal(long length)
		{
			if (length > this.TotalWrite)
			{
				this.TotalWrite = length;
				this.UpdateDateTime();
			}
		}

		internal void OnComplete()
		{
			this.Loading = false;
			this.UpdateDateTime();
		}
		
		internal void UpdateDateTime()
		{
			this.lastDateTime = DateTime.UtcNow;
		}
	}
}
