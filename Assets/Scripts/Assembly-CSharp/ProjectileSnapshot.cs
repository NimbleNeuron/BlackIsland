using Blis.Common;
using MessagePack;


[MessagePackObject()]
public class ProjectileSnapshot : ISnapshot
{
	
	protected const int LAST_KEY = 5;

	
	[Key(0)] public int code;

	
	[Key(3)] public int collisionCount;

	
	[Key(2)] public BlisVector createdPosition;

	
	[Key(1)] public int ownerId;

	
	[Key(4)] public byte[] snapshot;
}