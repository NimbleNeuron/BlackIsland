using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdProjectileCollision, false)]
	public class CmdProjectileCollision : LocalProjectileCommandPacket
	{
		
		[Key(1)] public int targetId;

		
		public override void Action(ClientService service, LocalProjectile self)
		{
			LocalCharacter localCharacter = service.World.Find<LocalCharacter>(targetId);
			if (localCharacter != null)
			{
				self.OnCollision(localCharacter);
			}
		}
	}
}