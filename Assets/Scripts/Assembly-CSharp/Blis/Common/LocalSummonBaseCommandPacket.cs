using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdResetSummonDuration))]
	[MessagePackObject()]
	public abstract class LocalSummonBaseCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalSummonBase>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalSummonBase self);
	}
}