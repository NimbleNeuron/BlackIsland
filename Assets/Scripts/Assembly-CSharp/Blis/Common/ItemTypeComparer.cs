namespace Blis.Common
{
	public class ItemTypeComparer : SingletonComparerEnum<ItemTypeComparer, ItemType>
	{
		public override bool Equals(ItemType x, ItemType y)
		{
			return x == y;
		}


		public override int GetHashCode(ItemType obj)
		{
			return (int) obj;
		}
	}
}