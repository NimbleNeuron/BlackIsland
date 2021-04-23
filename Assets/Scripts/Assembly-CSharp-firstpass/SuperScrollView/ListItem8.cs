using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class ListItem8 : MonoBehaviour
	{
		public Text mNameText;


		public Image mIcon;


		public Image[] mStarArray;


		public Text mDescText;


		public GameObject mExpandContentRoot;


		public Text mClickTip;


		public Button mExpandBtn;


		public Color32 mRedStarColor = new Color32(249, 227, 101, byte.MaxValue);


		public Color32 mGrayStarColor = new Color32(215, 215, 215, byte.MaxValue);


		private bool mIsExpand;


		private int mItemDataIndex = -1;


		public void Init()
		{
			for (int i = 0; i < mStarArray.Length; i++)
			{
				int index = i;
				ClickEventListener.Get(mStarArray[i].gameObject)
					.SetClickEventHandler(delegate { OnStarClicked(index); });
			}

			mExpandBtn.onClick.AddListener(OnExpandBtnClicked);
		}


		public void OnExpandChanged()
		{
			RectTransform component = gameObject.GetComponent<RectTransform>();
			if (mIsExpand)
			{
				component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 284f);
				mExpandContentRoot.SetActive(true);
				mClickTip.text = "Shrink";
				return;
			}

			component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 143f);
			mExpandContentRoot.SetActive(false);
			mClickTip.text = "Expand";
		}


		private void OnExpandBtnClicked()
		{
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(mItemDataIndex);
			if (itemDataByIndex == null)
			{
				return;
			}

			mIsExpand = !mIsExpand;
			itemDataByIndex.mIsExpand = mIsExpand;
			OnExpandChanged();
			LoopListViewItem2 component = gameObject.GetComponent<LoopListViewItem2>();
			component.ParentListView.OnItemSizeChanged(component.ItemIndex);
		}


		private void OnStarClicked(int index)
		{
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(mItemDataIndex);
			if (itemDataByIndex == null)
			{
				return;
			}

			if (index == 0 && itemDataByIndex.mStarCount == 1)
			{
				itemDataByIndex.mStarCount = 0;
			}
			else
			{
				itemDataByIndex.mStarCount = index + 1;
			}

			SetStarCount(itemDataByIndex.mStarCount);
		}


		public void SetStarCount(int count)
		{
			int i;
			for (i = 0; i < count; i++)
			{
				mStarArray[i].color = mRedStarColor;
			}

			while (i < mStarArray.Length)
			{
				mStarArray[i].color = mGrayStarColor;
				i++;
			}
		}


		public void SetItemData(ItemData itemData, int itemIndex)
		{
			mItemDataIndex = itemIndex;
			mNameText.text = itemData.mName;
			mDescText.text = itemData.mFileSize + "KB";
			mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
			SetStarCount(itemData.mStarCount);
			mIsExpand = itemData.mIsExpand;
			OnExpandChanged();
		}
	}
}