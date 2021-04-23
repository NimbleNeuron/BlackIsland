using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class MoveInCurveSnapshot
	{
		
		[Key(1)] public BlisFixedPoint angularSpeed;

		
		[Key(2)] public BlisVector currentDirection;

		
		[Key(3)] public bool moveFinished;

		
		[Key(0)] public BlisFixedPoint moveSpeed;

		
		[Key(4)] public int walkableNavMask;
	}
}