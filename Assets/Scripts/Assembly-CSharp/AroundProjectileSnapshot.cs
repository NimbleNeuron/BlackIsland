using Blis.Common;
using MessagePack;


[MessagePackObject()]
public class AroundProjectileSnapshot : ISnapshot
{
	
	[Key(5)] public int aroundTargetId;

	
	[Key(1)] public BlisFixedPoint createdAngle;

	
	[Key(4)] public BlisFixedPoint distance;

	
	[Key(2)] public BlisFixedPoint duration;

	
	[Key(3)] public BlisFixedPoint speed;

	
	[Key(0)] public BlisFixedPoint timeAfterCreated;
}