using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class ListItem3 : MonoBehaviour
	{
		public Text mNameText;


		public Image mIcon;


		public Text mDescText;


		public Toggle mToggle;


		private int mItemIndex = -1;


		public void Init()
		{
			mToggle.onValueChanged.AddListener(OnToggleValueChanged);
		}


		private void OnToggleValueChanged(bool check)
		{
			ItemData itemDataByIndex = DataSourceMgr.Get.GetItemDataByIndex(mItemIndex);
			if (itemDataByIndex == null)
			{
				return;
			}

			itemDataByIndex.mChecked = check;
		}


		public void SetItemData(ItemData itemData, int itemIndex)
		{
			mItemIndex = itemIndex;
			mNameText.text = itemData.mName;
			mDescText.text = itemData.mDesc;
			mIcon.sprite = ResManager.Get.GetSpriteByName(itemData.mIcon);
			mToggle.isOn = itemData.mChecked;
		}
	}
}