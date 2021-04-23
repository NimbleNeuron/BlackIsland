using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcNoticeWicklineKilled, false)]
	public class RpcNoticeWicklineKilled : RpcPacket
	{
		
		[Key(0)] public int attackerObjectId;

		
		public override void Action(ClientService clientService)
		{
			LocalCharacter localCharacter = null;
			if (clientService.World.TryFind<LocalCharacter>(attackerObjectId, ref localCharacter))
			{
				clientService.WicklineDead(localCharacter.ObjectId);
			}
		}
	}
}