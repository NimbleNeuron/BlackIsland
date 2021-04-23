using System;

namespace Vuplex.WebView
{
	
	public interface IWithPopups
	{
		
		void SetPopupMode(PopupMode popupMode);

		
		
		
		event EventHandler<PopupRequestedEventArgs> PopupRequested;
	}
}
