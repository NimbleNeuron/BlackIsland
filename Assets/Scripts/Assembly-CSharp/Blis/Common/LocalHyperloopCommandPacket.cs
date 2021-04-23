using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdHyperLoopAction))]
	[Union(1, typeof(CmdCancelHyperLoopAction))]
	[MessagePackObject()]
	public abstract class LocalHyperloopCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalHyperloop>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalHyperloop self);
	}
}