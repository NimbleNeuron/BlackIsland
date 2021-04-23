using System;

namespace Vuplex.WebView
{
	
	public class AuthRequestedEventArgs : EventArgs
	{
		
		public AuthRequestedEventArgs(string host, Action<string, string> continueCallback, Action cancelCallback)
		{
			this.Host = host;
			this._continueCallback = continueCallback;
			this._cancelCallback = cancelCallback;
		}

		
		public void Cancel()
		{
			this._cancelCallback();
		}

		
		public void Continue(string username, string password)
		{
			this._continueCallback(username, password);
		}

		
		public readonly string Host;

		
		private Action _cancelCallback;

		
		private Action<string, string> _continueCallback;
	}
}
