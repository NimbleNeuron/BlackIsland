using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[Union(0, typeof(RpcUpdateEquipment))]
	[Union(1, typeof(RpcBroadCastingUpdateBeforeStart))]
	[MessagePackObject()]
	public abstract class LocalPlayerCharacterRpcPacket : RpcPacket
	{
		
		[IgnoreMember] protected new const int LAST_KEY_IDX = 0;

		
		[Key(0)] public int objectId;

		
		public override void Action(ClientService clientService)
		{
			LocalPlayerCharacter self = null;
			if (!clientService.World.TryFind<LocalPlayerCharacter>(objectId, ref self))
			{
				return;
			}

			Action(clientService, self);
		}

		
		public abstract void Action(ClientService clientService, LocalPlayerCharacter self);
	}
}