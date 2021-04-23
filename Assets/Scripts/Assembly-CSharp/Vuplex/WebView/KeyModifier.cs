using System;

namespace Vuplex.WebView
{
	
	[Flags]
	public enum KeyModifier
	{
		
		None = 0,
		
		Shift = 1,
		
		Control = 2,
		
		Alt = 4,
		
		Meta = 8
	}
}
