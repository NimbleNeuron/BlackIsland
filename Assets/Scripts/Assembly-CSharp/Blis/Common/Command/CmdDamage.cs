using Blis.Client;
using Blis.Common.Utils;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdDamage, false)]
	public class CmdDamage : LocalCharacterCommandPacket
	{
		[Key(1)] public int attackerId;


		[Key(2)] public int damage;


		[Key(4)] public int effectCode;


		[Key(3)] public bool isCritical;


		public override void Action(ClientService service, LocalCharacter self)
		{
			if (self.ObjectId != attackerId && self.IsTypeOf<LocalPlayerCharacter>())
			{
				LocalPlayerCharacter target = null;
				if (MonoBehaviourInstance<ClientService>.inst.World.TryFind<LocalPlayerCharacter>(attackerId,
					ref target))
				{
					if (MonoBehaviourInstance<ClientService>.inst.IsPlayer)
					{
						if (service.IsAlly(self))
						{
							service.SetInBattleByPlayer(self.ObjectId);
						}

						if (service.IsAlly(target))
						{
							service.SetInBattleByPlayer(attackerId);
						}
					}
					else
					{
						service.SetInBattleByPlayer(self.ObjectId);
						service.SetInBattleByPlayer(attackerId);
					}
				}
			}

			self.AddDamage(attackerId, false, damage, isCritical, effectCode);
		}
	}
}