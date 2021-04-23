using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SuperScrollView
{
	public class DataSourceMgr : MonoBehaviour
	{
		private static DataSourceMgr instance;


		public int mTotalDataCount = 10000;


		private readonly List<ItemData> mItemDataList = new List<ItemData>();


		private float mDataLoadLeftTime;


		private float mDataRefreshLeftTime;


		private bool mIsWaitLoadingMoreData;


		private bool mIsWaittingRefreshData;


		private int mLoadMoreCount = 20;


		private Action mOnLoadMoreFinished;


		private Action mOnRefreshFinished;


		public static DataSourceMgr Get {
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<DataSourceMgr>();
				}

				return instance;
			}
		}


		public int TotalItemCount => mItemDataList.Count;


		private void Awake()
		{
			Init();
		}


		public void Update()
		{
			if (mIsWaittingRefreshData)
			{
				mDataRefreshLeftTime -= Time.deltaTime;
				if (mDataRefreshLeftTime <= 0f)
				{
					mIsWaittingRefreshData = false;
					DoRefreshDataSource();
					if (mOnRefreshFinished != null)
					{
						mOnRefreshFinished();
					}
				}
			}

			if (mIsWaitLoadingMoreData)
			{
				mDataLoadLeftTime -= Time.deltaTime;
				if (mDataLoadLeftTime <= 0f)
				{
					mIsWaitLoadingMoreData = false;
					DoLoadMoreDataSource();
					if (mOnLoadMoreFinished != null)
					{
						mOnLoadMoreFinished();
					}
				}
			}
		}


		public void Init()
		{
			DoRefreshDataSource();
		}


		public ItemData GetItemDataByIndex(int index)
		{
			if (index < 0 || index >= mItemDataList.Count)
			{
				return null;
			}

			return mItemDataList[index];
		}


		public ItemData GetItemDataById(int itemId)
		{
			int count = mItemDataList.Count;
			for (int i = 0; i < count; i++)
			{
				if (mItemDataList[i].mId == itemId)
				{
					return mItemDataList[i];
				}
			}

			return null;
		}


		public void RequestRefreshDataList(Action onReflushFinished)
		{
			mDataRefreshLeftTime = 1f;
			mOnRefreshFinished = onReflushFinished;
			mIsWaittingRefreshData = true;
		}


		public void RequestLoadMoreDataList(int loadCount, Action onLoadMoreFinished)
		{
			mLoadMoreCount = loadCount;
			mDataLoadLeftTime = 1f;
			mOnLoadMoreFinished = onLoadMoreFinished;
			mIsWaitLoadingMoreData = true;
		}


		public void SetDataTotalCount(int count)
		{
			mTotalDataCount = count;
			DoRefreshDataSource();
		}


		public void ExchangeData(int index1, int index2)
		{
			ItemData value = mItemDataList[index1];
			ItemData value2 = mItemDataList[index2];
			mItemDataList[index1] = value2;
			mItemDataList[index2] = value;
		}


		public void RemoveData(int index)
		{
			mItemDataList.RemoveAt(index);
		}


		public void InsertData(int index, ItemData data)
		{
			mItemDataList.Insert(index, data);
		}


		private void DoRefreshDataSource()
		{
			mItemDataList.Clear();
			for (int i = 0; i < mTotalDataCount; i++)
			{
				ItemData itemData = new ItemData();
				itemData.mId = i;
				itemData.mName = "Item" + i;
				itemData.mDesc = "Item Desc For Item " + i;
				itemData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
				itemData.mStarCount = Random.Range(0, 6);
				itemData.mFileSize = Random.Range(20, 999);
				itemData.mChecked = false;
				itemData.mIsExpand = false;
				mItemDataList.Add(itemData);
			}
		}


		private void DoLoadMoreDataSource()
		{
			int count = mItemDataList.Count;
			for (int i = 0; i < mLoadMoreCount; i++)
			{
				int num = i + count;
				ItemData itemData = new ItemData();
				itemData.mId = num;
				itemData.mName = "Item" + num;
				itemData.mDesc = "Item Desc For Item " + num;
				itemData.mIcon = ResManager.Get.GetSpriteNameByIndex(Random.Range(0, 24));
				itemData.mStarCount = Random.Range(0, 6);
				itemData.mFileSize = Random.Range(20, 999);
				itemData.mChecked = false;
				itemData.mIsExpand = false;
				mItemDataList.Add(itemData);
			}

			mTotalDataCount = mItemDataList.Count;
		}


		public void CheckAllItem()
		{
			int count = mItemDataList.Count;
			for (int i = 0; i < count; i++)
			{
				mItemDataList[i].mChecked = true;
			}
		}


		public void UnCheckAllItem()
		{
			int count = mItemDataList.Count;
			for (int i = 0; i < count; i++)
			{
				mItemDataList[i].mChecked = false;
			}
		}


		public bool DeleteAllCheckedItem()
		{
			int count = mItemDataList.Count;
			mItemDataList.RemoveAll(it => it.mChecked);
			return count != mItemDataList.Count;
		}
	}
}