using System;
using UnityEngine.EventSystems;

namespace Blis.Client
{
	public class UIResourceText : BaseTrackUI, IPointerClickHandler, IEventSystemHandler
	{
		private Action callback;


		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button == PointerEventData.InputButton.Left)
			{
				Action action = callback;
				if (action == null)
				{
					return;
				}

				action();
			}
		}

		public void SetCallback(Action callback)
		{
			this.callback = callback;
		}
	}
}