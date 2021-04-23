using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class LoopListView2 : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
	{
		[SerializeField] private List<ItemPrefabConfData> mItemPrefabDataList = new List<ItemPrefabConfData>();


		[SerializeField] private ListItemArrangeType mArrangeType;


		[SerializeField] private bool mSupportScrollBar = true;


		[SerializeField] private bool mItemSnapEnable;


		[SerializeField] private Vector2 mViewPortSnapPivot = Vector2.zero;


		[SerializeField] private Vector2 mItemSnapPivot = Vector2.zero;


		private readonly SnapData mCurSnapData = new SnapData();


		private readonly Dictionary<string, ItemPool> mItemPoolDict = new Dictionary<string, ItemPool>();


		private readonly List<ItemPool> mItemPoolList = new List<ItemPool>();


		private readonly Vector3[] mItemWorldCorners = new Vector3[4];


		private readonly Vector3[] mViewPortRectLocalCorners = new Vector3[4];


		private Vector2 mAdjustedVec;


		private RectTransform mContainerTrans;


		private int mCurReadyMaxItemIndex;


		private int mCurReadyMinItemIndex;


		private int mCurSnapNearestItemIndex = -1;


		private float mDistanceForNew0 = 200f;


		private float mDistanceForNew1 = 200f;


		private float mDistanceForRecycle0 = 300f;


		private float mDistanceForRecycle1 = 300f;


		private bool mIsDraging;


		private bool mIsVertList;


		private float mItemDefaultWithPaddingSize = 20f;


		private ItemPosMgr mItemPosMgr;


		private int mItemTotalCount;


		private Vector3 mLastFrameContainerPos = Vector3.zero;


		private int mLastItemIndex;


		private float mLastItemPadding;


		private Vector3 mLastSnapCheckPos = Vector3.zero;


		private int mLeftSnapUpdateExtraCount = 1;


		private int mListUpdateCheckFrameCount;


		private bool mListViewInited;


		private bool mNeedAdjustVec;


		private bool mNeedCheckNextMaxItem = true;


		private bool mNeedCheckNextMinItem = true;


		public Action mOnBeginDragAction;


		public Action mOnDragingAction;


		public Action mOnEndDragAction;


		private Func<LoopListView2, int, LoopListViewItem2> mOnGetItemByIndex;


		public Action<LoopListView2, LoopListViewItem2> mOnSnapItemFinished;


		public Action<LoopListView2, LoopListViewItem2> mOnSnapNearestChanged;


		private PointerEventData mPointerEventData;


		private ClickEventListener mScrollBarClickEventListener;


		private ScrollRect mScrollRect;


		private RectTransform mScrollRectTransform;


		private float mSmoothDumpRate = 0.3f;


		private float mSmoothDumpVel;


		private float mSnapFinishThreshold = 0.1f;


		private float mSnapVecThreshold = 145f;


		private RectTransform mViewPortRectTransform;


		
		public ListItemArrangeType ArrangeType {
			get => mArrangeType;
			set => mArrangeType = value;
		}


		private ItemPosMgr ItemPosMgr {
			get
			{
				ItemPosMgr result;
				if ((result = mItemPosMgr) == null)
				{
					result = mItemPosMgr = new ItemPosMgr(mItemDefaultWithPaddingSize);
				}

				return result;
			}
		}


		public List<ItemPrefabConfData> ItemPrefabDataList => mItemPrefabDataList;


		public List<LoopListViewItem2> ItemList { get; } = new List<LoopListViewItem2>();


		public bool IsVertList => mIsVertList;


		public int ItemTotalCount => mItemTotalCount;


		public RectTransform ContainerTrans => mContainerTrans;


		public ScrollRect ScrollRect => mScrollRect;


		public bool IsDraging => mIsDraging;


		
		public bool ItemSnapEnable {
			get => mItemSnapEnable;
			set => mItemSnapEnable = value;
		}


		
		public bool SupportScrollBar {
			get => mSupportScrollBar;
			set => mSupportScrollBar = value;
		}


		
		public float SnapMoveDefaultMaxAbsVec { get; set; } = 3400f;


		public int ShownItemCount => ItemList.Count;


		public float ViewPortSize {
			get
			{
				if (mIsVertList)
				{
					return mViewPortRectTransform.rect.height;
				}

				return mViewPortRectTransform.rect.width;
			}
		}


		public float ViewPortWidth => mViewPortRectTransform.rect.width;


		public float ViewPortHeight => mViewPortRectTransform.rect.height;


		public int CurSnapNearestItemIndex => mCurSnapNearestItemIndex;


		private void Update()
		{
			if (!mListViewInited)
			{
				return;
			}

			if (mNeedAdjustVec)
			{
				mNeedAdjustVec = false;
				if (mIsVertList)
				{
					if (mScrollRect.velocity.y * mAdjustedVec.y > 0f)
					{
						mScrollRect.velocity = mAdjustedVec;
					}
				}
				else if (mScrollRect.velocity.x * mAdjustedVec.x > 0f)
				{
					mScrollRect.velocity = mAdjustedVec;
				}
			}

			if (mSupportScrollBar)
			{
				ItemPosMgr.Update(false);
			}

			UpdateSnapMove();
			UpdateListView(mDistanceForRecycle0, mDistanceForRecycle1, mDistanceForNew0, mDistanceForNew1);
			ClearAllTmpRecycledItem();
			mLastFrameContainerPos = mContainerTrans.anchoredPosition3D;
		}


		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			mIsDraging = true;
			CacheDragPointerEventData(eventData);
			mCurSnapData.Clear();
			if (mOnBeginDragAction != null)
			{
				mOnBeginDragAction();
			}
		}


		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			CacheDragPointerEventData(eventData);
			if (mOnDragingAction != null)
			{
				mOnDragingAction();
			}
		}


		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			mIsDraging = false;
			mPointerEventData = null;
			if (mOnEndDragAction != null)
			{
				mOnEndDragAction();
			}

			ForceSnapUpdateCheck();
		}


		public ItemPrefabConfData GetItemPrefabConfData(string prefabName)
		{
			foreach (ItemPrefabConfData itemPrefabConfData in mItemPrefabDataList)
			{
				if (itemPrefabConfData.mItemPrefab == null)
				{
					Debug.LogError("A item prefab is null ");
				}
				else if (prefabName == itemPrefabConfData.mItemPrefab.name)
				{
					return itemPrefabConfData;
				}
			}

			return null;
		}


		public void OnItemPrefabChanged(string prefabName)
		{
			ItemPrefabConfData itemPrefabConfData = GetItemPrefabConfData(prefabName);
			if (itemPrefabConfData == null)
			{
				return;
			}

			ItemPool itemPool = null;
			if (!mItemPoolDict.TryGetValue(prefabName, out itemPool))
			{
				return;
			}

			int num = -1;
			Vector3 pos = Vector3.zero;
			if (ItemList.Count > 0)
			{
				num = ItemList[0].ItemIndex;
				pos = ItemList[0].CachedRectTransform.anchoredPosition3D;
			}

			RecycleAllItem();
			ClearAllTmpRecycledItem();
			itemPool.DestroyAllItem();
			itemPool.Init(itemPrefabConfData.mItemPrefab, itemPrefabConfData.mPadding,
				itemPrefabConfData.mStartPosOffset, itemPrefabConfData.mInitCreateCount, mContainerTrans);
			if (num >= 0)
			{
				RefreshAllShownItemWithFirstIndexAndPos(num, pos);
			}
		}


		public void InitListView(int itemTotalCount, Func<LoopListView2, int, LoopListViewItem2> onGetItemByIndex,
			LoopListViewInitParam initParam = null)
		{
			if (initParam != null)
			{
				mDistanceForRecycle0 = initParam.mDistanceForRecycle0;
				mDistanceForNew0 = initParam.mDistanceForNew0;
				mDistanceForRecycle1 = initParam.mDistanceForRecycle1;
				mDistanceForNew1 = initParam.mDistanceForNew1;
				mSmoothDumpRate = initParam.mSmoothDumpRate;
				mSnapFinishThreshold = initParam.mSnapFinishThreshold;
				mSnapVecThreshold = initParam.mSnapVecThreshold;
				mItemDefaultWithPaddingSize = initParam.mItemDefaultWithPaddingSize;
			}

			mScrollRect = gameObject.GetComponent<ScrollRect>();
			if (mScrollRect == null)
			{
				Debug.LogError("ListView Init Failed! ScrollRect component not found!");
				return;
			}

			if (mDistanceForRecycle0 <= mDistanceForNew0)
			{
				Debug.LogError("mDistanceForRecycle0 should be bigger than mDistanceForNew0");
			}

			if (mDistanceForRecycle1 <= mDistanceForNew1)
			{
				Debug.LogError("mDistanceForRecycle1 should be bigger than mDistanceForNew1");
			}

			mCurSnapData.Clear();
			mItemPosMgr = new ItemPosMgr(mItemDefaultWithPaddingSize);
			mScrollRectTransform = mScrollRect.GetComponent<RectTransform>();
			mContainerTrans = mScrollRect.content;
			mViewPortRectTransform = mScrollRect.viewport;
			if (mViewPortRectTransform == null)
			{
				mViewPortRectTransform = mScrollRectTransform;
			}

			if (mScrollRect.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport &&
			    mScrollRect.horizontalScrollbar != null)
			{
				Debug.LogError("ScrollRect.horizontalScrollbarVisibility cannot be set to AutoHideAndExpandViewport");
			}

			if (mScrollRect.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport &&
			    mScrollRect.verticalScrollbar != null)
			{
				Debug.LogError("ScrollRect.verticalScrollbarVisibility cannot be set to AutoHideAndExpandViewport");
			}

			mIsVertList = mArrangeType == ListItemArrangeType.TopToBottom ||
			              mArrangeType == ListItemArrangeType.BottomToTop;
			mScrollRect.horizontal = !mIsVertList;
			mScrollRect.vertical = mIsVertList;
			SetScrollbarListener();
			AdjustPivot(mViewPortRectTransform);
			AdjustAnchor(mContainerTrans);
			AdjustContainerPivot(mContainerTrans);
			InitItemPool();
			mOnGetItemByIndex = onGetItemByIndex;
			if (mListViewInited)
			{
				Debug.LogError("LoopListView2.InitListView method can be called only once.");
			}

			mListViewInited = true;
			ResetListView();
			mCurSnapData.Clear();
			mItemTotalCount = itemTotalCount;
			if (mItemTotalCount < 0)
			{
				mSupportScrollBar = false;
			}

			if (mSupportScrollBar)
			{
				ItemPosMgr.SetItemMaxCount(mItemTotalCount);
			}
			else
			{
				ItemPosMgr.SetItemMaxCount(0);
			}

			mCurReadyMaxItemIndex = 0;
			mCurReadyMinItemIndex = 0;
			mLeftSnapUpdateExtraCount = 1;
			mNeedCheckNextMaxItem = true;
			mNeedCheckNextMinItem = true;
			UpdateContentSize();
		}


		private void SetScrollbarListener()
		{
			mScrollBarClickEventListener = null;
			Scrollbar scrollbar = null;
			if (mIsVertList && mScrollRect.verticalScrollbar != null)
			{
				scrollbar = mScrollRect.verticalScrollbar;
			}

			if (!mIsVertList && mScrollRect.horizontalScrollbar != null)
			{
				scrollbar = mScrollRect.horizontalScrollbar;
			}

			if (scrollbar == null)
			{
				return;
			}

			ClickEventListener clickEventListener = ClickEventListener.Get(scrollbar.gameObject);
			mScrollBarClickEventListener = clickEventListener;
			clickEventListener.SetPointerUpHandler(OnPointerUpInScrollBar);
			clickEventListener.SetPointerDownHandler(OnPointerDownInScrollBar);
		}


		private void OnPointerDownInScrollBar(GameObject obj)
		{
			mCurSnapData.Clear();
		}


		private void OnPointerUpInScrollBar(GameObject obj)
		{
			ForceSnapUpdateCheck();
		}


		public void ResetListView(bool resetPos = true)
		{
			mViewPortRectTransform.GetLocalCorners(mViewPortRectLocalCorners);
			if (resetPos)
			{
				mContainerTrans.anchoredPosition3D = Vector3.zero;
			}

			ForceSnapUpdateCheck();
		}


		public void SetListItemCount(int itemCount, bool resetPos = true)
		{
			if (itemCount == mItemTotalCount)
			{
				return;
			}

			mCurSnapData.Clear();
			mItemTotalCount = itemCount;
			if (mItemTotalCount < 0)
			{
				mSupportScrollBar = false;
			}

			if (mSupportScrollBar)
			{
				ItemPosMgr.SetItemMaxCount(mItemTotalCount);
			}
			else
			{
				ItemPosMgr.SetItemMaxCount(0);
			}

			if (mItemTotalCount == 0)
			{
				mCurReadyMaxItemIndex = 0;
				mCurReadyMinItemIndex = 0;
				mNeedCheckNextMaxItem = false;
				mNeedCheckNextMinItem = false;
				RecycleAllItem();
				ClearAllTmpRecycledItem();
				UpdateContentSize();
				return;
			}

			if (mCurReadyMaxItemIndex >= mItemTotalCount)
			{
				mCurReadyMaxItemIndex = mItemTotalCount - 1;
			}

			mLeftSnapUpdateExtraCount = 1;
			mNeedCheckNextMaxItem = true;
			mNeedCheckNextMinItem = true;
			if (resetPos)
			{
				MovePanelToItemIndex(0, 0f);
				return;
			}

			if (ItemList.Count == 0)
			{
				MovePanelToItemIndex(0, 0f);
				return;
			}

			int num = mItemTotalCount - 1;
			if (ItemList[ItemList.Count - 1].ItemIndex <= num)
			{
				UpdateContentSize();
				UpdateAllShownItemsPos();
				return;
			}

			MovePanelToItemIndex(num, 0f);
		}


		public LoopListViewItem2 GetShownItemByItemIndex(int itemIndex)
		{
			int count = ItemList.Count;
			if (count == 0)
			{
				return null;
			}

			if (itemIndex < ItemList[0].ItemIndex || itemIndex > ItemList[count - 1].ItemIndex)
			{
				return null;
			}

			int index = itemIndex - ItemList[0].ItemIndex;
			return ItemList[index];
		}


		public LoopListViewItem2 GetShownItemNearestItemIndex(int itemIndex)
		{
			int count = ItemList.Count;
			if (count == 0)
			{
				return null;
			}

			if (itemIndex < ItemList[0].ItemIndex)
			{
				return ItemList[0];
			}

			if (itemIndex > ItemList[count - 1].ItemIndex)
			{
				return ItemList[count - 1];
			}

			int index = itemIndex - ItemList[0].ItemIndex;
			return ItemList[index];
		}


		public LoopListViewItem2 GetShownItemByIndex(int index)
		{
			int count = ItemList.Count;
			if (index < 0 || index >= count)
			{
				return null;
			}

			return ItemList[index];
		}


		public LoopListViewItem2 GetShownItemByIndexWithoutCheck(int index)
		{
			return ItemList[index];
		}


		public int GetIndexInShownItemList(LoopListViewItem2 item)
		{
			if (item == null)
			{
				return -1;
			}

			int count = ItemList.Count;
			if (count == 0)
			{
				return -1;
			}

			for (int i = 0; i < count; i++)
			{
				if (ItemList[i] == item)
				{
					return i;
				}
			}

			return -1;
		}


		public void DoActionForEachShownItem(Action<LoopListViewItem2, object> action, object param)
		{
			if (action == null)
			{
				return;
			}

			int count = ItemList.Count;
			if (count == 0)
			{
				return;
			}

			for (int i = 0; i < count; i++)
			{
				action(ItemList[i], param);
			}
		}


		public LoopListViewItem2 NewListViewItem(string itemPrefabName)
		{
			ItemPool itemPool = null;
			if (!mItemPoolDict.TryGetValue(itemPrefabName, out itemPool))
			{
				return null;
			}

			LoopListViewItem2 item = itemPool.GetItem();
			RectTransform component = item.GetComponent<RectTransform>();
			component.SetParent(mContainerTrans);
			component.anchoredPosition3D = Vector3.zero;
			component.localEulerAngles = Vector3.zero;
			item.ParentListView = this;
			return item;
		}


		public void OnItemSizeChanged(int itemIndex)
		{
			LoopListViewItem2 shownItemByItemIndex = GetShownItemByItemIndex(itemIndex);
			if (shownItemByItemIndex == null)
			{
				return;
			}

			if (mSupportScrollBar)
			{
				if (mIsVertList)
				{
					SetItemSize(itemIndex, shownItemByItemIndex.CachedRectTransform.rect.height,
						shownItemByItemIndex.Padding);
				}
				else
				{
					SetItemSize(itemIndex, shownItemByItemIndex.CachedRectTransform.rect.width,
						shownItemByItemIndex.Padding);
				}
			}

			UpdateContentSize();
			UpdateAllShownItemsPos();
		}


		public void RefreshItemByItemIndex(int itemIndex)
		{
			int count = ItemList.Count;
			if (count == 0)
			{
				return;
			}

			if (itemIndex < ItemList[0].ItemIndex || itemIndex > ItemList[count - 1].ItemIndex)
			{
				return;
			}

			int itemIndex2 = ItemList[0].ItemIndex;
			int index = itemIndex - itemIndex2;
			LoopListViewItem2 loopListViewItem = ItemList[index];
			Vector3 anchoredPosition3D = loopListViewItem.CachedRectTransform.anchoredPosition3D;
			RecycleItemTmp(loopListViewItem);
			LoopListViewItem2 newItemByIndex = GetNewItemByIndex(itemIndex);
			if (newItemByIndex == null)
			{
				RefreshAllShownItemWithFirstIndex(itemIndex2);
				return;
			}

			ItemList[index] = newItemByIndex;
			if (mIsVertList)
			{
				anchoredPosition3D.x = newItemByIndex.StartPosOffset;
			}
			else
			{
				anchoredPosition3D.y = newItemByIndex.StartPosOffset;
			}

			newItemByIndex.CachedRectTransform.anchoredPosition3D = anchoredPosition3D;
			OnItemSizeChanged(itemIndex);
			ClearAllTmpRecycledItem();
		}


		public void FinishSnapImmediately()
		{
			UpdateSnapMove(true);
		}


		public void MovePanelToItemIndex(int itemIndex, float offset)
		{
			mScrollRect.StopMovement();
			mCurSnapData.Clear();
			if (mItemTotalCount == 0)
			{
				return;
			}

			if (itemIndex < 0 && mItemTotalCount > 0)
			{
				return;
			}

			if (mItemTotalCount > 0 && itemIndex >= mItemTotalCount)
			{
				itemIndex = mItemTotalCount - 1;
			}

			if (offset < 0f)
			{
				offset = 0f;
			}

			Vector3 zero = Vector3.zero;
			float viewPortSize = ViewPortSize;
			if (offset > viewPortSize)
			{
				offset = viewPortSize;
			}

			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				float num = mContainerTrans.anchoredPosition3D.y;
				if (num < 0f)
				{
					num = 0f;
				}

				zero.y = -num - offset;
			}
			else if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				float num2 = mContainerTrans.anchoredPosition3D.y;
				if (num2 > 0f)
				{
					num2 = 0f;
				}

				zero.y = -num2 + offset;
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				float num3 = mContainerTrans.anchoredPosition3D.x;
				if (num3 > 0f)
				{
					num3 = 0f;
				}

				zero.x = -num3 + offset;
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				float num4 = mContainerTrans.anchoredPosition3D.x;
				if (num4 < 0f)
				{
					num4 = 0f;
				}

				zero.x = -num4 - offset;
			}

			RecycleAllItem();
			LoopListViewItem2 newItemByIndex = GetNewItemByIndex(itemIndex);
			if (newItemByIndex == null)
			{
				ClearAllTmpRecycledItem();
				return;
			}

			if (mIsVertList)
			{
				zero.x = newItemByIndex.StartPosOffset;
			}
			else
			{
				zero.y = newItemByIndex.StartPosOffset;
			}

			newItemByIndex.CachedRectTransform.anchoredPosition3D = zero;
			if (mSupportScrollBar)
			{
				if (mIsVertList)
				{
					SetItemSize(itemIndex, newItemByIndex.CachedRectTransform.rect.height, newItemByIndex.Padding);
				}
				else
				{
					SetItemSize(itemIndex, newItemByIndex.CachedRectTransform.rect.width, newItemByIndex.Padding);
				}
			}

			ItemList.Add(newItemByIndex);
			UpdateContentSize();
			UpdateListView(viewPortSize + 100f, viewPortSize + 100f, viewPortSize, viewPortSize);
			AdjustPanelPos();
			ClearAllTmpRecycledItem();
			ForceSnapUpdateCheck();
			UpdateSnapMove(false, true);
		}


		public void RefreshAllShownItem()
		{
			if (ItemList.Count == 0)
			{
				return;
			}

			RefreshAllShownItemWithFirstIndex(ItemList[0].ItemIndex);
		}


		public void RefreshAllShownItemWithFirstIndex(int firstItemIndex)
		{
			int count = ItemList.Count;
			if (count == 0)
			{
				return;
			}

			Vector3 anchoredPosition3D = ItemList[0].CachedRectTransform.anchoredPosition3D;
			RecycleAllItem();
			for (int i = 0; i < count; i++)
			{
				int num = firstItemIndex + i;
				LoopListViewItem2 newItemByIndex = GetNewItemByIndex(num);
				if (newItemByIndex == null)
				{
					break;
				}

				if (mIsVertList)
				{
					anchoredPosition3D.x = newItemByIndex.StartPosOffset;
				}
				else
				{
					anchoredPosition3D.y = newItemByIndex.StartPosOffset;
				}

				newItemByIndex.CachedRectTransform.anchoredPosition3D = anchoredPosition3D;
				if (mSupportScrollBar)
				{
					if (mIsVertList)
					{
						SetItemSize(num, newItemByIndex.CachedRectTransform.rect.height, newItemByIndex.Padding);
					}
					else
					{
						SetItemSize(num, newItemByIndex.CachedRectTransform.rect.width, newItemByIndex.Padding);
					}
				}

				ItemList.Add(newItemByIndex);
			}

			UpdateContentSize();
			UpdateAllShownItemsPos();
			ClearAllTmpRecycledItem();
		}


		public void RefreshAllShownItemWithFirstIndexAndPos(int firstItemIndex, Vector3 pos)
		{
			RecycleAllItem();
			LoopListViewItem2 newItemByIndex = GetNewItemByIndex(firstItemIndex);
			if (newItemByIndex == null)
			{
				return;
			}

			if (mIsVertList)
			{
				pos.x = newItemByIndex.StartPosOffset;
			}
			else
			{
				pos.y = newItemByIndex.StartPosOffset;
			}

			newItemByIndex.CachedRectTransform.anchoredPosition3D = pos;
			if (mSupportScrollBar)
			{
				if (mIsVertList)
				{
					SetItemSize(firstItemIndex, newItemByIndex.CachedRectTransform.rect.height, newItemByIndex.Padding);
				}
				else
				{
					SetItemSize(firstItemIndex, newItemByIndex.CachedRectTransform.rect.width, newItemByIndex.Padding);
				}
			}

			ItemList.Add(newItemByIndex);
			UpdateContentSize();
			UpdateAllShownItemsPos();
			UpdateListView(mDistanceForRecycle0, mDistanceForRecycle1, mDistanceForNew0, mDistanceForNew1);
			ClearAllTmpRecycledItem();
		}


		private void RecycleItemTmp(LoopListViewItem2 item)
		{
			if (item == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(item.ItemPrefabName))
			{
				return;
			}

			ItemPool itemPool = null;
			if (!mItemPoolDict.TryGetValue(item.ItemPrefabName, out itemPool))
			{
				return;
			}

			itemPool.RecycleItem(item);
		}


		private void ClearAllTmpRecycledItem()
		{
			int count = mItemPoolList.Count;
			for (int i = 0; i < count; i++)
			{
				mItemPoolList[i].ClearTmpRecycledItem();
			}
		}


		private void RecycleAllItem()
		{
			foreach (LoopListViewItem2 item in ItemList)
			{
				RecycleItemTmp(item);
			}

			ItemList.Clear();
		}


		private void AdjustContainerPivot(RectTransform rtf)
		{
			Vector2 pivot = rtf.pivot;
			if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				pivot.y = 0f;
			}
			else if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				pivot.y = 1f;
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				pivot.x = 0f;
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				pivot.x = 1f;
			}

			rtf.pivot = pivot;
		}


		private void AdjustPivot(RectTransform rtf)
		{
			Vector2 pivot = rtf.pivot;
			if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				pivot.y = 0f;
			}
			else if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				pivot.y = 1f;
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				pivot.x = 0f;
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				pivot.x = 1f;
			}

			rtf.pivot = pivot;
		}


		private void AdjustContainerAnchor(RectTransform rtf)
		{
			Vector2 anchorMin = rtf.anchorMin;
			Vector2 anchorMax = rtf.anchorMax;
			if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				anchorMin.y = 0f;
				anchorMax.y = 0f;
			}
			else if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				anchorMin.y = 1f;
				anchorMax.y = 1f;
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				anchorMin.x = 0f;
				anchorMax.x = 0f;
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				anchorMin.x = 1f;
				anchorMax.x = 1f;
			}

			rtf.anchorMin = anchorMin;
			rtf.anchorMax = anchorMax;
		}


		private void AdjustAnchor(RectTransform rtf)
		{
			Vector2 anchorMin = rtf.anchorMin;
			Vector2 anchorMax = rtf.anchorMax;
			if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				anchorMin.y = 0f;
				anchorMax.y = 0f;
			}
			else if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				anchorMin.y = 1f;
				anchorMax.y = 1f;
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				anchorMin.x = 0f;
				anchorMax.x = 0f;
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				anchorMin.x = 1f;
				anchorMax.x = 1f;
			}

			rtf.anchorMin = anchorMin;
			rtf.anchorMax = anchorMax;
		}


		private void InitItemPool()
		{
			foreach (ItemPrefabConfData itemPrefabConfData in mItemPrefabDataList)
			{
				if (itemPrefabConfData.mItemPrefab == null)
				{
					Debug.LogError("A item prefab is null ");
				}
				else
				{
					string name = itemPrefabConfData.mItemPrefab.name;
					if (mItemPoolDict.ContainsKey(name))
					{
						Debug.LogError("A item prefab with name " + name + " has existed!");
					}
					else
					{
						RectTransform component = itemPrefabConfData.mItemPrefab.GetComponent<RectTransform>();
						if (component == null)
						{
							Debug.LogError("RectTransform component is not found in the prefab " + name);
						}
						else
						{
							AdjustAnchor(component);
							AdjustPivot(component);
							if (itemPrefabConfData.mItemPrefab.GetComponent<LoopListViewItem2>() == null)
							{
								itemPrefabConfData.mItemPrefab.AddComponent<LoopListViewItem2>();
							}

							ItemPool itemPool = new ItemPool();
							itemPool.Init(itemPrefabConfData.mItemPrefab, itemPrefabConfData.mPadding,
								itemPrefabConfData.mStartPosOffset, itemPrefabConfData.mInitCreateCount,
								mContainerTrans);
							mItemPoolDict.Add(name, itemPool);
							mItemPoolList.Add(itemPool);
						}
					}
				}
			}
		}


		private void CacheDragPointerEventData(PointerEventData eventData)
		{
			if (mPointerEventData == null)
			{
				mPointerEventData = new PointerEventData(EventSystem.current);
			}

			mPointerEventData.button = eventData.button;
			mPointerEventData.position = eventData.position;
			mPointerEventData.pointerPressRaycast = eventData.pointerPressRaycast;
			mPointerEventData.pointerCurrentRaycast = eventData.pointerCurrentRaycast;
		}


		private LoopListViewItem2 GetNewItemByIndex(int index)
		{
			if (mSupportScrollBar && index < 0)
			{
				return null;
			}

			if (mItemTotalCount > 0 && index >= mItemTotalCount)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = mOnGetItemByIndex(this, index);
			if (loopListViewItem == null)
			{
				return null;
			}

			loopListViewItem.ItemIndex = index;
			loopListViewItem.ItemCreatedCheckFrameCount = mListUpdateCheckFrameCount;
			return loopListViewItem;
		}


		private void SetItemSize(int itemIndex, float itemSize, float padding)
		{
			ItemPosMgr.SetItemSize(itemIndex, itemSize + padding);
			if (itemIndex >= mLastItemIndex)
			{
				mLastItemIndex = itemIndex;
				mLastItemPadding = padding;
			}
		}


		private bool GetPlusItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
		{
			return ItemPosMgr.GetItemIndexAndPosAtGivenPos(pos, ref index, ref itemPos);
		}


		private float GetItemPos(int itemIndex)
		{
			return ItemPosMgr.GetItemPos(itemIndex);
		}


		public Vector3 GetItemCornerPosInViewPort(LoopListViewItem2 item,
			ItemCornerEnum corner = ItemCornerEnum.LeftBottom)
		{
			item.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
			return mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[(int) corner]);
		}


		private void AdjustPanelPos()
		{
			if (ItemList.Count == 0)
			{
				return;
			}

			UpdateAllShownItemsPos();
			float viewPortSize = ViewPortSize;
			float contentPanelSize = GetContentPanelSize();
			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				if (contentPanelSize <= viewPortSize)
				{
					Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D.y = 0f;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D;
					ItemList[0].CachedRectTransform.anchoredPosition3D =
						new Vector3(ItemList[0].StartPosOffset, 0f, 0f);
					UpdateAllShownItemsPos();
					return;
				}

				ItemList[0].CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				if (mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]).y <
				    mViewPortRectLocalCorners[1].y)
				{
					Vector3 anchoredPosition3D2 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D2.y = 0f;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D2;
					ItemList[0].CachedRectTransform.anchoredPosition3D =
						new Vector3(ItemList[0].StartPosOffset, 0f, 0f);
					UpdateAllShownItemsPos();
					return;
				}

				ItemList[ItemList.Count - 1].CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				float num = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]).y -
				            mViewPortRectLocalCorners[0].y;
				if (num > 0f)
				{
					Vector3 anchoredPosition3D3 = ItemList[0].CachedRectTransform.anchoredPosition3D;
					anchoredPosition3D3.y -= num;
					ItemList[0].CachedRectTransform.anchoredPosition3D = anchoredPosition3D3;
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				if (contentPanelSize <= viewPortSize)
				{
					Vector3 anchoredPosition3D4 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D4.y = 0f;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D4;
					ItemList[0].CachedRectTransform.anchoredPosition3D =
						new Vector3(ItemList[0].StartPosOffset, 0f, 0f);
					UpdateAllShownItemsPos();
					return;
				}

				ItemList[0].CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				if (mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]).y >
				    mViewPortRectLocalCorners[0].y)
				{
					Vector3 anchoredPosition3D5 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D5.y = 0f;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D5;
					ItemList[0].CachedRectTransform.anchoredPosition3D =
						new Vector3(ItemList[0].StartPosOffset, 0f, 0f);
					UpdateAllShownItemsPos();
					return;
				}

				ItemList[ItemList.Count - 1].CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				float num2 = mViewPortRectLocalCorners[1].y - vector.y;
				if (num2 > 0f)
				{
					Vector3 anchoredPosition3D6 = ItemList[0].CachedRectTransform.anchoredPosition3D;
					anchoredPosition3D6.y += num2;
					ItemList[0].CachedRectTransform.anchoredPosition3D = anchoredPosition3D6;
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				if (contentPanelSize <= viewPortSize)
				{
					Vector3 anchoredPosition3D7 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D7.x = 0f;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D7;
					ItemList[0].CachedRectTransform.anchoredPosition3D =
						new Vector3(0f, ItemList[0].StartPosOffset, 0f);
					UpdateAllShownItemsPos();
					return;
				}

				ItemList[0].CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				if (mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]).x >
				    mViewPortRectLocalCorners[1].x)
				{
					Vector3 anchoredPosition3D8 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D8.x = 0f;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D8;
					ItemList[0].CachedRectTransform.anchoredPosition3D =
						new Vector3(0f, ItemList[0].StartPosOffset, 0f);
					UpdateAllShownItemsPos();
					return;
				}

				ItemList[ItemList.Count - 1].CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				float num3 = mViewPortRectLocalCorners[2].x - vector2.x;
				if (num3 > 0f)
				{
					Vector3 anchoredPosition3D9 = ItemList[0].CachedRectTransform.anchoredPosition3D;
					anchoredPosition3D9.x += num3;
					ItemList[0].CachedRectTransform.anchoredPosition3D = anchoredPosition3D9;
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				if (contentPanelSize <= viewPortSize)
				{
					Vector3 anchoredPosition3D10 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D10.x = 0f;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D10;
					ItemList[0].CachedRectTransform.anchoredPosition3D =
						new Vector3(0f, ItemList[0].StartPosOffset, 0f);
					UpdateAllShownItemsPos();
					return;
				}

				ItemList[0].CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				if (mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]).x <
				    mViewPortRectLocalCorners[2].x)
				{
					Vector3 anchoredPosition3D11 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D11.x = 0f;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D11;
					ItemList[0].CachedRectTransform.anchoredPosition3D =
						new Vector3(0f, ItemList[0].StartPosOffset, 0f);
					UpdateAllShownItemsPos();
					return;
				}

				ItemList[ItemList.Count - 1].CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				float num4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]).x -
				             mViewPortRectLocalCorners[1].x;
				if (num4 > 0f)
				{
					Vector3 anchoredPosition3D12 = ItemList[0].CachedRectTransform.anchoredPosition3D;
					anchoredPosition3D12.x -= num4;
					ItemList[0].CachedRectTransform.anchoredPosition3D = anchoredPosition3D12;
					UpdateAllShownItemsPos();
				}
			}
		}


		private void UpdateSnapMove(bool immediate = false, bool forceSendEvent = false)
		{
			if (!mItemSnapEnable)
			{
				return;
			}

			if (mIsVertList)
			{
				UpdateSnapVertical(immediate, forceSendEvent);
				return;
			}

			UpdateSnapHorizontal(immediate, forceSendEvent);
		}


		public void UpdateAllShownItemSnapData()
		{
			if (!mItemSnapEnable)
			{
				return;
			}

			int count = ItemList.Count;
			if (count == 0)
			{
				return;
			}

			Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
			LoopListViewItem2 loopListViewItem = ItemList[0];
			loopListViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				float num = -(1f - mViewPortSnapPivot.y) * mViewPortRectTransform.rect.height;
				float num2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]).y;
				float num3 = num2 - loopListViewItem.ItemSizeWithPadding;
				float num4 = num2 - loopListViewItem.ItemSize * (1f - mItemSnapPivot.y);
				for (int i = 0; i < count; i++)
				{
					ItemList[i].DistanceWithViewPortSnapCenter = num - num4;
					if (i + 1 < count)
					{
						num2 = num3;
						num3 -= ItemList[i + 1].ItemSizeWithPadding;
						num4 = num2 - ItemList[i + 1].ItemSize * (1f - mItemSnapPivot.y);
					}
				}

				return;
			}

			if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				float num = mViewPortSnapPivot.y * mViewPortRectTransform.rect.height;
				float num2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]).y;
				float num3 = num2 + loopListViewItem.ItemSizeWithPadding;
				float num4 = num2 + loopListViewItem.ItemSize * mItemSnapPivot.y;
				for (int j = 0; j < count; j++)
				{
					ItemList[j].DistanceWithViewPortSnapCenter = num - num4;
					if (j + 1 < count)
					{
						num2 = num3;
						num3 += ItemList[j + 1].ItemSizeWithPadding;
						num4 = num2 + ItemList[j + 1].ItemSize * mItemSnapPivot.y;
					}
				}

				return;
			}

			if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				float num = -(1f - mViewPortSnapPivot.x) * mViewPortRectTransform.rect.width;
				float num2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]).x;
				float num3 = num2 - loopListViewItem.ItemSizeWithPadding;
				float num4 = num2 - loopListViewItem.ItemSize * (1f - mItemSnapPivot.x);
				for (int k = 0; k < count; k++)
				{
					ItemList[k].DistanceWithViewPortSnapCenter = num - num4;
					if (k + 1 < count)
					{
						num2 = num3;
						num3 -= ItemList[k + 1].ItemSizeWithPadding;
						num4 = num2 - ItemList[k + 1].ItemSize * (1f - mItemSnapPivot.x);
					}
				}

				return;
			}

			if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				float num = mViewPortSnapPivot.x * mViewPortRectTransform.rect.width;
				float num2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]).x;
				float num3 = num2 + loopListViewItem.ItemSizeWithPadding;
				float num4 = num2 + loopListViewItem.ItemSize * mItemSnapPivot.x;
				for (int l = 0; l < count; l++)
				{
					ItemList[l].DistanceWithViewPortSnapCenter = num - num4;
					if (l + 1 < count)
					{
						num2 = num3;
						num3 += ItemList[l + 1].ItemSizeWithPadding;
						num4 = num2 + ItemList[l + 1].ItemSize * mItemSnapPivot.x;
					}
				}
			}
		}


		private void UpdateSnapVertical(bool immediate = false, bool forceSendEvent = false)
		{
			if (!mItemSnapEnable)
			{
				return;
			}

			int count = ItemList.Count;
			if (count == 0)
			{
				return;
			}

			Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
			bool flag = anchoredPosition3D.y != mLastSnapCheckPos.y;
			mLastSnapCheckPos = anchoredPosition3D;
			if (!flag && mLeftSnapUpdateExtraCount > 0)
			{
				mLeftSnapUpdateExtraCount--;
				flag = true;
			}

			if (flag)
			{
				LoopListViewItem2 loopListViewItem = ItemList[0];
				loopListViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				int num = -1;
				float num2 = float.MaxValue;
				if (mArrangeType == ListItemArrangeType.TopToBottom)
				{
					float num3 = -(1f - mViewPortSnapPivot.y) * mViewPortRectTransform.rect.height;
					float num4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]).y;
					float num5 = num4 - loopListViewItem.ItemSizeWithPadding;
					float num6 = num4 - loopListViewItem.ItemSize * (1f - mItemSnapPivot.y);
					for (int i = 0; i < count; i++)
					{
						float num7 = Mathf.Abs(num3 - num6);
						if (num7 >= num2)
						{
							break;
						}

						num2 = num7;
						num = i;
						if (i + 1 < count)
						{
							num4 = num5;
							num5 -= ItemList[i + 1].ItemSizeWithPadding;
							num6 = num4 - ItemList[i + 1].ItemSize * (1f - mItemSnapPivot.y);
						}
					}
				}
				else if (mArrangeType == ListItemArrangeType.BottomToTop)
				{
					float num3 = mViewPortSnapPivot.y * mViewPortRectTransform.rect.height;
					float num4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]).y;
					float num5 = num4 + loopListViewItem.ItemSizeWithPadding;
					float num6 = num4 + loopListViewItem.ItemSize * mItemSnapPivot.y;
					for (int j = 0; j < count; j++)
					{
						float num7 = Mathf.Abs(num3 - num6);
						if (num7 >= num2)
						{
							break;
						}

						num2 = num7;
						num = j;
						if (j + 1 < count)
						{
							num4 = num5;
							num5 += ItemList[j + 1].ItemSizeWithPadding;
							num6 = num4 + ItemList[j + 1].ItemSize * mItemSnapPivot.y;
						}
					}
				}

				if (num >= 0)
				{
					int num8 = mCurSnapNearestItemIndex;
					mCurSnapNearestItemIndex = ItemList[num].ItemIndex;
					if ((forceSendEvent || ItemList[num].ItemIndex != num8) && mOnSnapNearestChanged != null)
					{
						mOnSnapNearestChanged(this, ItemList[num]);
					}
				}
				else
				{
					mCurSnapNearestItemIndex = -1;
				}
			}

			if (!CanSnap())
			{
				ClearSnapData();
				return;
			}

			float num9 = Mathf.Abs(mScrollRect.velocity.y);
			UpdateCurSnapData();
			if (mCurSnapData.mSnapStatus != SnapStatus.SnapMoving)
			{
				return;
			}

			if (num9 > 0f)
			{
				mScrollRect.StopMovement();
			}

			float mCurSnapVal = mCurSnapData.mCurSnapVal;
			if (!mCurSnapData.mIsTempTarget)
			{
				if (mSmoothDumpVel * mCurSnapData.mTargetSnapVal < 0f)
				{
					mSmoothDumpVel = 0f;
				}

				mCurSnapData.mCurSnapVal = Mathf.SmoothDamp(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal,
					ref mSmoothDumpVel, mSmoothDumpRate);
			}
			else
			{
				float mMoveMaxAbsVec = mCurSnapData.mMoveMaxAbsVec;
				if (mMoveMaxAbsVec <= 0f)
				{
					mMoveMaxAbsVec = SnapMoveDefaultMaxAbsVec;
				}

				mSmoothDumpVel = mMoveMaxAbsVec * Mathf.Sign(mCurSnapData.mTargetSnapVal);
				mCurSnapData.mCurSnapVal = Mathf.MoveTowards(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal,
					mMoveMaxAbsVec * Time.deltaTime);
			}

			float num10 = mCurSnapData.mCurSnapVal - mCurSnapVal;
			if (immediate || Mathf.Abs(mCurSnapData.mTargetSnapVal - mCurSnapData.mCurSnapVal) < mSnapFinishThreshold)
			{
				anchoredPosition3D.y = anchoredPosition3D.y + mCurSnapData.mTargetSnapVal - mCurSnapVal;
				mCurSnapData.mSnapStatus = SnapStatus.SnapMoveFinish;
				if (mOnSnapItemFinished != null)
				{
					LoopListViewItem2 shownItemByItemIndex = GetShownItemByItemIndex(mCurSnapNearestItemIndex);
					if (shownItemByItemIndex != null)
					{
						mOnSnapItemFinished(this, shownItemByItemIndex);
					}
				}
			}
			else
			{
				anchoredPosition3D.y += num10;
			}

			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				float max = mViewPortRectLocalCorners[0].y + mContainerTrans.rect.height;
				anchoredPosition3D.y = Mathf.Clamp(anchoredPosition3D.y, 0f, max);
				mContainerTrans.anchoredPosition3D = anchoredPosition3D;
				return;
			}

			if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				float min = mViewPortRectLocalCorners[1].y - mContainerTrans.rect.height;
				anchoredPosition3D.y = Mathf.Clamp(anchoredPosition3D.y, min, 0f);
				mContainerTrans.anchoredPosition3D = anchoredPosition3D;
			}
		}


		private void UpdateCurSnapData()
		{
			if (ItemList.Count == 0)
			{
				mCurSnapData.Clear();
				return;
			}

			if (mCurSnapData.mSnapStatus == SnapStatus.SnapMoveFinish)
			{
				if (mCurSnapData.mSnapTargetIndex == mCurSnapNearestItemIndex)
				{
					return;
				}

				mCurSnapData.mSnapStatus = SnapStatus.NoTargetSet;
			}

			if (mCurSnapData.mSnapStatus == SnapStatus.SnapMoving)
			{
				if (mCurSnapData.mIsForceSnapTo)
				{
					if (mCurSnapData.mIsTempTarget)
					{
						LoopListViewItem2 shownItemNearestItemIndex =
							GetShownItemNearestItemIndex(mCurSnapData.mSnapTargetIndex);
						if (shownItemNearestItemIndex == null)
						{
							mCurSnapData.Clear();
							return;
						}

						if (shownItemNearestItemIndex.ItemIndex == mCurSnapData.mSnapTargetIndex)
						{
							UpdateAllShownItemSnapData();
							mCurSnapData.mTargetSnapVal = shownItemNearestItemIndex.DistanceWithViewPortSnapCenter;
							mCurSnapData.mCurSnapVal = 0f;
							mCurSnapData.mIsTempTarget = false;
							mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
							return;
						}

						if (mCurSnapData.mTempTargetIndex != shownItemNearestItemIndex.ItemIndex)
						{
							UpdateAllShownItemSnapData();
							mCurSnapData.mTargetSnapVal = shownItemNearestItemIndex.DistanceWithViewPortSnapCenter;
							mCurSnapData.mCurSnapVal = 0f;
							mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
							mCurSnapData.mIsTempTarget = true;
							mCurSnapData.mTempTargetIndex = shownItemNearestItemIndex.ItemIndex;
							return;
						}
					}

					return;
				}

				if (mCurSnapData.mSnapTargetIndex == mCurSnapNearestItemIndex)
				{
					return;
				}

				mCurSnapData.mSnapStatus = SnapStatus.NoTargetSet;
			}

			if (mCurSnapData.mSnapStatus == SnapStatus.NoTargetSet)
			{
				if (GetShownItemByItemIndex(mCurSnapNearestItemIndex) == null)
				{
					return;
				}

				mCurSnapData.mSnapTargetIndex = mCurSnapNearestItemIndex;
				mCurSnapData.mSnapStatus = SnapStatus.TargetHasSet;
				mCurSnapData.mIsForceSnapTo = false;
			}

			if (mCurSnapData.mSnapStatus == SnapStatus.TargetHasSet)
			{
				LoopListViewItem2 shownItemNearestItemIndex2 =
					GetShownItemNearestItemIndex(mCurSnapData.mSnapTargetIndex);
				if (shownItemNearestItemIndex2 == null)
				{
					mCurSnapData.Clear();
					return;
				}

				if (shownItemNearestItemIndex2.ItemIndex == mCurSnapData.mSnapTargetIndex)
				{
					UpdateAllShownItemSnapData();
					mCurSnapData.mTargetSnapVal = shownItemNearestItemIndex2.DistanceWithViewPortSnapCenter;
					mCurSnapData.mCurSnapVal = 0f;
					mCurSnapData.mIsTempTarget = false;
					mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
					return;
				}

				UpdateAllShownItemSnapData();
				mCurSnapData.mTargetSnapVal = shownItemNearestItemIndex2.DistanceWithViewPortSnapCenter;
				mCurSnapData.mCurSnapVal = 0f;
				mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
				mCurSnapData.mIsTempTarget = true;
				mCurSnapData.mTempTargetIndex = shownItemNearestItemIndex2.ItemIndex;
			}
		}


		public void ClearSnapData()
		{
			mCurSnapData.Clear();
		}


		public void SetSnapTargetItemIndex(int itemIndex, float moveMaxAbsVec = -1f)
		{
			if (mItemTotalCount > 0)
			{
				if (itemIndex >= mItemTotalCount)
				{
					itemIndex = mItemTotalCount - 1;
				}

				if (itemIndex < 0)
				{
					itemIndex = 0;
				}
			}

			mScrollRect.StopMovement();
			mCurSnapData.mSnapTargetIndex = itemIndex;
			mCurSnapData.mSnapStatus = SnapStatus.TargetHasSet;
			mCurSnapData.mIsForceSnapTo = true;
			mCurSnapData.mMoveMaxAbsVec = moveMaxAbsVec;
		}


		public void ForceSnapUpdateCheck()
		{
			if (mLeftSnapUpdateExtraCount <= 0)
			{
				mLeftSnapUpdateExtraCount = 1;
			}
		}


		private void UpdateSnapHorizontal(bool immediate = false, bool forceSendEvent = false)
		{
			if (!mItemSnapEnable)
			{
				return;
			}

			int count = ItemList.Count;
			if (count == 0)
			{
				return;
			}

			Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
			bool flag = anchoredPosition3D.x != mLastSnapCheckPos.x;
			mLastSnapCheckPos = anchoredPosition3D;
			if (!flag && mLeftSnapUpdateExtraCount > 0)
			{
				mLeftSnapUpdateExtraCount--;
				flag = true;
			}

			if (flag)
			{
				LoopListViewItem2 loopListViewItem = ItemList[0];
				loopListViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				int num = -1;
				float num2 = float.MaxValue;
				if (mArrangeType == ListItemArrangeType.RightToLeft)
				{
					float num3 = -(1f - mViewPortSnapPivot.x) * mViewPortRectTransform.rect.width;
					float num4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]).x;
					float num5 = num4 - loopListViewItem.ItemSizeWithPadding;
					float num6 = num4 - loopListViewItem.ItemSize * (1f - mItemSnapPivot.x);
					for (int i = 0; i < count; i++)
					{
						float num7 = Mathf.Abs(num3 - num6);
						if (num7 >= num2)
						{
							break;
						}

						num2 = num7;
						num = i;
						if (i + 1 < count)
						{
							num4 = num5;
							num5 -= ItemList[i + 1].ItemSizeWithPadding;
							num6 = num4 - ItemList[i + 1].ItemSize * (1f - mItemSnapPivot.x);
						}
					}
				}
				else if (mArrangeType == ListItemArrangeType.LeftToRight)
				{
					float num3 = mViewPortSnapPivot.x * mViewPortRectTransform.rect.width;
					float num4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]).x;
					float num5 = num4 + loopListViewItem.ItemSizeWithPadding;
					float num6 = num4 + loopListViewItem.ItemSize * mItemSnapPivot.x;
					for (int j = 0; j < count; j++)
					{
						float num7 = Mathf.Abs(num3 - num6);
						if (num7 >= num2)
						{
							break;
						}

						num2 = num7;
						num = j;
						if (j + 1 < count)
						{
							num4 = num5;
							num5 += ItemList[j + 1].ItemSizeWithPadding;
							num6 = num4 + ItemList[j + 1].ItemSize * mItemSnapPivot.x;
						}
					}
				}

				if (num >= 0)
				{
					int num8 = mCurSnapNearestItemIndex;
					mCurSnapNearestItemIndex = ItemList[num].ItemIndex;
					if ((forceSendEvent || ItemList[num].ItemIndex != num8) && mOnSnapNearestChanged != null)
					{
						mOnSnapNearestChanged(this, ItemList[num]);
					}
				}
				else
				{
					mCurSnapNearestItemIndex = -1;
				}
			}

			if (!CanSnap())
			{
				ClearSnapData();
				return;
			}

			float num9 = Mathf.Abs(mScrollRect.velocity.x);
			UpdateCurSnapData();
			if (mCurSnapData.mSnapStatus != SnapStatus.SnapMoving)
			{
				return;
			}

			if (num9 > 0f)
			{
				mScrollRect.StopMovement();
			}

			float mCurSnapVal = mCurSnapData.mCurSnapVal;
			if (!mCurSnapData.mIsTempTarget)
			{
				if (mSmoothDumpVel * mCurSnapData.mTargetSnapVal < 0f)
				{
					mSmoothDumpVel = 0f;
				}

				mCurSnapData.mCurSnapVal = Mathf.SmoothDamp(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal,
					ref mSmoothDumpVel, mSmoothDumpRate);
			}
			else
			{
				float mMoveMaxAbsVec = mCurSnapData.mMoveMaxAbsVec;
				if (mMoveMaxAbsVec <= 0f)
				{
					mMoveMaxAbsVec = SnapMoveDefaultMaxAbsVec;
				}

				mSmoothDumpVel = mMoveMaxAbsVec * Mathf.Sign(mCurSnapData.mTargetSnapVal);
				mCurSnapData.mCurSnapVal = Mathf.MoveTowards(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal,
					mMoveMaxAbsVec * Time.deltaTime);
			}

			float num10 = mCurSnapData.mCurSnapVal - mCurSnapVal;
			if (immediate || Mathf.Abs(mCurSnapData.mTargetSnapVal - mCurSnapData.mCurSnapVal) < mSnapFinishThreshold)
			{
				anchoredPosition3D.x = anchoredPosition3D.x + mCurSnapData.mTargetSnapVal - mCurSnapVal;
				mCurSnapData.mSnapStatus = SnapStatus.SnapMoveFinish;
				if (mOnSnapItemFinished != null)
				{
					LoopListViewItem2 shownItemByItemIndex = GetShownItemByItemIndex(mCurSnapNearestItemIndex);
					if (shownItemByItemIndex != null)
					{
						mOnSnapItemFinished(this, shownItemByItemIndex);
					}
				}
			}
			else
			{
				anchoredPosition3D.x += num10;
			}

			if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				float min = mViewPortRectLocalCorners[2].x - mContainerTrans.rect.width;
				anchoredPosition3D.x = Mathf.Clamp(anchoredPosition3D.x, min, 0f);
				mContainerTrans.anchoredPosition3D = anchoredPosition3D;
				return;
			}

			if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				float max = mViewPortRectLocalCorners[1].x + mContainerTrans.rect.width;
				anchoredPosition3D.x = Mathf.Clamp(anchoredPosition3D.x, 0f, max);
				mContainerTrans.anchoredPosition3D = anchoredPosition3D;
			}
		}


		private bool CanSnap()
		{
			if (mIsDraging)
			{
				return false;
			}

			if (mScrollBarClickEventListener != null && mScrollBarClickEventListener.IsPressd)
			{
				return false;
			}

			if (mIsVertList)
			{
				if (mContainerTrans.rect.height <= ViewPortHeight)
				{
					return false;
				}
			}
			else if (mContainerTrans.rect.width <= ViewPortWidth)
			{
				return false;
			}

			float num;
			if (mIsVertList)
			{
				num = Mathf.Abs(mScrollRect.velocity.y);
			}
			else
			{
				num = Mathf.Abs(mScrollRect.velocity.x);
			}

			if (num > mSnapVecThreshold)
			{
				return false;
			}

			float num2 = 3f;
			Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
			if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				float num3 = mViewPortRectLocalCorners[2].x - mContainerTrans.rect.width;
				if (anchoredPosition3D.x < num3 - num2 || anchoredPosition3D.x > num2)
				{
					return false;
				}
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				float num4 = mViewPortRectLocalCorners[1].x + mContainerTrans.rect.width;
				if (anchoredPosition3D.x > num4 + num2 || anchoredPosition3D.x < -num2)
				{
					return false;
				}
			}
			else if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				float num5 = mViewPortRectLocalCorners[0].y + mContainerTrans.rect.height;
				if (anchoredPosition3D.y > num5 + num2 || anchoredPosition3D.y < -num2)
				{
					return false;
				}
			}
			else if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				float num6 = mViewPortRectLocalCorners[1].y - mContainerTrans.rect.height;
				if (anchoredPosition3D.y < num6 - num2 || anchoredPosition3D.y > num2)
				{
					return false;
				}
			}

			return true;
		}


		public void UpdateListView(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0,
			float distanceForNew1)
		{
			mListUpdateCheckFrameCount++;
			if (mIsVertList)
			{
				bool flag = true;
				int num = 0;
				int num2 = 9999;
				while (flag)
				{
					num++;
					if (num >= num2)
					{
						Debug.LogError("UpdateListView Vertical while loop " + num + " times! something is wrong!");
						return;
					}

					flag = UpdateForVertList(distanceForRecycle0, distanceForRecycle1, distanceForNew0,
						distanceForNew1);
				}

				return;
			}

			bool flag2 = true;
			int num3 = 0;
			int num4 = 9999;
			while (flag2)
			{
				num3++;
				if (num3 >= num4)
				{
					Debug.LogError("UpdateListView  Horizontal while loop " + num3 + " times! something is wrong!");
					return;
				}

				flag2 = UpdateForHorizontalList(distanceForRecycle0, distanceForRecycle1, distanceForNew0,
					distanceForNew1);
			}
		}


		private bool UpdateForVertList(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0,
			float distanceForNew1)
		{
			if (mItemTotalCount == 0)
			{
				if (ItemList.Count > 0)
				{
					RecycleAllItem();
				}

				return false;
			}

			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				if (ItemList.Count == 0)
				{
					float num = mContainerTrans.anchoredPosition3D.y;
					if (num < 0f)
					{
						num = 0f;
					}

					int num2 = 0;
					float num3 = -num;
					if (mSupportScrollBar)
					{
						if (!GetPlusItemIndexAndPosAtGivenPos(num, ref num2, ref num3))
						{
							return false;
						}

						num3 = -num3;
					}

					LoopListViewItem2 newItemByIndex = GetNewItemByIndex(num2);
					if (newItemByIndex == null)
					{
						return false;
					}

					if (mSupportScrollBar)
					{
						SetItemSize(num2, newItemByIndex.CachedRectTransform.rect.height, newItemByIndex.Padding);
					}

					ItemList.Add(newItemByIndex);
					newItemByIndex.CachedRectTransform.anchoredPosition3D =
						new Vector3(newItemByIndex.StartPosOffset, num3, 0f);
					UpdateContentSize();
					return true;
				}

				LoopListViewItem2 loopListViewItem = ItemList[0];
				loopListViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (!mIsDraging && loopListViewItem.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    vector2.y - mViewPortRectLocalCorners[1].y > distanceForRecycle0)
				{
					ItemList.RemoveAt(0);
					RecycleItemTmp(loopListViewItem);
					if (!mSupportScrollBar)
					{
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
					}

					return true;
				}

				LoopListViewItem2 loopListViewItem2 = ItemList[ItemList.Count - 1];
				loopListViewItem2.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector3 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (!mIsDraging && loopListViewItem2.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    mViewPortRectLocalCorners[0].y - vector3.y > distanceForRecycle1)
				{
					ItemList.RemoveAt(ItemList.Count - 1);
					RecycleItemTmp(loopListViewItem2);
					if (!mSupportScrollBar)
					{
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
					}

					return true;
				}

				if (mViewPortRectLocalCorners[0].y - vector4.y < distanceForNew1)
				{
					if (loopListViewItem2.ItemIndex > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopListViewItem2.ItemIndex;
						mNeedCheckNextMaxItem = true;
					}

					int num4 = loopListViewItem2.ItemIndex + 1;
					if (num4 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopListViewItem2 newItemByIndex2 = GetNewItemByIndex(num4);
						if (!(newItemByIndex2 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num4, newItemByIndex2.CachedRectTransform.rect.height,
									newItemByIndex2.Padding);
							}

							ItemList.Add(newItemByIndex2);
							float y = loopListViewItem2.CachedRectTransform.anchoredPosition3D.y -
							          loopListViewItem2.CachedRectTransform.rect.height - loopListViewItem2.Padding;
							newItemByIndex2.CachedRectTransform.anchoredPosition3D =
								new Vector3(newItemByIndex2.StartPosOffset, y, 0f);
							UpdateContentSize();
							CheckIfNeedUpdataItemPos();
							if (num4 > mCurReadyMaxItemIndex)
							{
								mCurReadyMaxItemIndex = num4;
							}

							return true;
						}

						mCurReadyMaxItemIndex = loopListViewItem2.ItemIndex;
						mNeedCheckNextMaxItem = false;
						CheckIfNeedUpdataItemPos();
					}
				}

				if (vector.y - mViewPortRectLocalCorners[1].y < distanceForNew0)
				{
					if (loopListViewItem.ItemIndex < mCurReadyMinItemIndex)
					{
						mCurReadyMinItemIndex = loopListViewItem.ItemIndex;
						mNeedCheckNextMinItem = true;
					}

					int num5 = loopListViewItem.ItemIndex - 1;
					if (num5 >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
					{
						LoopListViewItem2 newItemByIndex3 = GetNewItemByIndex(num5);
						if (!(newItemByIndex3 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num5, newItemByIndex3.CachedRectTransform.rect.height,
									newItemByIndex3.Padding);
							}

							ItemList.Insert(0, newItemByIndex3);
							float y2 = loopListViewItem.CachedRectTransform.anchoredPosition3D.y +
							           newItemByIndex3.CachedRectTransform.rect.height + newItemByIndex3.Padding;
							newItemByIndex3.CachedRectTransform.anchoredPosition3D =
								new Vector3(newItemByIndex3.StartPosOffset, y2, 0f);
							UpdateContentSize();
							CheckIfNeedUpdataItemPos();
							if (num5 < mCurReadyMinItemIndex)
							{
								mCurReadyMinItemIndex = num5;
							}

							return true;
						}

						mCurReadyMinItemIndex = loopListViewItem.ItemIndex;
						mNeedCheckNextMinItem = false;
					}
				}
			}
			else if (ItemList.Count == 0)
			{
				float num6 = mContainerTrans.anchoredPosition3D.y;
				if (num6 > 0f)
				{
					num6 = 0f;
				}

				int num7 = 0;
				float y3 = -num6;
				if (mSupportScrollBar && !GetPlusItemIndexAndPosAtGivenPos(-num6, ref num7, ref y3))
				{
					return false;
				}

				LoopListViewItem2 newItemByIndex4 = GetNewItemByIndex(num7);
				if (newItemByIndex4 == null)
				{
					return false;
				}

				if (mSupportScrollBar)
				{
					SetItemSize(num7, newItemByIndex4.CachedRectTransform.rect.height, newItemByIndex4.Padding);
				}

				ItemList.Add(newItemByIndex4);
				newItemByIndex4.CachedRectTransform.anchoredPosition3D =
					new Vector3(newItemByIndex4.StartPosOffset, y3, 0f);
				UpdateContentSize();
				return true;
			}
			else
			{
				LoopListViewItem2 loopListViewItem3 = ItemList[0];
				loopListViewItem3.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector5 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector6 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (!mIsDraging && loopListViewItem3.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    mViewPortRectLocalCorners[0].y - vector5.y > distanceForRecycle0)
				{
					ItemList.RemoveAt(0);
					RecycleItemTmp(loopListViewItem3);
					if (!mSupportScrollBar)
					{
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
					}

					return true;
				}

				LoopListViewItem2 loopListViewItem4 = ItemList[ItemList.Count - 1];
				loopListViewItem4.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector7 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector8 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (!mIsDraging && loopListViewItem4.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    vector8.y - mViewPortRectLocalCorners[1].y > distanceForRecycle1)
				{
					ItemList.RemoveAt(ItemList.Count - 1);
					RecycleItemTmp(loopListViewItem4);
					if (!mSupportScrollBar)
					{
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
					}

					return true;
				}

				if (vector7.y - mViewPortRectLocalCorners[1].y < distanceForNew1)
				{
					if (loopListViewItem4.ItemIndex > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopListViewItem4.ItemIndex;
						mNeedCheckNextMaxItem = true;
					}

					int num8 = loopListViewItem4.ItemIndex + 1;
					if (num8 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopListViewItem2 newItemByIndex5 = GetNewItemByIndex(num8);
						if (!(newItemByIndex5 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num8, newItemByIndex5.CachedRectTransform.rect.height,
									newItemByIndex5.Padding);
							}

							ItemList.Add(newItemByIndex5);
							float y4 = loopListViewItem4.CachedRectTransform.anchoredPosition3D.y +
							           loopListViewItem4.CachedRectTransform.rect.height + loopListViewItem4.Padding;
							newItemByIndex5.CachedRectTransform.anchoredPosition3D =
								new Vector3(newItemByIndex5.StartPosOffset, y4, 0f);
							UpdateContentSize();
							CheckIfNeedUpdataItemPos();
							if (num8 > mCurReadyMaxItemIndex)
							{
								mCurReadyMaxItemIndex = num8;
							}

							return true;
						}

						mNeedCheckNextMaxItem = false;
						CheckIfNeedUpdataItemPos();
					}
				}

				if (mViewPortRectLocalCorners[0].y - vector6.y < distanceForNew0)
				{
					if (loopListViewItem3.ItemIndex < mCurReadyMinItemIndex)
					{
						mCurReadyMinItemIndex = loopListViewItem3.ItemIndex;
						mNeedCheckNextMinItem = true;
					}

					int num9 = loopListViewItem3.ItemIndex - 1;
					if (num9 >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
					{
						LoopListViewItem2 newItemByIndex6 = GetNewItemByIndex(num9);
						if (newItemByIndex6 == null)
						{
							mNeedCheckNextMinItem = false;
							return false;
						}

						if (mSupportScrollBar)
						{
							SetItemSize(num9, newItemByIndex6.CachedRectTransform.rect.height, newItemByIndex6.Padding);
						}

						ItemList.Insert(0, newItemByIndex6);
						float y5 = loopListViewItem3.CachedRectTransform.anchoredPosition3D.y -
						           newItemByIndex6.CachedRectTransform.rect.height - newItemByIndex6.Padding;
						newItemByIndex6.CachedRectTransform.anchoredPosition3D =
							new Vector3(newItemByIndex6.StartPosOffset, y5, 0f);
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
						if (num9 < mCurReadyMinItemIndex)
						{
							mCurReadyMinItemIndex = num9;
						}

						return true;
					}
				}
			}

			return false;
		}


		private bool UpdateForHorizontalList(float distanceForRecycle0, float distanceForRecycle1,
			float distanceForNew0, float distanceForNew1)
		{
			if (mItemTotalCount == 0)
			{
				if (ItemList.Count > 0)
				{
					RecycleAllItem();
				}

				return false;
			}

			if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				if (ItemList.Count == 0)
				{
					float num = mContainerTrans.anchoredPosition3D.x;
					if (num > 0f)
					{
						num = 0f;
					}

					int num2 = 0;
					float x = -num;
					if (mSupportScrollBar && !GetPlusItemIndexAndPosAtGivenPos(-num, ref num2, ref x))
					{
						return false;
					}

					LoopListViewItem2 newItemByIndex = GetNewItemByIndex(num2);
					if (newItemByIndex == null)
					{
						return false;
					}

					if (mSupportScrollBar)
					{
						SetItemSize(num2, newItemByIndex.CachedRectTransform.rect.width, newItemByIndex.Padding);
					}

					ItemList.Add(newItemByIndex);
					newItemByIndex.CachedRectTransform.anchoredPosition3D =
						new Vector3(x, newItemByIndex.StartPosOffset, 0f);
					UpdateContentSize();
					return true;
				}

				LoopListViewItem2 loopListViewItem = ItemList[0];
				loopListViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				if (!mIsDraging && loopListViewItem.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    mViewPortRectLocalCorners[1].x - vector2.x > distanceForRecycle0)
				{
					ItemList.RemoveAt(0);
					RecycleItemTmp(loopListViewItem);
					if (!mSupportScrollBar)
					{
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
					}

					return true;
				}

				LoopListViewItem2 loopListViewItem2 = ItemList[ItemList.Count - 1];
				loopListViewItem2.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector3 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				if (!mIsDraging && loopListViewItem2.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    vector3.x - mViewPortRectLocalCorners[2].x > distanceForRecycle1)
				{
					ItemList.RemoveAt(ItemList.Count - 1);
					RecycleItemTmp(loopListViewItem2);
					if (!mSupportScrollBar)
					{
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
					}

					return true;
				}

				if (vector4.x - mViewPortRectLocalCorners[2].x < distanceForNew1)
				{
					if (loopListViewItem2.ItemIndex > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopListViewItem2.ItemIndex;
						mNeedCheckNextMaxItem = true;
					}

					int num3 = loopListViewItem2.ItemIndex + 1;
					if (num3 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopListViewItem2 newItemByIndex2 = GetNewItemByIndex(num3);
						if (!(newItemByIndex2 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num3, newItemByIndex2.CachedRectTransform.rect.width,
									newItemByIndex2.Padding);
							}

							ItemList.Add(newItemByIndex2);
							float x2 = loopListViewItem2.CachedRectTransform.anchoredPosition3D.x +
							           loopListViewItem2.CachedRectTransform.rect.width + loopListViewItem2.Padding;
							newItemByIndex2.CachedRectTransform.anchoredPosition3D =
								new Vector3(x2, newItemByIndex2.StartPosOffset, 0f);
							UpdateContentSize();
							CheckIfNeedUpdataItemPos();
							if (num3 > mCurReadyMaxItemIndex)
							{
								mCurReadyMaxItemIndex = num3;
							}

							return true;
						}

						mCurReadyMaxItemIndex = loopListViewItem2.ItemIndex;
						mNeedCheckNextMaxItem = false;
						CheckIfNeedUpdataItemPos();
					}
				}

				if (mViewPortRectLocalCorners[1].x - vector.x < distanceForNew0)
				{
					if (loopListViewItem.ItemIndex < mCurReadyMinItemIndex)
					{
						mCurReadyMinItemIndex = loopListViewItem.ItemIndex;
						mNeedCheckNextMinItem = true;
					}

					int num4 = loopListViewItem.ItemIndex - 1;
					if (num4 >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
					{
						LoopListViewItem2 newItemByIndex3 = GetNewItemByIndex(num4);
						if (!(newItemByIndex3 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num4, newItemByIndex3.CachedRectTransform.rect.width,
									newItemByIndex3.Padding);
							}

							ItemList.Insert(0, newItemByIndex3);
							float x3 = loopListViewItem.CachedRectTransform.anchoredPosition3D.x -
							           newItemByIndex3.CachedRectTransform.rect.width - newItemByIndex3.Padding;
							newItemByIndex3.CachedRectTransform.anchoredPosition3D =
								new Vector3(x3, newItemByIndex3.StartPosOffset, 0f);
							UpdateContentSize();
							CheckIfNeedUpdataItemPos();
							if (num4 < mCurReadyMinItemIndex)
							{
								mCurReadyMinItemIndex = num4;
							}

							return true;
						}

						mCurReadyMinItemIndex = loopListViewItem.ItemIndex;
						mNeedCheckNextMinItem = false;
					}
				}
			}
			else if (ItemList.Count == 0)
			{
				float num5 = mContainerTrans.anchoredPosition3D.x;
				if (num5 < 0f)
				{
					num5 = 0f;
				}

				int num6 = 0;
				float num7 = -num5;
				if (mSupportScrollBar)
				{
					if (!GetPlusItemIndexAndPosAtGivenPos(num5, ref num6, ref num7))
					{
						return false;
					}

					num7 = -num7;
				}

				LoopListViewItem2 newItemByIndex4 = GetNewItemByIndex(num6);
				if (newItemByIndex4 == null)
				{
					return false;
				}

				if (mSupportScrollBar)
				{
					SetItemSize(num6, newItemByIndex4.CachedRectTransform.rect.width, newItemByIndex4.Padding);
				}

				ItemList.Add(newItemByIndex4);
				newItemByIndex4.CachedRectTransform.anchoredPosition3D =
					new Vector3(num7, newItemByIndex4.StartPosOffset, 0f);
				UpdateContentSize();
				return true;
			}
			else
			{
				LoopListViewItem2 loopListViewItem3 = ItemList[0];
				loopListViewItem3.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector5 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector6 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				if (!mIsDraging && loopListViewItem3.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    vector5.x - mViewPortRectLocalCorners[2].x > distanceForRecycle0)
				{
					ItemList.RemoveAt(0);
					RecycleItemTmp(loopListViewItem3);
					if (!mSupportScrollBar)
					{
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
					}

					return true;
				}

				LoopListViewItem2 loopListViewItem4 = ItemList[ItemList.Count - 1];
				loopListViewItem4.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector7 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector8 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				if (!mIsDraging && loopListViewItem4.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    mViewPortRectLocalCorners[1].x - vector8.x > distanceForRecycle1)
				{
					ItemList.RemoveAt(ItemList.Count - 1);
					RecycleItemTmp(loopListViewItem4);
					if (!mSupportScrollBar)
					{
						UpdateContentSize();
						CheckIfNeedUpdataItemPos();
					}

					return true;
				}

				if (mViewPortRectLocalCorners[1].x - vector7.x < distanceForNew1)
				{
					if (loopListViewItem4.ItemIndex > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopListViewItem4.ItemIndex;
						mNeedCheckNextMaxItem = true;
					}

					int num8 = loopListViewItem4.ItemIndex + 1;
					if (num8 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopListViewItem2 newItemByIndex5 = GetNewItemByIndex(num8);
						if (!(newItemByIndex5 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num8, newItemByIndex5.CachedRectTransform.rect.width,
									newItemByIndex5.Padding);
							}

							ItemList.Add(newItemByIndex5);
							float x4 = loopListViewItem4.CachedRectTransform.anchoredPosition3D.x -
							           loopListViewItem4.CachedRectTransform.rect.width - loopListViewItem4.Padding;
							newItemByIndex5.CachedRectTransform.anchoredPosition3D =
								new Vector3(x4, newItemByIndex5.StartPosOffset, 0f);
							UpdateContentSize();
							CheckIfNeedUpdataItemPos();
							if (num8 > mCurReadyMaxItemIndex)
							{
								mCurReadyMaxItemIndex = num8;
							}

							return true;
						}

						mCurReadyMaxItemIndex = loopListViewItem4.ItemIndex;
						mNeedCheckNextMaxItem = false;
						CheckIfNeedUpdataItemPos();
					}
				}

				if (vector6.x - mViewPortRectLocalCorners[2].x < distanceForNew0)
				{
					if (loopListViewItem3.ItemIndex < mCurReadyMinItemIndex)
					{
						mCurReadyMinItemIndex = loopListViewItem3.ItemIndex;
						mNeedCheckNextMinItem = true;
					}

					int num9 = loopListViewItem3.ItemIndex - 1;
					if (num9 >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
					{
						LoopListViewItem2 newItemByIndex6 = GetNewItemByIndex(num9);
						if (!(newItemByIndex6 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num9, newItemByIndex6.CachedRectTransform.rect.width,
									newItemByIndex6.Padding);
							}

							ItemList.Insert(0, newItemByIndex6);
							float x5 = loopListViewItem3.CachedRectTransform.anchoredPosition3D.x +
							           newItemByIndex6.CachedRectTransform.rect.width + newItemByIndex6.Padding;
							newItemByIndex6.CachedRectTransform.anchoredPosition3D =
								new Vector3(x5, newItemByIndex6.StartPosOffset, 0f);
							UpdateContentSize();
							CheckIfNeedUpdataItemPos();
							if (num9 < mCurReadyMinItemIndex)
							{
								mCurReadyMinItemIndex = num9;
							}

							return true;
						}

						mCurReadyMinItemIndex = loopListViewItem3.ItemIndex;
						mNeedCheckNextMinItem = false;
					}
				}
			}

			return false;
		}


		private float GetContentPanelSize()
		{
			if (mSupportScrollBar)
			{
				float num = ItemPosMgr.mTotalSize > 0f ? ItemPosMgr.mTotalSize - mLastItemPadding : 0f;
				if (num < 0f)
				{
					num = 0f;
				}

				return num;
			}

			int count = ItemList.Count;
			if (count == 0)
			{
				return 0f;
			}

			if (count == 1)
			{
				return ItemList[0].ItemSize;
			}

			if (count == 2)
			{
				return ItemList[0].ItemSizeWithPadding + ItemList[1].ItemSize;
			}

			float num2 = 0f;
			for (int i = 0; i < count - 1; i++)
			{
				num2 += ItemList[i].ItemSizeWithPadding;
			}

			return num2 + ItemList[count - 1].ItemSize;
		}


		private void CheckIfNeedUpdataItemPos()
		{
			if (ItemList.Count == 0)
			{
				return;
			}

			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				LoopListViewItem2 loopListViewItem = ItemList[0];
				LoopListViewItem2 loopListViewItem2 = ItemList[ItemList.Count - 1];
				float contentPanelSize = GetContentPanelSize();
				if (loopListViewItem.TopY > 0f ||
				    loopListViewItem.ItemIndex == mCurReadyMinItemIndex && loopListViewItem.TopY != 0f)
				{
					UpdateAllShownItemsPos();
					return;
				}

				if (-loopListViewItem2.BottomY > contentPanelSize ||
				    loopListViewItem2.ItemIndex == mCurReadyMaxItemIndex &&
				    -loopListViewItem2.BottomY != contentPanelSize)
				{
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				LoopListViewItem2 loopListViewItem3 = ItemList[0];
				LoopListViewItem2 loopListViewItem4 = ItemList[ItemList.Count - 1];
				float contentPanelSize2 = GetContentPanelSize();
				if (loopListViewItem3.BottomY < 0f || loopListViewItem3.ItemIndex == mCurReadyMinItemIndex &&
					loopListViewItem3.BottomY != 0f)
				{
					UpdateAllShownItemsPos();
					return;
				}

				if (loopListViewItem4.TopY > contentPanelSize2 ||
				    loopListViewItem4.ItemIndex == mCurReadyMaxItemIndex && loopListViewItem4.TopY != contentPanelSize2)
				{
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				LoopListViewItem2 loopListViewItem5 = ItemList[0];
				LoopListViewItem2 loopListViewItem6 = ItemList[ItemList.Count - 1];
				float contentPanelSize3 = GetContentPanelSize();
				if (loopListViewItem5.LeftX < 0f || loopListViewItem5.ItemIndex == mCurReadyMinItemIndex &&
					loopListViewItem5.LeftX != 0f)
				{
					UpdateAllShownItemsPos();
					return;
				}

				if (loopListViewItem6.RightX > contentPanelSize3 ||
				    loopListViewItem6.ItemIndex == mCurReadyMaxItemIndex &&
				    loopListViewItem6.RightX != contentPanelSize3)
				{
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				LoopListViewItem2 loopListViewItem7 = ItemList[0];
				LoopListViewItem2 loopListViewItem8 = ItemList[ItemList.Count - 1];
				float contentPanelSize4 = GetContentPanelSize();
				if (loopListViewItem7.RightX > 0f || loopListViewItem7.ItemIndex == mCurReadyMinItemIndex &&
					loopListViewItem7.RightX != 0f)
				{
					UpdateAllShownItemsPos();
					return;
				}

				if (-loopListViewItem8.LeftX > contentPanelSize4 ||
				    loopListViewItem8.ItemIndex == mCurReadyMaxItemIndex &&
				    -loopListViewItem8.LeftX != contentPanelSize4)
				{
					UpdateAllShownItemsPos();
				}
			}
		}


		private void UpdateAllShownItemsPos()
		{
			int count = ItemList.Count;
			if (count == 0)
			{
				return;
			}

			mAdjustedVec = (mContainerTrans.anchoredPosition3D - mLastFrameContainerPos) / Time.deltaTime;
			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				float num = 0f;
				if (mSupportScrollBar)
				{
					num = -GetItemPos(ItemList[0].ItemIndex);
				}

				float y = ItemList[0].CachedRectTransform.anchoredPosition3D.y;
				float num2 = num - y;
				float num3 = num;
				for (int i = 0; i < count; i++)
				{
					LoopListViewItem2 loopListViewItem = ItemList[i];
					loopListViewItem.CachedRectTransform.anchoredPosition3D =
						new Vector3(loopListViewItem.StartPosOffset, num3, 0f);
					num3 = num3 - loopListViewItem.CachedRectTransform.rect.height - loopListViewItem.Padding;
				}

				if (num2 != 0f)
				{
					Vector2 vector = mContainerTrans.anchoredPosition3D;
					vector.y -= num2;
					mContainerTrans.anchoredPosition3D = vector;
				}
			}
			else if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				float num4 = 0f;
				if (mSupportScrollBar)
				{
					num4 = GetItemPos(ItemList[0].ItemIndex);
				}

				float y2 = ItemList[0].CachedRectTransform.anchoredPosition3D.y;
				float num5 = num4 - y2;
				float num6 = num4;
				for (int j = 0; j < count; j++)
				{
					LoopListViewItem2 loopListViewItem2 = ItemList[j];
					loopListViewItem2.CachedRectTransform.anchoredPosition3D =
						new Vector3(loopListViewItem2.StartPosOffset, num6, 0f);
					num6 = num6 + loopListViewItem2.CachedRectTransform.rect.height + loopListViewItem2.Padding;
				}

				if (num5 != 0f)
				{
					Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D.y -= num5;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D;
				}
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				float num7 = 0f;
				if (mSupportScrollBar)
				{
					num7 = GetItemPos(ItemList[0].ItemIndex);
				}

				float x = ItemList[0].CachedRectTransform.anchoredPosition3D.x;
				float num8 = num7 - x;
				float num9 = num7;
				for (int k = 0; k < count; k++)
				{
					LoopListViewItem2 loopListViewItem3 = ItemList[k];
					loopListViewItem3.CachedRectTransform.anchoredPosition3D =
						new Vector3(num9, loopListViewItem3.StartPosOffset, 0f);
					num9 = num9 + loopListViewItem3.CachedRectTransform.rect.width + loopListViewItem3.Padding;
				}

				if (num8 != 0f)
				{
					Vector3 anchoredPosition3D2 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D2.x -= num8;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D2;
				}
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				float num10 = 0f;
				if (mSupportScrollBar)
				{
					num10 = -GetItemPos(ItemList[0].ItemIndex);
				}

				float x2 = ItemList[0].CachedRectTransform.anchoredPosition3D.x;
				float num11 = num10 - x2;
				float num12 = num10;
				for (int l = 0; l < count; l++)
				{
					LoopListViewItem2 loopListViewItem4 = ItemList[l];
					loopListViewItem4.CachedRectTransform.anchoredPosition3D =
						new Vector3(num12, loopListViewItem4.StartPosOffset, 0f);
					num12 = num12 - loopListViewItem4.CachedRectTransform.rect.width - loopListViewItem4.Padding;
				}

				if (num11 != 0f)
				{
					Vector3 anchoredPosition3D3 = mContainerTrans.anchoredPosition3D;
					anchoredPosition3D3.x -= num11;
					mContainerTrans.anchoredPosition3D = anchoredPosition3D3;
				}
			}

			if (mIsDraging)
			{
				mScrollRect.OnBeginDrag(mPointerEventData);
				mScrollRect.Rebuild(CanvasUpdate.PostLayout);
				mScrollRect.velocity = mAdjustedVec;
				mNeedAdjustVec = true;
			}
		}


		private void UpdateContentSize()
		{
			float contentPanelSize = GetContentPanelSize();
			if (mIsVertList)
			{
				if (mContainerTrans.rect.height != contentPanelSize)
				{
					mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentPanelSize);
				}
			}
			else if (mContainerTrans.rect.width != contentPanelSize)
			{
				mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentPanelSize);
			}
		}


		private class SnapData
		{
			public float mCurSnapVal;


			public bool mIsForceSnapTo;


			public bool mIsTempTarget;


			public float mMoveMaxAbsVec = -1f;


			public SnapStatus mSnapStatus;


			public int mSnapTargetIndex;


			public float mTargetSnapVal;


			public int mTempTargetIndex = -1;


			public void Clear()
			{
				mSnapStatus = SnapStatus.NoTargetSet;
				mTempTargetIndex = -1;
				mIsForceSnapTo = false;
				mMoveMaxAbsVec = -1f;
			}
		}
	}
}