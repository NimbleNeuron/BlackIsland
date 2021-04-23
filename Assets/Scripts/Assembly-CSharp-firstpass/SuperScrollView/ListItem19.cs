using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class ListItem19 : MonoBehaviour
	{
		public Text mNameText;


		public Image mIcon;


		public Image mStarIcon;


		public Text mStarCount;


		public Text mRowText;


		public Text mColumnText;


		public Color32 mRedStarColor = new Color32(236, 217, 103, byte.MaxValue);


		public Color32 mGrayStarColor = new Color32(215, 215, 215, byte.MaxValue);


		public Toggle mToggle;


		private int mItemDataIndex = -1;


		public void Init()
		{
			ClickEventListener.Get(mStarIcon.gameObject).SetClickEventHandler(OnStarClicked);
			mToggle.onValueChanged.AddListener(OnToggleValueChanged);
		}


		private void OnToggleValueChanged(bool check)
		{
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(mItemDataIndex);
			if (itemDataByIndex == null)
			{
				return;
			}

			itemDataByIndex.mChecked = check;
		}


		private void OnStarClicked(GameObject obj)
		{
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(mItemDataIndex);
			if (itemDataByIndex == null)
			{
				return;
			}

			if (itemDataByIndex.mStarCount == 5)
			{
				itemDataByIndex.mStarCount = 0;
			}
			else
			{
				itemDataByIndex.mStarCount++;
			}

			SetStarCount(itemDataByIndex.mStarCount);
		}


		public void SetStarCount(int count)
		{
			mStarCount.text = count.ToString();
			if (count == 0)
			{
				mStarIcon.color = mGrayStarColor;
				return;
			}

			mStarIcon.color = mRedStarColor;
		}


		public void SetItemData(ItemData itemData, int itemIndex, int row, int column)
		{
			mItemDataIndex = itemIndex;
			mNameText.text = itemData.mName;
			mRowText.text = "Row: " + row;
			mColumnText.text = "Column: " + column;
			mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
			SetStarCount(itemData.mStarCount);
			mToggle.isOn = itemData.mChecked;
		}
	}
}