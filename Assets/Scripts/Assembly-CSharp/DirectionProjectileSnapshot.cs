using Blis.Common;
using MessagePack;


[MessagePackObject()]
public class DirectionProjectileSnapshot : ISnapshot
{
	
	[Key(1)] public BlisFixedPoint duration;

	
	[Key(2)] public BlisVector targetDirectionEndPos;

	
	[Key(0)] public BlisFixedPoint timeAfterCreated;
}