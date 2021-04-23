using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class ChangeViewPortHeightDemoScript : MonoBehaviour
	{
		public LoopListView2 mLoopListView;


		public DragChangSizeScript mDragChangSizeScript;


		private Button mAppendMsgButton;


		private Button mBackButton;


		private Button mScrollToButton;


		private InputField mScrollToInput;


		private void Start()
		{
			mLoopListView.InitListView(ChatMsgDataSourceMgr.Get.TotalItemCount, OnGetItemByIndex);
			mScrollToButton = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToButton").GetComponent<Button>();
			mScrollToInput = GameObject.Find("ButtonPanel/buttonGroup2/ScrollToInputField").GetComponent<InputField>();
			mScrollToButton.onClick.AddListener(OnJumpBtnClicked);
			mBackButton = GameObject.Find("ButtonPanel/BackButton").GetComponent<Button>();
			mBackButton.onClick.AddListener(OnBackBtnClicked);
			mAppendMsgButton = GameObject.Find("ButtonPanel/buttonGroup1/AppendButton").GetComponent<Button>();
			mAppendMsgButton.onClick.AddListener(OnAppendMsgBtnClicked);
			mDragChangSizeScript.mOnDragEndAction = OnViewPortHeightChanged;
		}


		private void OnViewPortHeightChanged()
		{
			mLoopListView.ResetListView(false);
		}


		private void OnBackBtnClicked()
		{
			SceneManager.LoadScene("Menu");
		}


		private void OnAppendMsgBtnClicked()
		{
			ChatMsgDataSourceMgr.Get.AppendOneMsg();
			mLoopListView.SetListItemCount(ChatMsgDataSourceMgr.Get.TotalItemCount, false);
			mLoopListView.MovePanelToItemIndex(ChatMsgDataSourceMgr.Get.TotalItemCount - 1, 0f);
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
				return;
			}

			mLoopListView.MovePanelToItemIndex(num, 0f);
		}


		private LoopListViewItem2 OnGetItemByIndex(LoopListView2 listView, int index)
		{
			if (index < 0 || index >= ChatMsgDataSourceMgr.Get.TotalItemCount)
			{
				return null;
			}

			ChatMsg chatMsgByIndex = ChatMsgDataSourceMgr.Get.GetChatMsgByIndex(index);
			if (chatMsgByIndex == null)
			{
				return null;
			}

			LoopListViewItem2 loopListViewItem;
			if (chatMsgByIndex.mPersonId == 0)
			{
				loopListViewItem = listView.NewListViewItem("ItemPrefab1");
			}
			else
			{
				loopListViewItem = listView.NewListViewItem("ItemPrefab2");
			}

			ListItem4 component = loopListViewItem.GetComponent<ListItem4>();
			if (!loopListViewItem.IsInitHandlerCalled)
			{
				loopListViewItem.IsInitHandlerCalled = true;
				component.Init();
			}

			component.SetItemData(chatMsgByIndex, index);
			return loopListViewItem;
		}
	}
}