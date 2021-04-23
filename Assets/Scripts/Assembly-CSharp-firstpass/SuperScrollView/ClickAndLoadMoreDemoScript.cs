using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class ClickAndLoadMoreDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		private readonly int mLoadMoreCount = 20;


		private Button mBackButton;


		private LoadingTipStatus mLoadingTipStatus;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private void Start()
		{
			mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount + 1, OnGetItemByIndex);
			mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
			mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
			mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
			mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
			mBackButton.onClick.AddListener(OnBackBtnClicked);
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

			LoopListViewItem2 loopListViewItem;
			if (index == DataSourceMgr.Get.TotalItemCount)
			{
				loopListViewItem = listView.NewListViewItem("ItemPrefab0");
				if (!loopListViewItem.IsInitHandlerCalled)
				{
					loopListViewItem.IsInitHandlerCalled = true;
					loopListViewItem.GetComponent<ListItem11>().mRootButton.onClick.AddListener(OnLoadMoreBtnClicked);
				}

				UpdateLoadingTip(loopListViewItem);
				return loopListViewItem;
			}

			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(index);
			if (itemDataByIndex == null)
			{
				return null;
			}

			loopListViewItem = listView.NewListViewItem("ItemPrefab1");
			ListItem2 component = loopListViewItem.GetComponent<ListItem2>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			component.SetItemData(itemDataByIndex, index);
			return loopListViewItem;
		}


		private void UpdateLoadingTip(LoopListViewItem2 item)
		{
			if (item == null)
			{
				return;
			}

			ListItem11 component = item.GetComponent<ListItem11>();
			if (component == null)
			{
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.None)
			{
				component.mText.text = "Click to Load More";
				component.mWaitingIcon.SetActive(false);
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
			{
				component.mWaitingIcon.SetActive(true);
				component.mText.text = "Loading ...";
			}
		}


		private void OnLoadMoreBtnClicked()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus != LoadingTipStatus.None)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex =
				mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			mLoadingTipStatus = LoadingTipStatus.WaitLoad;
			UpdateLoadingTip(shownItemByItemIndex);
			DataSourceMgr.Get.RequestLoadMoreDataList(mLoadMoreCount, OnDataSourceLoadMoreFinished);
		}


		private void OnDataSourceLoadMoreFinished()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
			{
				mLoadingTipStatus = LoadingTipStatus.None;
				mLoopListView.SetListItemCount(DataSourceMgr.Get.TotalItemCount + 1, false);
				mLoopListView.RefreshAllShownItem();
			}
		}


		private void OnJumpBtnClicked()
		{
			int num = 0;
			if (!int.TryParse(mScrollToInput.text, out num))
			{
				return;
			}

			if (num < 0)
			{
				return;
			}

			mLoopListView.MovePanelToItemIndex(num, 0f);
		}
	}
}