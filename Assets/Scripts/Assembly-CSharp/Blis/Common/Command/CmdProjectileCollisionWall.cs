using Blis.Client;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdProjectileCollisionWall, false)]
	public class CmdProjectileCollisionWall : LocalProjectileCommandPacket
	{
		
		[Key(1)] public Vector3 targetPosition;

		
		public override void Action(ClientService service, LocalProjectile self)
		{
			self.OnCollisionWall(targetPosition);
		}
	}
}