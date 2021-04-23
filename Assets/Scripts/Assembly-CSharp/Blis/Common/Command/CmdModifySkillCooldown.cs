using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdModifySkillCooldown, false)]
	public class CmdModifySkillCooldown : LocalPlayerCharacterCommandPacket
	{
		
		[Key(1)] public SkillSlotSet skillSlotSetFlag;

		
		[Key(2)] public BlisFixedPoint time;

		
		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			foreach (SkillSlotSet skillSlotSet in GameDB.skill.allSkillSlotSet)
			{
				if (skillSlotSet != SkillSlotSet.None && skillSlotSetFlag.HasFlag(skillSlotSet))
				{
					self.ModifySkillCooldown(skillSlotSet, time.Value);
				}
			}
		}
	}
}