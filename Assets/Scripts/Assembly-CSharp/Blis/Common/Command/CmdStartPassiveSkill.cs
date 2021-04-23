using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdStartPassiveSkill, false)]
	public class CmdStartPassiveSkill : LocalObjectCommandPacket
	{
		
		[Key(2)] public int skillCode;

		
		[Key(3)] public int skillEvolutionLevel;

		
		[Key(1)] public SkillId skillId;

		
		[Key(4)] public int targetObjectId;

		
		public override void Action(ClientService service, LocalObject self)
		{
			self.StartPassiveSkill(skillId, GameDB.skill.GetSkillData(skillCode), skillEvolutionLevel, targetObjectId);
		}
	}
}