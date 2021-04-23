namespace Blis.Common
{
	public class ItemGradeComparer : SingletonComparerEnum<ItemGradeComparer, ItemGrade>
	{
		public override bool Equals(ItemGrade x, ItemGrade y)
		{
			return x == y;
		}


		public override int GetHashCode(ItemGrade obj)
		{
			return (int) obj;
		}
	}
}