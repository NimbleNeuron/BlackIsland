using System;

namespace Vuplex.WebView
{
	
	public interface IWithKeyModifiers
	{
		
		[Obsolete("The IWithKeyModifiers interface is now deprecated. Please use the IWithKeyDownAndUp interface instead.")]
		void HandleKeyboardInput(string key, KeyModifier modifiers);
	}
}
