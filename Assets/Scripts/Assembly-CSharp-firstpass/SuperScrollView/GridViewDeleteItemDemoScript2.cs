using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class GridViewDeleteItemDemoScript2 : MonoBehaviour
	{
		public LoopGridView mLoopGridView;


		public Button mSelectAllButton;


		public Button mCancelAllButton;


		public Button mDeleteButton;


		public Button mBackButton;


		private void Start()
		{
			mLoopGridView.InitGridView(DataSourceMgr.Get.TotalItemCount, OnGetItemByRowColumn);
			mBackButton.onClick.AddListener(OnBackBtnClicked);
			mSelectAllButton.onClick.AddListener(OnSelectAllBtnClicked);
			mCancelAllButton.onClick.AddListener(OnCancelAllBtnClicked);
			mDeleteButton.onClick.AddListener(OnDeleteBtnClicked);
		}


		private void OnBackBtnClicked()
		{
			SceneManager.LoadScene("Menu");
		}


		private LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
		{
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(itemIndex);
			if (itemDataByIndex == null)
			{
				return null;
			}

			LoopGridViewItem loopGridViewItem = gridView.NewListViewItem("ItemPrefab0");
			ListItem19 component = loopGridViewItem.GetComponent<ListItem19>();
			if (!loopGridViewItem.IsInitHandlerCalled)
			{
				loopGridViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			component.SetItemData(itemDataByIndex, itemIndex, row, column);
			return loopGridViewItem;
		}


		private void OnSelectAllBtnClicked()
		{
			DataSourceMgr.Get.CheckAllItem();
			mLoopGridView.RefreshAllShownItem();
		}


		private void OnCancelAllBtnClicked()
		{
			DataSourceMgr.Get.UnCheckAllItem();
			mLoopGridView.RefreshAllShownItem();
		}


		private void OnDeleteBtnClicked()
		{
			if (!DataSourceMgr.Get.DeleteAllCheckedItem())
			{
				return;
			}

			mLoopGridView.SetListItemCount(DataSourceMgr.Get.TotalItemCount, false);
			mLoopGridView.RefreshAllShownItem();
		}
	}
}