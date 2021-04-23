using System;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class PopupButton : MonoBehaviour
	{
		[SerializeField] private Button button = default;


		[SerializeField] private Text text = default;

		public void SetText(string text)
		{
			this.text.text = text;
		}


		public void SetEvent(Action action)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate
			{
				Action action2 = action;
				if (action2 == null)
				{
					return;
				}

				action2();
			});
		}
	}
}