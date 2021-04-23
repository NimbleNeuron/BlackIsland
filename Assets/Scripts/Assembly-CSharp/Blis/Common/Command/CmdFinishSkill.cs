using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdFinishSkill, false)]
	public class CmdFinishSkill : LocalObjectCommandPacket
	{
		[Key(2)] public bool cancel;


		[Key(1)] public SkillId skillId;


		[Key(3)] public SkillSlotSet skillSlotSet;


		public override void Action(ClientService service, LocalObject self)
		{
			self.FinishSkill(skillId, cancel, skillSlotSet);
		}
	}
}