using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqInsertItem, false)]
	public class ReqInsertItem : ReqPacket
	{
		[Key(1)] public int itemId;


		[Key(2)] public ItemMadeType madeType;


		[Key(0)] public int targetId;


		public override void Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldObject worldObject = null;
			if (!gameService.World.TryFind<WorldObject>(targetId, ref worldObject))
			{
				throw new GameException(ErrorType.ObjectNotFound);
			}

			if (worldObject.ItemBox == null)
			{
				throw new GameException(ErrorType.ObjectNotFound);
			}

			if (worldObject.ItemBox.IsFullCapacity())
			{
				throw new GameException(ErrorType.NotEnoughItemBox);
			}

			if (!playerSession.Character.CanAnyAction(ActionType.ItemDrop))
			{
				return;
			}

			if (!playerSession.Character.IsInInteractableDistance(worldObject))
			{
				return;
			}

			Item item = playerSession.Character.FindInventoryItemById(itemId, madeType);
			Item item2 = playerSession.Character.FindEquipById(itemId);
			Item item3 = item != null ? item : item2;
			if (item3 == null)
			{
				throw new GameException(ErrorType.ItemNotFound);
			}

			if (item != null)
			{
				playerSession.Character.FinishBulletCooldown(itemId);
				playerSession.Character.RemoveInventoryItem(item3.id, item3.madeType);
				playerSession.Character.SendInventoryUpdate(UpdateInventoryType.InsertItem);
			}
			else
			{
				playerSession.Character.UnequipItem(item3);
				playerSession.Character.SendEquipmentUpdate();
			}

			worldObject.ItemBox.AddItem(item3);
			Singleton<ItemRoutingRecoder>.inst.AddRoutingLog(playerSession.userId, playerSession.OpenBoxId, itemId);
		}
	}
}