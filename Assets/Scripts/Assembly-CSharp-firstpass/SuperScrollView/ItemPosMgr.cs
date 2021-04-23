using System.Collections.Generic;

namespace SuperScrollView
{
	public class ItemPosMgr
	{
		public const int mItemMaxCountPerGroup = 100;


		private readonly List<ItemSizeGroup> mItemSizeGroupList = new List<ItemSizeGroup>();


		private int mDirtyBeginIndex = int.MaxValue;


		public float mItemDefaultSize = 20f;


		private int mMaxNotEmptyGroupIndex;


		public float mTotalSize;


		public ItemPosMgr(float itemDefaultSize)
		{
			mItemDefaultSize = itemDefaultSize;
		}


		public void SetItemMaxCount(int maxCount)
		{
			mDirtyBeginIndex = 0;
			mTotalSize = 0f;
			int itemCount;
			int num = itemCount = maxCount % 100;
			int num2 = maxCount / 100;
			if (num > 0)
			{
				num2++;
			}
			else
			{
				itemCount = 100;
			}

			int count = mItemSizeGroupList.Count;
			if (count > num2)
			{
				int count2 = count - num2;
				mItemSizeGroupList.RemoveRange(num2, count2);
			}
			else if (count < num2)
			{
				if (count > 0)
				{
					mItemSizeGroupList[count - 1].ClearOldData();
				}

				int num3 = num2 - count;
				for (int i = 0; i < num3; i++)
				{
					ItemSizeGroup item = new ItemSizeGroup(count + i, mItemDefaultSize);
					mItemSizeGroupList.Add(item);
				}
			}
			else if (count > 0)
			{
				mItemSizeGroupList[count - 1].ClearOldData();
			}

			count = mItemSizeGroupList.Count;
			if (count - 1 < mMaxNotEmptyGroupIndex)
			{
				mMaxNotEmptyGroupIndex = count - 1;
			}

			if (mMaxNotEmptyGroupIndex < 0)
			{
				mMaxNotEmptyGroupIndex = 0;
			}

			if (count == 0)
			{
				return;
			}

			for (int j = 0; j < count - 1; j++)
			{
				mItemSizeGroupList[j].SetItemCount(100);
			}

			mItemSizeGroupList[count - 1].SetItemCount(itemCount);
			for (int k = 0; k < count; k++)
			{
				mTotalSize += mItemSizeGroupList[k].mGroupSize;
			}
		}


		public void SetItemSize(int itemIndex, float size)
		{
			int num = itemIndex / 100;
			int index = itemIndex % 100;
			float num2 = mItemSizeGroupList[num].SetItemSize(index, size);
			if (num2 != 0f && num < mDirtyBeginIndex)
			{
				mDirtyBeginIndex = num;
			}

			mTotalSize += num2;
			if (num > mMaxNotEmptyGroupIndex && size > 0f)
			{
				mMaxNotEmptyGroupIndex = num;
			}
		}


		public float GetItemPos(int itemIndex)
		{
			Update(true);
			int index = itemIndex / 100;
			int index2 = itemIndex % 100;
			return mItemSizeGroupList[index].GetItemStartPos(index2);
		}


		public bool GetItemIndexAndPosAtGivenPos(float pos, ref int index, ref float itemPos)
		{
			Update(true);
			index = 0;
			itemPos = 0f;
			int count = mItemSizeGroupList.Count;
			if (count == 0)
			{
				return true;
			}

			ItemSizeGroup itemSizeGroup = null;
			int i = 0;
			int num = count - 1;
			if (mItemDefaultSize == 0f)
			{
				if (mMaxNotEmptyGroupIndex < 0)
				{
					mMaxNotEmptyGroupIndex = 0;
				}

				num = mMaxNotEmptyGroupIndex;
			}

			while (i <= num)
			{
				int num2 = (i + num) / 2;
				ItemSizeGroup itemSizeGroup2 = mItemSizeGroupList[num2];
				if (itemSizeGroup2.mGroupStartPos <= pos && itemSizeGroup2.mGroupEndPos >= pos)
				{
					itemSizeGroup = itemSizeGroup2;
					break;
				}

				if (pos > itemSizeGroup2.mGroupEndPos)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}

			if (itemSizeGroup == null)
			{
				return false;
			}

			int itemIndexByPos = itemSizeGroup.GetItemIndexByPos(pos - itemSizeGroup.mGroupStartPos);
			if (itemIndexByPos < 0)
			{
				return false;
			}

			index = itemIndexByPos + itemSizeGroup.mGroupIndex * 100;
			itemPos = itemSizeGroup.GetItemStartPos(itemIndexByPos);
			return true;
		}


		public void Update(bool updateAll)
		{
			int count = mItemSizeGroupList.Count;
			if (count == 0)
			{
				return;
			}

			if (mDirtyBeginIndex >= count)
			{
				return;
			}

			int num = 0;
			for (int i = mDirtyBeginIndex; i < count; i++)
			{
				num++;
				ItemSizeGroup itemSizeGroup = mItemSizeGroupList[i];
				mDirtyBeginIndex++;
				itemSizeGroup.UpdateAllItemStartPos();
				if (i == 0)
				{
					itemSizeGroup.mGroupStartPos = 0f;
					itemSizeGroup.mGroupEndPos = itemSizeGroup.mGroupSize;
				}
				else
				{
					itemSizeGroup.mGroupStartPos = mItemSizeGroupList[i - 1].mGroupEndPos;
					itemSizeGroup.mGroupEndPos = itemSizeGroup.mGroupStartPos + itemSizeGroup.mGroupSize;
				}

				if (!updateAll && num > 1)
				{
					return;
				}
			}
		}
	}
}