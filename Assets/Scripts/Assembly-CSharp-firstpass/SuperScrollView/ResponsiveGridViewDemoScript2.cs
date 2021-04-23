using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class ResponsiveGridViewDemoScript2 : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		public DragChangSizeScript mDragChangSizeScript;


		private readonly float mLoadingTipItemHeight1 = 100f;


		private readonly float mLoadingTipItemHeight2 = 100f;


		private readonly int mLoadMoreCount = 20;


		private Button mBackButton;


		private float mDataLoadedTipShowLeftTime;


		private int mItemCountPerRow = 3;


		private LoadingTipStatus mLoadingTipStatus1;


		private LoadingTipStatus mLoadingTipStatus2;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private void Start()
		{
			mLoopListView.InitListView(GetMaxRowCount() + 2, OnGetItemByIndex);
			mDragChangSizeScript.mOnDragEndAction = OnViewPortSizeChanged;
			mLoopListView.mOnDragingAction = OnDraging;
			mLoopListView.mOnEndDragAction = OnEndDrag;
			OnViewPortSizeChanged();
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


		private void UpdateItemPrefab()
		{
			GameObject mItemPrefab = mLoopListView.GetItemPrefabConfData("ItemPrefab2").mItemPrefab;
			RectTransform component = mItemPrefab.GetComponent<RectTransform>();
			ListItem6 component2 = mItemPrefab.GetComponent<ListItem6>();
			float viewPortWidth = mLoopListView.ViewPortWidth;
			int count = component2.mItemList.Count;
			GameObject gameObject = component2.mItemList[0].gameObject;
			float width = gameObject.GetComponent<RectTransform>().rect.width;
			int num = Mathf.FloorToInt(viewPortWidth / width);
			if (num == 0)
			{
				num = 1;
			}

			mItemCountPerRow = num;
			float num2 = (viewPortWidth - width * num) / (num + 1);
			if (num2 < 0f)
			{
				num2 = 0f;
			}

			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, viewPortWidth);
			if (num > count)
			{
				int num3 = num - count;
				for (int i = 0; i < num3; i++)
				{
					GameObject gameObject2 =
						Instantiate<GameObject>(gameObject, Vector3.zero, Quaternion.identity, component);
					RectTransform component3 = gameObject2.GetComponent<RectTransform>();
					component3.localScale = Vector3.one;
					component3.anchoredPosition3D = Vector3.zero;
					component3.rotation = Quaternion.identity;
					ListItem5 component4 = gameObject2.GetComponent<ListItem5>();
					component2.mItemList.Add(component4);
				}
			}
			else if (num < count)
			{
				int num4 = count - num;
				for (int j = 0; j < num4; j++)
				{
					Component component5 = component2.mItemList[component2.mItemList.Count - 1];
					component2.mItemList.RemoveAt(component2.mItemList.Count - 1);
					DestroyImmediate(component5.gameObject);
				}
			}

			float num5 = num2;
			for (int k = 0; k < component2.mItemList.Count; k++)
			{
				component2.mItemList[k].gameObject.GetComponent<RectTransform>().anchoredPosition3D =
					new Vector3(num5, 0f, 0f);
				num5 = num5 + width + num2;
			}

			mLoopListView.OnItemPrefabChanged("ItemPrefab2");
		}


		private void OnViewPortSizeChanged()
		{
			UpdateItemPrefab();
			mLoopListView.SetListItemCount(GetMaxRowCount() + 2, false);
			mLoopListView.RefreshAllShownItem();
		}


		private int GetMaxRowCount()
		{
			int num = DataSourceMgr.Get.TotalItemCount / mItemCountPerRow;
			if (DataSourceMgr.Get.TotalItemCount % mItemCountPerRow > 0)
			{
				num++;
			}

			return num;
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int row)
		{
			if (row < 0)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem;
			if (row == 0)
			{
				loopListViewItem = listView.NewListViewItem("ItemPrefab0");
				UpdateLoadingTip1(loopListViewItem);
				return loopListViewItem;
			}

			if (row == GetMaxRowCount() + 1)
			{
				loopListViewItem = listView.NewListViewItem("ItemPrefab1");
				UpdateLoadingTip2(loopListViewItem);
				return loopListViewItem;
			}

			int num = row - 1;
			loopListViewItem = listView.NewListViewItem("ItemPrefab2");
			ListItem6 component = loopListViewItem.GetComponent<ListItem6>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			for (int i = 0; i < mItemCountPerRow; i++)
			{
				int num2 = num * mItemCountPerRow + i;
				if (num2 >= DataSourceMgr.Get.TotalItemCount)
				{
					component.mItemList[i].gameObject.SetActive(false);
				}
				else
				{
					ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(num2);
					if (itemDataByIndex != null)
					{
						component.mItemList[i].gameObject.SetActive(true);
						component.mItemList[i].SetItemData(itemDataByIndex, num2);
					}
					else
					{
						component.mItemList[i].gameObject.SetActive(false);
					}
				}
			}

			return loopListViewItem;
		}


		private void UpdateLoadingTip1(LoopListViewItem2 item)
		{
			if (item == null)
			{
				return;
			}

			item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
				mLoopListView.ViewPortWidth);
			ListItem17 component = item.GetComponent<ListItem17>();
			if (component == null)
			{
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.None)
			{
				component.mRoot1.SetActive(false);
				component.mRoot.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.WaitContinureDrag)
			{
				component.mRoot1.SetActive(true);
				component.mRoot.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.WaitRelease)
			{
				component.mRoot1.SetActive(false);
				component.mRoot.SetActive(true);
				component.mText.text = "Release to Refresh";
				component.mArrow.SetActive(true);
				component.mWaitingIcon.SetActive(false);
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.WaitLoad)
			{
				component.mRoot1.SetActive(false);
				component.mRoot.SetActive(true);
				component.mArrow.SetActive(false);
				component.mWaitingIcon.SetActive(true);
				component.mText.text = "Loading ...";
				item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, mLoadingTipItemHeight1);
				return;
			}

			if (mLoadingTipStatus1 == LoadingTipStatus.Loaded)
			{
				component.mRoot1.SetActive(false);
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

			if (mLoadingTipStatus1 != LoadingTipStatus.None && mLoadingTipStatus1 != LoadingTipStatus.WaitRelease &&
			    mLoadingTipStatus1 != LoadingTipStatus.WaitContinureDrag)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			Vector3 anchoredPosition3D = mLoopListView.ScrollRect.content.anchoredPosition3D;
			if (anchoredPosition3D.y >= 0f)
			{
				if (mLoadingTipStatus1 == LoadingTipStatus.WaitContinureDrag)
				{
					mLoadingTipStatus1 = LoadingTipStatus.None;
					UpdateLoadingTip1(shownItemByItemIndex);
					shownItemByItemIndex.CachedRectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				}
			}
			else if (anchoredPosition3D.y < 0f && anchoredPosition3D.y > -mLoadingTipItemHeight1)
			{
				if (mLoadingTipStatus1 == LoadingTipStatus.None || mLoadingTipStatus1 == LoadingTipStatus.WaitRelease)
				{
					mLoadingTipStatus1 = LoadingTipStatus.WaitContinureDrag;
					UpdateLoadingTip1(shownItemByItemIndex);
					shownItemByItemIndex.CachedRectTransform.anchoredPosition3D = new Vector3(0f, 0f, 0f);
				}
			}
			else if (anchoredPosition3D.y <= -mLoadingTipItemHeight1 &&
			         mLoadingTipStatus1 == LoadingTipStatus.WaitContinureDrag)
			{
				mLoadingTipStatus1 = LoadingTipStatus.WaitRelease;
				UpdateLoadingTip1(shownItemByItemIndex);
				shownItemByItemIndex.CachedRectTransform.anchoredPosition3D =
					new Vector3(0f, mLoadingTipItemHeight1, 0f);
			}
		}


		private void OnEndDrag1()
		{
			if (mLoopListView.ShownItemCount == 0)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(0);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			mLoopListView.OnItemSizeChanged(shownItemByItemIndex.ItemIndex);
			if (mLoadingTipStatus1 == LoadingTipStatus.WaitRelease)
			{
				mLoadingTipStatus1 = LoadingTipStatus.WaitLoad;
				UpdateLoadingTip1(shownItemByItemIndex);
				DataSourceMgr.Get.RequestRefreshDataList(OnDataSourceRefreshFinished);
			}
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

			item.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
				mLoopListView.ViewPortWidth);
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

			LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount() + 1);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			LoopListViewItem2 shownItemByItemIndex2 = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount());
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

			LoopListViewItem2 shownItemByItemIndex = mLoopListView.GetShownItemByItemIndex(GetMaxRowCount() + 1);
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
				mLoopListView.SetListItemCount(GetMaxRowCount() + 2, false);
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
				num = 0;
			}

			num++;
			int num2 = num / mItemCountPerRow;
			if (num % mItemCountPerRow > 0)
			{
				num2++;
			}

			if (num2 > 0)
			{
				num2--;
			}

			num2++;
			mLoopListView.MovePanelToItemIndex(num2, 0f);
		}
	}
}