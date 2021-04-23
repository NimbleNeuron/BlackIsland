using System;

namespace Vuplex.WebView
{
	
	[Serializable]
	internal class VideoRectChangedMessage : BridgeMessage
	{
		
		public VideoRectChangedMessageValue value = default;
	}
}
