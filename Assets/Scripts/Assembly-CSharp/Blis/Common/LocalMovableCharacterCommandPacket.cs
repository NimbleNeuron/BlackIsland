using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(CmdUpdateMoveSpeed))]
	[Union(1, typeof(CmdStartConcentration))]
	[Union(2, typeof(CmdEndConcentration))]
	[Union(3, typeof(CmdCrowdControl))]
	[MessagePackObject()]
	public abstract class LocalMovableCharacterCommandPacket : ObjectCommandPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		public override void Action(ClientService service)
		{
			Action(service, service.World.Find<LocalMovableCharacter>(objectId));
		}

		
		public abstract void Action(ClientService service, LocalMovableCharacter self);
	}
}