using Blis.Common;
using SuperScrollView;
using UnityEngine;

namespace Blis.Client
{
	public class ShopDLC : BasePage, ILnEventHander
	{
		private GameObject blank;


		private bool initialized;


		private float lastRequestTime = float.MinValue;


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
			listView.InitListView(ShopProductService.GetPurchasedDLCCount(), OnGetItemByIndex);
		}


		protected override void OnOpenPage()
		{
			base.OnOpenPage();
			if (Time.time - lastRequestTime < ShopProductService.RefreshTime)
			{
				UpdateScrollView(true);
				return;
			}

			lastRequestTime = Time.time;
			ShopProductService.RequestPurchasedDLCList(delegate { UpdateScrollView(true); });
		}


		public void UpdateScrollView(bool resetPos)
		{
			if (initialized)
			{
				listView.SetListItemCount(ShopProductService.GetPurchasedDLCCount(), resetPos);
				listView.RefreshAllShownItem();
				blank.SetActive(ShopProductService.GetPurchasedDLCCount() == 0);
			}
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (index > ShopProductService.GetPurchasedDLCCount() - 1)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("ProductDlcSlot");
			loopListViewItem.GetComponent<ProductDlcSlot>().SetSlot(ShopProductService.GetPurchasedDLC(index));
			return loopListViewItem;
		}
	}
}