using Blis.Common;
using MessagePack;


[MessagePackObject()]
public class InstantArrivalProjectileSnapshot : ISnapshot
{
	
	[Key(1)] public BlisVector projectileDirection;

	
	[Key(0)] public BlisFixedPoint projectileSpeed;
}