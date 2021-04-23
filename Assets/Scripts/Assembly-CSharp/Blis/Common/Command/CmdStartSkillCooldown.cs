using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdStartSkillCooldown, false)]
	public class CmdStartSkillCooldown : LocalPlayerCharacterCommandPacket
	{
		
		[Key(3)] public BlisFixedPoint cooldown;

		
		[Key(2)] public MasteryType masteryType;

		
		[Key(1)] public SkillSlotSet skillSlotSet;

		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.StartSkillCooldown(this.skillSlotSet, masteryType, cooldown.Value);
			if (service.MyObjectId == self.ObjectId)
			{
				SkillSlotSet skillSlotSet = this.skillSlotSet;
				SkillSlotSet? currentSkillSlotSet =
					SingletonMonoBehaviour<PlayerController>.inst.PlayerSkill.currentSkillSlotSet;
				if ((skillSlotSet == currentSkillSlotSet.GetValueOrDefault()) & (currentSkillSlotSet != null))
				{
					SingletonMonoBehaviour<PlayerController>.inst.SetCursorStatus(CursorStatus.Normal);
				}
			}
		}
	}
}