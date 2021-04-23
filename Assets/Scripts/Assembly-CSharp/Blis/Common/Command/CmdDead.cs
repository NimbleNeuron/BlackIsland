using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdDead, false)]
	public class CmdDead : LocalCharacterCommandPacket
	{
		[Key(2)] public List<int> assistantIds;


		[Key(1)] public int finishingAttacker;


		public override void Action(ClientService service, LocalCharacter self)
		{
			LocalCharacter attacker =
				finishingAttacker > 0 ? service.World.Find<LocalCharacter>(finishingAttacker) : null;
			self.OnDead(attacker);
			self.IfTypeOf<LocalPlayerCharacter>(delegate(LocalPlayerCharacter character)
			{
				character.SetIsRest(false);
				character.SetWhoKilledMe(finishingAttacker);
			});
		}
	}
}