namespace SuperScrollView
{
	public struct RowColumnPair
	{
		public RowColumnPair(int row1, int column1)
		{
			mRow = row1;
			mColumn = column1;
		}


		public bool Equals(RowColumnPair other)
		{
			return mRow == other.mRow && mColumn == other.mColumn;
		}


		public static bool operator ==(RowColumnPair a, RowColumnPair b)
		{
			return a.mRow == b.mRow && a.mColumn == b.mColumn;
		}


		public static bool operator !=(RowColumnPair a, RowColumnPair b)
		{
			return a.mRow != b.mRow || a.mColumn != b.mColumn;
		}


		public override int GetHashCode()
		{
			return 0;
		}


		public override bool Equals(object obj)
		{
			return obj != null && obj is RowColumnPair && Equals((RowColumnPair) obj);
		}


		public int mRow;


		public int mColumn;
	}
}