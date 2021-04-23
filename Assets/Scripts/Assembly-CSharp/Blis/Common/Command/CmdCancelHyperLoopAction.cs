using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.CmdCancelHyperLoopAction, false)]
	public class CmdCancelHyperLoopAction : LocalHyperloopCommandPacket
	{
		
		public override void Action(ClientService service, LocalHyperloop self)
		{
			self.StopHyperLoopAnimation();
		}
	}
}