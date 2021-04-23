using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using ICSharpCode.SharpZipLib.GZip;

namespace Neptune.Http
{
	public class NetHttpRequest : HttpRequest
	{
		public NetHttpRequest() { }
		public NetHttpRequest(string method, string url) : base(method, url) { }

		protected override bool CanCache => false;
		
		protected override void InternalOpen(byte[] data, string temporaryFile)
		{
			request = WebRequest.Create(Url) as HttpWebRequest;
			request.Method = Method;
			request.Timeout = Timeout;
			request.KeepAlive = KeepAlive;
			Dictionary<string, string> requestHeaders = this.requestHeaders;
			lock (requestHeaders)
			{
				foreach (KeyValuePair<string, string> keyValuePair in this.requestHeaders)
				{
					if (keyValuePair.Key == "Content-Type")
					{
						request.ContentType = keyValuePair.Value;
					}
					else if (keyValuePair.Key == "Content-Length")
					{
						request.ContentLength = long.Parse(keyValuePair.Value);
					}
					else if (keyValuePair.Key == "User-Agent")
					{
						request.UserAgent = keyValuePair.Value;
					}
					else if (keyValuePair.Key == "Accept")
					{
						request.Accept = keyValuePair.Value;
					}
					else if (keyValuePair.Key != "Connection")
					{
						request.Headers[keyValuePair.Key] = keyValuePair.Value;
					}
				}
			}

			thread = new Thread(Worker);
			thread.Start(data);
		}

		protected override void InternalAbort()
		{
			try
			{
				if (thread != null)
				{
					if (isThreadAlive)
					{
						if (isThreadAlive)
						{
							try
							{
								Blis.Common.Log.H("***************[NetHttpRequest.InternalAbort] Thread State = " +
								                  thread.ThreadState);
								thread.Abort();
							}
							catch (Exception ex)
							{
								Blis.Common.Log.E(ex.ToString());
							}
							finally
							{
								isThreadAlive = false;
							}

							thread = null;
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Blis.Common.Log.E("[NetHttpRequest.InternalAbort] error = " + ex2.Message);
				Blis.Common.Log.E("[NetHttpRequest.InternalAbort] error stack trace = " + ex2.StackTrace);
			}
		}


		private void Worker(object data)
		{
			isThreadAlive = true;
			try
			{
				if (data != null)
				{
					byte[] array = data as byte[];
					request.GetRequestStream().Write(array, 0, array.Length);
				}

				IAsyncResult asyncResult = request.BeginGetResponse(null, null);
				OnInternelSent();
				if (!asyncResult.AsyncWaitHandle.WaitOne())
				{
					throw new TimeoutException("WebRequest.GetResponse timeout");
				}

				HttpWebResponse httpWebResponse;
				try
				{
					httpWebResponse = request.EndGetResponse(asyncResult) as HttpWebResponse;
				}
				catch (WebException ex)
				{
					if (ex.Status == WebExceptionStatus.Success)
					{
						throw ex;
					}

					httpWebResponse = ex.Response as HttpWebResponse;
					if (httpWebResponse == null)
					{
						throw ex;
					}
				}

				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (string text in httpWebResponse.Headers.AllKeys)
				{
					dictionary[text] = httpWebResponse.Headers[text];
				}

				byte[] array2 = new byte[16384];
				string a;
				bool flag;
				OnInternalHeaderReceived((int) httpWebResponse.StatusCode, dictionary, -1L, out a, out flag);
				
				if (a == "gzip")
				{
					using (ResponseStream responseStream = new ResponseStream(httpWebResponse.GetResponseStream()))
					{
						using (BufferedStream bufferedStream = new BufferedStream(responseStream, 16384))
						{
							Blis.Common.Log.V("[HttpRequest] start uncompress gzip by SharpZipLib");
							using (GZipInputStream gzipInputStream = new GZipInputStream(bufferedStream))
							{
								long num = 0L;
								int length;
								while ((length = gzipInputStream.Read(array2, 0, array2.Length)) > 0)
								{
									OnInternalReceiveData(array2, length, (int) (responseStream.Position - num));
									num = responseStream.Position;
								}
							}
						}

						OnInternalFinishLoading();
						isThreadAlive = false;
						
						return;
					}
				}

				using (Stream responseStream2 = httpWebResponse.GetResponseStream())
				{
					int num2;
					while ((num2 = responseStream2.Read(array2, 0, array2.Length)) > 0)
					{
						OnInternalReceiveData(array2, num2, num2);
					}
				}

				OnInternalFinishLoading();
			}
			catch (ThreadAbortException e)
			{
				request.Abort();
				OnInternalFailWithError(e, HttpRequestStates.Aborted);
			}
			catch (TimeoutException e2)
			{
				OnInternalFailWithError(e2, HttpRequestStates.TimedOut);
			}
			catch (Exception e3)
			{
				OnInternalFailWithError(e3);
			}
			finally
			{
				isThreadAlive = false;
			}
		}


		private const int ReceiveBufferSize = 16384;
		private const int DefaultWait = 60000;
		private volatile HttpWebRequest request;
		private Thread thread;
		private bool HighVersion;
		private bool isThreadAlive;

		private class ResponseStream : Stream
		{
			public ResponseStream(Stream stream)
			{
				this.stream = stream;
				totalRead = 0L;
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				int num = stream.Read(buffer, offset, count);
				totalRead += num;
				return num;
			}

			public override void Close()
			{
				if (stream != null)
				{
					stream.Close();
					stream = null;
				}

				GC.SuppressFinalize(this);
			}

			public override void Flush()
			{
				throw new NotImplementedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotImplementedException();
			}

			public override void SetLength(long value)
			{
				throw new NotImplementedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}

			public override bool CanRead => true;
			public override bool CanSeek => false;
			public override bool CanWrite => false;
			public override long Length => throw new NotImplementedException();

			public override long Position {
				get => totalRead;
				set => throw new NotImplementedException();
			}

			private Stream stream;
			private long totalRead;
		}
	}
}