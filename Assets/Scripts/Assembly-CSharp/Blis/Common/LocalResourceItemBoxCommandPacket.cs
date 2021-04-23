using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdResourceBoxChildReady))]
	[Union(1, typeof(CmdResourceBoxChildActive))]
	[Union(2, typeof(CmdUpdateResourceBoxCooldown))]
	[MessagePackObject()]
	public abstract class LocalResourceItemBoxCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalResourceItemBox>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalResourceItemBox self);
	}
}