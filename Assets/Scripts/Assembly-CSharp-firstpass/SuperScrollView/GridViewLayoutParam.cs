using UnityEngine;

namespace SuperScrollView
{
	public class GridViewLayoutParam
	{
		public int mColumnOrRowCount;


		public float[] mCustomColumnOrRowOffsetArray;


		public float mItemWidthOrHeight;


		public float mPadding1;


		public float mPadding2;


		public bool CheckParam()
		{
			if (mColumnOrRowCount <= 0)
			{
				Debug.LogError("mColumnOrRowCount shoud be > 0");
				return false;
			}

			if (mItemWidthOrHeight <= 0f)
			{
				Debug.LogError("mItemWidthOrHeight shoud be > 0");
				return false;
			}

			if (mCustomColumnOrRowOffsetArray != null && mCustomColumnOrRowOffsetArray.Length != mColumnOrRowCount)
			{
				Debug.LogError("mGroupOffsetArray.Length != mColumnOrRowCount");
				return false;
			}

			return true;
		}
	}
}