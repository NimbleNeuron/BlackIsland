using System;
using UnityEngine;

namespace Vuplex.WebView
{
	
	[Serializable]
	internal class SerializableRect
	{
		
		public Rect toRect()
		{
			return new Rect(this.left, this.top, this.width, this.height);
		}

		
		public float left = default;

		
		public float top = default;

		
		public float width = default;

		
		public float height = default;
	}
}
