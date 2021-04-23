using System.Collections.Generic;
using Blis.Client;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.RpcUpdateInventory, false)]
	public class RpcUpdateInventory : RpcPacket
	{
		[Key(0)] public List<InvenItem> updates;


		[Key(1)] public UpdateInventoryType updateType;


		public override void Action(ClientService clientService)
		{
			MyPlayerContext myPlayer = clientService.MyPlayer;
			if (myPlayer != null)
			{
				myPlayer.StopBulletCooldownIds();
			}

			MyPlayerContext myPlayer2 = clientService.MyPlayer;
			if (myPlayer2 == null)
			{
				return;
			}

			myPlayer2.OnUpdateInventory(updates, updateType);
		}
	}
}