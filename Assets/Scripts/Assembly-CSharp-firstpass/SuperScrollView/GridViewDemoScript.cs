using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class GridViewDemoScript : MonoBehaviour
	{
		private const int mItemCountPerRow = 3;


		public LoopListView2 mLoopListView;


		private Button mAddItemButton;


		private InputField mAddItemInput;


		private Button mBackButton;


		private int mListItemTotalCount;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private Button mSetCountButton;


		private InputField mSetCountInput;


		private void Start()
		{
			mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
			int num = mListItemTotalCount / 3;
			if (mListItemTotalCount % 3 > 0)
			{
				num++;
			}

			mLoopListView.InitListView(num, OnGetItemByIndex);
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


		private void SetListItemTotalCount(int count)
		{
			mListItemTotalCount = count;
			if (mListItemTotalCount < 0)
			{
				mListItemTotalCount = 0;
			}

			if (mListItemTotalCount > DataSourceMgr.Get.TotalItemCount)
			{
				mListItemTotalCount = DataSourceMgr.Get.TotalItemCount;
			}

			int num = mListItemTotalCount / 3;
			if (mListItemTotalCount % 3 > 0)
			{
				num++;
			}

			mLoopListView.SetListItemCount(num, false);
			mLoopListView.RefreshAllShownItem();
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem = listView.NewListViewItem("ItemPrefab1");
			ListItem6 component = loopListViewItem.GetComponent<ListItem6>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			for (int i = 0; i < 3; i++)
			{
				int num = index * 3 + i;
				if (num >= mListItemTotalCount)
				{
					component.mItemList[i].gameObject.SetActive(false);
				}
				else
				{
					ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(num);
					if (itemDataByIndex != null)
					{
						component.mItemList[i].gameObject.SetActive(true);
						component.mItemList[i].SetItemData(itemDataByIndex, num);
					}
					else
					{
						component.mItemList[i].gameObject.SetActive(false);
					}
				}
			}

			return loopListViewItem;
		}


		private void OnJumpBtnClicked()
		{
			int num = 0;
			if (!int.TryParse(mScrollToInput.text, out num))
			{
				return;
			}

			if (num < 0)
			{
				num = 0;
			}

			num++;
			int num2 = num / 3;
			if (num % 3 > 0)
			{
				num2++;
			}

			if (num2 > 0)
			{
				num2--;
			}

			mLoopListView.MovePanelToItemIndex(num2, 0f);
		}


		private void OnAddItemBtnClicked()
		{
			int num = 0;
			if (!int.TryParse(mAddItemInput.text, out num))
			{
				return;
			}

			SetListItemTotalCount(mListItemTotalCount + num);
		}


		private void OnSetItemCountBtnClicked()
		{
			int listItemTotalCount = 0;
			if (!int.TryParse(mSetCountInput.text, out listItemTotalCount))
			{
				return;
			}

			SetListItemTotalCount(listItemTotalCount);
		}
	}
}