using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class MoveToDestinationSnapshot
	{
		
		[Key(2)] public bool arrived;

		
		[Key(5)] public BlisVector[] corners;

		
		[Key(1)] public BlisVector destination;

		
		[Key(3)] public BlisFixedPoint moveSpeed;

		
		[Key(0)] public BlisVector startPosition;

		
		[Key(4)] public int walkableNavMask;
	}
}