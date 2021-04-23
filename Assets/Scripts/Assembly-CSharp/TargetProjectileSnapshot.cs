using Blis.Common;
using MessagePack;


[MessagePackObject()]
public class TargetProjectileSnapshot : ISnapshot
{
	
	[Key(1)] public BlisFixedPoint projectileSpeed;

	
	[Key(0)] public int targetId;
}