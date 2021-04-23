using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class StaggeredGridItemGroup
	{
		private readonly List<LoopStaggeredGridViewItem> mItemList = new List<LoopStaggeredGridViewItem>();


		private readonly Vector3[] mItemWorldCorners = new Vector3[4];


		private readonly Vector3[] mViewPortRectLocalCorners = new Vector3[4];

		private ListItemArrangeType mArrangeType = default;


		private RectTransform mContainerTrans = default;


		private int mCurReadyMaxItemIndex = default;


		private int mCurReadyMinItemIndex = default;


		private GameObject mGameObject = default;


		public int mGroupIndex = default;


		private bool mIsVertList = default;


		private readonly float mItemDefaultWithPaddingSize = default;


		private ItemPosMgr mItemPosMgr;


		private int mItemTotalCount;


		private Vector3 mLastFrameContainerPos = Vector3.zero;


		private int mLastItemIndex;


		private float mLastItemPadding;


		private int mListUpdateCheckFrameCount;


		private bool mNeedCheckNextMaxItem = true;


		private bool mNeedCheckNextMinItem = true;


		private Func<int, int, LoopStaggeredGridViewItem> mOnGetItemByIndex;


		private LoopStaggeredGridView mParentGridView;


		private ScrollRect mScrollRect;


		private RectTransform mScrollRectTransform;


		private bool mSupportScrollBar = true;


		private RectTransform mViewPortRectTransform;


		public List<int> ItemIndexMap { get; } = new List<int>();


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


		private bool IsDraging => mParentGridView.IsDraging;


		public int HadCreatedItemCount => ItemIndexMap.Count;


		public void Init(LoopStaggeredGridView parent, int itemTotalCount, int groupIndex,
			Func<int, int, LoopStaggeredGridViewItem> onGetItemByIndex)
		{
			mGroupIndex = groupIndex;
			mParentGridView = parent;
			mArrangeType = mParentGridView.ArrangeType;
			mGameObject = mParentGridView.gameObject;
			mScrollRect = mGameObject.GetComponent<ScrollRect>();
			mItemPosMgr = new ItemPosMgr(mItemDefaultWithPaddingSize);
			mScrollRectTransform = mScrollRect.GetComponent<RectTransform>();
			mContainerTrans = mScrollRect.content;
			mViewPortRectTransform = mScrollRect.viewport;
			if (mViewPortRectTransform == null)
			{
				mViewPortRectTransform = mScrollRectTransform;
			}

			mIsVertList = mArrangeType == ListItemArrangeType.TopToBottom ||
			              mArrangeType == ListItemArrangeType.BottomToTop;
			mOnGetItemByIndex = onGetItemByIndex;
			mItemTotalCount = itemTotalCount;
			mViewPortRectTransform.GetLocalCorners(mViewPortRectLocalCorners);
			if (mItemTotalCount < 0)
			{
				mSupportScrollBar = false;
			}

			if (mSupportScrollBar)
			{
				mItemPosMgr.SetItemMaxCount(mItemTotalCount);
			}
			else
			{
				mItemPosMgr.SetItemMaxCount(0);
			}

			mCurReadyMaxItemIndex = 0;
			mCurReadyMinItemIndex = 0;
			mNeedCheckNextMaxItem = true;
			mNeedCheckNextMinItem = true;
		}


		public void ResetListView()
		{
			mViewPortRectTransform.GetLocalCorners(mViewPortRectLocalCorners);
		}


		public LoopStaggeredGridViewItem GetShownItemByItemIndex(int itemIndex)
		{
			int count = mItemList.Count;
			if (count == 0)
			{
				return null;
			}

			if (itemIndex < mItemList[0].ItemIndex || itemIndex > mItemList[count - 1].ItemIndex)
			{
				return null;
			}

			for (int i = 0; i < count; i++)
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[i];
				if (loopStaggeredGridViewItem.ItemIndex == itemIndex)
				{
					return loopStaggeredGridViewItem;
				}
			}

			return null;
		}


		public LoopStaggeredGridViewItem GetShownItemByIndexInGroup(int indexInGroup)
		{
			int count = mItemList.Count;
			if (count == 0)
			{
				return null;
			}

			if (indexInGroup < mItemList[0].ItemIndexInGroup || indexInGroup > mItemList[count - 1].ItemIndexInGroup)
			{
				return null;
			}

			int index = indexInGroup - mItemList[0].ItemIndexInGroup;
			return mItemList[index];
		}


		public int GetIndexInShownItemList(LoopStaggeredGridViewItem item)
		{
			if (item == null)
			{
				return -1;
			}

			int count = mItemList.Count;
			if (count == 0)
			{
				return -1;
			}

			for (int i = 0; i < count; i++)
			{
				if (mItemList[i] == item)
				{
					return i;
				}
			}

			return -1;
		}


		public void RefreshAllShownItem()
		{
			if (mItemList.Count == 0)
			{
				return;
			}

			RefreshAllShownItemWithFirstIndexInGroup(mItemList[0].ItemIndexInGroup);
		}


		public void OnItemSizeChanged(int indexInGroup)
		{
			LoopStaggeredGridViewItem shownItemByIndexInGroup = GetShownItemByIndexInGroup(indexInGroup);
			if (shownItemByIndexInGroup == null)
			{
				return;
			}

			if (mSupportScrollBar)
			{
				if (mIsVertList)
				{
					SetItemSize(indexInGroup, shownItemByIndexInGroup.CachedRectTransform.rect.height,
						shownItemByIndexInGroup.Padding);
				}
				else
				{
					SetItemSize(indexInGroup, shownItemByIndexInGroup.CachedRectTransform.rect.width,
						shownItemByIndexInGroup.Padding);
				}
			}

			UpdateAllShownItemsPos();
		}


		public void RefreshItemByIndexInGroup(int indexInGroup)
		{
			int count = mItemList.Count;
			if (count == 0)
			{
				return;
			}

			if (indexInGroup < mItemList[0].ItemIndexInGroup || indexInGroup > mItemList[count - 1].ItemIndexInGroup)
			{
				return;
			}

			int itemIndexInGroup = mItemList[0].ItemIndexInGroup;
			int index = indexInGroup - itemIndexInGroup;
			LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[index];
			Vector3 anchoredPosition3D = loopStaggeredGridViewItem.CachedRectTransform.anchoredPosition3D;
			RecycleItemTmp(loopStaggeredGridViewItem);
			LoopStaggeredGridViewItem newItemByIndexInGroup = GetNewItemByIndexInGroup(indexInGroup);
			if (newItemByIndexInGroup == null)
			{
				RefreshAllShownItemWithFirstIndexInGroup(itemIndexInGroup);
				return;
			}

			mItemList[index] = newItemByIndexInGroup;
			if (mIsVertList)
			{
				anchoredPosition3D.x = newItemByIndexInGroup.StartPosOffset;
			}
			else
			{
				anchoredPosition3D.y = newItemByIndexInGroup.StartPosOffset;
			}

			newItemByIndexInGroup.CachedRectTransform.anchoredPosition3D = anchoredPosition3D;
			OnItemSizeChanged(indexInGroup);
			ClearAllTmpRecycledItem();
		}


		public void RefreshAllShownItemWithFirstIndexInGroup(int firstItemIndexInGroup)
		{
			int count = mItemList.Count;
			if (count == 0)
			{
				return;
			}

			Vector3 anchoredPosition3D = mItemList[0].CachedRectTransform.anchoredPosition3D;
			RecycleAllItem();
			for (int i = 0; i < count; i++)
			{
				int num = firstItemIndexInGroup + i;
				LoopStaggeredGridViewItem newItemByIndexInGroup = GetNewItemByIndexInGroup(num);
				if (newItemByIndexInGroup == null)
				{
					break;
				}

				if (mIsVertList)
				{
					anchoredPosition3D.x = newItemByIndexInGroup.StartPosOffset;
				}
				else
				{
					anchoredPosition3D.y = newItemByIndexInGroup.StartPosOffset;
				}

				newItemByIndexInGroup.CachedRectTransform.anchoredPosition3D = anchoredPosition3D;
				if (mSupportScrollBar)
				{
					if (mIsVertList)
					{
						SetItemSize(num, newItemByIndexInGroup.CachedRectTransform.rect.height,
							newItemByIndexInGroup.Padding);
					}
					else
					{
						SetItemSize(num, newItemByIndexInGroup.CachedRectTransform.rect.width,
							newItemByIndexInGroup.Padding);
					}
				}

				mItemList.Add(newItemByIndexInGroup);
			}

			UpdateAllShownItemsPos();
			ClearAllTmpRecycledItem();
		}


		public void RefreshAllShownItemWithFirstIndexAndPos(int firstItemIndexInGroup, Vector3 pos)
		{
			RecycleAllItem();
			LoopStaggeredGridViewItem newItemByIndexInGroup = GetNewItemByIndexInGroup(firstItemIndexInGroup);
			if (newItemByIndexInGroup == null)
			{
				return;
			}

			if (mIsVertList)
			{
				pos.x = newItemByIndexInGroup.StartPosOffset;
			}
			else
			{
				pos.y = newItemByIndexInGroup.StartPosOffset;
			}

			newItemByIndexInGroup.CachedRectTransform.anchoredPosition3D = pos;
			if (mSupportScrollBar)
			{
				if (mIsVertList)
				{
					SetItemSize(firstItemIndexInGroup, newItemByIndexInGroup.CachedRectTransform.rect.height,
						newItemByIndexInGroup.Padding);
				}
				else
				{
					SetItemSize(firstItemIndexInGroup, newItemByIndexInGroup.CachedRectTransform.rect.width,
						newItemByIndexInGroup.Padding);
				}
			}

			mItemList.Add(newItemByIndexInGroup);
			UpdateAllShownItemsPos();
			mParentGridView.UpdateListViewWithDefault();
			ClearAllTmpRecycledItem();
		}


		private void SetItemSize(int itemIndex, float itemSize, float padding)
		{
			mItemPosMgr.SetItemSize(itemIndex, itemSize + padding);
			if (itemIndex >= mLastItemIndex)
			{
				mLastItemIndex = itemIndex;
				mLastItemPadding = padding;
			}
		}


		private bool GetPlusItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
		{
			return mItemPosMgr.GetItemIndexAndPosAtGivenPos(pos, ref index, ref itemPos);
		}


		public float GetItemPos(int itemIndex)
		{
			return mItemPosMgr.GetItemPos(itemIndex);
		}


		public Vector3 GetItemCornerPosInViewPort(LoopStaggeredGridViewItem item,
			ItemCornerEnum corner = ItemCornerEnum.LeftBottom)
		{
			item.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
			return mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[(int) corner]);
		}


		public void RecycleItemTmp(LoopStaggeredGridViewItem item)
		{
			mParentGridView.RecycleItemTmp(item);
		}


		public void RecycleAllItem()
		{
			foreach (LoopStaggeredGridViewItem item in mItemList)
			{
				RecycleItemTmp(item);
			}

			mItemList.Clear();
		}


		public void ClearAllTmpRecycledItem()
		{
			mParentGridView.ClearAllTmpRecycledItem();
		}


		private LoopStaggeredGridViewItem GetNewItemByIndexInGroup(int indexInGroup)
		{
			return mParentGridView.GetNewItemByGroupAndIndex(mGroupIndex, indexInGroup);
		}


		public void SetListItemCount(int itemCount)
		{
			if (itemCount == mItemTotalCount)
			{
				return;
			}

			int num = mItemTotalCount;
			mItemTotalCount = itemCount;
			UpdateItemIndexMap(num);
			if (num < mItemTotalCount)
			{
				mItemPosMgr.SetItemMaxCount(mItemTotalCount);
			}
			else
			{
				mItemPosMgr.SetItemMaxCount(HadCreatedItemCount);
				mItemPosMgr.SetItemMaxCount(mItemTotalCount);
			}

			RecycleAllItem();
			if (mItemTotalCount == 0)
			{
				mCurReadyMaxItemIndex = 0;
				mCurReadyMinItemIndex = 0;
				mNeedCheckNextMaxItem = false;
				mNeedCheckNextMinItem = false;
				ItemIndexMap.Clear();
				return;
			}

			if (mCurReadyMaxItemIndex >= mItemTotalCount)
			{
				mCurReadyMaxItemIndex = mItemTotalCount - 1;
			}

			mNeedCheckNextMaxItem = true;
			mNeedCheckNextMinItem = true;
		}


		private void UpdateItemIndexMap(int oldItemTotalCount)
		{
			int count = ItemIndexMap.Count;
			if (count == 0)
			{
				return;
			}

			if (mItemTotalCount == 0)
			{
				ItemIndexMap.Clear();
				return;
			}

			if (mItemTotalCount >= oldItemTotalCount)
			{
				return;
			}

			int itemTotalCount = mParentGridView.ItemTotalCount;
			if (ItemIndexMap[count - 1] < itemTotalCount)
			{
				return;
			}

			int i = 0;
			int num = count - 1;
			int num2 = 0;
			while (i <= num)
			{
				int num3 = (i + num) / 2;
				int num4 = ItemIndexMap[num3];
				if (num4 == itemTotalCount)
				{
					num2 = num3;
					break;
				}

				if (num4 >= itemTotalCount)
				{
					break;
				}

				i = num3 + 1;
				num2 = i;
			}

			int num5 = 0;
			for (int j = num2; j < count; j++)
			{
				if (ItemIndexMap[j] >= itemTotalCount)
				{
					num5 = j;
					break;
				}
			}

			ItemIndexMap.RemoveRange(num5, count - num5);
		}


		public void UpdateListViewPart1(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0,
			float distanceForNew1)
		{
			if (mSupportScrollBar)
			{
				mItemPosMgr.Update(false);
			}

			mListUpdateCheckFrameCount = mParentGridView.ListUpdateCheckFrameCount;
			bool flag = true;
			int num = 0;
			int num2 = 9999;
			while (flag)
			{
				num++;
				if (num >= num2)
				{
					Debug.LogError("UpdateListViewPart1 while loop " + num + " times! something is wrong!");
					break;
				}

				if (mIsVertList)
				{
					flag = UpdateForVertListPart1(distanceForRecycle0, distanceForRecycle1, distanceForNew0,
						distanceForNew1);
				}
				else
				{
					flag = UpdateForHorizontalListPart1(distanceForRecycle0, distanceForRecycle1, distanceForNew0,
						distanceForNew1);
				}
			}

			mLastFrameContainerPos = mContainerTrans.anchoredPosition3D;
		}


		public bool UpdateListViewPart2(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0,
			float distanceForNew1)
		{
			if (mIsVertList)
			{
				return UpdateForVertListPart2(distanceForRecycle0, distanceForRecycle1, distanceForNew0,
					distanceForNew1);
			}

			return UpdateForHorizontalListPart2(distanceForRecycle0, distanceForRecycle1, distanceForNew0,
				distanceForNew1);
		}


		public bool UpdateForVertListPart1(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0,
			float distanceForNew1)
		{
			if (mItemTotalCount == 0)
			{
				if (mItemList.Count > 0)
				{
					RecycleAllItem();
				}

				return false;
			}

			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				if (mItemList.Count == 0)
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

					LoopStaggeredGridViewItem newItemByIndexInGroup = GetNewItemByIndexInGroup(num2);
					if (newItemByIndexInGroup == null)
					{
						return false;
					}

					if (mSupportScrollBar)
					{
						SetItemSize(num2, newItemByIndexInGroup.CachedRectTransform.rect.height,
							newItemByIndexInGroup.Padding);
					}

					mItemList.Add(newItemByIndexInGroup);
					newItemByIndexInGroup.CachedRectTransform.anchoredPosition3D =
						new Vector3(newItemByIndexInGroup.StartPosOffset, num3, 0f);
					return true;
				}

				LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[0];
				loopStaggeredGridViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (!IsDraging && loopStaggeredGridViewItem.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    vector2.y - mViewPortRectLocalCorners[1].y > distanceForRecycle0)
				{
					mItemList.RemoveAt(0);
					RecycleItemTmp(loopStaggeredGridViewItem);
					if (!mSupportScrollBar)
					{
						CheckIfNeedUpdateItemPos();
					}

					return true;
				}

				LoopStaggeredGridViewItem loopStaggeredGridViewItem2 = mItemList[mItemList.Count - 1];
				loopStaggeredGridViewItem2.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector3 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (!IsDraging && loopStaggeredGridViewItem2.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    mViewPortRectLocalCorners[0].y - vector3.y > distanceForRecycle1)
				{
					mItemList.RemoveAt(mItemList.Count - 1);
					RecycleItemTmp(loopStaggeredGridViewItem2);
					if (!mSupportScrollBar)
					{
						CheckIfNeedUpdateItemPos();
					}

					return true;
				}

				if (vector.y - mViewPortRectLocalCorners[1].y < distanceForNew0)
				{
					if (loopStaggeredGridViewItem.ItemIndexInGroup < mCurReadyMinItemIndex)
					{
						mCurReadyMinItemIndex = loopStaggeredGridViewItem.ItemIndexInGroup;
						mNeedCheckNextMinItem = true;
					}

					int num4 = loopStaggeredGridViewItem.ItemIndexInGroup - 1;
					if (num4 >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup2 = GetNewItemByIndexInGroup(num4);
						if (!(newItemByIndexInGroup2 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num4, newItemByIndexInGroup2.CachedRectTransform.rect.height,
									newItemByIndexInGroup2.Padding);
							}

							mItemList.Insert(0, newItemByIndexInGroup2);
							float y = loopStaggeredGridViewItem.CachedRectTransform.anchoredPosition3D.y +
							          newItemByIndexInGroup2.CachedRectTransform.rect.height +
							          newItemByIndexInGroup2.Padding;
							newItemByIndexInGroup2.CachedRectTransform.anchoredPosition3D =
								new Vector3(newItemByIndexInGroup2.StartPosOffset, y, 0f);
							CheckIfNeedUpdateItemPos();
							if (num4 < mCurReadyMinItemIndex)
							{
								mCurReadyMinItemIndex = num4;
							}

							return true;
						}

						mCurReadyMinItemIndex = loopStaggeredGridViewItem.ItemIndexInGroup;
						mNeedCheckNextMinItem = false;
					}
				}

				if (mViewPortRectLocalCorners[0].y - vector4.y < distanceForNew1)
				{
					if (loopStaggeredGridViewItem2.ItemIndexInGroup > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopStaggeredGridViewItem2.ItemIndexInGroup;
						mNeedCheckNextMaxItem = true;
					}

					int num5 = loopStaggeredGridViewItem2.ItemIndexInGroup + 1;
					if (num5 >= ItemIndexMap.Count)
					{
						return false;
					}

					if (num5 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup3 = GetNewItemByIndexInGroup(num5);
						if (newItemByIndexInGroup3 == null)
						{
							mCurReadyMaxItemIndex = loopStaggeredGridViewItem2.ItemIndexInGroup;
							mNeedCheckNextMaxItem = false;
							CheckIfNeedUpdateItemPos();
							return false;
						}

						if (mSupportScrollBar)
						{
							SetItemSize(num5, newItemByIndexInGroup3.CachedRectTransform.rect.height,
								newItemByIndexInGroup3.Padding);
						}

						mItemList.Add(newItemByIndexInGroup3);
						float y2 = loopStaggeredGridViewItem2.CachedRectTransform.anchoredPosition3D.y -
						           loopStaggeredGridViewItem2.CachedRectTransform.rect.height -
						           loopStaggeredGridViewItem2.Padding;
						newItemByIndexInGroup3.CachedRectTransform.anchoredPosition3D =
							new Vector3(newItemByIndexInGroup3.StartPosOffset, y2, 0f);
						CheckIfNeedUpdateItemPos();
						if (num5 > mCurReadyMaxItemIndex)
						{
							mCurReadyMaxItemIndex = num5;
						}

						return true;
					}
				}
			}
			else if (mItemList.Count == 0)
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

				LoopStaggeredGridViewItem newItemByIndexInGroup4 = GetNewItemByIndexInGroup(num7);
				if (newItemByIndexInGroup4 == null)
				{
					return false;
				}

				if (mSupportScrollBar)
				{
					SetItemSize(num7, newItemByIndexInGroup4.CachedRectTransform.rect.height,
						newItemByIndexInGroup4.Padding);
				}

				mItemList.Add(newItemByIndexInGroup4);
				newItemByIndexInGroup4.CachedRectTransform.anchoredPosition3D =
					new Vector3(newItemByIndexInGroup4.StartPosOffset, y3, 0f);
				return true;
			}
			else
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem3 = mItemList[0];
				loopStaggeredGridViewItem3.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector5 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector6 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (!IsDraging && loopStaggeredGridViewItem3.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    mViewPortRectLocalCorners[0].y - vector5.y > distanceForRecycle0)
				{
					mItemList.RemoveAt(0);
					RecycleItemTmp(loopStaggeredGridViewItem3);
					if (!mSupportScrollBar)
					{
						CheckIfNeedUpdateItemPos();
					}

					return true;
				}

				LoopStaggeredGridViewItem loopStaggeredGridViewItem4 = mItemList[mItemList.Count - 1];
				loopStaggeredGridViewItem4.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector7 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector8 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (!IsDraging && loopStaggeredGridViewItem4.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    vector8.y - mViewPortRectLocalCorners[1].y > distanceForRecycle1)
				{
					mItemList.RemoveAt(mItemList.Count - 1);
					RecycleItemTmp(loopStaggeredGridViewItem4);
					if (!mSupportScrollBar)
					{
						CheckIfNeedUpdateItemPos();
					}

					return true;
				}

				if (mViewPortRectLocalCorners[0].y - vector6.y < distanceForNew0)
				{
					if (loopStaggeredGridViewItem3.ItemIndexInGroup < mCurReadyMinItemIndex)
					{
						mCurReadyMinItemIndex = loopStaggeredGridViewItem3.ItemIndexInGroup;
						mNeedCheckNextMinItem = true;
					}

					int num8 = loopStaggeredGridViewItem3.ItemIndexInGroup - 1;
					if (num8 >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup5 = GetNewItemByIndexInGroup(num8);
						if (!(newItemByIndexInGroup5 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num8, newItemByIndexInGroup5.CachedRectTransform.rect.height,
									newItemByIndexInGroup5.Padding);
							}

							mItemList.Insert(0, newItemByIndexInGroup5);
							float y4 = loopStaggeredGridViewItem3.CachedRectTransform.anchoredPosition3D.y -
							           newItemByIndexInGroup5.CachedRectTransform.rect.height -
							           newItemByIndexInGroup5.Padding;
							newItemByIndexInGroup5.CachedRectTransform.anchoredPosition3D =
								new Vector3(newItemByIndexInGroup5.StartPosOffset, y4, 0f);
							CheckIfNeedUpdateItemPos();
							if (num8 < mCurReadyMinItemIndex)
							{
								mCurReadyMinItemIndex = num8;
							}

							return true;
						}

						mCurReadyMinItemIndex = loopStaggeredGridViewItem3.ItemIndexInGroup;
						mNeedCheckNextMinItem = false;
					}
				}

				if (vector7.y - mViewPortRectLocalCorners[1].y < distanceForNew1)
				{
					if (loopStaggeredGridViewItem4.ItemIndexInGroup > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopStaggeredGridViewItem4.ItemIndexInGroup;
						mNeedCheckNextMaxItem = true;
					}

					int num9 = loopStaggeredGridViewItem4.ItemIndexInGroup + 1;
					if (num9 >= ItemIndexMap.Count)
					{
						return false;
					}

					if (num9 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup6 = GetNewItemByIndexInGroup(num9);
						if (newItemByIndexInGroup6 == null)
						{
							mCurReadyMaxItemIndex = loopStaggeredGridViewItem4.ItemIndexInGroup;
							mNeedCheckNextMaxItem = false;
							CheckIfNeedUpdateItemPos();
							return false;
						}

						if (mSupportScrollBar)
						{
							SetItemSize(num9, newItemByIndexInGroup6.CachedRectTransform.rect.height,
								newItemByIndexInGroup6.Padding);
						}

						mItemList.Add(newItemByIndexInGroup6);
						float y5 = loopStaggeredGridViewItem4.CachedRectTransform.anchoredPosition3D.y +
						           loopStaggeredGridViewItem4.CachedRectTransform.rect.height +
						           loopStaggeredGridViewItem4.Padding;
						newItemByIndexInGroup6.CachedRectTransform.anchoredPosition3D =
							new Vector3(newItemByIndexInGroup6.StartPosOffset, y5, 0f);
						CheckIfNeedUpdateItemPos();
						if (num9 > mCurReadyMaxItemIndex)
						{
							mCurReadyMaxItemIndex = num9;
						}

						return true;
					}
				}
			}

			return false;
		}


		public bool UpdateForVertListPart2(float distanceForRecycle0, float distanceForRecycle1, float distanceForNew0,
			float distanceForNew1)
		{
			if (mItemTotalCount == 0)
			{
				if (mItemList.Count > 0)
				{
					RecycleAllItem();
				}

				return false;
			}

			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				if (mItemList.Count == 0)
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

					LoopStaggeredGridViewItem newItemByIndexInGroup = GetNewItemByIndexInGroup(num2);
					if (newItemByIndexInGroup == null)
					{
						return false;
					}

					if (mSupportScrollBar)
					{
						SetItemSize(num2, newItemByIndexInGroup.CachedRectTransform.rect.height,
							newItemByIndexInGroup.Padding);
					}

					mItemList.Add(newItemByIndexInGroup);
					newItemByIndexInGroup.CachedRectTransform.anchoredPosition3D =
						new Vector3(newItemByIndexInGroup.StartPosOffset, num3, 0f);
					return true;
				}

				LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[mItemList.Count - 1];
				loopStaggeredGridViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[0]);
				if (mViewPortRectLocalCorners[0].y - vector.y < distanceForNew1)
				{
					if (loopStaggeredGridViewItem.ItemIndexInGroup > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopStaggeredGridViewItem.ItemIndexInGroup;
						mNeedCheckNextMaxItem = true;
					}

					int num4 = loopStaggeredGridViewItem.ItemIndexInGroup + 1;
					if (num4 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup2 = GetNewItemByIndexInGroup(num4);
						if (newItemByIndexInGroup2 == null)
						{
							mCurReadyMaxItemIndex = loopStaggeredGridViewItem.ItemIndexInGroup;
							mNeedCheckNextMaxItem = false;
							CheckIfNeedUpdateItemPos();
							return false;
						}

						if (mSupportScrollBar)
						{
							SetItemSize(num4, newItemByIndexInGroup2.CachedRectTransform.rect.height,
								newItemByIndexInGroup2.Padding);
						}

						mItemList.Add(newItemByIndexInGroup2);
						float y = loopStaggeredGridViewItem.CachedRectTransform.anchoredPosition3D.y -
						          loopStaggeredGridViewItem.CachedRectTransform.rect.height -
						          loopStaggeredGridViewItem.Padding;
						newItemByIndexInGroup2.CachedRectTransform.anchoredPosition3D =
							new Vector3(newItemByIndexInGroup2.StartPosOffset, y, 0f);
						CheckIfNeedUpdateItemPos();
						if (num4 > mCurReadyMaxItemIndex)
						{
							mCurReadyMaxItemIndex = num4;
						}

						return true;
					}
				}
			}
			else if (mItemList.Count == 0)
			{
				float num5 = mContainerTrans.anchoredPosition3D.y;
				if (num5 > 0f)
				{
					num5 = 0f;
				}

				int num6 = 0;
				float y2 = -num5;
				if (mSupportScrollBar && !GetPlusItemIndexAndPosAtGivenPos(-num5, ref num6, ref y2))
				{
					return false;
				}

				LoopStaggeredGridViewItem newItemByIndexInGroup3 = GetNewItemByIndexInGroup(num6);
				if (newItemByIndexInGroup3 == null)
				{
					return false;
				}

				if (mSupportScrollBar)
				{
					SetItemSize(num6, newItemByIndexInGroup3.CachedRectTransform.rect.height,
						newItemByIndexInGroup3.Padding);
				}

				mItemList.Add(newItemByIndexInGroup3);
				newItemByIndexInGroup3.CachedRectTransform.anchoredPosition3D =
					new Vector3(newItemByIndexInGroup3.StartPosOffset, y2, 0f);
				return true;
			}
			else
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem2 = mItemList[mItemList.Count - 1];
				loopStaggeredGridViewItem2.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				if (mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]).y -
					mViewPortRectLocalCorners[1].y < distanceForNew1)
				{
					if (loopStaggeredGridViewItem2.ItemIndexInGroup > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopStaggeredGridViewItem2.ItemIndexInGroup;
						mNeedCheckNextMaxItem = true;
					}

					int num7 = loopStaggeredGridViewItem2.ItemIndexInGroup + 1;
					if (num7 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup4 = GetNewItemByIndexInGroup(num7);
						if (newItemByIndexInGroup4 == null)
						{
							mCurReadyMaxItemIndex = loopStaggeredGridViewItem2.ItemIndexInGroup;
							mNeedCheckNextMaxItem = false;
							CheckIfNeedUpdateItemPos();
							return false;
						}

						if (mSupportScrollBar)
						{
							SetItemSize(num7, newItemByIndexInGroup4.CachedRectTransform.rect.height,
								newItemByIndexInGroup4.Padding);
						}

						mItemList.Add(newItemByIndexInGroup4);
						float y3 = loopStaggeredGridViewItem2.CachedRectTransform.anchoredPosition3D.y +
						           loopStaggeredGridViewItem2.CachedRectTransform.rect.height +
						           loopStaggeredGridViewItem2.Padding;
						newItemByIndexInGroup4.CachedRectTransform.anchoredPosition3D =
							new Vector3(newItemByIndexInGroup4.StartPosOffset, y3, 0f);
						CheckIfNeedUpdateItemPos();
						if (num7 > mCurReadyMaxItemIndex)
						{
							mCurReadyMaxItemIndex = num7;
						}

						return true;
					}
				}
			}

			return false;
		}


		public bool UpdateForHorizontalListPart1(float distanceForRecycle0, float distanceForRecycle1,
			float distanceForNew0, float distanceForNew1)
		{
			if (mItemTotalCount == 0)
			{
				if (mItemList.Count > 0)
				{
					RecycleAllItem();
				}

				return false;
			}

			if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				if (mItemList.Count == 0)
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

					LoopStaggeredGridViewItem newItemByIndexInGroup = GetNewItemByIndexInGroup(num2);
					if (newItemByIndexInGroup == null)
					{
						return false;
					}

					if (mSupportScrollBar)
					{
						SetItemSize(num2, newItemByIndexInGroup.CachedRectTransform.rect.width,
							newItemByIndexInGroup.Padding);
					}

					mItemList.Add(newItemByIndexInGroup);
					newItemByIndexInGroup.CachedRectTransform.anchoredPosition3D =
						new Vector3(x, newItemByIndexInGroup.StartPosOffset, 0f);
					return true;
				}

				LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[0];
				loopStaggeredGridViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector2 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				if (!IsDraging && loopStaggeredGridViewItem.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    mViewPortRectLocalCorners[1].x - vector2.x > distanceForRecycle0)
				{
					mItemList.RemoveAt(0);
					RecycleItemTmp(loopStaggeredGridViewItem);
					if (!mSupportScrollBar)
					{
						CheckIfNeedUpdateItemPos();
					}

					return true;
				}

				LoopStaggeredGridViewItem loopStaggeredGridViewItem2 = mItemList[mItemList.Count - 1];
				loopStaggeredGridViewItem2.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector3 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector4 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				if (!IsDraging && loopStaggeredGridViewItem2.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    vector3.x - mViewPortRectLocalCorners[2].x > distanceForRecycle1)
				{
					mItemList.RemoveAt(mItemList.Count - 1);
					RecycleItemTmp(loopStaggeredGridViewItem2);
					if (!mSupportScrollBar)
					{
						CheckIfNeedUpdateItemPos();
					}

					return true;
				}

				if (mViewPortRectLocalCorners[1].x - vector.x < distanceForNew0)
				{
					if (loopStaggeredGridViewItem.ItemIndexInGroup < mCurReadyMinItemIndex)
					{
						mCurReadyMinItemIndex = loopStaggeredGridViewItem.ItemIndexInGroup;
						mNeedCheckNextMinItem = true;
					}

					int num3 = loopStaggeredGridViewItem.ItemIndexInGroup - 1;
					if (num3 >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup2 = GetNewItemByIndexInGroup(num3);
						if (!(newItemByIndexInGroup2 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num3, newItemByIndexInGroup2.CachedRectTransform.rect.width,
									newItemByIndexInGroup2.Padding);
							}

							mItemList.Insert(0, newItemByIndexInGroup2);
							float x2 = loopStaggeredGridViewItem.CachedRectTransform.anchoredPosition3D.x -
							           newItemByIndexInGroup2.CachedRectTransform.rect.width -
							           newItemByIndexInGroup2.Padding;
							newItemByIndexInGroup2.CachedRectTransform.anchoredPosition3D =
								new Vector3(x2, newItemByIndexInGroup2.StartPosOffset, 0f);
							CheckIfNeedUpdateItemPos();
							if (num3 < mCurReadyMinItemIndex)
							{
								mCurReadyMinItemIndex = num3;
							}

							return true;
						}

						mCurReadyMinItemIndex = loopStaggeredGridViewItem.ItemIndexInGroup;
						mNeedCheckNextMinItem = false;
					}
				}

				if (vector4.x - mViewPortRectLocalCorners[2].x < distanceForNew1)
				{
					if (loopStaggeredGridViewItem2.ItemIndexInGroup > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopStaggeredGridViewItem2.ItemIndexInGroup;
						mNeedCheckNextMaxItem = true;
					}

					int num4 = loopStaggeredGridViewItem2.ItemIndexInGroup + 1;
					if (num4 >= ItemIndexMap.Count)
					{
						return false;
					}

					if (num4 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup3 = GetNewItemByIndexInGroup(num4);
						if (!(newItemByIndexInGroup3 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num4, newItemByIndexInGroup3.CachedRectTransform.rect.width,
									newItemByIndexInGroup3.Padding);
							}

							mItemList.Add(newItemByIndexInGroup3);
							float x3 = loopStaggeredGridViewItem2.CachedRectTransform.anchoredPosition3D.x +
							           loopStaggeredGridViewItem2.CachedRectTransform.rect.width +
							           loopStaggeredGridViewItem2.Padding;
							newItemByIndexInGroup3.CachedRectTransform.anchoredPosition3D =
								new Vector3(x3, newItemByIndexInGroup3.StartPosOffset, 0f);
							CheckIfNeedUpdateItemPos();
							if (num4 > mCurReadyMaxItemIndex)
							{
								mCurReadyMaxItemIndex = num4;
							}

							return true;
						}

						mCurReadyMaxItemIndex = loopStaggeredGridViewItem2.ItemIndexInGroup;
						mNeedCheckNextMaxItem = false;
						CheckIfNeedUpdateItemPos();
					}
				}
			}
			else if (mItemList.Count == 0)
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

				LoopStaggeredGridViewItem newItemByIndexInGroup4 = GetNewItemByIndexInGroup(num6);
				if (newItemByIndexInGroup4 == null)
				{
					return false;
				}

				if (mSupportScrollBar)
				{
					SetItemSize(num6, newItemByIndexInGroup4.CachedRectTransform.rect.width,
						newItemByIndexInGroup4.Padding);
				}

				mItemList.Add(newItemByIndexInGroup4);
				newItemByIndexInGroup4.CachedRectTransform.anchoredPosition3D =
					new Vector3(num7, newItemByIndexInGroup4.StartPosOffset, 0f);
				return true;
			}
			else
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem3 = mItemList[0];
				loopStaggeredGridViewItem3.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector5 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector6 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				if (!IsDraging && loopStaggeredGridViewItem3.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    vector5.x - mViewPortRectLocalCorners[2].x > distanceForRecycle0)
				{
					mItemList.RemoveAt(0);
					RecycleItemTmp(loopStaggeredGridViewItem3);
					if (!mSupportScrollBar)
					{
						CheckIfNeedUpdateItemPos();
					}

					return true;
				}

				LoopStaggeredGridViewItem loopStaggeredGridViewItem4 = mItemList[mItemList.Count - 1];
				loopStaggeredGridViewItem4.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector7 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				Vector3 vector8 = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]);
				if (!IsDraging && loopStaggeredGridViewItem4.ItemCreatedCheckFrameCount != mListUpdateCheckFrameCount &&
				    mViewPortRectLocalCorners[1].x - vector8.x > distanceForRecycle1)
				{
					mItemList.RemoveAt(mItemList.Count - 1);
					RecycleItemTmp(loopStaggeredGridViewItem4);
					if (!mSupportScrollBar)
					{
						CheckIfNeedUpdateItemPos();
					}

					return true;
				}

				if (vector6.x - mViewPortRectLocalCorners[2].x < distanceForNew0)
				{
					if (loopStaggeredGridViewItem3.ItemIndexInGroup < mCurReadyMinItemIndex)
					{
						mCurReadyMinItemIndex = loopStaggeredGridViewItem3.ItemIndexInGroup;
						mNeedCheckNextMinItem = true;
					}

					int num8 = loopStaggeredGridViewItem3.ItemIndexInGroup - 1;
					if (num8 >= mCurReadyMinItemIndex || mNeedCheckNextMinItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup5 = GetNewItemByIndexInGroup(num8);
						if (!(newItemByIndexInGroup5 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num8, newItemByIndexInGroup5.CachedRectTransform.rect.width,
									newItemByIndexInGroup5.Padding);
							}

							mItemList.Insert(0, newItemByIndexInGroup5);
							float x4 = loopStaggeredGridViewItem3.CachedRectTransform.anchoredPosition3D.x +
							           newItemByIndexInGroup5.CachedRectTransform.rect.width +
							           newItemByIndexInGroup5.Padding;
							newItemByIndexInGroup5.CachedRectTransform.anchoredPosition3D =
								new Vector3(x4, newItemByIndexInGroup5.StartPosOffset, 0f);
							CheckIfNeedUpdateItemPos();
							if (num8 < mCurReadyMinItemIndex)
							{
								mCurReadyMinItemIndex = num8;
							}

							return true;
						}

						mCurReadyMinItemIndex = loopStaggeredGridViewItem3.ItemIndexInGroup;
						mNeedCheckNextMinItem = false;
					}
				}

				if (mViewPortRectLocalCorners[1].x - vector7.x < distanceForNew1)
				{
					if (loopStaggeredGridViewItem4.ItemIndexInGroup > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopStaggeredGridViewItem4.ItemIndexInGroup;
						mNeedCheckNextMaxItem = true;
					}

					int num9 = loopStaggeredGridViewItem4.ItemIndexInGroup + 1;
					if (num9 >= ItemIndexMap.Count)
					{
						return false;
					}

					if (num9 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup6 = GetNewItemByIndexInGroup(num9);
						if (!(newItemByIndexInGroup6 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num9, newItemByIndexInGroup6.CachedRectTransform.rect.width,
									newItemByIndexInGroup6.Padding);
							}

							mItemList.Add(newItemByIndexInGroup6);
							float x5 = loopStaggeredGridViewItem4.CachedRectTransform.anchoredPosition3D.x -
							           loopStaggeredGridViewItem4.CachedRectTransform.rect.width -
							           loopStaggeredGridViewItem4.Padding;
							newItemByIndexInGroup6.CachedRectTransform.anchoredPosition3D =
								new Vector3(x5, newItemByIndexInGroup6.StartPosOffset, 0f);
							CheckIfNeedUpdateItemPos();
							if (num9 > mCurReadyMaxItemIndex)
							{
								mCurReadyMaxItemIndex = num9;
							}

							return true;
						}

						mCurReadyMaxItemIndex = loopStaggeredGridViewItem4.ItemIndexInGroup;
						mNeedCheckNextMaxItem = false;
						CheckIfNeedUpdateItemPos();
					}
				}
			}

			return false;
		}


		public bool UpdateForHorizontalListPart2(float distanceForRecycle0, float distanceForRecycle1,
			float distanceForNew0, float distanceForNew1)
		{
			if (mItemTotalCount == 0)
			{
				if (mItemList.Count > 0)
				{
					RecycleAllItem();
				}

				return false;
			}

			if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				if (mItemList.Count == 0)
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

					LoopStaggeredGridViewItem newItemByIndexInGroup = GetNewItemByIndexInGroup(num2);
					if (newItemByIndexInGroup == null)
					{
						return false;
					}

					if (mSupportScrollBar)
					{
						SetItemSize(num2, newItemByIndexInGroup.CachedRectTransform.rect.width,
							newItemByIndexInGroup.Padding);
					}

					mItemList.Add(newItemByIndexInGroup);
					newItemByIndexInGroup.CachedRectTransform.anchoredPosition3D =
						new Vector3(x, newItemByIndexInGroup.StartPosOffset, 0f);
					return true;
				}

				LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[mItemList.Count - 1];
				loopStaggeredGridViewItem.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				if (mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[2]).x -
					mViewPortRectLocalCorners[2].x < distanceForNew1)
				{
					if (loopStaggeredGridViewItem.ItemIndexInGroup > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopStaggeredGridViewItem.ItemIndexInGroup;
						mNeedCheckNextMaxItem = true;
					}

					int num3 = loopStaggeredGridViewItem.ItemIndexInGroup + 1;
					if (num3 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup2 = GetNewItemByIndexInGroup(num3);
						if (!(newItemByIndexInGroup2 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num3, newItemByIndexInGroup2.CachedRectTransform.rect.width,
									newItemByIndexInGroup2.Padding);
							}

							mItemList.Add(newItemByIndexInGroup2);
							float x2 = loopStaggeredGridViewItem.CachedRectTransform.anchoredPosition3D.x +
							           loopStaggeredGridViewItem.CachedRectTransform.rect.width +
							           loopStaggeredGridViewItem.Padding;
							newItemByIndexInGroup2.CachedRectTransform.anchoredPosition3D =
								new Vector3(x2, newItemByIndexInGroup2.StartPosOffset, 0f);
							CheckIfNeedUpdateItemPos();
							if (num3 > mCurReadyMaxItemIndex)
							{
								mCurReadyMaxItemIndex = num3;
							}

							return true;
						}

						mCurReadyMaxItemIndex = loopStaggeredGridViewItem.ItemIndexInGroup;
						mNeedCheckNextMaxItem = false;
						CheckIfNeedUpdateItemPos();
					}
				}
			}
			else if (mItemList.Count == 0)
			{
				float num4 = mContainerTrans.anchoredPosition3D.x;
				if (num4 < 0f)
				{
					num4 = 0f;
				}

				int num5 = 0;
				float num6 = -num4;
				if (mSupportScrollBar)
				{
					if (!GetPlusItemIndexAndPosAtGivenPos(num4, ref num5, ref num6))
					{
						return false;
					}

					num6 = -num6;
				}

				LoopStaggeredGridViewItem newItemByIndexInGroup3 = GetNewItemByIndexInGroup(num5);
				if (newItemByIndexInGroup3 == null)
				{
					return false;
				}

				if (mSupportScrollBar)
				{
					SetItemSize(num5, newItemByIndexInGroup3.CachedRectTransform.rect.width,
						newItemByIndexInGroup3.Padding);
				}

				mItemList.Add(newItemByIndexInGroup3);
				newItemByIndexInGroup3.CachedRectTransform.anchoredPosition3D =
					new Vector3(num6, newItemByIndexInGroup3.StartPosOffset, 0f);
				return true;
			}
			else
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem2 = mItemList[mItemList.Count - 1];
				loopStaggeredGridViewItem2.CachedRectTransform.GetWorldCorners(mItemWorldCorners);
				Vector3 vector = mViewPortRectTransform.InverseTransformPoint(mItemWorldCorners[1]);
				if (mViewPortRectLocalCorners[1].x - vector.x < distanceForNew1)
				{
					if (loopStaggeredGridViewItem2.ItemIndexInGroup > mCurReadyMaxItemIndex)
					{
						mCurReadyMaxItemIndex = loopStaggeredGridViewItem2.ItemIndexInGroup;
						mNeedCheckNextMaxItem = true;
					}

					int num7 = loopStaggeredGridViewItem2.ItemIndexInGroup + 1;
					if (num7 <= mCurReadyMaxItemIndex || mNeedCheckNextMaxItem)
					{
						LoopStaggeredGridViewItem newItemByIndexInGroup4 = GetNewItemByIndexInGroup(num7);
						if (!(newItemByIndexInGroup4 == null))
						{
							if (mSupportScrollBar)
							{
								SetItemSize(num7, newItemByIndexInGroup4.CachedRectTransform.rect.width,
									newItemByIndexInGroup4.Padding);
							}

							mItemList.Add(newItemByIndexInGroup4);
							float x3 = loopStaggeredGridViewItem2.CachedRectTransform.anchoredPosition3D.x -
							           loopStaggeredGridViewItem2.CachedRectTransform.rect.width -
							           loopStaggeredGridViewItem2.Padding;
							newItemByIndexInGroup4.CachedRectTransform.anchoredPosition3D =
								new Vector3(x3, newItemByIndexInGroup4.StartPosOffset, 0f);
							CheckIfNeedUpdateItemPos();
							if (num7 > mCurReadyMaxItemIndex)
							{
								mCurReadyMaxItemIndex = num7;
							}

							return true;
						}

						mCurReadyMaxItemIndex = loopStaggeredGridViewItem2.ItemIndexInGroup;
						mNeedCheckNextMaxItem = false;
						CheckIfNeedUpdateItemPos();
					}
				}
			}

			return false;
		}


		public float GetContentPanelSize()
		{
			float num = mItemPosMgr.mTotalSize > 0f ? mItemPosMgr.mTotalSize - mLastItemPadding : 0f;
			if (num < 0f)
			{
				num = 0f;
			}

			return num;
		}


		public float GetShownItemPosMaxValue()
		{
			if (mItemList.Count == 0)
			{
				return 0f;
			}

			LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[mItemList.Count - 1];
			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				return Mathf.Abs(loopStaggeredGridViewItem.BottomY);
			}

			if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				return Mathf.Abs(loopStaggeredGridViewItem.TopY);
			}

			if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				return Mathf.Abs(loopStaggeredGridViewItem.RightX);
			}

			if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				return Mathf.Abs(loopStaggeredGridViewItem.LeftX);
			}

			return 0f;
		}


		public void CheckIfNeedUpdateItemPos()
		{
			if (mItemList.Count == 0)
			{
				return;
			}

			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[0];
				LoopStaggeredGridViewItem loopStaggeredGridViewItem2 = mItemList[mItemList.Count - 1];
				if (loopStaggeredGridViewItem.TopY > 0f ||
				    loopStaggeredGridViewItem.ItemIndexInGroup == mCurReadyMinItemIndex &&
				    loopStaggeredGridViewItem.TopY != 0f)
				{
					UpdateAllShownItemsPos();
					return;
				}

				float contentPanelSize = GetContentPanelSize();
				if (-loopStaggeredGridViewItem2.BottomY > contentPanelSize ||
				    loopStaggeredGridViewItem2.ItemIndexInGroup == mCurReadyMaxItemIndex &&
				    -loopStaggeredGridViewItem2.BottomY != contentPanelSize)
				{
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem3 = mItemList[0];
				LoopStaggeredGridViewItem loopStaggeredGridViewItem4 = mItemList[mItemList.Count - 1];
				if (loopStaggeredGridViewItem3.BottomY < 0f ||
				    loopStaggeredGridViewItem3.ItemIndexInGroup == mCurReadyMinItemIndex &&
				    loopStaggeredGridViewItem3.BottomY != 0f)
				{
					UpdateAllShownItemsPos();
					return;
				}

				float contentPanelSize2 = GetContentPanelSize();
				if (loopStaggeredGridViewItem4.TopY > contentPanelSize2 ||
				    loopStaggeredGridViewItem4.ItemIndexInGroup == mCurReadyMaxItemIndex &&
				    loopStaggeredGridViewItem4.TopY != contentPanelSize2)
				{
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem5 = mItemList[0];
				LoopStaggeredGridViewItem loopStaggeredGridViewItem6 = mItemList[mItemList.Count - 1];
				if (loopStaggeredGridViewItem5.LeftX < 0f ||
				    loopStaggeredGridViewItem5.ItemIndexInGroup == mCurReadyMinItemIndex &&
				    loopStaggeredGridViewItem5.LeftX != 0f)
				{
					UpdateAllShownItemsPos();
					return;
				}

				float contentPanelSize3 = GetContentPanelSize();
				if (loopStaggeredGridViewItem6.RightX > contentPanelSize3 ||
				    loopStaggeredGridViewItem6.ItemIndexInGroup == mCurReadyMaxItemIndex &&
				    loopStaggeredGridViewItem6.RightX != contentPanelSize3)
				{
					UpdateAllShownItemsPos();
				}
			}
			else if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				LoopStaggeredGridViewItem loopStaggeredGridViewItem7 = mItemList[0];
				LoopStaggeredGridViewItem loopStaggeredGridViewItem8 = mItemList[mItemList.Count - 1];
				if (loopStaggeredGridViewItem7.RightX > 0f ||
				    loopStaggeredGridViewItem7.ItemIndexInGroup == mCurReadyMinItemIndex &&
				    loopStaggeredGridViewItem7.RightX != 0f)
				{
					UpdateAllShownItemsPos();
					return;
				}

				float contentPanelSize4 = GetContentPanelSize();
				if (-loopStaggeredGridViewItem8.LeftX > contentPanelSize4 ||
				    loopStaggeredGridViewItem8.ItemIndexInGroup == mCurReadyMaxItemIndex &&
				    -loopStaggeredGridViewItem8.LeftX != contentPanelSize4)
				{
					UpdateAllShownItemsPos();
				}
			}
		}


		public void UpdateAllShownItemsPos()
		{
			int count = mItemList.Count;
			if (count == 0)
			{
				return;
			}

			if (mArrangeType == ListItemArrangeType.TopToBottom)
			{
				float num = 0f;
				if (mSupportScrollBar)
				{
					num = -GetItemPos(mItemList[0].ItemIndexInGroup);
				}

				float num2 = num;
				for (int i = 0; i < count; i++)
				{
					LoopStaggeredGridViewItem loopStaggeredGridViewItem = mItemList[i];
					loopStaggeredGridViewItem.CachedRectTransform.anchoredPosition3D =
						new Vector3(loopStaggeredGridViewItem.StartPosOffset, num2, 0f);
					num2 = num2 - loopStaggeredGridViewItem.CachedRectTransform.rect.height -
					       loopStaggeredGridViewItem.Padding;
				}

				return;
			}

			if (mArrangeType == ListItemArrangeType.BottomToTop)
			{
				float num3 = 0f;
				if (mSupportScrollBar)
				{
					num3 = GetItemPos(mItemList[0].ItemIndexInGroup);
				}

				float num4 = num3;
				for (int j = 0; j < count; j++)
				{
					LoopStaggeredGridViewItem loopStaggeredGridViewItem2 = mItemList[j];
					loopStaggeredGridViewItem2.CachedRectTransform.anchoredPosition3D =
						new Vector3(loopStaggeredGridViewItem2.StartPosOffset, num4, 0f);
					num4 = num4 + loopStaggeredGridViewItem2.CachedRectTransform.rect.height +
					       loopStaggeredGridViewItem2.Padding;
				}

				return;
			}

			if (mArrangeType == ListItemArrangeType.LeftToRight)
			{
				float num5 = 0f;
				if (mSupportScrollBar)
				{
					num5 = GetItemPos(mItemList[0].ItemIndexInGroup);
				}

				float num6 = num5;
				for (int k = 0; k < count; k++)
				{
					LoopStaggeredGridViewItem loopStaggeredGridViewItem3 = mItemList[k];
					loopStaggeredGridViewItem3.CachedRectTransform.anchoredPosition3D =
						new Vector3(num6, loopStaggeredGridViewItem3.StartPosOffset, 0f);
					num6 = num6 + loopStaggeredGridViewItem3.CachedRectTransform.rect.width +
					       loopStaggeredGridViewItem3.Padding;
				}

				return;
			}

			if (mArrangeType == ListItemArrangeType.RightToLeft)
			{
				float num7 = 0f;
				if (mSupportScrollBar)
				{
					num7 = -GetItemPos(mItemList[0].ItemIndexInGroup);
				}

				float num8 = num7;
				for (int l = 0; l < count; l++)
				{
					LoopStaggeredGridViewItem loopStaggeredGridViewItem4 = mItemList[l];
					loopStaggeredGridViewItem4.CachedRectTransform.anchoredPosition3D =
						new Vector3(num8, loopStaggeredGridViewItem4.StartPosOffset, 0f);
					num8 = num8 - loopStaggeredGridViewItem4.CachedRectTransform.rect.width -
					       loopStaggeredGridViewItem4.Padding;
				}
			}
		}
	}
}