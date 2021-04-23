using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(LocalObjectCommandPacket))]
	[Union(1, typeof(LocalCharacterCommandPacket))]
	[Union(2, typeof(LocalMovableCharacterCommandPacket))]
	[Union(3, typeof(LocalPlayerCharacterCommandPacket))]
	[Union(5, typeof(LocalSummonBaseCommandPacket))]
	[Union(6, typeof(LocalSummonTrapCommandPacket))]
	[Union(8, typeof(LocalProjectileCommandPacket))]
	[Union(9, typeof(LocalHyperloopCommandPacket))]
	[Union(10, typeof(LocalSecurityConsoleCommandPacket))]
	[Union(12, typeof(LocalResourceItemBoxCommandPacket))]
	[MessagePackObject()]
	public abstract class ObjectCommandPacket : CommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		[Key(0)] public int objectId;
	}
}