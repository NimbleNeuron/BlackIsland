using System;
using Blis.Common.Utils;

namespace Blis.Client
{
	public class MainCameraEvent : MonoBehaviourInstance<MainCameraEvent>
	{
		public void OnPreRender()
		{
			Action onPreRenderEvent = OnPreRenderEvent;
			if (onPreRenderEvent != null)
			{
				onPreRenderEvent();
			}

			Action onLatePreRenderEvent = OnLatePreRenderEvent;
			if (onLatePreRenderEvent == null)
			{
				return;
			}

			onLatePreRenderEvent();
		}

		
		
		public event Action OnPreRenderEvent = delegate { };


		
		
		public event Action OnLatePreRenderEvent = delegate { };
	}
}