using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdHoldSkillCooldown, false)]
	public class CmdHoldSkillCooldown : LocalPlayerCharacterCommandPacket
	{
		
		[Key(3)] public bool isHold;

		
		[Key(2)] public MasteryType masteryType;

		
		[Key(1)] public SkillSlotSet skillSlotSet;

		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.HoldSkillCooldown(skillSlotSet, masteryType, isHold);
		}
	}
}