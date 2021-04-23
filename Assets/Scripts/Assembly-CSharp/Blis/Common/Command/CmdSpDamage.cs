using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdSpDamage, false)]
	public class CmdSpDamage : LocalCharacterCommandPacket
	{
		[Key(1)] public int attackerId;


		[Key(2)] public int damage;


		[Key(4)] public int effectCode;


		public override void Action(ClientService service, LocalCharacter self)
		{
			self.OnSpDamage(attackerId, damage, effectCode);
		}
	}
}