using System;

namespace Vuplex.WebView
{
	
	public class UrlChangedEventArgs : EventArgs
	{
		
		public UrlChangedEventArgs(string url, string title, string type)
		{
			this.Url = url;
			this.Title = title;
			this.Type = type;
		}

		
		public string Url;

		
		public string Title;

		
		public string Type;
	}
}
