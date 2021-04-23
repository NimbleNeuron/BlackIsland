using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class ListItem13 : MonoBehaviour
	{
		public Text mNameText;


		public Image mIcon;


		public Image[] mStarArray;


		public Text mDescText;


		public Text mDescText2;


		public Color32 mRedStarColor = new Color32(249, 227, 101, byte.MaxValue);


		public Color32 mGrayStarColor = new Color32(215, 215, 215, byte.MaxValue);


		public GameObject mContentRootObj;


		private int mChildDataIndex = -1;


		private int mItemDataIndex = -1;


		public void Init()
		{
			for (int i = 0; i < mStarArray.Length; i++)
			{
				int index = i;
				ClickEventListener.Get(mStarArray[i].gameObject)
					.SetClickEventHandler(delegate { OnStarClicked(index); });
			}
		}


		private void OnStarClicked(int index)
		{
			ItemData itemChildDataByIndex =
				TreeViewDataSourceMgr.Get.GetItemChildDataByIndex(mItemDataIndex, mChildDataIndex);
			if (itemChildDataByIndex == null)
			{
				return;
			}

			if (index == 0 && itemChildDataByIndex.mStarCount == 1)
			{
				itemChildDataByIndex.mStarCount = 0;
			}
			else
			{
				itemChildDataByIndex.mStarCount = index + 1;
			}

			SetStarCount(itemChildDataByIndex.mStarCount);
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


		public void SetItemData(ItemData itemData, int itemIndex, int childIndex)
		{
			mItemDataIndex = itemIndex;
			mChildDataIndex = childIndex;
			mNameText.text = itemData.mName;
			mDescText.text = itemData.mFileSize + "KB";
			mDescText2.text = itemData.mDesc;
			mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
			SetStarCount(itemData.mStarCount);
		}
	}
}