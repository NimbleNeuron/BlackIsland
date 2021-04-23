namespace SuperScrollView
{
	public class ItemSizeGroup
	{
		private readonly float mItemDefaultSize;

		private int mDirtyBeginIndex = 100;


		public float mGroupEndPos;


		public int mGroupIndex;


		public float mGroupSize;


		public float mGroupStartPos;


		public int mItemCount;


		public float[] mItemSizeArray;


		public float[] mItemStartPosArray;


		private int mMaxNoZeroIndex;


		public ItemSizeGroup(int index, float itemDefaultSize)
		{
			mGroupIndex = index;
			mItemDefaultSize = itemDefaultSize;
			Init();
		}


		public bool IsDirty => mDirtyBeginIndex < mItemCount;


		public void Init()
		{
			mItemSizeArray = new float[100];
			if (mItemDefaultSize != 0f)
			{
				for (int i = 0; i < mItemSizeArray.Length; i++)
				{
					mItemSizeArray[i] = mItemDefaultSize;
				}
			}

			mItemStartPosArray = new float[100];
			mItemStartPosArray[0] = 0f;
			mItemCount = 100;
			mGroupSize = mItemDefaultSize * mItemSizeArray.Length;
			if (mItemDefaultSize != 0f)
			{
				mDirtyBeginIndex = 0;
				return;
			}

			mDirtyBeginIndex = 100;
		}


		public float GetItemStartPos(int index)
		{
			return mGroupStartPos + mItemStartPosArray[index];
		}


		public float SetItemSize(int index, float size)
		{
			if (index > mMaxNoZeroIndex && size > 0f)
			{
				mMaxNoZeroIndex = index;
			}

			float num = mItemSizeArray[index];
			if (num == size)
			{
				return 0f;
			}

			mItemSizeArray[index] = size;
			if (index < mDirtyBeginIndex)
			{
				mDirtyBeginIndex = index;
			}

			float num2 = size - num;
			mGroupSize += num2;
			return num2;
		}


		public void SetItemCount(int count)
		{
			if (count < mMaxNoZeroIndex)
			{
				mMaxNoZeroIndex = count;
			}

			if (mItemCount == count)
			{
				return;
			}

			mItemCount = count;
			RecalcGroupSize();
		}


		public void RecalcGroupSize()
		{
			mGroupSize = 0f;
			for (int i = 0; i < mItemCount; i++)
			{
				mGroupSize += mItemSizeArray[i];
			}
		}


		public int GetItemIndexByPos(float pos)
		{
			if (mItemCount == 0)
			{
				return -1;
			}

			int i = 0;
			int num = mItemCount - 1;
			if (mItemDefaultSize == 0f)
			{
				if (mMaxNoZeroIndex < 0)
				{
					mMaxNoZeroIndex = 0;
				}

				num = mMaxNoZeroIndex;
			}

			while (i <= num)
			{
				int num2 = (i + num) / 2;
				float num3 = mItemStartPosArray[num2];
				float num4 = num3 + mItemSizeArray[num2];
				if (num3 <= pos && num4 >= pos)
				{
					return num2;
				}

				if (pos > num4)
				{
					i = num2 + 1;
				}
				else
				{
					num = num2 - 1;
				}
			}

			return -1;
		}


		public void UpdateAllItemStartPos()
		{
			if (mDirtyBeginIndex >= mItemCount)
			{
				return;
			}

			for (int i = mDirtyBeginIndex < 1 ? 1 : mDirtyBeginIndex; i < mItemCount; i++)
			{
				mItemStartPosArray[i] = mItemStartPosArray[i - 1] + mItemSizeArray[i - 1];
			}

			mDirtyBeginIndex = mItemCount;
		}


		public void ClearOldData()
		{
			for (int i = mItemCount; i < 100; i++)
			{
				mItemSizeArray[i] = 0f;
			}
		}
	}
}