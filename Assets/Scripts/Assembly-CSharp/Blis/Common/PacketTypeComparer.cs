namespace Blis.Common
{
	public class PacketTypeComparer : SingletonComparerEnum<PacketTypeComparer, PacketType>
	{
		public override bool Equals(PacketType x, PacketType y)
		{
			return x == y;
		}


		public override int GetHashCode(PacketType obj)
		{
			return (int) obj;
		}
	}
}