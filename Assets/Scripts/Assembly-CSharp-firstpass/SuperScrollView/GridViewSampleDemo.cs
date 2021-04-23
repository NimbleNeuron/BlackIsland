using UnityEngine;

namespace SuperScrollView
{
	public class GridViewSampleDemo : MonoBehaviour
	{
		private const int mItemCountPerRow = 3;


		public LoopListView2 mLoopListView;


		private readonly int mItemTotalCount = 100;


		private void Start()
		{
			int num = mItemTotalCount / 3;
			if (mItemTotalCount % 3 > 0)
			{
				num++;
			}

			mLoopListView.InitListView(num, OnGetItemByIndex);
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int rowIndex)
		{
			if (rowIndex < 0)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("RowPrefab");
			ListItem15 component = loopListViewItem.GetComponent<ListItem15>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			for (int i = 0; i < 3; i++)
			{
				int num = rowIndex * 3 + i;
				if (num >= mItemTotalCount)
				{
					component.mItemList[i].gameObject.SetActive(false);
				}
				else
				{
					component.mItemList[i].gameObject.SetActive(true);
					component.mItemList[i].mNameText.text = "Item" + num;
				}
			}

			return loopListViewItem;
		}
	}
}