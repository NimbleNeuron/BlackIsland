using System;
using UnityEngine;
using UnityEngine.UI;

namespace SuperScrollView
{
	public class ListItem12 : MonoBehaviour
	{
		public Text mText;


		public GameObject mArrow;


		public Button mButton;


		private Action<int> mClickHandler;


		private int mTreeItemIndex = -1;


		public int TreeItemIndex => mTreeItemIndex;


		public void Init()
		{
			mButton.onClick.AddListener(OnButtonClicked);
		}


		public void SetClickCallBack(Action<int> clickHandler)
		{
			mClickHandler = clickHandler;
		}


		private void OnButtonClicked()
		{
			if (mClickHandler != null)
			{
				mClickHandler(mTreeItemIndex);
			}
		}


		public void SetExpand(bool expand)
		{
			if (expand)
			{
				mArrow.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
				return;
			}

			mArrow.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
		}


		public void SetItemData(int treeItemIndex, bool expand)
		{
			mTreeItemIndex = treeItemIndex;
			SetExpand(expand);
		}
	}
}