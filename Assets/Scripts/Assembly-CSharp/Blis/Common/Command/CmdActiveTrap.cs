using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdActiveTrap, false)]
	public class CmdActiveTrap : LocalSummonTrapCommandPacket
	{
		public override void Action(ClientService service, LocalSummonTrap self)
		{
			self.OnActiveTrap();
		}
	}
}