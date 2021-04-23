using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqDropItemFromInventory, false)]
	public class ReqDropItemFromInventory : ReqPacket
	{
		
		[Key(0)] public int itemId;

		
		[Key(2)] public ItemMadeType madeType;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (!playerSession.Character.CanAnyAction(ActionType.ItemDrop))
			{
				return;
			}

			Item item = playerSession.Character.RemoveInventoryItem(itemId, madeType);
			if (item == null)
			{
				return;
			}

			playerSession.Character.FinishBulletCooldown(itemId);
			playerSession.Character.SendInventoryUpdate(UpdateInventoryType.DropItem);
			Vector3 position = playerSession.Character.GetPosition();
			gameService.Spawn.SpawnItem(position, item, gameService.GetDropItemPosition(position));
		}
	}
}