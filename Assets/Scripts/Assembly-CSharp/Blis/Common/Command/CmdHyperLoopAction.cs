using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.CmdHyperLoopAction, false)]
	public class CmdHyperLoopAction : LocalHyperloopCommandPacket
	{
		public override void Action(ClientService service, LocalHyperloop self)
		{
			self.PlayHyperLoopAnimation();
		}
	}
}