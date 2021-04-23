using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	public class HookLineProjectileSnapshot : ProjectileSnapshot
	{
		
		[Key(5)] public byte[] hookLineInfoSnapshot;
	}
}