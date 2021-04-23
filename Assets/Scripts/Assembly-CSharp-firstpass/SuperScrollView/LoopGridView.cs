using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class LoopGridView : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler
	{
		[SerializeField]
		private List<GridViewItemPrefabConfData> mItemPrefabDataList = new List<GridViewItemPrefabConfData>();


		[SerializeField] private GridItemArrangeType mArrangeType;


		[SerializeField] private int mFixedRowOrColumnCount;


		[SerializeField] private RectOffset mPadding = new RectOffset();


		[SerializeField] private Vector2 mItemPadding = Vector2.zero;


		[SerializeField] private Vector2 mItemSize = Vector2.zero;


		[SerializeField] private Vector2 mItemRecycleDistance = new Vector2(50f, 50f);


		[SerializeField] private bool mItemSnapEnable;


		[SerializeField] private GridFixedType mGridFixedType;


		[SerializeField] private Vector2 mViewPortSnapPivot = Vector2.zero;


		[SerializeField] private Vector2 mItemSnapPivot = Vector2.zero;


		private readonly ItemRangeData mCurFrameItemRangeData = new ItemRangeData();


		private readonly SnapData mCurSnapData = new SnapData();


		private readonly List<GridItemGroup> mItemGroupList = new List<GridItemGroup>();


		private readonly List<GridItemGroup> mItemGroupObjPool = new List<GridItemGroup>();


		private readonly Dictionary<string, GridItemPool> mItemPoolDict = new Dictionary<string, GridItemPool>();


		private readonly List<GridItemPool> mItemPoolList = new List<GridItemPool>();


		private int mColumnCount;


		private RectTransform mContainerTrans;


		private RowColumnPair mCurSnapNearestItemRowColumn;


		private Vector2 mEndPadding;


		private bool mIsDraging;


		private Vector2 mItemSizeWithPadding = Vector2.zero;


		private int mItemTotalCount;


		private Vector3 mLastSnapCheckPos = Vector3.zero;


		private int mLeftSnapUpdateExtraCount = 1;


		private int mListUpdateCheckFrameCount;


		private bool mListViewInited;


		private int mNeedCheckContentPosLeftCount = 1;


		public Action<PointerEventData> mOnBeginDragAction;


		public Action<PointerEventData> mOnDragingAction;


		public Action<PointerEventData> mOnEndDragAction;


		private Func<LoopGridView, int, int, int, LoopGridViewItem> mOnGetItemByRowColumn;


		public Action<LoopGridView, LoopGridViewItem> mOnSnapItemFinished;


		public Action<LoopGridView> mOnSnapNearestChanged;


		private int mRowCount;


		private ClickEventListener mScrollBarClickEventListener1;


		private ClickEventListener mScrollBarClickEventListener2;


		private ScrollRect mScrollRect;


		private RectTransform mScrollRectTransform;


		private float mSmoothDumpRate = 0.3f;


		private float mSmoothDumpVel;


		private float mSnapFinishThreshold = 0.1f;


		private float mSnapVecThreshold = 145f;


		private Vector2 mStartPadding;


		private RectTransform mViewPortRectTransform;


		
		public GridItemArrangeType ArrangeType {
			get => mArrangeType;
			set => mArrangeType = value;
		}


		public List<GridViewItemPrefabConfData> ItemPrefabDataList => mItemPrefabDataList;


		public int ItemTotalCount => mItemTotalCount;


		public RectTransform ContainerTrans => mContainerTrans;


		public float ViewPortWidth => mViewPortRectTransform.rect.width;


		public float ViewPortHeight => mViewPortRectTransform.rect.height;


		public ScrollRect ScrollRect => mScrollRect;


		public bool IsDraging => mIsDraging;


		
		public bool ItemSnapEnable {
			get => mItemSnapEnable;
			set => mItemSnapEnable = value;
		}


		
		public Vector2 ItemSize {
			get => mItemSize;
			set => SetItemSize(value);
		}


		
		public Vector2 ItemPadding {
			get => mItemPadding;
			set => SetItemPadding(value);
		}


		public Vector2 ItemSizeWithPadding => mItemSizeWithPadding;


		
		public RectOffset Padding {
			get => mPadding;
			set => SetPadding(value);
		}


		public RowColumnPair CurSnapNearestItemRowColumn => mCurSnapNearestItemRowColumn;


		private void Update()
		{
			if (!mListViewInited)
			{
				return;
			}

			UpdateSnapMove();
			UpdateGridViewContent();
			ClearAllTmpRecycledItem();
		}


		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			mCurSnapData.Clear();
			mIsDraging = true;
			if (mOnBeginDragAction != null)
			{
				mOnBeginDragAction(eventData);
			}
		}


		public virtual void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if (mOnDragingAction != null)
			{
				mOnDragingAction(eventData);
			}
		}


		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			mIsDraging = false;
			ForceSnapUpdateCheck();
			if (mOnEndDragAction != null)
			{
				mOnEndDragAction(eventData);
			}
		}


		public GridViewItemPrefabConfData GetItemPrefabConfData(string prefabName)
		{
			foreach (GridViewItemPrefabConfData gridViewItemPrefabConfData in mItemPrefabDataList)
			{
				if (gridViewItemPrefabConfData.mItemPrefab == null)
				{
					Debug.LogError("A item prefab is null ");
				}
				else if (prefabName == gridViewItemPrefabConfData.mItemPrefab.name)
				{
					return gridViewItemPrefabConfData;
				}
			}

			return null;
		}


		public void InitGridView(int itemTotalCount,
			Func<LoopGridView, int, int, int, LoopGridViewItem> onGetItemByRowColumn,
			LoopGridViewSettingParam settingParam = null, LoopGridViewInitParam initParam = null)
		{
			if (mListViewInited)
			{
				Debug.LogError("LoopGridView.InitListView method can be called only once.");
				return;
			}

			mListViewInited = true;
			if (itemTotalCount < 0)
			{
				Debug.LogError("itemTotalCount is  < 0");
				itemTotalCount = 0;
			}

			if (settingParam != null)
			{
				UpdateFromSettingParam(settingParam);
			}

			if (initParam != null)
			{
				mSmoothDumpRate = initParam.mSmoothDumpRate;
				mSnapFinishThreshold = initParam.mSnapFinishThreshold;
				mSnapVecThreshold = initParam.mSnapVecThreshold;
			}

			mScrollRect = gameObject.GetComponent<ScrollRect>();
			if (mScrollRect == null)
			{
				Debug.LogError("ListView Init Failed! ScrollRect component not found!");
				return;
			}

			mCurSnapData.Clear();
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

			SetScrollbarListener();
			AdjustViewPortPivot();
			AdjustContainerAnchorAndPivot();
			InitItemPool();
			mOnGetItemByRowColumn = onGetItemByRowColumn;
			mNeedCheckContentPosLeftCount = 4;
			mCurSnapData.Clear();
			mItemTotalCount = itemTotalCount;
			UpdateAllGridSetting();
		}


		public void SetListItemCount(int itemCount, bool resetPos = true)
		{
			if (itemCount < 0)
			{
				return;
			}

			if (itemCount == mItemTotalCount)
			{
				return;
			}

			mCurSnapData.Clear();
			mItemTotalCount = itemCount;
			UpdateColumnRowCount();
			UpdateContentSize();
			ForceToCheckContentPos();
			if (mItemTotalCount == 0)
			{
				RecycleAllItem();
				ClearAllTmpRecycledItem();
				return;
			}

			VaildAndSetContainerPos();
			UpdateGridViewContent();
			ClearAllTmpRecycledItem();
			if (resetPos)
			{
				MovePanelToItemByRowColumn(0, 0);
			}
		}


		public LoopGridViewItem NewListViewItem(string itemPrefabName)
		{
			GridItemPool gridItemPool = null;
			if (!mItemPoolDict.TryGetValue(itemPrefabName, out gridItemPool))
			{
				return null;
			}

			LoopGridViewItem item = gridItemPool.GetItem();
			RectTransform component = item.GetComponent<RectTransform>();
			component.SetParent(mContainerTrans);
			component.localScale = Vector3.one;
			component.anchoredPosition3D = Vector3.zero;
			component.localEulerAngles = Vector3.zero;
			item.ParentGridView = this;
			return item;
		}


		public void RefreshItemByItemIndex(int itemIndex)
		{
			if (itemIndex < 0 || itemIndex >= ItemTotalCount)
			{
				return;
			}

			if (mItemGroupList.Count == 0)
			{
				return;
			}

			RowColumnPair rowColumnByItemIndex = GetRowColumnByItemIndex(itemIndex);
			RefreshItemByRowColumn(rowColumnByItemIndex.mRow, rowColumnByItemIndex.mColumn);
		}


		public void RefreshItemByRowColumn(int row, int column)
		{
			if (mItemGroupList.Count == 0)
			{
				return;
			}

			if (mGridFixedType == GridFixedType.ColumnCountFixed)
			{
				GridItemGroup shownGroup = GetShownGroup(row);
				if (shownGroup == null)
				{
					return;
				}

				LoopGridViewItem itemByColumn = shownGroup.GetItemByColumn(column);
				if (itemByColumn == null)
				{
					return;
				}

				LoopGridViewItem newItemByRowColumn = GetNewItemByRowColumn(row, column);
				if (newItemByRowColumn == null)
				{
					return;
				}

				Vector3 anchoredPosition3D = itemByColumn.CachedRectTransform.anchoredPosition3D;
				shownGroup.ReplaceItem(itemByColumn, newItemByRowColumn);
				RecycleItemTmp(itemByColumn);
				newItemByRowColumn.CachedRectTransform.anchoredPosition3D = anchoredPosition3D;
				ClearAllTmpRecycledItem();
			}
			else
			{
				GridItemGroup shownGroup2 = GetShownGroup(column);
				if (shownGroup2 == null)
				{
					return;
				}

				LoopGridViewItem itemByRow = shownGroup2.GetItemByRow(row);
				if (itemByRow == null)
				{
					return;
				}

				LoopGridViewItem newItemByRowColumn2 = GetNewItemByRowColumn(row, column);
				if (newItemByRowColumn2 == null)
				{
					return;
				}

				Vector3 anchoredPosition3D2 = itemByRow.CachedRectTransform.anchoredPosition3D;
				shownGroup2.ReplaceItem(itemByRow, newItemByRowColumn2);
				RecycleItemTmp(itemByRow);
				newItemByRowColumn2.CachedRectTransform.anchoredPosition3D = anchoredPosition3D2;
				ClearAllTmpRecycledItem();
			}
		}


		public void ClearSnapData()
		{
			mCurSnapData.Clear();
		}


		public void SetSnapTargetItemRowColumn(int row, int column)
		{
			if (row < 0)
			{
				row = 0;
			}

			if (column < 0)
			{
				column = 0;
			}

			mCurSnapData.mSnapTarget.mRow = row;
			mCurSnapData.mSnapTarget.mColumn = column;
			mCurSnapData.mSnapStatus = SnapStatus.TargetHasSet;
			mCurSnapData.mIsForceSnapTo = true;
		}


		public void ForceSnapUpdateCheck()
		{
			if (mLeftSnapUpdateExtraCount <= 0)
			{
				mLeftSnapUpdateExtraCount = 1;
			}
		}


		public void ForceToCheckContentPos()
		{
			if (mNeedCheckContentPosLeftCount <= 0)
			{
				mNeedCheckContentPosLeftCount = 1;
			}
		}


		public void MovePanelToItemByIndex(int itemIndex, float offsetX = 0f, float offsetY = 0f)
		{
			if (ItemTotalCount == 0)
			{
				return;
			}

			if (itemIndex >= ItemTotalCount)
			{
				itemIndex = ItemTotalCount - 1;
			}

			if (itemIndex < 0)
			{
				itemIndex = 0;
			}

			RowColumnPair rowColumnByItemIndex = GetRowColumnByItemIndex(itemIndex);
			MovePanelToItemByRowColumn(rowColumnByItemIndex.mRow, rowColumnByItemIndex.mColumn, offsetX, offsetY);
		}


		public void MovePanelToItemByRowColumn(int row, int column, float offsetX = 0f, float offsetY = 0f)
		{
			mScrollRect.StopMovement();
			mCurSnapData.Clear();
			if (mItemTotalCount == 0)
			{
				return;
			}

			Vector2 itemPos = GetItemPos(row, column);
			Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
			if (mScrollRect.horizontal)
			{
				float num = Mathf.Max(ContainerTrans.rect.width - ViewPortWidth, 0f);
				if (num > 0f)
				{
					float num2 = -itemPos.x + offsetX;
					num2 = Mathf.Min(Mathf.Abs(num2), num) * Mathf.Sign(num2);
					anchoredPosition3D.x = num2;
				}
			}

			if (mScrollRect.vertical)
			{
				float num3 = Mathf.Max(ContainerTrans.rect.height - ViewPortHeight, 0f);
				if (num3 > 0f)
				{
					float num4 = -itemPos.y + offsetY;
					num4 = Mathf.Min(Mathf.Abs(num4), num3) * Mathf.Sign(num4);
					anchoredPosition3D.y = num4;
				}
			}

			if (anchoredPosition3D != mContainerTrans.anchoredPosition3D)
			{
				mContainerTrans.anchoredPosition3D = anchoredPosition3D;
			}

			VaildAndSetContainerPos();
			ForceToCheckContentPos();
		}


		public void RefreshAllShownItem()
		{
			if (mItemGroupList.Count == 0)
			{
				return;
			}

			ForceToCheckContentPos();
			RecycleAllItem();
			UpdateGridViewContent();
		}


		public int GetItemIndexByRowColumn(int row, int column)
		{
			if (mGridFixedType == GridFixedType.ColumnCountFixed)
			{
				return row * mFixedRowOrColumnCount + column;
			}

			return column * mFixedRowOrColumnCount + row;
		}


		public RowColumnPair GetRowColumnByItemIndex(int itemIndex)
		{
			if (itemIndex < 0)
			{
				itemIndex = 0;
			}

			if (mGridFixedType == GridFixedType.ColumnCountFixed)
			{
				int row = itemIndex / mFixedRowOrColumnCount;
				int column = itemIndex % mFixedRowOrColumnCount;
				return new RowColumnPair(row, column);
			}

			int column2 = itemIndex / mFixedRowOrColumnCount;
			return new RowColumnPair(itemIndex % mFixedRowOrColumnCount, column2);
		}


		public Vector2 GetItemAbsPos(int row, int column)
		{
			float x = mStartPadding.x + column * mItemSizeWithPadding.x;
			float y = mStartPadding.y + row * mItemSizeWithPadding.y;
			return new Vector2(x, y);
		}


		public Vector2 GetItemPos(int row, int column)
		{
			Vector2 itemAbsPos = GetItemAbsPos(row, column);
			float x = itemAbsPos.x;
			float y = itemAbsPos.y;
			if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
			{
				return new Vector2(x, -y);
			}

			if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
			{
				return new Vector2(x, y);
			}

			if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
			{
				return new Vector2(-x, -y);
			}

			if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
			{
				return new Vector2(-x, y);
			}

			return Vector2.zero;
		}


		public LoopGridViewItem GetShownItemByItemIndex(int itemIndex)
		{
			if (itemIndex < 0 || itemIndex >= ItemTotalCount)
			{
				return null;
			}

			if (mItemGroupList.Count == 0)
			{
				return null;
			}

			RowColumnPair rowColumnByItemIndex = GetRowColumnByItemIndex(itemIndex);
			return GetShownItemByRowColumn(rowColumnByItemIndex.mRow, rowColumnByItemIndex.mColumn);
		}


		public LoopGridViewItem GetShownItemByRowColumn(int row, int column)
		{
			if (mItemGroupList.Count == 0)
			{
				return null;
			}

			if (mGridFixedType == GridFixedType.ColumnCountFixed)
			{
				GridItemGroup shownGroup = GetShownGroup(row);
				if (shownGroup == null)
				{
					return null;
				}

				return shownGroup.GetItemByColumn(column);
			}

			GridItemGroup shownGroup2 = GetShownGroup(column);
			if (shownGroup2 == null)
			{
				return null;
			}

			return shownGroup2.GetItemByRow(row);
		}


		public void UpdateAllGridSetting()
		{
			UpdateStartEndPadding();
			UpdateItemSize();
			UpdateColumnRowCount();
			UpdateContentSize();
			ForceSnapUpdateCheck();
			ForceToCheckContentPos();
		}


		public void SetGridFixedGroupCount(GridFixedType fixedType, int count)
		{
			if (mGridFixedType == fixedType && mFixedRowOrColumnCount == count)
			{
				return;
			}

			mGridFixedType = fixedType;
			mFixedRowOrColumnCount = count;
			UpdateColumnRowCount();
			UpdateContentSize();
			if (mItemGroupList.Count == 0)
			{
				return;
			}

			RecycleAllItem();
			ForceSnapUpdateCheck();
			ForceToCheckContentPos();
		}


		public void SetItemSize(Vector2 newSize)
		{
			if (newSize == mItemSize)
			{
				return;
			}

			mItemSize = newSize;
			UpdateItemSize();
			UpdateContentSize();
			if (mItemGroupList.Count == 0)
			{
				return;
			}

			RecycleAllItem();
			ForceSnapUpdateCheck();
			ForceToCheckContentPos();
		}


		public void SetItemPadding(Vector2 newPadding)
		{
			if (newPadding == mItemPadding)
			{
				return;
			}

			mItemPadding = newPadding;
			UpdateItemSize();
			UpdateContentSize();
			if (mItemGroupList.Count == 0)
			{
				return;
			}

			RecycleAllItem();
			ForceSnapUpdateCheck();
			ForceToCheckContentPos();
		}


		public void SetPadding(RectOffset newPadding)
		{
			if (newPadding == mPadding)
			{
				return;
			}

			mPadding = newPadding;
			UpdateStartEndPadding();
			UpdateContentSize();
			if (mItemGroupList.Count == 0)
			{
				return;
			}

			RecycleAllItem();
			ForceSnapUpdateCheck();
			ForceToCheckContentPos();
		}


		public void UpdateContentSize()
		{
			float num = mStartPadding.x + mColumnCount * mItemSizeWithPadding.x - mItemPadding.x + mEndPadding.x;
			float num2 = mStartPadding.y + mRowCount * mItemSizeWithPadding.y - mItemPadding.y + mEndPadding.y;
			if (mContainerTrans.rect.height != num2)
			{
				mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num2);
			}

			if (mContainerTrans.rect.width != num)
			{
				mContainerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num);
			}
		}


		public void VaildAndSetContainerPos()
		{
			Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
			mContainerTrans.anchoredPosition3D = GetContainerVaildPos(anchoredPosition3D.x, anchoredPosition3D.y);
		}


		public void ClearAllTmpRecycledItem()
		{
			int count = mItemPoolList.Count;
			for (int i = 0; i < count; i++)
			{
				mItemPoolList[i].ClearTmpRecycledItem();
			}
		}


		public void RecycleAllItem()
		{
			foreach (GridItemGroup group in mItemGroupList)
			{
				RecycleItemGroupTmp(group);
			}

			mItemGroupList.Clear();
		}


		public void UpdateGridViewContent()
		{
			mListUpdateCheckFrameCount++;
			if (mItemTotalCount == 0)
			{
				if (mItemGroupList.Count > 0)
				{
					RecycleAllItem();
				}

				return;
			}

			UpdateCurFrameItemRangeData();
			if (mGridFixedType == GridFixedType.ColumnCountFixed)
			{
				int count = mItemGroupList.Count;
				int mMinRow = mCurFrameItemRangeData.mMinRow;
				int mMaxRow = mCurFrameItemRangeData.mMaxRow;
				for (int i = count - 1; i >= 0; i--)
				{
					GridItemGroup gridItemGroup = mItemGroupList[i];
					if (gridItemGroup.GroupIndex < mMinRow || gridItemGroup.GroupIndex > mMaxRow)
					{
						RecycleItemGroupTmp(gridItemGroup);
						mItemGroupList.RemoveAt(i);
					}
				}

				if (mItemGroupList.Count == 0)
				{
					GridItemGroup item = CreateItemGroup(mMinRow);
					mItemGroupList.Add(item);
				}

				while (mItemGroupList[0].GroupIndex > mMinRow)
				{
					GridItemGroup item2 = CreateItemGroup(mItemGroupList[0].GroupIndex - 1);
					mItemGroupList.Insert(0, item2);
				}

				while (mItemGroupList[mItemGroupList.Count - 1].GroupIndex < mMaxRow)
				{
					GridItemGroup item3 = CreateItemGroup(mItemGroupList[mItemGroupList.Count - 1].GroupIndex + 1);
					mItemGroupList.Add(item3);
				}

				int count2 = mItemGroupList.Count;
				for (int j = 0; j < count2; j++)
				{
					UpdateRowItemGroupForRecycleAndNew(mItemGroupList[j]);
				}

				return;
			}

			int count3 = mItemGroupList.Count;
			int mMinColumn = mCurFrameItemRangeData.mMinColumn;
			int mMaxColumn = mCurFrameItemRangeData.mMaxColumn;
			for (int k = count3 - 1; k >= 0; k--)
			{
				GridItemGroup gridItemGroup2 = mItemGroupList[k];
				if (gridItemGroup2.GroupIndex < mMinColumn || gridItemGroup2.GroupIndex > mMaxColumn)
				{
					RecycleItemGroupTmp(gridItemGroup2);
					mItemGroupList.RemoveAt(k);
				}
			}

			if (mItemGroupList.Count == 0)
			{
				GridItemGroup item4 = CreateItemGroup(mMinColumn);
				mItemGroupList.Add(item4);
			}

			while (mItemGroupList[0].GroupIndex > mMinColumn)
			{
				GridItemGroup item5 = CreateItemGroup(mItemGroupList[0].GroupIndex - 1);
				mItemGroupList.Insert(0, item5);
			}

			while (mItemGroupList[mItemGroupList.Count - 1].GroupIndex < mMaxColumn)
			{
				GridItemGroup item6 = CreateItemGroup(mItemGroupList[mItemGroupList.Count - 1].GroupIndex + 1);
				mItemGroupList.Add(item6);
			}

			int count4 = mItemGroupList.Count;
			for (int l = 0; l < count4; l++)
			{
				UpdateColumnItemGroupForRecycleAndNew(mItemGroupList[l]);
			}
		}


		public void UpdateStartEndPadding()
		{
			if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
			{
				mStartPadding.x = mPadding.left;
				mStartPadding.y = mPadding.top;
				mEndPadding.x = mPadding.right;
				mEndPadding.y = mPadding.bottom;
				return;
			}

			if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
			{
				mStartPadding.x = mPadding.left;
				mStartPadding.y = mPadding.bottom;
				mEndPadding.x = mPadding.right;
				mEndPadding.y = mPadding.top;
				return;
			}

			if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
			{
				mStartPadding.x = mPadding.right;
				mStartPadding.y = mPadding.top;
				mEndPadding.x = mPadding.left;
				mEndPadding.y = mPadding.bottom;
				return;
			}

			if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
			{
				mStartPadding.x = mPadding.right;
				mStartPadding.y = mPadding.bottom;
				mEndPadding.x = mPadding.left;
				mEndPadding.y = mPadding.top;
			}
		}


		public void UpdateItemSize()
		{
			if (mItemSize.x > 0f && mItemSize.y > 0f)
			{
				mItemSizeWithPadding = mItemSize + mItemPadding;
				return;
			}

			if (mItemPrefabDataList.Count != 0)
			{
				GameObject mItemPrefab = mItemPrefabDataList[0].mItemPrefab;
				if (!(mItemPrefab == null))
				{
					RectTransform component = mItemPrefab.GetComponent<RectTransform>();
					if (!(component == null))
					{
						mItemSize = component.rect.size;
						mItemSizeWithPadding = mItemSize + mItemPadding;
					}
				}
			}

			if (mItemSize.x <= 0f || mItemSize.y <= 0f)
			{
				Debug.LogError("Error, ItemSize is invaild.");
			}
		}


		public void UpdateColumnRowCount()
		{
			if (mGridFixedType == GridFixedType.ColumnCountFixed)
			{
				mColumnCount = mFixedRowOrColumnCount;
				mRowCount = mItemTotalCount / mColumnCount;
				if (mItemTotalCount % mColumnCount > 0)
				{
					mRowCount++;
				}

				if (mItemTotalCount <= mColumnCount)
				{
					mColumnCount = mItemTotalCount;
				}
			}
			else
			{
				mRowCount = mFixedRowOrColumnCount;
				mColumnCount = mItemTotalCount / mRowCount;
				if (mItemTotalCount % mRowCount > 0)
				{
					mColumnCount++;
				}

				if (mItemTotalCount <= mRowCount)
				{
					mRowCount = mItemTotalCount;
				}
			}
		}


		private bool IsContainerTransCanMove()
		{
			return mItemTotalCount != 0 && (mScrollRect.horizontal && ContainerTrans.rect.width > ViewPortWidth ||
			                                mScrollRect.vertical && ContainerTrans.rect.height > ViewPortHeight);
		}


		private void RecycleItemGroupTmp(GridItemGroup group)
		{
			if (group == null)
			{
				return;
			}

			while (group.First != null)
			{
				LoopGridViewItem item = group.RemoveFirst();
				RecycleItemTmp(item);
			}

			group.Clear();
			RecycleOneItemGroupObj(group);
		}


		private void RecycleItemTmp(LoopGridViewItem item)
		{
			if (item == null)
			{
				return;
			}

			if (string.IsNullOrEmpty(item.ItemPrefabName))
			{
				return;
			}

			GridItemPool gridItemPool = null;
			if (!mItemPoolDict.TryGetValue(item.ItemPrefabName, out gridItemPool))
			{
				return;
			}

			gridItemPool.RecycleItem(item);
		}


		private void AdjustViewPortPivot()
		{
			RectTransform rectTransform = mViewPortRectTransform;
			if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
			{
				rectTransform.pivot = new Vector2(0f, 1f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
			{
				rectTransform.pivot = new Vector2(0f, 0f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
			{
				rectTransform.pivot = new Vector2(1f, 1f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
			{
				rectTransform.pivot = new Vector2(1f, 0f);
			}
		}


		private void AdjustContainerAnchorAndPivot()
		{
			RectTransform containerTrans = ContainerTrans;
			if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
			{
				containerTrans.anchorMin = new Vector2(0f, 1f);
				containerTrans.anchorMax = new Vector2(0f, 1f);
				containerTrans.pivot = new Vector2(0f, 1f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
			{
				containerTrans.anchorMin = new Vector2(0f, 0f);
				containerTrans.anchorMax = new Vector2(0f, 0f);
				containerTrans.pivot = new Vector2(0f, 0f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
			{
				containerTrans.anchorMin = new Vector2(1f, 1f);
				containerTrans.anchorMax = new Vector2(1f, 1f);
				containerTrans.pivot = new Vector2(1f, 1f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
			{
				containerTrans.anchorMin = new Vector2(1f, 0f);
				containerTrans.anchorMax = new Vector2(1f, 0f);
				containerTrans.pivot = new Vector2(1f, 0f);
			}
		}


		private void AdjustItemAnchorAndPivot(RectTransform rtf)
		{
			if (ArrangeType == GridItemArrangeType.TopLeftToBottomRight)
			{
				rtf.anchorMin = new Vector2(0f, 1f);
				rtf.anchorMax = new Vector2(0f, 1f);
				rtf.pivot = new Vector2(0f, 1f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.BottomLeftToTopRight)
			{
				rtf.anchorMin = new Vector2(0f, 0f);
				rtf.anchorMax = new Vector2(0f, 0f);
				rtf.pivot = new Vector2(0f, 0f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.TopRightToBottomLeft)
			{
				rtf.anchorMin = new Vector2(1f, 1f);
				rtf.anchorMax = new Vector2(1f, 1f);
				rtf.pivot = new Vector2(1f, 1f);
				return;
			}

			if (ArrangeType == GridItemArrangeType.BottomRightToTopLeft)
			{
				rtf.anchorMin = new Vector2(1f, 0f);
				rtf.anchorMax = new Vector2(1f, 0f);
				rtf.pivot = new Vector2(1f, 0f);
			}
		}


		private void InitItemPool()
		{
			foreach (GridViewItemPrefabConfData gridViewItemPrefabConfData in mItemPrefabDataList)
			{
				if (gridViewItemPrefabConfData.mItemPrefab == null)
				{
					Debug.LogError("A item prefab is null ");
				}
				else
				{
					string name = gridViewItemPrefabConfData.mItemPrefab.name;
					if (mItemPoolDict.ContainsKey(name))
					{
						Debug.LogError("A item prefab with name " + name + " has existed!");
					}
					else
					{
						RectTransform component = gridViewItemPrefabConfData.mItemPrefab.GetComponent<RectTransform>();
						if (component == null)
						{
							Debug.LogError("RectTransform component is not found in the prefab " + name);
						}
						else
						{
							AdjustItemAnchorAndPivot(component);
							if (gridViewItemPrefabConfData.mItemPrefab.GetComponent<LoopGridViewItem>() == null)
							{
								gridViewItemPrefabConfData.mItemPrefab.AddComponent<LoopGridViewItem>();
							}

							GridItemPool gridItemPool = new GridItemPool();
							gridItemPool.Init(gridViewItemPrefabConfData.mItemPrefab,
								gridViewItemPrefabConfData.mInitCreateCount, mContainerTrans);
							mItemPoolDict.Add(name, gridItemPool);
							mItemPoolList.Add(gridItemPool);
						}
					}
				}
			}
		}


		private LoopGridViewItem GetNewItemByRowColumn(int row, int column)
		{
			int itemIndexByRowColumn = GetItemIndexByRowColumn(row, column);
			if (itemIndexByRowColumn < 0 || itemIndexByRowColumn >= ItemTotalCount)
			{
				return null;
			}

			LoopGridViewItem loopGridViewItem = mOnGetItemByRowColumn(this, itemIndexByRowColumn, row, column);
			if (loopGridViewItem == null)
			{
				return null;
			}

			loopGridViewItem.NextItem = null;
			loopGridViewItem.PrevItem = null;
			loopGridViewItem.Row = row;
			loopGridViewItem.Column = column;
			loopGridViewItem.ItemIndex = itemIndexByRowColumn;
			loopGridViewItem.ItemCreatedCheckFrameCount = mListUpdateCheckFrameCount;
			return loopGridViewItem;
		}


		private RowColumnPair GetCeilItemRowColumnAtGivenAbsPos(float ax, float ay)
		{
			ax = Mathf.Abs(ax);
			ay = Mathf.Abs(ay);
			int num = Mathf.CeilToInt((ay - mStartPadding.y) / mItemSizeWithPadding.y) - 1;
			int num2 = Mathf.CeilToInt((ax - mStartPadding.x) / mItemSizeWithPadding.x) - 1;
			if (num < 0)
			{
				num = 0;
			}

			if (num >= mRowCount)
			{
				num = mRowCount - 1;
			}

			if (num2 < 0)
			{
				num2 = 0;
			}

			if (num2 >= mColumnCount)
			{
				num2 = mColumnCount - 1;
			}

			return new RowColumnPair(num, num2);
		}


		private GridItemGroup CreateItemGroup(int groupIndex)
		{
			GridItemGroup oneItemGroupObj = GetOneItemGroupObj();
			oneItemGroupObj.GroupIndex = groupIndex;
			return oneItemGroupObj;
		}


		private Vector2 GetContainerMovedDistance()
		{
			Vector2 containerVaildPos =
				GetContainerVaildPos(ContainerTrans.anchoredPosition3D.x, ContainerTrans.anchoredPosition3D.y);
			return new Vector2(Mathf.Abs(containerVaildPos.x), Mathf.Abs(containerVaildPos.y));
		}


		private Vector2 GetContainerVaildPos(float curX, float curY)
		{
			float num = Mathf.Max(ContainerTrans.rect.width - ViewPortWidth, 0f);
			float num2 = Mathf.Max(ContainerTrans.rect.height - ViewPortHeight, 0f);
			if (mArrangeType == GridItemArrangeType.TopLeftToBottomRight)
			{
				curX = Mathf.Clamp(curX, -num, 0f);
				curY = Mathf.Clamp(curY, 0f, num2);
			}
			else if (mArrangeType == GridItemArrangeType.BottomLeftToTopRight)
			{
				curX = Mathf.Clamp(curX, -num, 0f);
				curY = Mathf.Clamp(curY, -num2, 0f);
			}
			else if (mArrangeType == GridItemArrangeType.BottomRightToTopLeft)
			{
				curX = Mathf.Clamp(curX, 0f, num);
				curY = Mathf.Clamp(curY, -num2, 0f);
			}
			else if (mArrangeType == GridItemArrangeType.TopRightToBottomLeft)
			{
				curX = Mathf.Clamp(curX, 0f, num);
				curY = Mathf.Clamp(curY, 0f, num2);
			}

			return new Vector2(curX, curY);
		}


		private void UpdateCurFrameItemRangeData()
		{
			Vector2 containerMovedDistance = GetContainerMovedDistance();
			if (mNeedCheckContentPosLeftCount <= 0 && mCurFrameItemRangeData.mCheckedPosition == containerMovedDistance)
			{
				return;
			}

			if (mNeedCheckContentPosLeftCount > 0)
			{
				mNeedCheckContentPosLeftCount--;
			}

			float num = containerMovedDistance.x - mItemRecycleDistance.x;
			float num2 = containerMovedDistance.y - mItemRecycleDistance.y;
			if (num < 0f)
			{
				num = 0f;
			}

			if (num2 < 0f)
			{
				num2 = 0f;
			}

			RowColumnPair ceilItemRowColumnAtGivenAbsPos = GetCeilItemRowColumnAtGivenAbsPos(num, num2);
			mCurFrameItemRangeData.mMinColumn = ceilItemRowColumnAtGivenAbsPos.mColumn;
			mCurFrameItemRangeData.mMinRow = ceilItemRowColumnAtGivenAbsPos.mRow;
			num = containerMovedDistance.x + mItemRecycleDistance.x + ViewPortWidth;
			num2 = containerMovedDistance.y + mItemRecycleDistance.y + ViewPortHeight;
			ceilItemRowColumnAtGivenAbsPos = GetCeilItemRowColumnAtGivenAbsPos(num, num2);
			mCurFrameItemRangeData.mMaxColumn = ceilItemRowColumnAtGivenAbsPos.mColumn;
			mCurFrameItemRangeData.mMaxRow = ceilItemRowColumnAtGivenAbsPos.mRow;
			mCurFrameItemRangeData.mCheckedPosition = containerMovedDistance;
		}


		private void UpdateRowItemGroupForRecycleAndNew(GridItemGroup group)
		{
			int mMinColumn = mCurFrameItemRangeData.mMinColumn;
			int mMaxColumn = mCurFrameItemRangeData.mMaxColumn;
			int groupIndex = group.GroupIndex;
			while (group.First != null)
			{
				if (group.First.Column >= mMinColumn)
				{
					break;
				}

				RecycleItemTmp(group.RemoveFirst());
			}

			while (group.Last != null && (group.Last.Column > mMaxColumn || group.Last.ItemIndex >= ItemTotalCount))
			{
				RecycleItemTmp(group.RemoveLast());
			}

			if (group.First == null)
			{
				LoopGridViewItem newItemByRowColumn = GetNewItemByRowColumn(groupIndex, mMinColumn);
				if (newItemByRowColumn == null)
				{
					return;
				}

				newItemByRowColumn.CachedRectTransform.anchoredPosition3D =
					GetItemPos(newItemByRowColumn.Row, newItemByRowColumn.Column);
				group.AddFirst(newItemByRowColumn);
			}

			while (group.First.Column > mMinColumn)
			{
				LoopGridViewItem newItemByRowColumn2 = GetNewItemByRowColumn(groupIndex, group.First.Column - 1);
				if (newItemByRowColumn2 == null)
				{
					while (group.Last.Column < mMaxColumn)
					{
						LoopGridViewItem newItemByRowColumn3 = GetNewItemByRowColumn(groupIndex, group.Last.Column + 1);
						if (newItemByRowColumn3 == null)
						{
							break;
						}

						newItemByRowColumn3.CachedRectTransform.anchoredPosition3D =
							GetItemPos(newItemByRowColumn3.Row, newItemByRowColumn3.Column);
						group.AddLast(newItemByRowColumn3);
					}

					return;
				}

				newItemByRowColumn2.CachedRectTransform.anchoredPosition3D =
					GetItemPos(newItemByRowColumn2.Row, newItemByRowColumn2.Column);
				group.AddFirst(newItemByRowColumn2);
			}

			// co: goto IL
			// goto IL_182;
		}


		private void UpdateColumnItemGroupForRecycleAndNew(GridItemGroup group)
		{
			int mMinRow = mCurFrameItemRangeData.mMinRow;
			int mMaxRow = mCurFrameItemRangeData.mMaxRow;
			int groupIndex = group.GroupIndex;
			while (group.First != null)
			{
				if (group.First.Row >= mMinRow)
				{
					break;
				}

				RecycleItemTmp(group.RemoveFirst());
			}

			while (group.Last != null && (group.Last.Row > mMaxRow || group.Last.ItemIndex >= ItemTotalCount))
			{
				RecycleItemTmp(group.RemoveLast());
			}

			if (group.First == null)
			{
				LoopGridViewItem newItemByRowColumn = GetNewItemByRowColumn(mMinRow, groupIndex);
				if (newItemByRowColumn == null)
				{
					return;
				}

				newItemByRowColumn.CachedRectTransform.anchoredPosition3D =
					GetItemPos(newItemByRowColumn.Row, newItemByRowColumn.Column);
				group.AddFirst(newItemByRowColumn);
			}

			while (group.First.Row > mMinRow)
			{
				LoopGridViewItem newItemByRowColumn2 = GetNewItemByRowColumn(group.First.Row - 1, groupIndex);
				if (newItemByRowColumn2 == null)
				{
					while (group.Last.Row < mMaxRow)
					{
						LoopGridViewItem newItemByRowColumn3 = GetNewItemByRowColumn(group.Last.Row + 1, groupIndex);
						if (newItemByRowColumn3 == null)
						{
							break;
						}

						newItemByRowColumn3.CachedRectTransform.anchoredPosition3D =
							GetItemPos(newItemByRowColumn3.Row, newItemByRowColumn3.Column);
						group.AddLast(newItemByRowColumn3);
					}

					return;
				}

				newItemByRowColumn2.CachedRectTransform.anchoredPosition3D =
					GetItemPos(newItemByRowColumn2.Row, newItemByRowColumn2.Column);
				group.AddFirst(newItemByRowColumn2);
			}

			// co: goto IL
			// goto IL_182;
		}


		private void SetScrollbarListener()
		{
			if (!ItemSnapEnable)
			{
				return;
			}

			mScrollBarClickEventListener1 = null;
			mScrollBarClickEventListener2 = null;
			Scrollbar scrollbar = null;
			Scrollbar scrollbar2 = null;
			if (mScrollRect.vertical && mScrollRect.verticalScrollbar != null)
			{
				scrollbar = mScrollRect.verticalScrollbar;
			}

			if (mScrollRect.horizontal && mScrollRect.horizontalScrollbar != null)
			{
				scrollbar2 = mScrollRect.horizontalScrollbar;
			}

			if (scrollbar != null)
			{
				ClickEventListener clickEventListener = ClickEventListener.Get(scrollbar.gameObject);
				mScrollBarClickEventListener1 = clickEventListener;
				clickEventListener.SetPointerUpHandler(OnPointerUpInScrollBar);
				clickEventListener.SetPointerDownHandler(OnPointerDownInScrollBar);
			}

			if (scrollbar2 != null)
			{
				ClickEventListener clickEventListener2 = ClickEventListener.Get(scrollbar2.gameObject);
				mScrollBarClickEventListener2 = clickEventListener2;
				clickEventListener2.SetPointerUpHandler(OnPointerUpInScrollBar);
				clickEventListener2.SetPointerDownHandler(OnPointerDownInScrollBar);
			}
		}


		private void OnPointerDownInScrollBar(GameObject obj)
		{
			mCurSnapData.Clear();
		}


		private void OnPointerUpInScrollBar(GameObject obj)
		{
			ForceSnapUpdateCheck();
		}


		private RowColumnPair FindNearestItemWithLocalPos(float x, float y)
		{
			Vector2 vector = new Vector2(x, y);
			RowColumnPair ceilItemRowColumnAtGivenAbsPos = GetCeilItemRowColumnAtGivenAbsPos(vector.x, vector.y);
			int mRow = ceilItemRowColumnAtGivenAbsPos.mRow;
			int mColumn = ceilItemRowColumnAtGivenAbsPos.mColumn;
			RowColumnPair result = new RowColumnPair(-1, -1);
			Vector2 zero = Vector2.zero;
			float num = float.MaxValue;
			for (int i = mRow - 1; i <= mRow + 1; i++)
			{
				for (int j = mColumn - 1; j <= mColumn + 1; j++)
				{
					if (i >= 0 && i < mRowCount && j >= 0 && j < mColumnCount)
					{
						float sqrMagnitude = (GetItemSnapPivotLocalPos(i, j) - vector).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							num = sqrMagnitude;
							result.mRow = i;
							result.mColumn = j;
						}
					}
				}
			}

			return result;
		}


		private Vector2 GetItemSnapPivotLocalPos(int row, int column)
		{
			Vector2 itemAbsPos = GetItemAbsPos(row, column);
			if (mArrangeType == GridItemArrangeType.TopLeftToBottomRight)
			{
				float x = itemAbsPos.x + mItemSize.x * mItemSnapPivot.x;
				float y = -itemAbsPos.y - mItemSize.y * (1f - mItemSnapPivot.y);
				return new Vector2(x, y);
			}

			if (mArrangeType == GridItemArrangeType.BottomLeftToTopRight)
			{
				float x2 = itemAbsPos.x + mItemSize.x * mItemSnapPivot.x;
				float y2 = itemAbsPos.y + mItemSize.y * mItemSnapPivot.y;
				return new Vector2(x2, y2);
			}

			if (mArrangeType == GridItemArrangeType.TopRightToBottomLeft)
			{
				float x3 = -itemAbsPos.x - mItemSize.x * (1f - mItemSnapPivot.x);
				float y3 = -itemAbsPos.y - mItemSize.y * (1f - mItemSnapPivot.y);
				return new Vector2(x3, y3);
			}

			if (mArrangeType == GridItemArrangeType.BottomRightToTopLeft)
			{
				float x4 = -itemAbsPos.x - mItemSize.x * (1f - mItemSnapPivot.x);
				float y4 = itemAbsPos.y + mItemSize.y * mItemSnapPivot.y;
				return new Vector2(x4, y4);
			}

			return Vector2.zero;
		}


		private Vector2 GetViewPortSnapPivotLocalPos(Vector2 pos)
		{
			float x = 0f;
			float y = 0f;
			if (mArrangeType == GridItemArrangeType.TopLeftToBottomRight)
			{
				x = -pos.x + ViewPortWidth * mViewPortSnapPivot.x;
				y = -pos.y - ViewPortHeight * (1f - mViewPortSnapPivot.y);
			}
			else if (mArrangeType == GridItemArrangeType.BottomLeftToTopRight)
			{
				x = -pos.x + ViewPortWidth * mViewPortSnapPivot.x;
				y = -pos.y + ViewPortHeight * mViewPortSnapPivot.y;
			}
			else if (mArrangeType == GridItemArrangeType.TopRightToBottomLeft)
			{
				x = -pos.x - ViewPortWidth * (1f - mViewPortSnapPivot.x);
				y = -pos.y - ViewPortHeight * (1f - mViewPortSnapPivot.y);
			}
			else if (mArrangeType == GridItemArrangeType.BottomRightToTopLeft)
			{
				x = -pos.x - ViewPortWidth * (1f - mViewPortSnapPivot.x);
				y = -pos.y + ViewPortHeight * mViewPortSnapPivot.y;
			}

			return new Vector2(x, y);
		}


		private void UpdateNearestSnapItem(bool forceSendEvent)
		{
			if (!mItemSnapEnable)
			{
				return;
			}

			if (mItemGroupList.Count == 0)
			{
				return;
			}

			if (!IsContainerTransCanMove())
			{
				return;
			}

			Vector2 containerVaildPos =
				GetContainerVaildPos(ContainerTrans.anchoredPosition3D.x, ContainerTrans.anchoredPosition3D.y);
			bool flag = containerVaildPos.y != mLastSnapCheckPos.y || containerVaildPos.x != mLastSnapCheckPos.x;
			mLastSnapCheckPos = containerVaildPos;
			if (!flag && mLeftSnapUpdateExtraCount > 0)
			{
				mLeftSnapUpdateExtraCount--;
				flag = true;
			}

			if (flag)
			{
				RowColumnPair rowColumnPair = new RowColumnPair(-1, -1);
				Vector2 viewPortSnapPivotLocalPos = GetViewPortSnapPivotLocalPos(containerVaildPos);
				rowColumnPair = FindNearestItemWithLocalPos(viewPortSnapPivotLocalPos.x, viewPortSnapPivotLocalPos.y);
				if (rowColumnPair.mRow >= 0)
				{
					RowColumnPair a = mCurSnapNearestItemRowColumn;
					mCurSnapNearestItemRowColumn = rowColumnPair;
					if ((forceSendEvent || a != mCurSnapNearestItemRowColumn) && mOnSnapNearestChanged != null)
					{
						mOnSnapNearestChanged(this);
					}
				}
				else
				{
					mCurSnapNearestItemRowColumn.mRow = -1;
					mCurSnapNearestItemRowColumn.mColumn = -1;
				}
			}
		}


		private void UpdateFromSettingParam(LoopGridViewSettingParam param)
		{
			if (param == null)
			{
				return;
			}

			if (param.mItemSize != null)
			{
				mItemSize = (Vector2) param.mItemSize;
			}

			if (param.mItemPadding != null)
			{
				mItemPadding = (Vector2) param.mItemPadding;
			}

			if (param.mPadding != null)
			{
				mPadding = (RectOffset) param.mPadding;
			}

			if (param.mGridFixedType != null)
			{
				mGridFixedType = (GridFixedType) param.mGridFixedType;
			}

			if (param.mFixedRowOrColumnCount != null)
			{
				mFixedRowOrColumnCount = (int) param.mFixedRowOrColumnCount;
			}
		}


		public void FinishSnapImmediately()
		{
			UpdateSnapMove(true);
		}


		private void UpdateSnapMove(bool immediate = false, bool forceSendEvent = false)
		{
			if (!mItemSnapEnable)
			{
				return;
			}

			UpdateNearestSnapItem(false);
			Vector2 vector = mContainerTrans.anchoredPosition3D;
			if (!CanSnap())
			{
				ClearSnapData();
				return;
			}

			UpdateCurSnapData();
			if (mCurSnapData.mSnapStatus != SnapStatus.SnapMoving)
			{
				return;
			}

			if (Mathf.Abs(mScrollRect.velocity.x) + Mathf.Abs(mScrollRect.velocity.y) > 0f)
			{
				mScrollRect.StopMovement();
			}

			float mCurSnapVal = mCurSnapData.mCurSnapVal;
			mCurSnapData.mCurSnapVal = Mathf.SmoothDamp(mCurSnapData.mCurSnapVal, mCurSnapData.mTargetSnapVal,
				ref mSmoothDumpVel, mSmoothDumpRate);
			float d = mCurSnapData.mCurSnapVal - mCurSnapVal;
			if (immediate || Mathf.Abs(mCurSnapData.mTargetSnapVal - mCurSnapData.mCurSnapVal) < mSnapFinishThreshold)
			{
				vector += (mCurSnapData.mTargetSnapVal - mCurSnapVal) * mCurSnapData.mSnapNeedMoveDir;
				mCurSnapData.mSnapStatus = SnapStatus.SnapMoveFinish;
				if (mOnSnapItemFinished != null)
				{
					LoopGridViewItem shownItemByRowColumn = GetShownItemByRowColumn(mCurSnapNearestItemRowColumn.mRow,
						mCurSnapNearestItemRowColumn.mColumn);
					if (shownItemByRowColumn != null)
					{
						mOnSnapItemFinished(this, shownItemByRowColumn);
					}
				}
			}
			else
			{
				vector += d * mCurSnapData.mSnapNeedMoveDir;
			}

			mContainerTrans.anchoredPosition3D = GetContainerVaildPos(vector.x, vector.y);
		}


		private GridItemGroup GetShownGroup(int groupIndex)
		{
			if (groupIndex < 0)
			{
				return null;
			}

			int count = mItemGroupList.Count;
			if (count == 0)
			{
				return null;
			}

			if (groupIndex < mItemGroupList[0].GroupIndex || groupIndex > mItemGroupList[count - 1].GroupIndex)
			{
				return null;
			}

			int index = groupIndex - mItemGroupList[0].GroupIndex;
			return mItemGroupList[index];
		}


		private void FillCurSnapData(int row, int column)
		{
			Vector2 itemSnapPivotLocalPos = GetItemSnapPivotLocalPos(row, column);
			Vector2 containerVaildPos =
				GetContainerVaildPos(ContainerTrans.anchoredPosition3D.x, ContainerTrans.anchoredPosition3D.y);
			Vector2 vector = GetViewPortSnapPivotLocalPos(containerVaildPos) - itemSnapPivotLocalPos;
			if (!mScrollRect.horizontal)
			{
				vector.x = 0f;
			}

			if (!mScrollRect.vertical)
			{
				vector.y = 0f;
			}

			mCurSnapData.mTargetSnapVal = vector.magnitude;
			mCurSnapData.mCurSnapVal = 0f;
			mCurSnapData.mSnapNeedMoveDir = vector.normalized;
		}


		private void UpdateCurSnapData()
		{
			if (mItemGroupList.Count == 0)
			{
				mCurSnapData.Clear();
				return;
			}

			if (mCurSnapData.mSnapStatus == SnapStatus.SnapMoveFinish)
			{
				if (mCurSnapData.mSnapTarget == mCurSnapNearestItemRowColumn)
				{
					return;
				}

				mCurSnapData.mSnapStatus = SnapStatus.NoTargetSet;
			}

			if (mCurSnapData.mSnapStatus == SnapStatus.SnapMoving)
			{
				if (mCurSnapData.mSnapTarget == mCurSnapNearestItemRowColumn || mCurSnapData.mIsForceSnapTo)
				{
					return;
				}

				mCurSnapData.mSnapStatus = SnapStatus.NoTargetSet;
			}

			if (mCurSnapData.mSnapStatus == SnapStatus.NoTargetSet)
			{
				if (GetShownItemByRowColumn(mCurSnapNearestItemRowColumn.mRow, mCurSnapNearestItemRowColumn.mColumn) ==
				    null)
				{
					return;
				}

				mCurSnapData.mSnapTarget = mCurSnapNearestItemRowColumn;
				mCurSnapData.mSnapStatus = SnapStatus.TargetHasSet;
				mCurSnapData.mIsForceSnapTo = false;
			}

			if (mCurSnapData.mSnapStatus == SnapStatus.TargetHasSet)
			{
				LoopGridViewItem shownItemByRowColumn =
					GetShownItemByRowColumn(mCurSnapData.mSnapTarget.mRow, mCurSnapData.mSnapTarget.mColumn);
				if (shownItemByRowColumn == null)
				{
					mCurSnapData.Clear();
					return;
				}

				FillCurSnapData(shownItemByRowColumn.Row, shownItemByRowColumn.Column);
				mCurSnapData.mSnapStatus = SnapStatus.SnapMoving;
			}
		}


		private bool CanSnap()
		{
			if (mIsDraging)
			{
				return false;
			}

			if (mScrollBarClickEventListener1 != null && mScrollBarClickEventListener1.IsPressd)
			{
				return false;
			}

			if (mScrollBarClickEventListener2 != null && mScrollBarClickEventListener2.IsPressd)
			{
				return false;
			}

			if (!IsContainerTransCanMove())
			{
				return false;
			}

			if (Mathf.Abs(mScrollRect.velocity.x) > mSnapVecThreshold)
			{
				return false;
			}

			if (Mathf.Abs(mScrollRect.velocity.y) > mSnapVecThreshold)
			{
				return false;
			}

			Vector3 anchoredPosition3D = mContainerTrans.anchoredPosition3D;
			Vector2 containerVaildPos = GetContainerVaildPos(anchoredPosition3D.x, anchoredPosition3D.y);
			return Mathf.Abs(anchoredPosition3D.x - containerVaildPos.x) <= 3f &&
			       Mathf.Abs(anchoredPosition3D.y - containerVaildPos.y) <= 3f;
		}


		private GridItemGroup GetOneItemGroupObj()
		{
			int count = mItemGroupObjPool.Count;
			if (count == 0)
			{
				return new GridItemGroup();
			}

			GridItemGroup result = mItemGroupObjPool[count - 1];
			mItemGroupObjPool.RemoveAt(count - 1);
			return result;
		}


		private void RecycleOneItemGroupObj(GridItemGroup obj)
		{
			mItemGroupObjPool.Add(obj);
		}


		private class SnapData
		{
			public float mCurSnapVal;


			public bool mIsForceSnapTo;


			public Vector2 mSnapNeedMoveDir;


			public SnapStatus mSnapStatus;


			public RowColumnPair mSnapTarget;


			public float mTargetSnapVal;


			public void Clear()
			{
				mSnapStatus = SnapStatus.NoTargetSet;
				mIsForceSnapTo = false;
			}
		}


		private class ItemRangeData
		{
			public Vector2 mCheckedPosition;


			public int mMaxColumn;


			public int mMaxRow;


			public int mMinColumn;


			public int mMinRow;
		}
	}
}