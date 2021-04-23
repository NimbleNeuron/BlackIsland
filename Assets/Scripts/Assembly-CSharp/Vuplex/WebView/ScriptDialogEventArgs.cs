using System;

namespace Vuplex.WebView
{
	
	public class ScriptDialogEventArgs : EventArgs
	{
		
		public ScriptDialogEventArgs(string message, Action continueCallback)
		{
			this.Message = message;
			this.Continue = continueCallback;
		}

		
		public readonly string Message;

		
		public readonly Action Continue;
	}
}
