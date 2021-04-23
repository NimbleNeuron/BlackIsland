using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class TreeViewDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		private readonly TreeViewItemCountMgr mTreeItemCountMgr = new TreeViewItemCountMgr();


		private Button mBackButton;


		private Button mCollapseAllButton;


		private Button mExpandAllButton;


		private Button mScrollToButton;


		private InputField mScrollToInputChild;


		private InputField mScrollToInputItem;


		private void Start()
		{
			int treeViewItemCount = TreeViewDataSourceMgr.Get.TreeViewItemCount;
			for (int i = 0; i < treeViewItemCount; i++)
			{
				int childCount = TreeViewDataSourceMgr.Get.GetItemDataByIndex(i).ChildCount;
				mTreeItemCountMgr.AddTreeItem(childCount, true);
			}

			mLoopListView.InitListView(mTreeItemCountMgr.GetTotalItemAndChildCount(), OnGetItemByIndex);
			mExpandAllButton = GameObject.Find("ButtonPanel/buttonGroup1/ExpandAllButton").GetComponent<Button>();
			mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
			mCollapseAllButton = GameObject.Find("ButtonPanel/buttonGroup3/CollapseAllButton").GetComponent<Button>();
			mScrollToInputItem = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputFieldItem")
				.GetComponent<InputField>();
			mScrollToInputChild = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputFieldChild")
				.GetComponent<InputField>();
			mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
			mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
			mBackButton.onClick.AddListener(OnBackBtnClicked);
			mExpandAllButton.onClick.AddListener(OnExpandAllBtnClicked);
			mCollapseAllButton.onClick.AddListener(OnCollapseAllBtnClicked);
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

			TreeViewItemCountData treeViewItemCountData = mTreeItemCountMgr.QueryTreeItemByTotalIndex(index);
			if (treeViewItemCountData == null)
			{
				return null;
			}

			int mTreeItemIndex = treeViewItemCountData.mTreeItemIndex;
			TreeViewItemData itemDataByIndex = TreeViewDataSourceMgr.Get.GetItemDataByIndex(mTreeItemIndex);
			if (!treeViewItemCountData.IsChild(index))
			{
				LoopListViewItem2 loopListViewItem = listView.NewListViewItem("ItemPrefab1");
				ListItem12 component = loopListViewItem.GetComponent<ListItem12>();
				if (!loopListViewItem.IsInitHandlerCalled)
				{
					loopListViewItem.IsInitHandlerCalled = true;
					component.Init();
					component.SetClickCallBack(OnExpandClicked);
				}

				component.mText.text = itemDataByIndex.mName;
				component.SetItemData(mTreeItemIndex, treeViewItemCountData.mIsExpand);
				return loopListViewItem;
			}

			int childIndex = treeViewItemCountData.GetChildIndex(index);
			ItemData child = itemDataByIndex.GetChild(childIndex);
			if (child == null)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem2 = listView.NewListViewItem("ItemPrefab2");
			ListItem13 component2 = loopListViewItem2.GetComponent<ListItem13>();
			if (!loopListViewItem2.IsInitHandlerCalled)
			{
				loopListViewItem2.IsInitHandlerCalled = true;
				component2.Init();
			}

			component2.SetItemData(child, mTreeItemIndex, childIndex);
			return loopListViewItem2;
		}


		public void OnExpandClicked(int index)
		{
			mTreeItemCountMgr.ToggleItemExpand(index);
			mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
			mLoopListView.RefreshAllShownItem();
		}


		private void OnJumpBtnClicked()
		{
			int treeIndex = 0;
			int num = 0;
			if (!int.TryParse(mScrollToInputItem.text, out treeIndex))
			{
				return;
			}

			if (!int.TryParse(mScrollToInputChild.text, out num))
			{
				num = 0;
			}

			if (num < 0)
			{
				num = 0;
			}

			TreeViewItemCountData treeItem = mTreeItemCountMgr.GetTreeItem(treeIndex);
			if (treeItem == null)
			{
				return;
			}

			int mChildCount = treeItem.mChildCount;
			int itemIndex;
			if (!treeItem.mIsExpand || mChildCount == 0 || num == 0)
			{
				itemIndex = treeItem.mBeginIndex;
			}
			else
			{
				if (num > mChildCount)
				{
					num = mChildCount;
				}

				if (num < 1)
				{
					num = 1;
				}

				itemIndex = treeItem.mBeginIndex + num;
			}

			mLoopListView.MovePanelToItemIndex(itemIndex, 0f);
		}


		private void OnExpandAllBtnClicked()
		{
			int treeViewItemCount = mTreeItemCountMgr.TreeViewItemCount;
			for (int i = 0; i < treeViewItemCount; i++)
			{
				mTreeItemCountMgr.SetItemExpand(i, true);
			}

			mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
			mLoopListView.RefreshAllShownItem();
		}


		private void OnCollapseAllBtnClicked()
		{
			int treeViewItemCount = mTreeItemCountMgr.TreeViewItemCount;
			for (int i = 0; i < treeViewItemCount; i++)
			{
				mTreeItemCountMgr.SetItemExpand(i, false);
			}

			mLoopListView.SetListItemCount(mTreeItemCountMgr.GetTotalItemAndChildCount(), false);
			mLoopListView.RefreshAllShownItem();
		}
	}
}