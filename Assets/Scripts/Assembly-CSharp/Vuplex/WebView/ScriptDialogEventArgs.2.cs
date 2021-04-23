using System;

namespace Vuplex.WebView
{
	
	public class ScriptDialogEventArgs<T> : EventArgs
	{
		
		public ScriptDialogEventArgs(string message, Action<T> continueCallback)
		{
			this.Message = message;
			this.Continue = continueCallback;
		}

		
		public readonly string Message;

		
		public readonly Action<T> Continue;
	}
}
