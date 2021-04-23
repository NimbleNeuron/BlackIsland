using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class PullToRefreshDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		private readonly float mLoadingTipItemHeight = 100f;


		private Button mAddItemButton;


		private InputField mAddItemInput;


		private Button mBackButton;


		private float mDataLoadedTipShowLeftTime;


		private LoadingTipStatus mLoadingTipStatus;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private Button mSetCountButton;


		private InputField mSetCountInput;


		private void Start()
		{
			mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount + 1, OnGetItemByIndex);
			mLoopListView.mOnBeginDragAction = OnBeginDrag;
			mLoopListView.mOnDragingAction = OnDraging;
			mLoopListView.mOnEndDragAction = OnEndDrag;
			mSetCountButton = GameObject.Find("ButtonPanel/buttonGroup1/SetCountButton").GetComponent<Button>();
			mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
			mAddItemButton = GameObject.Find("ButtonPanel/buttonGroup3/AddItemButton").GetComponent<Button>();
			mSetCountInput = GameObject.Find("ButtonPanel/buttonGroup1/SetCountInputField").GetComponent<InputField>();
			mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
			mAddItemInput = GameObject.Find("ButtonPanel/buttonGroup3/AddItemInputField").GetComponent<InputField>();
			mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
			mAddItemButton.onClick.AddListener(OnAddItemBtnClicked);
			mSetCountButton.onClick.AddListener(OnSetItemCountBtnClicked);
			mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
			mBackButton.onClick.AddListener(OnBackBtnClicked);
		}


		private void Update()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.Loaded)
			{
				mDataLoadedTipShowLeftTime -= Time.deltaTime;
				if (mDataLoadedTipShowLeftTime <= 0f)
				{
					mLoadingTipStatus = LoadingTipStatus.None;
					LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
					if (shownItemByItemIndex == null)
					{
						return;
					}

					UpdateLoadingTip(shownItemByItemIndex);
					shownItemByItemIndex.CachedRectTransform.anchoredPosition3D =
						new Vector3(0f, -mLoadingTipItemHeight, 0f);
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
			if (index < 0 || index > DataSourceMgr.Get.TotalItemCount)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem;
			if (index == 0)
			{
				loopListViewItem = listView.NewListViewItem("ItemPrefab0");
				UpdateLoadingTip(loopListViewItem);
				return loopListViewItem;
			}

			int num = index - 1;
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(num);
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

			component.SetItemData(itemDataByIndex, num);
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
				component.mText.text = "Release to Refresh";
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
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.Loaded)
			{
				component.mRoot.SetActive(true);
				component.mArrow.SetActive(false);
				component.mWaitingIcon.SetActive(false);
				component.mText.text = "Refreshed Success";
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

			LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			if (mLoopListView.ScrollRect.content.anchoredPosition3D.y < -mLoadingTipItemHeight)
			{
				if (mLoadingTipStatus != LoadingTipStatus.None)
				{
					return;
				}

				mLoadingTipStatus = LoadingTipStatus.WaitRelease;
				UpdateLoadingTip(shownItemByItemIndex);
				shownItemByItemIndex.CachedRectTransform.anchoredPosition3D =
					new Vector3(0f, mLoadingTipItemHeight, 0f);
			}
			else
			{
				if (mLoadingTipStatus != LoadingTipStatus.WaitRelease)
				{
					return;
				}

				mLoadingTipStatus = LoadingTipStatus.None;
				UpdateLoadingTip(shownItemByItemIndex);
				shownItemByItemIndex.CachedRectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
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

			LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
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
			DataSourceMgr.Get.RequestRefreshDataList(OnDataSourceRefreshFinished);
		}


		private void OnDataSourceRefreshFinished()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			if (mLoadingTipStatus == LoadingTipStatus.WaitLoad)
			{
				mLoadingTipStatus = LoadingTipStatus.Loaded;
				mDataLoadedTipShowLeftTime = 0.7f;
				LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
				if (shownItemByItemIndex == null)
				{
					return;
				}

				UpdateLoadingTip(shownItemByItemIndex);
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


		private void OnAddItemBtnClicked()
		{
			if (mLoopListView.ItemTotalCount < 0)
			{
				return;
			}

			int num = 0;
			if (!int.TryParse(mAddItemInput.text, out num))
			{
				return;
			}

			num = mLoopListView.ItemTotalCount + num;
			if (num < 0 || num > DataSourceMgr.Get.TotalItemCount + 1)
			{
				return;
			}

			mLoopListView.SetListItemCount(num, false);
		}


		private void OnSetItemCountBtnClicked()
		{
			int num = 0;
			if (!int.TryParse(mSetCountInput.text, out num))
			{
				return;
			}

			if (num < 0 || num > DataSourceMgr.Get.TotalItemCount)
			{
				return;
			}

			num++;
			mLoopListView.SetListItemCount(num, false);
		}
	}
}