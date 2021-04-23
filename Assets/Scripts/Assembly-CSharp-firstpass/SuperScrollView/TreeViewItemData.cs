using System.Collections.Generic;

namespace SuperScrollView
{
	public class TreeViewItemData
	{
		public List<ItemData> mChildItemDataList = new List<ItemData>();


		public string mIcon;


		public string mName;


		public int ChildCount => mChildItemDataList.Count;


		public void AddChild(ItemData data)
		{
			mChildItemDataList.Add(data);
		}


		public ItemData GetChild(int index)
		{
			if (index < 0 || index >= mChildItemDataList.Count)
			{
				return null;
			}

			return mChildItemDataList[index];
		}
	}
}