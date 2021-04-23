using System;
using System.Collections;
using System.Collections.Generic;
using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class DialogPopup : BasePopup, IEnumerator
	{
		[SerializeField] private Text contentLabel = default;


		[SerializeField] private List<PopupButton> buttons = default;


		public Button btnClose = default;


		private float lastUpdateTime = 1f;


		private string messageText = default;


		public Action OnCloseAction = default;


		private float openTime = default;


		private DateTime? targetTime = default;


		private void Update()
		{
			if (targetTime == null)
			{
				return;
			}

			lastUpdateTime += Time.deltaTime;
			if (lastUpdateTime >= 1f)
			{
				contentLabel.text = LnUtil.GetMaintenanceRemainTimeText(messageText,
					TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
						MonoBehaviourInstance<LobbyClient>.inst.timezoneInfo),
					TimeZoneInfo.ConvertTimeFromUtc(targetTime.Value,
						MonoBehaviourInstance<LobbyClient>.inst.timezoneInfo));
			}
		}


		public bool MoveNext()
		{
			return IsOpen;
		}

#pragma warning disable CS0114

		public void Reset() { }
#pragma warning restore CS0114


		public object Current => null;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			Clear();
			btnClose.onClick.AddListener(Close);
		}


		public void Clear()
		{
			contentLabel.text = null;
			buttons.ForEach(delegate(PopupButton x) { x.gameObject.SetActive(false); });
		}


		public void SetContentLabel(string text)
		{
			messageText = text;
			contentLabel.text = text;
		}


		public void SetTargetTime(DateTime? targetTime)
		{
			this.targetTime = targetTime;
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


		public override void Open()
		{
			openTime = Time.time;
			base.Open();
		}


		protected override void OnClose()
		{
			base.OnClose();
			Clear();
			Action onCloseAction = OnCloseAction;
			if (onCloseAction != null)
			{
				onCloseAction();
			}

			OnCloseAction = null;
			btnClose.gameObject.SetActive(false);
		}


		public override void Close()
		{
			if (openTime == Time.time)
			{
				return;
			}

			base.Close();
		}
	}
}