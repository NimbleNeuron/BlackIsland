using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class GridViewDeleteItemDemoScript : MonoBehaviour
	{
		private const int mItemCountPerRow = 3;


		public LoopListView2 mLoopListView;


		public Button mSelectAllButton;


		public Button mCancelAllButton;


		public Button mDeleteButton;


		public Button mBackButton;


		private int mListItemTotalCount;


		private void Start()
		{
			mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
			int num = mListItemTotalCount / 3;
			if (mListItemTotalCount % 3 > 0)
			{
				num++;
			}

			mLoopListView.InitListView(num, OnGetItemByIndex);
			mBackButton.onClick.AddListener(OnBackBtnClicked);
			mSelectAllButton.onClick.AddListener(OnSelectAllBtnClicked);
			mCancelAllButton.onClick.AddListener(OnCancelAllBtnClicked);
			mDeleteButton.onClick.AddListener(OnDeleteBtnClicked);
		}


		private void OnBackBtnClicked()
		{
			SceneManager.LoadScene("Menu");
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("ItemPrefab1");
			ListItem10 component = loopListViewItem.GetComponent<ListItem10>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			for (int i = 0; i < 3; i++)
			{
				int num = index * 3 + i;
				if (num >= mListItemTotalCount)
				{
					component.mItemList[i].gameObject.SetActive(false);
				}
				else
				{
					ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(num);
					if (itemDataByIndex != null)
					{
						component.mItemList[i].gameObject.SetActive(true);
						component.mItemList[i].SetItemData(itemDataByIndex, num);
					}
					else
					{
						component.mItemList[i].gameObject.SetActive(false);
					}
				}
			}

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

			SetListItemTotalCount(DataSourceMgr.Get.TotalItemCount);
		}


		private void SetListItemTotalCount(int count)
		{
			mListItemTotalCount = count;
			if (mListItemTotalCount < 0)
			{
				mListItemTotalCount = 0;
			}

			if (mListItemTotalCount > DataSourceMgr.Get.TotalItemCount)
			{
				mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
			}

			int num = mListItemTotalCount / 3;
			if (mListItemTotalCount % 3 > 0)
			{
				num++;
			}

			mLoopListView.SetListItemCount(num, false);
			mLoopListView.RefreshAllShownItem();
		}
	}
}