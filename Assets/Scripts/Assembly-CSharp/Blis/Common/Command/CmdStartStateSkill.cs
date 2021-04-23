using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdStartStateSkill, false)]
	public class CmdStartStateSkill : LocalObjectCommandPacket
	{
		[Key(4)] public int casterId;


		[Key(2)] public int skillCode;


		[Key(3)] public int skillEvolutionLevel;


		[Key(1)] public SkillId skillId;


		public override void Action(ClientService service, LocalObject self)
		{
			self.StartStateSkill(skillId, GameDB.skill.GetSkillData(skillCode), skillEvolutionLevel, casterId);
		}
	}
}