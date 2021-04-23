using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdProjectileExplosion, false)]
	public class CmdProjectileExplosion : LocalProjectileCommandPacket
	{
		
		public override void Action(ClientService service, LocalProjectile self)
		{
			self.OnExplosion();
		}
	}
}