using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Blis.Client;
using Blis.Common;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Neptune.Http
{
	public abstract class HttpRequest : IEnumerator, IDisposable
	{
		protected abstract void InternalOpen(byte[] data, string temporaryFile);
		
		protected abstract void InternalAbort();
		
		public string TemporaryPath;
		public const int DefaultTimeout = 30000;
		public const int DefaultMaxRetry = 1;
		private static int temporaryFileIndex;
		private bool disposed;
		private bool keepAlive = true;
		private bool useCaches;
		
		protected HttpRequestStates state;
		protected Dictionary<string, string> requestHeaders = new Dictionary<string, string>();

		protected volatile HttpRequestStates internalState;
		private volatile HttpResponseBody internalResponseBody;
		private volatile int statusCode;
		private volatile Dictionary<string, string> responseHeaders;
		
		protected HttpResponseProgress responseProgress = new HttpResponseProgress();
		private byte[] requestBody;
		private int retried;
		private DateTime retryAfter = DateTime.MaxValue;
		private HttpResponseBody responseBody;
		private string temporaryFile;
		private string downloadToFile;

		public delegate void StateChangeHandler(HttpRequest sender);
		
		public byte[] RequestBody => requestBody;

		// public string RequestContentType {
		// 	get
		// 	{
		// 		Dictionary<string, string> obj = requestHeaders;
		// 		string orDefault;
		// 		lock (obj)
		// 		{
		// 			orDefault = requestHeaders.GetOrDefault("Content-Type");
		// 		}
		//
		// 		return orDefault;
		// 	}
		// }

		private event StateChangeHandler onStateChange;

		public event StateChangeHandler OnStateChange {
			add
			{
				if (State.IsFinal())
				{
					value(this);
					return;
				}

				onStateChange += value;
			}
			remove
			{
				if (onStateChange != null)
				{
					onStateChange -= value;
				}
			}
		}

		public HttpRequestStates State {
			get => state;
			protected set
			{
				if (state != value)
				{
					if (value == HttpRequestStates.Done)
					{
						responseBody = internalResponseBody;
						internalResponseBody = null;
					}
					else if (value.IsFinal())
					{
						requestBody = null;
					}

					state = value;
					
					if (onStateChange != null)
					{
						try
						{
							onStateChange(this);
						}
						catch (Exception e)
						{
							Blis.Common.Log.Exception(e);
						}
						finally
						{
							if (state.IsFinal())
							{
								Dispose();
								onStateChange = null;
							}
						}
					}
				}
			}
		}

		public string Method { get; protected set; }
		public string Url { get; protected set; }
		public int Timeout { get; set; }
		public int MaxRetry { get; set; }
		public bool DecompressGZip { get; set; }
		public bool ComputeHash { get; set; }

		public bool KeepAlive {
			get => keepAlive;
			set
			{
				if (state != HttpRequestStates.Opened)
				{
					throw new Exception("Not opened state : " + state);
				}

				keepAlive = value && CanKeepAlive;
			}
		}

		public bool UseCaches {
			get => useCaches;
			set
			{
				if (state != HttpRequestStates.Opened)
				{
					throw new Exception("Not opened state : " + state);
				}

				useCaches = value && CanCache;
			}
		}

		public Exception Exception { get; protected set; }

		public HttpResultStates ResultState {
			get
			{
				if (State.IsFinal())
				{
					switch (State)
					{
						case HttpRequestStates.Done:
							if (StatusCode >= 200 && StatusCode < 300)
							{
								return HttpResultStates.OK;
							}

							if (StatusCode == 404 && Method == "DELETE")
							{
								return HttpResultStates.OK;
							}

							if (StatusCode >= 300 && StatusCode < 400)
							{
								return HttpResultStates.Redirected;
							}

							if (StatusCode >= 400 && StatusCode < 500)
							{
								return HttpResultStates.ClientError;
							}

							if (StatusCode >= 500 && StatusCode < 9000)
							{
								return HttpResultStates.ServerError;
							}

							if (StatusCode >= 9000)
							{
								return HttpResultStates.OK;
							}

							break;
						case HttpRequestStates.Aborted:
							return HttpResultStates.Aborted;
						case HttpRequestStates.Error:
							return HttpResultStates.Exception;
						case HttpRequestStates.TimedOut:
							return HttpResultStates.TimedOut;
					}

					return HttpResultStates.ServerError;
				}

				return HttpResultStates.NotFinished;
			}
		}

		public object Tag { get; set; }

		public int StatusCode => statusCode;

		public long ExpectedResponseLength => responseProgress.ExpectedLength;

		public long TotalRead => responseProgress.TotalRead;

		public string ContentType => GetResponseHeader("Content-Type");

		public string ContentEncoding => GetResponseHeader("Content-Encoding");

		public long ContentLength {
			get
			{
				string responseHeader = GetResponseHeader("Content-Length");
				if (responseHeader != null)
				{
					return long.Parse(responseHeader);
				}

				return -1L;
			}
		}

		public string ResponseHash {
			get
			{
				if (responseBody != null)
				{
					return responseBody.ResponseHash;
				}

				return null;
			}
		}

		public virtual string ResponseText {
			get
			{
				if (responseBody != null)
				{
					return responseBody.ResponseText;
				}

				return null;
			}
		}

		public virtual Texture2D ResponseTexture {
			get
			{
				if (responseBody == null)
				{
					return null;
				}

				Texture2D responseTexture = responseBody.ResponseTexture;
				if (responseTexture != null)
				{
					responseTexture.name = Url;
				}

				return responseTexture;
			}
		}

		public byte[] ResponseBytes {
			get
			{
				if (responseBody == null)
				{
					return null;
				}

				return responseBody.GetBytes();
			}
		}

		public T ResponseJson<T>()
		{
			return responseBody.ResponseJson<T>();
		}

		public HttpRequest()
		{
			useCaches = CanCache;
			Timeout = 30000;
			MaxRetry = 1;
			DecompressGZip = true;
			state = HttpRequestStates.Unsent;
		}

		public HttpRequest(string method, string url) : this()
		{
			Open(method, url);
		}

		~HttpRequest()
		{
			Dispose(false);
		}


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		public void Open(string method, string url)
		{
			if (state != HttpRequestStates.Unsent)
			{
				throw new Exception("Not openable state : " + state);
			}

			Method = method.ToUpper();
			Url = url;
			State = HttpRequestStates.Opened;
		}


		public void SetRequestHeader(string header, string value)
		{
			if (state != HttpRequestStates.Opened)
			{
				throw new Exception("Not opened state : " + state);
			}

			if (header.Equals("Connection"))
			{
				throw new Exception("Use KeepAlive property.");
			}

			Dictionary<string, string> obj = requestHeaders;
			lock (obj)
			{
				requestHeaders[header] = value;
			}
		}


		public bool RemoveRequestHeader(string header)
		{
			if (state != HttpRequestStates.Opened)
			{
				throw new Exception("Not opened state : " + state);
			}

			Dictionary<string, string> obj = requestHeaders;
			bool result;
			lock (obj)
			{
				result = requestHeaders.Remove(header);
			}

			return result;
		}


		public void Send()
		{
			Send(null);
		}


		public void Send(byte[] data, string contentType)
		{
			SetRequestHeader("Content-Type", contentType);
			Send(data);
		}


		public void Send(string data, string contentType)
		{
			Send(Encoding.UTF8.GetBytes(data), contentType + "; charset=utf-8");
		}


		public void SendForm(WWWForm form)
		{
			Send(form.data, form.headers["Content-Type"]);
		}


		public void SendForm(params object[] param)
		{
			SendForm(new KeyValueList(param).ToForm());
		}


		public void SendObject(object data)
		{
			if (data == null)
			{
				Send();
				return;
			}

			if (data is WWWForm)
			{
				SendForm(data as WWWForm);
				return;
			}

			if (data is string)
			{
				SendString(data as string);
				return;
			}

			SendJson(data);
		}


		public void SendJson(object data)
		{
			Send(JsonConvert.SerializeObject(data), "application/json");
		}


		public void SendString(string data)
		{
			Send(data, "application/json");
		}


		public void SendJson(params object[] param)
		{
			SendJson(new KeyValueList(param).ToHashtable());
		}


		public void Send(byte[] data)
		{
			if (state != HttpRequestStates.Opened)
			{
				throw new Exception("Not opened state : " + state);
			}

			try
			{
				Dictionary<string, string> obj = requestHeaders;
				lock (obj)
				{
					requestHeaders["Connection"] = KeepAlive ? "Keep-Alive" : "Close";
					if (data != null)
					{
						requestHeaders["Content-Length"] = data.Length.ToString();
					}
				}

				requestBody = data;
				State = HttpRequestStates.Sending;
				internalState = State;
				responseProgress.OnBegin(Timeout);
				InternalOpen(requestBody, temporaryFile);
				SingletonMonoBehaviour<HttpRequestUpdater>.Instance.UpdateState(this, UpdateState());
			}
			catch (Exception exception)
			{
				Dispose();
				Exception = exception;
				State = HttpRequestStates.Error;
			}
		}


		public string GetTemporaryPath(string filepath)
		{
			return Path.Combine(Path.GetDirectoryName(filepath), "http-file-cache");
		}


		public void DownloadTo(string filePath)
		{
			if (state != HttpRequestStates.Opened)
			{
				throw new Exception("Not downloadable state : " + state);
			}

			string fileName = Path.GetFileName(filePath);
			TemporaryPath = GetTemporaryPath(filePath);
			downloadToFile = filePath;
			temporaryFile = Path.Combine(TemporaryPath,
				string.Format("{0}.{1}", fileName, ++temporaryFileIndex));
			MaxRetry = 0;
			Send();
		}


		public void Abort()
		{
			if (state.IsFinal())
			{
				throw new Exception("Not abortable state : " + state);
			}

			Abort(HttpRequestStates.Aborted);
		}


		public string GetResponseHeader(string header)
		{
			string result;
			if (responseHeaders != null && responseHeaders.TryGetValue(header, out result))
			{
				CurrencyApi.CheckChangeCurrency(responseHeaders);
				return result;
			}

			return null;
		}


		public IDictionary<string, string> GetAllResponseHeaders()
		{
			return responseHeaders;
		}


		public object Current => null;


		public bool MoveNext()
		{
			return !state.IsFinal();
		}


		public void Reset()
		{
			throw new NotImplementedException();
		}

		private void Dispose(bool disposing)
		{
			if (disposed)
			{
				return;
			}

			InternalAbort();
			if (disposing && responseBody != null)
			{
				responseBody.Dispose();
			}

			disposed = true;
		}

		private void Abort(HttpRequestStates state)
		{
			InternalAbort();
			if (!State.IsFinal())
			{
				if (responseBody != null)
				{
					responseBody.Dispose();
					responseBody = null;
				}

				State = state;
			}
		}

		protected virtual IEnumerator UpdateState()
		{
			while (!internalState.IsFinal() && !State.IsFinal())
			{
				if (State != internalState && internalState == HttpRequestStates.Retrying)
				{
					float num = 0.5f * Mathf.Pow(1.5f, retried);
					float num2 = 0.5f * num;
					float num3 = Random.Range(num - num2, num + num2);
					retryAfter = DateTime.UtcNow.AddSeconds(num3);
				}

				State = internalState;
				HttpRequestStates httpRequestStates = State;
				if (httpRequestStates != HttpRequestStates.Unsent)
				{
					if (httpRequestStates == HttpRequestStates.Retrying)
					{
						Retry();
					}
					else
					{
						UpdateProgress();
						if (responseProgress.IsTimedOut)
						{
							if (!IsRetryable)
							{
								Abort(HttpRequestStates.TimedOut);
								continue;
							}

							InternalAbort();
						}
					}
				}

				yield return null;
			}

			if (!State.IsFinal())
			{
				State = internalState;
			}

			while (!internalState.IsFinal())
			{
				yield return null;
			}
		}


		private bool IsRetryable =>
			retried < MaxRetry && (Method == "GET" || Method == "PUT" ||
			                       Method == "DELETE" || Method == "HEAD" ||
			                       Method == "OPTIONS");


		protected abstract bool CanCache { get; }

		protected virtual bool CanKeepAlive => true;

		protected virtual bool CanComputeMD5 => false;

		protected virtual bool CanWriteToFile => false;

		protected virtual void UpdateProgress() { }


		private void Retry()
		{
			try
			{
				if (State == HttpRequestStates.Retrying && DateTime.UtcNow >= retryAfter)
				{
					retried++;
					retryAfter = DateTime.MaxValue;
					statusCode = 0;
					responseHeaders = null;
					responseProgress = new HttpResponseProgress();
					State = HttpRequestStates.Sending;
					internalState = State;
					responseProgress.OnBegin(Timeout);
					InternalOpen(requestBody, temporaryFile);
				}
			}
			catch (Exception exception)
			{
				Exception = exception;
				State = HttpRequestStates.Error;
			}
		}


		protected void OnInternelSent()
		{
			internalState = HttpRequestStates.Sent;
		}


		protected void OnInternalHeaderReceived(int statusCode, Dictionary<string, string> responseHeaders,
			long expectedContentLength, out string encoding, out bool writeToFile)
		{
			bool flag = statusCode >= 200 && statusCode < 300;
			this.statusCode = statusCode;
			this.responseHeaders = responseHeaders;
			responseProgress.OnHeader(ContentLength, expectedContentLength, ContentEncoding);
			encoding = string.Empty;
			writeToFile = false;
			if (Method != "HEAD")
			{
				if (internalResponseBody == null)
				{
					internalResponseBody = new HttpResponseBody(responseProgress,
						ComputeHash && !CanComputeMD5, MD5.Create);
				}

				if (flag && !string.IsNullOrEmpty(downloadToFile))
				{
					writeToFile = true;
					internalResponseBody.DownloadToFile(downloadToFile, temporaryFile, ContentType,
						!CanWriteToFile);
				}
				else
				{
					internalResponseBody.DownloadToMemory(ContentType, expectedContentLength);
				}

				if (DecompressGZip && ((ContentType ?? string.Empty).ToLower().Contains("application/gzip") ||
				                       flag && Url.ToLower().EndsWith(".gz")))
				{
					encoding = "gzip";
				}
			}

			internalState = HttpRequestStates.HeaderReceived;
		}

		protected void OnInternalReceiveData(byte[] data, int length, int read)
		{
			responseProgress.OnRead(read);
			if (internalResponseBody != null)
			{
				internalResponseBody.Write(data, length);
			}

			internalState = HttpRequestStates.Loading;
		}
		
		protected void OnInternalFinishLoading(string md5 = "")
		{
			try
			{
				if (internalResponseBody != null)
				{
					internalResponseBody.SetCompleted(md5);
				}

				responseProgress.OnComplete();
				internalState = HttpRequestStates.Done;
			}
			catch (Exception e)
			{
				OnInternalFailWithError(e, HttpRequestStates.Error, false);
			}
		}
		
		protected void OnInternalFailWithError(Exception e, HttpRequestStates state = HttpRequestStates.Error,
			bool retry = true)
		{
			responseProgress.OnComplete();
			if (internalResponseBody != null)
			{
				internalResponseBody.Dispose();
			}

			internalResponseBody = null;
			if (retry && IsRetryable)
			{
				internalState = HttpRequestStates.Retrying;
				return;
			}

			Exception = e;
			internalState = e is TimeoutException ? HttpRequestStates.TimedOut : state;
		}
	}
}