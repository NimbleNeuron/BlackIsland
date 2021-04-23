using System;

namespace Vuplex.WebView
{
	
	internal class WebViewUnavailableException : Exception
	{
		
		public WebViewUnavailableException(string message) : base(message)
		{
		}
	}
}
