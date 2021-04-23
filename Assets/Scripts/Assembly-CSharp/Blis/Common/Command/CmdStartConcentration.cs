using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdStartConcentration, false)]
	public class CmdStartConcentration : LocalMovableCharacterCommandPacket
	{
		
		[Key(1)] public int skillCode;

		
		public override void Action(ClientService service, LocalMovableCharacter self)
		{
			self.StartConcentration(GameDB.skill.GetSkillData(skillCode));
		}
	}
}