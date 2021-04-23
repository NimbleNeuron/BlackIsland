using System;

namespace Vuplex.WebView
{
	
	public class DownloadRequestedEventArgs : EventArgs
	{
		
		public DownloadRequestedEventArgs(string url, string fileName, string contentType)
		{
			this.Url = url;
			this.FileName = fileName;
			this.ContentType = contentType;
		}

		
		public readonly string Url;

		
		public readonly string FileName;

		
		public readonly string ContentType;
	}
}
