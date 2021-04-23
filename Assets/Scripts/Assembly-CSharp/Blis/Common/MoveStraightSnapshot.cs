using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class MoveStraightSnapshot
	{
		
		[Key(0)] public bool arrived;

		
		[Key(5)] public bool canRotate;

		
		[Key(2)] public BlisVector destination;

		
		[Key(3)] public BlisFixedPoint duration;

		
		[Key(4)] public EasingFunction.Ease ease;

		
		[Key(6)] public BlisFixedPoint elapsedTime;

		
		[Key(1)] public BlisVector startPosition;

		
		[Key(7)] public int walkableNavMask;
	}
}