using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class PullToRefreshAndLoadMoreDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		private readonly float mLoadingTipItemHeight1 = 100f;


		private readonly float mLoadingTipItemHeight2 = 100f;


		private readonly int mLoadMoreCount = 20;


		private Button mAddItemButton;


		private InputField mAddItemInput;


		private Button mBackButton;


		private float mDataLoadedTipShowLeftTime;


		private LoadingTipStatus mLoadingTipStatus1;


		private LoadingTipStatus mLoadingTipStatus2;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private Button mSetCountButton;


		private InputField mSetCountInput;


		private void Start()
		{
			mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount + 2, OnGetItemByIndex);
			mLoopListView.mOnDragingAction = OnDraging;
			mLoopListView.mOnEndDragAction = OnEndDrag;
			mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
			mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
			mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
			mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
			mBackButton.onClick.AddListener(OnBackBtnClicked);
		}


		private void Update()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.Loaded)
			{
				mDataLoadedTipShowLeftTime -= Time.deltaTime;
				if (mDataLoadedTipShowLeftTime <= 0f)
				{
					mLoadingTipStatus1 = LoadingTipStatus.None;
					LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
					if (shownItemByItemIndex == null)
					{
						return;
					}

					UpdateLoadingTip1(shownItemByItemIndex);
					shownItemByItemIndex.CachedRectTransform.anchoredPosition3D =
						new Vector3(0f, -mLoadingTipItemHeight1, 0f);
					mLoopListView.OnItemSizeChanged(0);
				}
			}
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
			if (index == 0)
			{
				loopListViewItem = listView.NewListViewItem("ItemPrefab0");
				UpdateLoadingTip1(loopListViewItem);
				return loopListViewItem;
			}

			if (index == DataSourceMgr.Get.TotalItemCount + 1)
			{
				loopListViewItem = listView.NewListViewItem("ItemPrefab1");
				UpdateLoadingTip2(loopListViewItem);
				return loopListViewItem;
			}

			int num = index - 1;
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(num);
			if (itemDataByIndex == null)
			{
				return null;
			}

			loopListViewItem = listView.NewListViewItem("ItemPrefab2");
			ListItem2 component = loopListViewItem.GetComponent<ListItem2>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			if (index == DataSourceMgr.Get.TotalItemCount)
			{
				loopListViewItem.Padding = 0f;
			}

			component.SetItemData(itemDataByIndex, num);
			return loopListViewItem;
		}


		private void UpdateLoadingTip1(LoopListViewItem2 item)
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

			if (mLoadingTipStatus1 == LoadingTipStatus.None)
			{
				component.mRoot.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.WaitRelease)
			{
				component.mRoot.SetActive(true);
				component.mText.text = "Release to Refresh";
				component.mArrow.SetActive(true);
				component.mWaitingIcon.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.WaitLoad)
			{
				component.mRoot.SetActive(true);
				component.mArrow.SetActive(false);
				component.mWaitingIcon.SetActive(true);
				component.mText.text = "Loading ...";
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.Loaded)
			{
				component.mRoot.SetActive(true);
				component.mArrow.SetActive(false);
				component.mWaitingIcon.SetActive(false);
				component.mText.text = "Refreshed Success";
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
			}
		}


		private void OnDraging()
		{
			OnDraging1();
			OnDraging2();
		}


		private void OnEndDrag()
		{
			OnEndDrag1();
			OnEndDrag2();
		}


		private void OnDraging1()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus1 != LoadingTipStatus.None && mLoadingTipStatus1 != LoadingTipStatus.WaitRelease)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			if (mLoopListView.ScrollRect.content.anchoredPosition3D.y < -mLoadingTipItemHeight1)
			{
				if (mLoadingTipStatus1 != LoadingTipStatus.None)
				{
					return;
				}

				mLoadingTipStatus1 = LoadingTipStatus.WaitRelease;
				UpdateLoadingTip1(shownItemByItemIndex);
				shownItemByItemIndex.CachedRectTransform.anchoredPosition3D =
					new Vector3(0f, mLoadingTipItemHeight1, 0f);
			}
			else
			{
				if (mLoadingTipStatus1 != LoadingTipStatus.WaitRelease)
				{
					return;
				}

				mLoadingTipStatus1 = LoadingTipStatus.None;
				UpdateLoadingTip1(shownItemByItemIndex);
				shownItemByItemIndex.CachedRectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
			}
		}


		private void OnEndDrag1()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus1 != LoadingTipStatus.None && mLoadingTipStatus1 != LoadingTipStatus.WaitRelease)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			mLoopListView.OnItemSizeChanged(shownItemByItemIndex.ItemIndex);
			if (mLoadingTipStatus1 != LoadingTipStatus.WaitRelease)
			{
				return;
			}

			mLoadingTipStatus1 = LoadingTipStatus.WaitLoad;
			UpdateLoadingTip1(shownItemByItemIndex);
			DataSourceMgr.Get.RequestRefreshDataList(OnDataSourceRefreshFinished);
		}


		private void OnDataSourceRefreshFinished()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.WaitLoad)
			{
				mLoadingTipStatus1 = LoadingTipStatus.Loaded;
				mDataLoadedTipShowLeftTime = 0.7f;
				LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
				if (shownItemByItemIndex == null)
				{
					return;
				}

				UpdateLoadingTip1(shownItemByItemIndex);
				mLoopListView.RefreshAllShownItem();
			}
		}


		private void UpdateLoadingTip2(LoopListViewItem2 item)
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

			if (mLoadingTipStatus2 == LoadingTipStatus.None)
			{
				component.mRoot.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
				return;
			}

			if (mLoadingTipStatus2 == LoadingTipStatus.WaitRelease)
			{
				component.mRoot.SetActive(true);
				component.mText.text = "Release to Load More";
				component.mArrow.SetActive(true);
				component.mWaitingIcon.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight2);
				return;
			}

			if (mLoadingTipStatus2 == LoadingTipStatus.WaitLoad)
			{
				component.mRoot.SetActive(true);
				component.mArrow.SetActive(false);
				component.mWaitingIcon.SetActive(true);
				component.mText.text = "Loading ...";
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight2);
			}
		}


		private void OnDraging2()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus2 != LoadingTipStatus.None && mLoadingTipStatus2 != LoadingTipStatus.WaitRelease)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex =
				mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount + 1);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex2 =
				mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount);
			if (shownItemByItemIndex2 == null)
			{
				return;
			}

			if (mLoopListView.GetItemCornerPosInViewPort(shownItemByItemIndex2).y + mLoopListView.ViewPortSize >=
			    mLoadingTipItemHeight2)
			{
				if (mLoadingTipStatus2 != LoadingTipStatus.None)
				{
					return;
				}

				mLoadingTipStatus2 = LoadingTipStatus.WaitRelease;
				UpdateLoadingTip2(shownItemByItemIndex);
			}
			else
			{
				if (mLoadingTipStatus2 != LoadingTipStatus.WaitRelease)
				{
					return;
				}

				mLoadingTipStatus2 = LoadingTipStatus.None;
				UpdateLoadingTip2(shownItemByItemIndex);
			}
		}


		private void OnEndDrag2()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus2 != LoadingTipStatus.None && mLoadingTipStatus2 != LoadingTipStatus.WaitRelease)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex =
				mLoopListView.GetShownItemByItemIndex(DataSourceMgr.Get.TotalItemCount + 1);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			mLoopListView.OnItemSizeChanged(shownItemByItemIndex.ItemIndex);
			if (mLoadingTipStatus2 != LoadingTipStatus.WaitRelease)
			{
				return;
			}

			mLoadingTipStatus2 = LoadingTipStatus.WaitLoad;
			UpdateLoadingTip2(shownItemByItemIndex);
			DataSourceMgr.Get.RequestLoadMoreDataList(mLoadMoreCount, OnDataSourceLoadMoreFinished);
		}


		private void OnDataSourceLoadMoreFinished()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus2 == LoadingTipStatus.WaitLoad)
			{
				mLoadingTipStatus2 = LoadingTipStatus.None;
				mLoopListView.SetListItemCount(DataSourceMgr.Get.TotalItemCount + 2, false);
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

			num++;
			mLoopListView.MovePanelToItemIndex(num, 0f);
		}
	}
}