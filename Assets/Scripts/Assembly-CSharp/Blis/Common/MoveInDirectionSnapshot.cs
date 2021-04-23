using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class MoveInDirectionSnapshot
	{
		
		[Key(0)] public BlisVector direction;

		
		[Key(2)] public bool moveFinished;

		
		[Key(1)] public BlisFixedPoint moveSpeed;

		
		[Key(3)] public int walkableNavMask;
	}
}