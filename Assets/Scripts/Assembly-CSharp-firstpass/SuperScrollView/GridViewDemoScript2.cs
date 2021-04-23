using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class GridViewDemoScript2 : MonoBehaviour
	{
		public LoopGridView mLoopGridView;


		private Button mAddItemButton;


		private InputField mAddItemInput;


		private Button mBackButton;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private Button mSetCountButton;


		private InputField mSetCountInput;


		private void Start()
		{
			mLoopGridView.InitGridView(DataSourceMgr.Get.TotalItemCount, OnGetItemByRowColumn);
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


		private LoopGridViewItem OnGetItemByRowColumn(LoopGridView gridView, int itemIndex, int row, int column)
		{
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(itemIndex);
			if (itemDataByIndex == null)
			{
				return null;
			}

			LoopGridViewItem loopGridViewItem = gridView.NewListViewItem("ItemPrefab0");
			ListItem18 component = loopGridViewItem.GetComponent<ListItem18>();
			if (!loopGridViewItem.IsInitHandlerCalled)
			{
				loopGridViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			component.SetItemData(itemDataByIndex, itemIndex, row, column);
			return loopGridViewItem;
		}


		private void OnJumpBtnClicked()
		{
			int itemIndex = 0;
			if (!int.TryParse(mScrollToInput.text, out itemIndex))
			{
				return;
			}

			mLoopGridView.MovePanelToItemByIndex(itemIndex);
		}


		private void OnAddItemBtnClicked()
		{
			int num = 0;
			if (!int.TryParse(mAddItemInput.text, out num))
			{
				return;
			}

			mLoopGridView.SetListItemCount(num + mLoopGridView.ItemTotalCount, false);
		}


		private void OnSetItemCountBtnClicked()
		{
			int num = 0;
			if (!int.TryParse(mSetCountInput.text, out num))
			{
				return;
			}

			if (num > DataSourceMgr.Get.TotalItemCount)
			{
				num = DataSourceMgr.Get.TotalItemCount;
			}

			if (num < 0)
			{
				num = 0;
			}

			mLoopGridView.SetListItemCount(num);
		}
	}
}