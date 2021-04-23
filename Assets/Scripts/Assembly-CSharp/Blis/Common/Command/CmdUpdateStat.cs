using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateStat, false)]
	public class CmdUpdateStat : CommandPacket
	{
		[Key(0)] public List<CharacterStatValue> updates;


		public override void Action(ClientService service)
		{
			service.MyPlayer.Character.UpdateStat(updates);
		}
	}
}