using System;

namespace Vuplex.WebView
{
	
	public class PopupRequestedEventArgs : EventArgs
	{
		
		public PopupRequestedEventArgs(string url, IWebView webView)
		{
			this.Url = url;
			this.WebView = webView;
		}

		
		public readonly string Url;

		
		public readonly IWebView WebView;
	}
}
