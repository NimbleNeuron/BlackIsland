using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public class ClickedEventArgs : EventArgs
	{
		
		public ClickedEventArgs(Vector2 point)
		{
			this.Point = point;
		}

		
		public readonly Vector2 Point;
	}
}
