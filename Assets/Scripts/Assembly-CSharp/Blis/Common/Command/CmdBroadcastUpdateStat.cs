using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdBroadcastUpdateStat, false)]
	public class CmdBroadcastUpdateStat : LocalCharacterCommandPacket
	{
		
		[Key(1)] public List<CharacterStatValue> updates;

		
		public override void Action(ClientService service, LocalCharacter self)
		{
			self.UpdateStat(updates);
		}
	}
}