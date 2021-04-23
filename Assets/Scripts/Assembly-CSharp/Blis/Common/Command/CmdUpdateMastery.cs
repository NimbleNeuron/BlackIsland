using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdUpdateMastery, false)]
	public class CmdUpdateMastery : CommandPacket
	{
		[Key(0)] public List<MasteryValue> updates;


		public override void Action(ClientService service)
		{
			service.MyPlayer.OnUpdateMastery(updates, true);
		}
	}
}