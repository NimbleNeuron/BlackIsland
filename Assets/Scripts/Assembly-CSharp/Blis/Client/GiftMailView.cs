using Blis.Common;
using SuperScrollView;
using UnityEngine;

namespace Blis.Client
{
	public class GiftMailView : BaseUI
	{
		private LnText blank;


		private CanvasGroup canvasGroup;


		private bool initialized;


		private LoopListView2 listView;

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			GameUtil.Bind<CanvasGroup>(gameObject, ref canvasGroup);
			listView = GameUtil.Bind<LoopListView2>(gameObject, "Scroll View");
			blank = GameUtil.Bind<LnText>(gameObject, "BlankTxt");
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			InitListView();
			blank.enabled = NoticeService.GetGiftMailCount() == 0;
		}


		private void InitListView()
		{
			if (initialized)
			{
				return;
			}

			initialized = true;
			listView.InitListView(NoticeService.GetGiftMailCount(), OnGetItemByIndex);
		}


		public void Show()
		{
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}


		public void UpdateScrollView(bool resetPos)
		{
			if (initialized)
			{
				listView.SetListItemCount(NoticeService.GetGiftMailCount(), resetPos);
				listView.RefreshAllShownItem();
				blank.enabled = NoticeService.GetGiftMailCount() == 0;
			}
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (index > NoticeService.GetGiftMailCount() - 1)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("GiftMailSlot");
			loopListViewItem.GetComponent<GiftMailSlot>().SetSlot(NoticeService.GetGiftMail(index));
			return loopListViewItem;
		}


		public void Hide()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}
	}
}