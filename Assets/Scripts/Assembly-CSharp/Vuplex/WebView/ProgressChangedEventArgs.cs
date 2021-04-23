using System;

namespace Vuplex.WebView
{
	
	public class ProgressChangedEventArgs : EventArgs
	{
		
		public ProgressChangedEventArgs(ProgressChangeType type, float progress)
		{
			this.Type = type;
			this.Progress = progress;
		}

		
		public readonly float Progress;

		
		public readonly ProgressChangeType Type;
	}
}
