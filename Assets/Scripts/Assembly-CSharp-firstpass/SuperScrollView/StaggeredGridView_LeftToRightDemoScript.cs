using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class StaggeredGridView_LeftToRightDemoScript : MonoBehaviour
	{
		public LoopStaggeredGridView mLoopListView;


		private Button mAddItemButton;


		private InputField mAddItemInput;


		private Button mBackButton;


		private int[] mItemWidthArrayForDemo;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private Button mSetCountButton;


		private InputField mSetCountInput;


		private void Start()
		{
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
			InitItemHeightArrayForDemo();
			GridViewLayoutParam gridViewLayoutParam = new GridViewLayoutParam();
			gridViewLayoutParam.mPadding1 = 10f;
			gridViewLayoutParam.mPadding2 = 10f;
			gridViewLayoutParam.mColumnOrRowCount = 2;
			gridViewLayoutParam.mItemWidthOrHeight = 219f;
			mLoopListView.InitListView(DataSourceMgr.Get.TotalItemCount, gridViewLayoutParam, OnGetItemByIndex);
		}


		private LoopStaggeredGridViewItem OnGetItemByIndex(LoopStaggeredGridView listView, int index)
		{
			if (index < 0)
			{
				return null;
			}

			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(index);
			if (itemDataByIndex == null)
			{
				return null;
			}

			LoopStaggeredGridViewItem loopStaggeredGridViewItem = listView.NewListViewItem("ItemPrefab1");
			ListItem5 component = loopStaggeredGridViewItem.GetComponent<ListItem5>();
			if (!loopStaggeredGridViewItem.IsInitHandlerCalled)
			{
				loopStaggeredGridViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			component.SetItemData(itemDataByIndex, index);
			float size = 390f + mItemWidthArrayForDemo[index % mItemWidthArrayForDemo.Length] * 10f;
			loopStaggeredGridViewItem.CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
				size);
			return loopStaggeredGridViewItem;
		}


		private void InitItemHeightArrayForDemo()
		{
			mItemWidthArrayForDemo = new int[100];
			for (int i = 0; i < mItemWidthArrayForDemo.Length; i++)
			{
				mItemWidthArrayForDemo[i] = Random.Range(0, 20);
			}
		}


		private void OnBackBtnClicked()
		{
			SceneManager.LoadScene("Menu");
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

			mLoopListView.MovePanelToItemIndex(num, 0f);
		}


		private void OnAddItemBtnClicked()
		{
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