using System;
using System.Collections.Generic;
using Blis.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class InputPopup : BasePopup
	{
		[SerializeField] private InputField inputField = default;


		[SerializeField] private LnText inputDec = default;


		[SerializeField] private LnText title = default;


		[SerializeField] private LnText dec = default;


		[SerializeField] private List<PopupButton> buttons = default;


		public Action<string> OnConfirmAction;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			enableBackShadeEvent = false;
		}


		public void Clear()
		{
			inputField.text = null;
			buttons.ForEach(delegate(PopupButton x) { x.gameObject.SetActive(false); });
		}


		public void SetContentLabel(string title, string inputDec, string dec)
		{
			this.inputDec.text = inputDec;
			this.title.text = title;
			this.dec.text = dec;
		}


		public void AddButton(string text, Action callback)
		{
			PopupButton popupButton = buttons.Find(x => !x.gameObject.activeSelf);
			if (popupButton == null)
			{
				Log.E("Not enough Button");
				return;
			}

			popupButton.SetText(text);
			popupButton.SetEvent(callback);
			popupButton.gameObject.SetActive(true);
		}


		protected override void OnClose()
		{
			base.OnClose();
			OnConfirmAction = null;
			Clear();
		}


		public void CloseConfirm()
		{
			if (!string.IsNullOrEmpty(inputField.text))
			{
				Action<string> onConfirmAction = OnConfirmAction;
				if (onConfirmAction != null)
				{
					onConfirmAction(inputField.text);
				}
			}

			Close();
		}


		public void CloseCancel()
		{
			Close();
		}
	}
}