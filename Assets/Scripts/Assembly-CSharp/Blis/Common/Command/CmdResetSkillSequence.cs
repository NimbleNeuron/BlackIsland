using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdResetSkillSequence, false)]
	public class CmdResetSkillSequence : CommandPacket
	{
		
		[Key(0)] public SkillSlotSet skillSlotSet;

		
		public override void Action(ClientService service)
		{
			service.MyPlayer.ResetSkillSequence(skillSlotSet);
		}
	}
}