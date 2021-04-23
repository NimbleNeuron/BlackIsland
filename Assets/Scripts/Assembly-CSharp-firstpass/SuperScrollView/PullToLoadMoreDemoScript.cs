using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class PullToLoadMoreDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		private readonly float mLoadingTipItemHeight = 100f;


		private readonly int mLoadMoreCount = 20;


		private Button mBackButton;


		private LoadingTipStatus mLoadingTipStatus;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private void Start()
		{
			mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount + 1, OnGetItemByIndex);
			mLoopListView.mOnBeginDragAction = OnBeginDrag;
			mLoopListView.mOnDragingAction = OnDraging;
			mLoopListView.mOnEndDragAction = OnEndDrag;
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

			if (index == DataSourceMgr.Get.TotalItemCount - 1)
			{
				loopListViewItem.Padding = 0f;
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

			ListItem0 component = item.GetComponent<ListItem0>();
			if (component == null)
			{
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.None)
			{
				component.mRoot.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.WaitRelease)
			{
				component.mRoot.SetActive(true);
				component.mText.text = "Release to Load More";
				component.mArrow.SetActive(true);
				component.mWaitingIcon.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight);
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
			{
				component.mRoot.SetActive(true);
				component.mArrow.SetActive(false);
				component.mWaitingIcon.SetActive(true);
				component.mText.text = "Loading ...";
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight);
			}
		}


		private void OnBeginDrag() { }


		private void OnDraging()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus != LoadingTipStatus.None && mLoadingTipStatus != LoadingTipStatus.WaitRelease)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex =
				mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex2 =
				mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount - 1);
			if (shownItemByItemIndex2 == null)
			{
				return;
			}

			if (mLoopListView.GetItemCornerPosInViewPort(shownItemByItemIndex2).y + mLoopListView.ViewPortSize >=
			    mLoadingTipItemHeight)
			{
				if (mLoadingTipStatus != LoadingTipStatus.None)
				{
					return;
				}

				mLoadingTipStatus = LoadingTipStatus.WaitRelease;
				UpdateLoadingTip(shownItemByItemIndex);
			}
			else
			{
				if (mLoadingTipStatus != LoadingTipStatus.WaitRelease)
				{
					return;
				}

				mLoadingTipStatus = LoadingTipStatus.None;
				UpdateLoadingTip(shownItemByItemIndex);
			}
		}


		private void OnEndDrag()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus != LoadingTipStatus.None && mLoadingTipStatus != LoadingTipStatus.WaitRelease)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex =
				mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			mLoopListView.OnItemSizeChanged(shownItemByItemIndex.ItemIndex);
			if (mLoadingTipStatus != LoadingTipStatus.WaitRelease)
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