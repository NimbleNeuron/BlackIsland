using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	public class PointerEventArgs : EventArgs
	{
		
		public PointerOptions ToPointerOptions()
		{
			return new PointerOptions
			{
				Button = this.Button,
				ClickCount = this.ClickCount
			};
		}

		
		public MouseButton Button;

		
		public int ClickCount = 1;

		
		public Vector2 Point;
	}
}
