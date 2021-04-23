using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdEndConcentration, false)]
	public class CmdEndConcentration : LocalMovableCharacterCommandPacket
	{
		
		[Key(2)] public bool cancel;

		
		[Key(1)] public int skillCode;

		
		public override void Action(ClientService service, LocalMovableCharacter self)
		{
			self.EndConcentration(GameDB.skill.GetSkillData(skillCode), cancel);
		}
	}
}