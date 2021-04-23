using System;
using System.Collections;
using Blis.Common.Utils;
using UnityEngine;

namespace Blis.Client
{
	public class Popup : MonoBehaviourInstance<Popup>
	{
		public enum ButtonType
		{
			Confirm,

			Cancel,

			Link
		}


		[SerializeField] private AccessTermsUI accessTermsUI = default;


		[SerializeField] private GameObject gameGrade = default;


		[SerializeField] private DialogPopup dialogPopup = default;


		[SerializeField] private InputPopup intputPopup = default;


		[SerializeField] private ChangeKeyPopup changeKeyPopup = default;


		[SerializeField] private OneMoreAskPopup oneMoreAskPopup = default;


		[SerializeField] private GameObject gameGradeToast = default;


		[SerializeField] private GameObject blackCurtain = default;


		public AccessTermsUI AccessTermsUI => accessTermsUI;


		public GameObject GameGrade => gameGrade;


		public GameObject GameGradeToast => gameGradeToast;


		public GameObject BlackCurtain => blackCurtain;


		public ChangeKeyPopup ChangeKeyPopup => changeKeyPopup;


		public OneMoreAskPopup OneMoreAskPopup => oneMoreAskPopup;


		public bool IsOpen =>
			dialogPopup.IsOpen || intputPopup.IsOpen || changeKeyPopup.IsOpen || oneMoreAskPopup.IsOpen;


		public IEnumerator Error(string msg, Action callback = null)
		{
			if (msg.Contains("알 수 없는"))
			{
				
			}
			
			dialogPopup.Clear();
			dialogPopup.SetContentLabel(msg);
			dialogPopup.AddButton(Ln.Get("확인"), LapCallback(ButtonType.Cancel, callback));
			dialogPopup.Open();
			return dialogPopup;
		}


		public IEnumerator Message(string msg, params Button[] buttonData)
		{
			return Message(msg, null, null, buttonData);
		}


		public IEnumerator Message(string msg, DateTime time, params Button[] buttonData)
		{
			return Message(msg, null, time, buttonData);
		}


		public IEnumerator Message(string msg, Action onCloseCallback, params Button[] buttonData)
		{
			return Message(msg, onCloseCallback, null, buttonData);
		}


		public IEnumerator Message(string msg, Action onCloseCallback, DateTime? time, params Button[] buttonData)
		{
			dialogPopup.Clear();
			dialogPopup.SetContentLabel(msg);
			dialogPopup.SetTargetTime(time);
			for (int i = 0; i < buttonData.Length; i++)
			{
				dialogPopup.AddButton(buttonData[i].text, LapCallback(buttonData[i].type, buttonData[i].callback));
			}

			dialogPopup.OnCloseAction = onCloseCallback;
			dialogPopup.Open();
			return dialogPopup;
		}


		public void ShowCloseBtn()
		{
			dialogPopup.btnClose.gameObject.SetActive(true);
		}


		private Action LapCallback(ButtonType type, Action callback)
		{
			if (type <= ButtonType.Cancel)
			{
				return delegate
				{
					Action callback2 = callback;
					if (callback2 != null)
					{
						callback2();
					}

					if (dialogPopup.gameObject.activeSelf)
					{
						dialogPopup.Close();
					}
				};
			}

			return callback;
		}


		public void Input(string title, string inputMsg, string msg, Action<string> onConfirmCallback,
			params Button[] buttonData)
		{
			intputPopup.Clear();
			intputPopup.SetContentLabel(title, inputMsg, msg);
			for (int i = 0; i < buttonData.Length; i++)
			{
				intputPopup.AddButton(buttonData[i].text, LapInputCallback(buttonData[i].type, buttonData[i].callback));
			}

			intputPopup.OnConfirmAction = onConfirmCallback;
			intputPopup.Open();
		}


		private Action LapInputCallback(ButtonType type, Action callback)
		{
			if (type == ButtonType.Confirm)
			{
				return delegate
				{
					Action callback2 = callback;
					if (callback2 != null)
					{
						callback2();
					}

					if (intputPopup.gameObject.activeSelf)
					{
						intputPopup.CloseConfirm();
					}
				};
			}

			if (type != ButtonType.Cancel)
			{
				return callback;
			}

			return delegate
			{
				Action callback2 = callback;
				if (callback2 != null)
				{
					callback2();
				}

				if (intputPopup.gameObject.activeSelf)
				{
					intputPopup.CloseCancel();
				}
			};
		}


		public class Button
		{
			public Action callback;


			public string text;

			public ButtonType type;
		}
	}
}