using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdSwitchSkillSet, false)]
	public class CmdSwitchSkillSet : LocalPlayerCharacterCommandPacket
	{
		[Key(1)] public SkillSlotIndex skillSlotIndex;


		[Key(2)] public SkillSlotSet skillSlotSet;


		public override void Action(ClientService service, LocalPlayerCharacter self)
		{
			self.PlayerContext.SwitchSkillSet(skillSlotIndex, skillSlotSet);
		}
	}
}