using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	public class SightSnapshot
	{
		[Key(0)] public int attachSightId;


		[Key(3)] public int ownerId;


		[Key(2)] public BlisFixedPoint sightRange;


		[Key(1)] public int targetId;
	}
}