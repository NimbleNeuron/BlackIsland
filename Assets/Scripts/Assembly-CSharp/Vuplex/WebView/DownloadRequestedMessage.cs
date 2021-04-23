using System;

namespace Vuplex.WebView
{
	
	[Serializable]
	internal class DownloadRequestedMessage
	{
		
		public string Url = default;

		
		public string FileName = default;

		
		public string ContentType = default;
	}
}
