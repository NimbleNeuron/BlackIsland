using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class MoveStraightWithoutNavSnapshot
	{
		
		[Key(2)] public BlisFixedPoint duration;

		
		[Key(3)] public BlisFixedPoint lerpRate;

		
		[Key(1)] public BlisVector moveEndPos;

		
		[Key(0)] public BlisVector moveStartPos;

		
		[Key(4)] public BlisFixedPoint timeStack;

		
		[Key(5)] public int walkableNavMask;
	}
}