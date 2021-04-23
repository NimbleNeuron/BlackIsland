using UnityEngine;

namespace Vuplex.WebView
{
	
	public interface IWithPointerDownAndUp
	{
		
		void PointerDown(Vector2 point);

		
		void PointerDown(Vector2 point, PointerOptions options);

		
		void PointerUp(Vector2 point);

		
		void PointerUp(Vector2 point, PointerOptions options);
	}
}
