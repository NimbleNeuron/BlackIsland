using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdActiveTrap))]
	[Union(1, typeof(CmdBurstTrap))]
	[Union(2, typeof(CmdInstallRopeTrap))]
	[MessagePackObject()]
	public abstract class LocalSummonTrapCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalSummonTrap>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalSummonTrap self);
	}
}