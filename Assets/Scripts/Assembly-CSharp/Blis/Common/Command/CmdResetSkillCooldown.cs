using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdResetSkillCooldown, false)]
	public class CmdResetSkillCooldown : LocalPlayerCharacterCommandPacket
	{
		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.ResetSkillCooldown();
		}
	}
}