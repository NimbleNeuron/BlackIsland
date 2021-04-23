using UnityEngine;

namespace Common.Utils
{
	
	public class RendererEvent : MonoBehaviour
	{
		
		public void SetEventHandler(IRendererEventHandler handler)
		{
			this.handler = handler;
		}

		
		private void OnBecameVisible()
		{
			IRendererEventHandler rendererEventHandler = this.handler;
			if (rendererEventHandler == null)
			{
				return;
			}
			rendererEventHandler.OnVisible();
		}

		
		private void OnBecameInvisible()
		{
			IRendererEventHandler rendererEventHandler = this.handler;
			if (rendererEventHandler == null)
			{
				return;
			}
			rendererEventHandler.OnInvisible();
		}

		
		private IRendererEventHandler handler;
	}
}
