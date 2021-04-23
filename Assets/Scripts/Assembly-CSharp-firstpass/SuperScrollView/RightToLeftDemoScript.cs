using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class RightToLeftDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		private Button mAddItemButton;


		private InputField mAddItemInput;


		private Button mBackButton;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private Button mSetCountButton;


		private InputField mSetCountInput;


		private void Start()
		{
			mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);
			mSetCountButton = GameObject.Find("ButtonPanel/buttonGroup1/SetCountButton").GetComponent<Button>();
			mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
			mAddItemButton = GameObject.Find("ButtonPanel/buttonGroup3/AddItemButton").GetComponent<Button>();
			mSetCountInput = GameObject.Find("ButtonPanel/buttonGroup1/SetCountInputField").GetComponent<InputField>();
			mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
			mAddItemInput = GameObject.Find("ButtonPanel/buttonGroup3/AddItemInputField").GetComponent<InputField>();
			mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
			mAddItemButton.onClick.AddListener(OnAddItemBtnClicked);
			mSetCountButton.onClick.AddListener(OnSetItemCountBtnClicked);
			mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
			mBackButton.onClick.AddListener(OnBackBtnClicked);
		}


		private void OnBackBtnClicked()
		{
			SceneManager.LoadScene("Menu");
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0 || index > DataSourceMgr.Get.TotalItemCount)
			{
				return null;
			}

			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(index);
			if (itemDataByIndex == null)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("ItemPrefab1");
			ListItem5 component = loopListViewItem.GetComponent<ListItem5>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			component.SetItemData(itemDataByIndex, index);
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


		private void OnAddItemBtnClicked()
		{
			if (mLoopListView.ItemTotalCount < 0)
			{
				return;
			}

			int num = 0;
			if (!int.TryParse(mAddItemInput.text, out num))
			{
				return;
			}

			num = mLoopListView.ItemTotalCount + num;
			if (num < 0 || num > DataSourceMgr.Get.TotalItemCount)
			{
				return;
			}

			mLoopListView.SetListItemCount(num, false);
		}


		private void OnSetItemCountBtnClicked()
		{
			int num = 0;
			if (!int.TryParse(mSetCountInput.text, out num))
			{
				return;
			}

			if (num < 0 || num > DataSourceMgr.Get.TotalItemCount)
			{
				return;
			}

			mLoopListView.SetListItemCount(num, false);
		}
	}
}