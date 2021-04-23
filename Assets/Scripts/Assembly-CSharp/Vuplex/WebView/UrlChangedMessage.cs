using System;

namespace Vuplex.WebView
{
	
	[Serializable]
	internal class UrlChangedMessage : BridgeMessage
	{
		
		public UrlAction urlAction= default;
	}
}
