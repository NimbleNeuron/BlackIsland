using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Neptune.Http
{
	public class HttpResponseBody : IDisposable
	{
		private bool completed;
		private bool disposed;
		private bool computeHash;
		private HttpResponseProgress progress;
		private Func<HashAlgorithm> hashAlgorithm;
		
		private string contentType;
		private string tempFilePath;
		private string downloadToFile;
		
		private Stream stream;
		private HashAlgorithm hash;
		private long downloadedLength;
		private string responseText;
		private Texture2D responseTexture;
		private object responseJson;
		
		public string ResponseHash { get; private set; }

		public bool IsCompleted
		{
			get
			{
				return this.completed;
			}
		}

		public string ResponseText
		{
			get
			{
				if (this.responseText != null)
				{
					return this.responseText;
				}
				if (!this.completed)
				{
					return null;
				}
				if (this.downloadedLength > 2147483647L)
				{
					throw new Exception("Too large content : length=" + this.downloadedLength);
				}
				if (this.downloadedLength == 0L)
				{
					this.responseText = string.Empty;
					return this.responseText;
				}
				if (!string.IsNullOrEmpty(this.downloadToFile))
				{
					using (StreamReader streamReader = new StreamReader(this.downloadToFile, Encoding.UTF8))
					{
						this.responseText = streamReader.ReadToEnd();
						goto IL_D1;
					}
				}
				if (this.stream != null && this.stream is MemoryStream)
				{
					MemoryStream memoryStream = this.stream as MemoryStream;
					this.responseText = Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
				}
				IL_D1:
				return this.responseText;
			}
		}
		
		public virtual Texture2D ResponseTexture
		{
			get
			{
				if (this.responseTexture != null)
				{
					return this.responseTexture;
				}
				
				if (!this.completed)
				{
					return null;
				}
				
				if (this.downloadedLength == 0L)
				{
					return null;
				}
				
				if (this.downloadedLength > 2147483647L)
				{
					throw new Exception("Too large content : length=" + this.downloadedLength);
				}
				
				this.responseTexture = new Texture2D(0, 0, TextureFormat.ARGB32, false);
				if (!string.IsNullOrEmpty(this.downloadToFile))
				{
					using (FileStream fileStream = File.OpenRead(this.downloadToFile))
					{
						using (BinaryReader binaryReader = new BinaryReader(fileStream))
						{
							this.responseTexture.LoadImage(binaryReader.ReadBytes((int)this.downloadedLength));
							return responseTexture;
						}
					}
				}
				
				if (this.stream != null && this.stream is MemoryStream)
				{
					MemoryStream memoryStream = this.stream as MemoryStream;
					
					if ((long)memoryStream.GetBuffer().Length == memoryStream.Length)
					{
						this.responseTexture.LoadImage(memoryStream.GetBuffer());
					}
					else
					{
						this.responseTexture.LoadImage(memoryStream.ToArray());
					}
				}

				return this.responseTexture;
			}
		}
		
		public virtual byte[] GetBytes()
		{
			if (!string.IsNullOrEmpty(this.downloadToFile))
			{
				using (FileStream fileStream = File.OpenRead(this.downloadToFile))
				{
					using (BinaryReader binaryReader = new BinaryReader(fileStream))
					{
						return binaryReader.ReadBytes((int)this.downloadedLength);
					}
				}
			}
			return null;
		}

		public T ResponseJson<T>()
		{
			if (!this.completed)
			{
				return default(T);
			}
			if (this.downloadedLength == 0L)
			{
				return default(T);
			}
			if (this.downloadedLength > 2147483647L)
			{
				throw new Exception("Too large content : length=" + this.downloadedLength);
			}
			if (this.contentType != null && this.contentType.Contains("application/json"))
			{
				if (this.responseText != null)
				{
					this.responseJson = JsonConvert.DeserializeObject<T>(this.responseText, new JsonSerializerSettings
					{
						DateFormatString = "d MMMM, yyyy",
						Formatting = Formatting.Indented
					});
				}
				else
				{
					if (!string.IsNullOrEmpty(this.downloadToFile))
					{
						using (StreamReader streamReader = new StreamReader(this.downloadToFile, Encoding.UTF8))
						{
							this.responseText = streamReader.ReadToEnd();
							this.responseJson = JsonConvert.DeserializeObject<T>(this.responseText);

							return (T) ((object) this.responseJson);
						}
					}
					if (this.stream != null && this.stream is MemoryStream)
					{
						MemoryStream memoryStream = this.stream as MemoryStream;
						memoryStream.Seek(0L, SeekOrigin.Begin);
						using (StreamReader streamReader2 = new StreamReader(memoryStream, Encoding.UTF8))
						{
							this.responseJson = JsonConvert.DeserializeObject<T>(streamReader2.ReadToEnd());
						}
					}
				}
				
				return (T)((object)this.responseJson);
			}
			
			return default(T);
		}
		
		internal HttpResponseBody(HttpResponseProgress progress, bool computeHash, Func<HashAlgorithm> hashAlgorithm)
		{
			this.progress = progress;
			this.computeHash = computeHash;
			this.hashAlgorithm = hashAlgorithm;
		}

		~HttpResponseBody()
		{
			this.Dispose(false);
		}

		internal void DownloadToMemory(string contentType, long expectedContentLength)
		{
			if (expectedContentLength > 2147483647L)
			{
				throw new ArgumentOutOfRangeException("Too large expectedContentLength : " + expectedContentLength);
			}
			int num = (int)expectedContentLength;
			this.Create(contentType, new MemoryStream((num > 0) ? num : 0));
		}
		
		internal void DownloadToFile(string downloadTo, string tempFilePath, string contentType, bool createTempFile)
		{
			string directoryName = Path.GetDirectoryName(tempFilePath);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			this.Create(contentType, (!createTempFile) ? null : File.OpenWrite(tempFilePath));
			this.tempFilePath = tempFilePath;
			this.downloadToFile = downloadTo;
		}
		
		private void Create(string contentType, Stream stream)
		{
			this.Cancel();
			this.contentType = contentType;
			this.stream = stream;
			if (this.computeHash && this.hashAlgorithm != null)
			{
				this.hash = this.hashAlgorithm();
			}
		}
		
		internal void Write(byte[] buffer, int length)
		{
			if (this.stream != null && length > 0)
			{
				this.stream.Write(buffer, 0, length);
				this.downloadedLength += (long)length;
				if (this.hash != null)
				{
					this.hash.TransformBlock(buffer, 0, length, buffer, 0);
				}
				this.progress.OnWrite(length);
			}
		}

		internal void SetCompleted(string md5)
		{
			if (!this.completed)
			{
				if (this.hash != null)
				{
					if (this.downloadedLength > 0L)
					{
						this.hash.TransformFinalBlock(new byte[1], 0, 0);
						this.ResponseHash = BitConverter.ToString(this.hash.Hash).Replace("-", string.Empty).ToLower();
					}
					this.hash.Clear();
					this.hash = null;
				}
				else
				{
					this.ResponseHash = md5;
				}
				if (!string.IsNullOrEmpty(this.downloadToFile))
				{
					if (this.stream != null)
					{
						this.stream.Flush();
						this.stream.Dispose();
						this.stream = null;
					}
					else if (!File.Exists(this.tempFilePath))
					{
						throw new Exception("Temp file not found!");
					}
					if (File.Exists(this.downloadToFile))
					{
						File.Delete(this.downloadToFile);
					}
					Debug.Log("[HttpResponseBody] FileMove");
					File.Move(this.tempFilePath, this.downloadToFile);
				}
				this.completed = true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		private void Cancel()
		{
			try
			{
				if (this.stream != null)
				{
					this.stream.Dispose();
				}
				if (this.hash != null)
				{
					this.hash.Clear();
				}
				if (!string.IsNullOrEmpty(this.tempFilePath) && File.Exists(this.tempFilePath))
				{
					File.Delete(this.tempFilePath);
				}
			}
			catch (Exception ex)
			{
				Neptune.Log.Logger.Exception(ex);
			}
			finally
			{
				this.downloadedLength = 0L;
				this.contentType = null;
				this.tempFilePath = null;
				this.downloadToFile = null;
				this.stream = null;
				this.hash = null;
			}
		}
		
		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			try
			{
				if (!string.IsNullOrEmpty(this.tempFilePath) && File.Exists(this.tempFilePath))
				{
					if (this.stream != null)
					{
						this.stream.Dispose();
					}
					File.Delete(this.tempFilePath);
				}
			}
			catch (Exception ex)
			{
				Neptune.Log.Logger.Exception(ex);
			}
			if (disposing)
			{
				if (this.stream != null && !(this.stream is MemoryStream))
				{
					this.stream.Dispose();
				}
				if (this.hash != null)
				{
					this.hash.Clear();
				}
			}
			this.disposed = true;
		}
	}
}
