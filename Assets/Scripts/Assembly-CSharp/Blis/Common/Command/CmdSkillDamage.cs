using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdSkillDamage, false)]
	public class CmdSkillDamage : LocalCharacterCommandPacket
	{
		[Key(1)] public int attackerId;


		[Key(2)] public int damage;


		[Key(3)] public int effectCode;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.AddDamage(attackerId, true, damage, false, effectCode);
		}
	}
}