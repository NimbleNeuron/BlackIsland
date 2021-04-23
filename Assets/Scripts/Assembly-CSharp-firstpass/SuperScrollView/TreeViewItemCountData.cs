namespace SuperScrollView
{
	public class TreeViewItemCountData
	{
		public int mBeginIndex;


		public int mChildCount;


		public int mEndIndex;


		public bool mIsExpand = true;


		public int mTreeItemIndex;


		public bool IsChild(int index)
		{
			return index != mBeginIndex;
		}


		public int GetChildIndex(int index)
		{
			if (!IsChild(index))
			{
				return -1;
			}

			return index - mBeginIndex - 1;
		}
	}
}