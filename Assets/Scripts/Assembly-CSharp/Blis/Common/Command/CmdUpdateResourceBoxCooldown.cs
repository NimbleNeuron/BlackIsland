using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdUpdateResourceBoxCooldown, false)]
	public class CmdUpdateResourceBoxCooldown : LocalResourceItemBoxCommandPacket
	{
		
		[Key(1)] public BlisFixedPoint cooldown;

		
		public override void Action(ClientService service, LocalResourceItemBox self)
		{
			self.StartCooldown(cooldown.Value);
		}
	}
}