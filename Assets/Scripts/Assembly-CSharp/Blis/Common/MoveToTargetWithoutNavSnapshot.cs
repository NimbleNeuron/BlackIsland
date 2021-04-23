using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class MoveToTargetWithoutNavSnapshot
	{
		
		[Key(4)] public bool arrived;

		
		[Key(3)] public BlisFixedPoint arriveRadius;

		
		[Key(0)] public BlisVector moveStartPos;

		
		[Key(2)] public BlisFixedPoint speed;

		
		[Key(1)] public int target;

		
		[Key(5)] public int walkableNavMask;
	}
}