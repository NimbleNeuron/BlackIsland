using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class ResourceItemBoxSnapshot : ItemBoxSnapshot
	{
		
		[Key(1)] public BlisFixedPoint cooldownUntil;
	}
}