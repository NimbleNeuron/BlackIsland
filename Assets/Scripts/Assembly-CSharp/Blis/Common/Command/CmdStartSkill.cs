using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdStartSkill, false)]
	public class CmdStartSkill : LocalObjectCommandPacket
	{
		[Key(1)] public SkillId skillId;
		[Key(2)] public int skillCode;
		[Key(3)] public int skillEvolutionLevel;
		[Key(4)] public int targetObjectId;

		public override void Action(ClientService service, LocalObject self)
		{
			self.StartSkill(skillId, GameDB.skill.GetSkillData(skillCode), skillEvolutionLevel, targetObjectId);
			self.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter character)
			{
				character.PlayStartSkillVoice(skillCode, skillId);
			});
		}
	}
}