using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class TopToBottomSampleDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		private Button mAppendItemButton;


		private List<CustomData> mDataList;


		private Button mInsertItemButton;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private int mTotalInsertedCount;


		private void Start()
		{
			InitData();
			mLoopListView.InitListView(mDataList.Count, OnGetItemByIndex);
			mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
			mAppendItemButton = GameObject.Find("ButtonPanel/buttonGroup3/AppendItemButton").GetComponent<Button>();
			mInsertItemButton = GameObject.Find("ButtonPanel/buttonGroup3/InsertItemButton").GetComponent<Button>();
			mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
			mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
			mAppendItemButton.onClick.AddListener(OnAppendItemBtnClicked);
			mInsertItemButton.onClick.AddListener(OnInsertItemBtnClicked);
		}


		private void InitData()
		{
			mDataList = new List<CustomData>();
			int num = 100;
			for (int i = 0; i < num; i++)
			{
				CustomData customData = new CustomData();
				customData.mContent = "Item" + i;
				mDataList.Add(customData);
			}
		}


		private void AppendOneData()
		{
			CustomData customData = new CustomData();
			customData.mContent = "Item" + mDataList.Count;
			mDataList.Add(customData);
		}


		private void InsertOneData()
		{
			mTotalInsertedCount++;
			CustomData customData = new CustomData();
			customData.mContent = "Item(-" + mTotalInsertedCount + ")";
			mDataList.Insert(0, customData);
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0 || index >= mDataList.Count)
			{
				return null;
			}

			CustomData customData = mDataList[index];
			if (customData == null)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("ItemPrefab1");
			ListItem16 component = loopListViewItem.GetComponent<ListItem16>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			component.mNameText.text = customData.mContent;
			return loopListViewItem;
		}


		private void OnJumpBtnClicked()
		{
			int itemIndex = 0;
			if (!int.TryParse(mScrollToInput.text, out itemIndex))
			{
				return;
			}

			mLoopListView.MovePanelToItemIndex(itemIndex, 0f);
		}


		private void OnAppendItemBtnClicked()
		{
			AppendOneData();
			mLoopListView.SetListItemCount(mDataList.Count, false);
			mLoopListView.RefreshAllShownItem();
		}


		private void OnInsertItemBtnClicked()
		{
			InsertOneData();
			mLoopListView.SetListItemCount(mDataList.Count, false);
			mLoopListView.RefreshAllShownItem();
		}
	}
}