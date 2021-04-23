using System;

namespace Vuplex.WebView
{
	
	[Serializable]
	public class StringWithIdBridgeMessage : BridgeMessage
	{
		
		public string id;

		
		public string value;
	}
}
