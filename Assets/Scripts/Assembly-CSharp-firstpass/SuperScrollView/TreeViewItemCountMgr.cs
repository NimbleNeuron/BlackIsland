using System.Collections.Generic;

namespace SuperScrollView
{
	public class TreeViewItemCountMgr
	{
		private readonly List<TreeViewItemCountData> mTreeItemDataList = new List<TreeViewItemCountData>();

		private bool mIsDirty = true;


		private TreeViewItemCountData mLastQueryResult;


		public int TreeViewItemCount => mTreeItemDataList.Count;


		public void AddTreeItem(int count, bool isExpand)
		{
			TreeViewItemCountData treeViewItemCountData = new TreeViewItemCountData();
			treeViewItemCountData.mTreeItemIndex = mTreeItemDataList.Count;
			treeViewItemCountData.mChildCount = count;
			treeViewItemCountData.mIsExpand = isExpand;
			mTreeItemDataList.Add(treeViewItemCountData);
			mIsDirty = true;
		}


		public void Clear()
		{
			mTreeItemDataList.Clear();
			mLastQueryResult = null;
			mIsDirty = true;
		}


		public TreeViewItemCountData GetTreeItem(int treeIndex)
		{
			if (treeIndex < 0 || treeIndex >= mTreeItemDataList.Count)
			{
				return null;
			}

			return mTreeItemDataList[treeIndex];
		}


		public void SetItemChildCount(int treeIndex, int count)
		{
			if (treeIndex < 0 || treeIndex >= mTreeItemDataList.Count)
			{
				return;
			}

			mIsDirty = true;
			mTreeItemDataList[treeIndex].mChildCount = count;
		}


		public void SetItemExpand(int treeIndex, bool isExpand)
		{
			if (treeIndex < 0 || treeIndex >= mTreeItemDataList.Count)
			{
				return;
			}

			mIsDirty = true;
			mTreeItemDataList[treeIndex].mIsExpand = isExpand;
		}


		public void ToggleItemExpand(int treeIndex)
		{
			if (treeIndex < 0 || treeIndex >= mTreeItemDataList.Count)
			{
				return;
			}

			mIsDirty = true;
			TreeViewItemCountData treeViewItemCountData = mTreeItemDataList[treeIndex];
			treeViewItemCountData.mIsExpand = !treeViewItemCountData.mIsExpand;
		}


		public bool IsTreeItemExpand(int treeIndex)
		{
			TreeViewItemCountData treeItem = GetTreeItem(treeIndex);
			return treeItem != null && treeItem.mIsExpand;
		}


		private void UpdateAllTreeItemDataIndex()
		{
			if (!mIsDirty)
			{
				return;
			}

			mLastQueryResult = null;
			mIsDirty = false;
			int count = mTreeItemDataList.Count;
			if (count == 0)
			{
				return;
			}

			TreeViewItemCountData treeViewItemCountData = mTreeItemDataList[0];
			treeViewItemCountData.mBeginIndex = 0;
			treeViewItemCountData.mEndIndex = treeViewItemCountData.mIsExpand ? treeViewItemCountData.mChildCount : 0;
			int mEndIndex = treeViewItemCountData.mEndIndex;
			for (int i = 1; i < count; i++)
			{
				TreeViewItemCountData treeViewItemCountData2 = mTreeItemDataList[i];
				treeViewItemCountData2.mBeginIndex = mEndIndex + 1;
				treeViewItemCountData2.mEndIndex = treeViewItemCountData2.mBeginIndex +
				                                   (treeViewItemCountData2.mIsExpand
					                                   ? treeViewItemCountData2.mChildCount
					                                   : 0);
				mEndIndex = treeViewItemCountData2.mEndIndex;
			}
		}


		public int GetTotalItemAndChildCount()
		{
			int count = mTreeItemDataList.Count;
			if (count == 0)
			{
				return 0;
			}

			UpdateAllTreeItemDataIndex();
			return mTreeItemDataList[count - 1].mEndIndex + 1;
		}


		public TreeViewItemCountData QueryTreeItemByTotalIndex(int totalIndex)
		{
			if (totalIndex < 0)
			{
				return null;
			}

			int count = mTreeItemDataList.Count;
			if (count == 0)
			{
				return null;
			}

			UpdateAllTreeItemDataIndex();
			if (mLastQueryResult != null && mLastQueryResult.mBeginIndex <= totalIndex &&
			    mLastQueryResult.mEndIndex >= totalIndex)
			{
				return mLastQueryResult;
			}

			int i = 0;
			int num = count - 1;
			while (i <= num)
			{
				int num2 = (i + num) / 2;
				TreeViewItemCountData treeViewItemCountData = mTreeItemDataList[num2];
				if (treeViewItemCountData.mBeginIndex <= totalIndex && treeViewItemCountData.mEndIndex >= totalIndex)
				{
					mLastQueryResult = treeViewItemCountData;
					return treeViewItemCountData;
				}

				if (totalIndex > treeViewItemCountData.mEndIndex)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}

			return null;
		}
	}
}