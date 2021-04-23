using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdConsoleCheckOut))]
	[Union(1, typeof(CmdActiveConsoleSafeArea))]
	[Union(2, typeof(CmdConsoleAction))]
	[Union(3, typeof(CmdCancelConsoleAction))]
	[MessagePackObject()]
	public abstract class LocalSecurityConsoleCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalSecurityConsole>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalSecurityConsole self);
	}
}