namespace Blis.Common
{
	public class ObjectTypeComparer : SingletonComparerEnum<ObjectTypeComparer, ObjectType>
	{
		public override bool Equals(ObjectType x, ObjectType y)
		{
			return x == y;
		}


		public override int GetHashCode(ObjectType obj)
		{
			return (int) obj;
		}
	}
}