using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class SightObjectSnapshot
	{
		
		[Key(1)] public int attachSightId;

		
		[Key(6)] public bool isDetect;

		
		[Key(5)] public bool isDetectShare;

		
		[Key(0)] public int ownerId;

		
		[Key(3)] public int sightAngle;

		
		[Key(2)] public float sightRange;
	}
}