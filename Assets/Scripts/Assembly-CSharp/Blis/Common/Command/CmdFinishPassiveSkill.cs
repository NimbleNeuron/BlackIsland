using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdFinishPassiveSkill, false)]
	public class CmdFinishPassiveSkill : LocalObjectCommandPacket
	{
		
		[Key(2)] public bool cancel;

		
		[Key(1)] public SkillId skillId;

		
		public override void Action(ClientService service, LocalObject self)
		{
			self.FinishPassiveSkill(skillId, cancel);
		}
	}
}