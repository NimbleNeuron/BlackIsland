using System;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class UIDropRect : BaseUI, IDropHandler, IEventSystemHandler
	{
		public void OnDrop(PointerEventData eventData)
		{
			OnDropHandler(DraggingUI, eventData);
		}

		
		
		public event Action<BaseUI, PointerEventData> OnDropHandler = delegate { };
	}
}