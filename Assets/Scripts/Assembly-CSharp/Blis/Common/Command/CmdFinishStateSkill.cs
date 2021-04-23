using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdFinishStateSkill, false)]
	public class CmdFinishStateSkill : LocalObjectCommandPacket
	{
		
		[Key(2)] public bool cancel;

		
		[Key(1)] public SkillId skillId;

		
		public override void Action(ClientService service, LocalObject self)
		{
			self.FinishStateSkill(skillId, cancel);
		}
	}
}