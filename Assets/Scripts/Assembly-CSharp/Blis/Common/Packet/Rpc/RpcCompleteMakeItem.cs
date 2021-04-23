using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.RpcCompleteMakeItem, false)]
	public class RpcCompleteMakeItem : RpcPacket
	{
		
		public override void Action(ClientService clientService)
		{
			MyPlayerContext myPlayer = clientService.MyPlayer;
			if (myPlayer == null)
			{
				return;
			}

			myPlayer.MakeSourceItemBulletCooldownClean();
		}
	}
}