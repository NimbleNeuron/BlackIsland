using System;

namespace Vuplex.WebView
{
	
	[Serializable]
	public class UrlAction
	{
		
		public UrlAction()
		{
		}

		
		public UrlAction(string url, string title, string type)
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
