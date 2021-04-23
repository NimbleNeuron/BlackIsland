namespace SuperScrollView
{
	public class LoopListViewInitParam
	{
		public float mDistanceForNew0 = 200f;


		public float mDistanceForNew1 = 200f;


		public float mDistanceForRecycle0 = 300f;


		public float mDistanceForRecycle1 = 300f;


		public float mItemDefaultWithPaddingSize = 20f;


		public float mSmoothDumpRate = 0.3f;


		public float mSnapFinishThreshold = 0.01f;


		public float mSnapVecThreshold = 145f;


		public static LoopListViewInitParam CopyDefaultInitParam()
		{
			return new LoopListViewInitParam();
		}
	}
}