using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class DeleteItemDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		public Button mSelectAllButton;


		public Button mCancelAllButton;


		public Button mDeleteButton;


		public Button mBackButton;


		private void Start()
		{
			mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);
			mSelectAllButton.onClick.AddListener(OnSelectAllBtnClicked);
			mCancelAllButton.onClick.AddListener(OnCancelAllBtnClicked);
			mDeleteButton.onClick.AddListener(OnDeleteBtnClicked);
			mBackButton.onClick.AddListener(OnBackBtnClicked);
		}


		private void OnBackBtnClicked()
		{
			SceneManager.LoadScene("Menu");
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0 || index >= DataSourceMgr.Get.TotalItemCount)
			{
				return null;
			}

			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(index);
			if (itemDataByIndex == null)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("ItemPrefab1");
			ListItem3 component = loopListViewItem.GetComponent<ListItem3>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			component.SetItemData(itemDataByIndex, index);
			return loopListViewItem;
		}


		private void OnSelectAllBtnClicked()
		{
			DataSourceMgr.Get.CheckAllItem();
			mLoopListView.RefreshAllShownItem();
		}


		private void OnCancelAllBtnClicked()
		{
			DataSourceMgr.Get.UnCheckAllItem();
			mLoopListView.RefreshAllShownItem();
		}


		private void OnDeleteBtnClicked()
		{
			if (!DataSourceMgr.Get.DeleteAllCheckedItem())
			{
				return;
			}

			mLoopListView.SetListItemCount(DataSourceMgr.Get.TotalItemCount, false);
			mLoopListView.RefreshAllShownItem();
		}
	}
}