using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdProjectileExplosion))]
	[Union(1, typeof(CmdProjectileCollision))]
	[Union(2, typeof(CmdProjectileCollisionWall))]
	[MessagePackObject()]
	public abstract class LocalProjectileCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalProjectile>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalProjectile self);
	}
}