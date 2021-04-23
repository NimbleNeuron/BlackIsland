using Blis.Common;
using Blis.Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Blis.Client
{
	public class NoticeHUD : BaseUI
	{
		public enum NoticeViewType
		{
			NOTICE,

			GIFTMAIL
		}


		private CanvasGroup canvasGroup;


		private GiftMailView giftMailView;


		private Image giftRedDot;


		private Toggle giftToggle;


		private Image noticeRedDot;


		private Toggle noticeToggle;


		private NoticeView noticeView;


		private ToggleGroup toggleGroup;


		private NoticeViewType viewType;


		public NoticeViewType ViewType => viewType;


		public bool IsOpen => canvasGroup.interactable;


		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			toggleGroup = GameUtil.Bind<ToggleGroup>(gameObject, "Tab");
			noticeToggle = GameUtil.Bind<Toggle>(toggleGroup.gameObject, "Notice");
			giftToggle = GameUtil.Bind<Toggle>(toggleGroup.gameObject, "Gift");
			noticeRedDot = GameUtil.Bind<Image>(noticeToggle.gameObject, "RedDot");
			giftRedDot = GameUtil.Bind<Image>(giftToggle.gameObject, "RedDot");
			noticeView = GameUtil.Bind<NoticeView>(gameObject, "NoticeView");
			giftMailView = GameUtil.Bind<GiftMailView>(gameObject, "GiftMailView");
			noticeToggle.onValueChanged.AddListener(OnNoticeToggleValueChange);
			giftToggle.onValueChanged.AddListener(OnGiftToggleValueChange);
			noticeToggle.isOn = true;
			noticeRedDot.enabled = false;
			giftRedDot.enabled = false;
			viewType = NoticeViewType.NOTICE;
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
		}


		public void Open()
		{
			if (IsOpen)
			{
				return;
			}

			Show();
			RefreshView();
			NoticeService.RequestAll(delegate { UpdateScrollView(true); });
		}


		public void Close()
		{
			if (!IsOpen)
			{
				return;
			}

			Hide();
		}


		private void Show()
		{
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}


		private void Hide()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}


		private void RefreshView()
		{
			NoticeViewType noticeViewType = viewType;
			if (noticeViewType == NoticeViewType.NOTICE)
			{
				noticeView.Show();
				giftMailView.Hide();
				noticeRedDot.enabled = false;
				return;
			}

			if (noticeViewType != NoticeViewType.GIFTMAIL)
			{
				return;
			}

			giftMailView.Show();
			noticeView.Hide();
			giftRedDot.enabled = false;
		}


		public void UpdateScrollView(bool resetPos)
		{
			NoticeViewType noticeViewType = viewType;
			if (noticeViewType != NoticeViewType.NOTICE)
			{
				if (noticeViewType == NoticeViewType.GIFTMAIL)
				{
					NoticeService.CheckingGiftMail();
					giftMailView.UpdateScrollView(resetPos);
					noticeRedDot.enabled = NoticeService.AnyNewNotice();
					MonoBehaviourInstance<LobbyUI>.inst.MainMenu.EnableNoticeRedDot(noticeRedDot.enabled);
				}
			}
			else
			{
				NoticeService.CheckingNotice();
				noticeView.UpdateScrollView(resetPos);
				giftRedDot.enabled = NoticeService.AnyNewGiftMail();
				MonoBehaviourInstance<LobbyUI>.inst.MainMenu.EnableNoticeRedDot(giftRedDot.enabled);
			}

			MonoBehaviourInstance<LobbyUI>.inst.MainMenu.EnableNoticeRedDot(noticeRedDot.enabled || giftRedDot.enabled);
		}


		private void OnNoticeToggleValueChange(bool isOn)
		{
			if (isOn)
			{
				viewType = NoticeViewType.NOTICE;
				RefreshView();
				UpdateScrollView(true);
			}
		}


		private void OnGiftToggleValueChange(bool isOn)
		{
			if (isOn)
			{
				viewType = NoticeViewType.GIFTMAIL;
				RefreshView();
				UpdateScrollView(true);
			}
		}
	}
}