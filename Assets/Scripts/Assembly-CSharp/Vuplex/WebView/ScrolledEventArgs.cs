using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public class ScrolledEventArgs : EventArgs
	{
		
		public ScrolledEventArgs(Vector2 scrollDelta, Vector2 point)
		{
			this.ScrollDelta = scrollDelta;
			this.Point = point;
		}

		
		public readonly Vector2 ScrollDelta;

		
		public readonly Vector2 Point;
	}
}
