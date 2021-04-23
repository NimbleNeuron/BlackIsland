using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class AirSupplyItemBoxSnapshot : ItemBoxSnapshot
	{
		
		[Key(2)] public int itemSpawnPointCode;

		
		[Key(1)] public ItemGrade maxItemGrade;
	}
}