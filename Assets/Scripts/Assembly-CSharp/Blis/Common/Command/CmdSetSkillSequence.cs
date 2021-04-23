using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdSetSkillSequence, false)]
	public class CmdSetSkillSequence : CommandPacket
	{
		
		[Key(3)] public BlisFixedPoint duration;

		
		[Key(1)] public MasteryType masteryType;

		
		[Key(2)] public int sequence;

		
		[Key(4)] public BlisFixedPoint sequenceCooldown;

		
		[Key(0)] public SkillSlotSet skillSlotSet;

		
		public override void Action(ClientService service)
		{
			service.MyPlayer.SetSkillSequence(skillSlotSet, masteryType, sequence, duration.Value,
				sequenceCooldown.Value);
		}
	}
}