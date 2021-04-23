using System;

namespace Vuplex.WebView
{
	
	public class ConsoleMessageEventArgs : EventArgs
	{
		
		public ConsoleMessageEventArgs(ConsoleMessageLevel level, string message)
		{
			this.Level = level;
			this.Message = message;
		}

		
		public readonly ConsoleMessageLevel Level;

		
		public readonly string Message;
	}
}
