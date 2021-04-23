using System.Collections.Generic;
using UnityEngine;

namespace SuperScrollView
{
	public class StaggeredGridItemPool
	{
		private static int mCurItemIdCount;


		private readonly List<LoopStaggeredGridViewItem> mPooledItemList = new List<LoopStaggeredGridViewItem>();


		private readonly List<LoopStaggeredGridViewItem> mTmpPooledItemList = new List<LoopStaggeredGridViewItem>();


		private int mInitCreateCount = 1;


		private RectTransform mItemParent;


		private float mPadding;


		private string mPrefabName;


		private GameObject mPrefabObj;


		public void Init(GameObject prefabObj, float padding, int createCount, RectTransform parent)
		{
			mPrefabObj = prefabObj;
			mPrefabName = mPrefabObj.name;
			mInitCreateCount = createCount;
			mPadding = padding;
			mItemParent = parent;
			mPrefabObj.SetActive(false);
			for (int i = 0; i < mInitCreateCount; i++)
			{
				LoopStaggeredGridViewItem item = CreateItem();
				RecycleItemReal(item);
			}
		}


		public LoopStaggeredGridViewItem GetItem()
		{
			mCurItemIdCount++;
			LoopStaggeredGridViewItem loopStaggeredGridViewItem;
			if (mTmpPooledItemList.Count > 0)
			{
				int count = mTmpPooledItemList.Count;
				loopStaggeredGridViewItem = mTmpPooledItemList[count - 1];
				mTmpPooledItemList.RemoveAt(count - 1);
				loopStaggeredGridViewItem.gameObject.SetActive(true);
			}
			else
			{
				int count2 = mPooledItemList.Count;
				if (count2 == 0)
				{
					loopStaggeredGridViewItem = CreateItem();
				}
				else
				{
					loopStaggeredGridViewItem = mPooledItemList[count2 - 1];
					mPooledItemList.RemoveAt(count2 - 1);
					loopStaggeredGridViewItem.gameObject.SetActive(true);
				}
			}

			loopStaggeredGridViewItem.Padding = mPadding;
			loopStaggeredGridViewItem.ItemId = mCurItemIdCount;
			return loopStaggeredGridViewItem;
		}


		public void DestroyAllItem()
		{
			ClearTmpRecycledItem();
			int count = mPooledItemList.Count;
			for (int i = 0; i < count; i++)
			{
				Object.DestroyImmediate(mPooledItemList[i].gameObject);
			}

			mPooledItemList.Clear();
		}


		public LoopStaggeredGridViewItem CreateItem()
		{
			GameObject gameObject =
				Object.Instantiate<GameObject>(mPrefabObj, Vector3.zero, Quaternion.identity, mItemParent);
			gameObject.SetActive(true);
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.localScale = Vector3.one;
			component.anchoredPosition3D = Vector3.zero;
			component.localEulerAngles = Vector3.zero;
			LoopStaggeredGridViewItem component2 = gameObject.GetComponent<LoopStaggeredGridViewItem>();
			component2.ItemPrefabName = mPrefabName;
			component2.StartPosOffset = 0f;
			return component2;
		}


		private void RecycleItemReal(LoopStaggeredGridViewItem item)
		{
			item.gameObject.SetActive(false);
			mPooledItemList.Add(item);
		}


		public void RecycleItem(LoopStaggeredGridViewItem item)
		{
			mTmpPooledItemList.Add(item);
		}


		public void ClearTmpRecycledItem()
		{
			int count = mTmpPooledItemList.Count;
			if (count == 0)
			{
				return;
			}

			for (int i = 0; i < count; i++)
			{
				RecycleItemReal(mTmpPooledItemList[i]);
			}

			mTmpPooledItemList.Clear();
		}
	}
}