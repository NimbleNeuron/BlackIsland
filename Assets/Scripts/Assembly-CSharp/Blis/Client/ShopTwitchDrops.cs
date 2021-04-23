using Blis.Common;
using SuperScrollView;
using UnityEngine;

namespace Blis.Client
{
	public class ShopTwitchDrops : BasePage, ILnEventHander
	{
		private GameObject blank;


		private bool initialized;


		private LoopListView2 listView;


		protected override void OnDestroy()
		{
			base.OnDestroy();
		}


		public void OnLnDataChange()
		{
			UpdateScrollView(true);
		}

		protected override void OnAwakeUI()
		{
			base.OnAwakeUI();
			listView = GameUtil.Bind<LoopListView2>(gameObject, "ProductScrollView/Scroll View");
			blank = GameUtil.Bind<RectTransform>(gameObject, "ProductScrollView/Blank").gameObject;
		}


		protected override void OnStartUI()
		{
			base.OnStartUI();
			InitListView();
		}


		private void InitListView()
		{
			if (initialized)
			{
				return;
			}

			initialized = true;
			listView.InitListView(ShopTwitchDropsService.GetTwitchDropsCount(), OnGetItemByIndex);
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			ShopTwitchDropsService.RequestTwitchDropsList(delegate { UpdateScrollView(true); });
		}


		public void UpdateScrollView(bool resetPos)
		{
			if (initialized)
			{
				listView.SetListItemCount(ShopTwitchDropsService.GetTwitchDropsCount(), resetPos);
				listView.RefreshAllShownItem();
				blank.SetActive(ShopTwitchDropsService.GetTwitchDropsCount() == 0);
			}
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (index > ShopTwitchDropsService.GetTwitchDropsCount() - 1)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("TwitchDropsSlot");
			loopListViewItem.GetComponent<TwitchDropsSlot>().SetSlot(ShopTwitchDropsService.GetTwitchDrops(index));
			return loopListViewItem;
		}
	}
}