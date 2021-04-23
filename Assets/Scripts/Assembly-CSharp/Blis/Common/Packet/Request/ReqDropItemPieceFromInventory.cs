using Blis.Server;
using MessagePack;
using UnityEngine;

namespace Blis.Common
{
	
	[MessagePackObject()]
	[PacketAttr(PacketType.ReqDropItemPieceFromInventory, false)]
	public class ReqDropItemPieceFromInventory : ReqPacket
	{
		
		[Key(0)] public int itemId;

		
		[Key(1)] public ItemMadeType madeType;

		
		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			if (!playerSession.Character.CanAnyAction(ActionType.ItemDrop))
			{
				return;
			}

			Item item = playerSession.Character.FindInventoryItemById(itemId, madeType);
			if (item == null || item.Amount == 0)
			{
				return;
			}

			item.SubAmount(1);
			if (item.IsEmpty())
			{
				playerSession.Character.FinishBulletCooldown(item.id);
				playerSession.Character.RemoveInventoryItem(item.id, item.madeType);
			}

			playerSession.Character.SendInventoryUpdate(UpdateInventoryType.DropItem);
			Item item2 = gameService.Spawn.CreateItem(item.ItemData, 1);
			item2.AddRecoveryItem(item.addRecovery);
			item2.SetItemSpecialType(item.madeType);
			Vector3 position = playerSession.Character.GetPosition();
			gameService.Spawn.SpawnItem(position, item2, gameService.GetDropItemPosition(position));
		}
	}
}