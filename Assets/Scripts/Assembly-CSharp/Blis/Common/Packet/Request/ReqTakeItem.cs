using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqTakeItem, false)]
	public class ReqTakeItem : ReqPacket
	{
		[Key(1)] public int itemId;


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
				throw new GameException(ErrorType.InvalidAction);
			}

			if (!playerSession.Character.CanAnyAction(ActionType.ItemPickup))
			{
				return;
			}

			if (!playerSession.Character.IsInInteractableDistance(worldObject))
			{
				return;
			}

			Item item = worldObject.ItemBox.FindItem(itemId);
			if (item != null)
			{
				ItemData itemData = item.ItemData;
				bool flag = false;
				if (playerSession.Character.CanAnyAction(ActionType.ItemEquipOrUnequip) &&
				    playerSession.Character.IsEquipableItem(itemData))
				{
					if (itemData.itemType == ItemType.Weapon)
					{
						if (playerSession.Character.GetWeapon() == null)
						{
							flag = true;
						}
					}
					else if (itemData.itemType == ItemType.Armor)
					{
						ItemArmorData subTypeData = itemData.GetSubTypeData<ItemArmorData>();
						if (playerSession.Character.GetArmor(subTypeData.armorType) == null)
						{
							flag = true;
						}
					}
				}

				int num = 0;
				if (flag)
				{
					playerSession.Character.EquipItem(item);
					playerSession.Character.SendEquipmentUpdate();
				}
				else
				{
					playerSession.Character.AddInventoryItem(item, out num);
				}

				if (num == 0)
				{
					worldObject.ItemBox.RemoveItem(item.id);
					if (worldObject.ObjectType == ObjectType.AirSupplyItemBox && worldObject.ItemBox.IsEmpty())
					{
						SingletonMonoBehaviour<BattleEventCollector>.inst.OnAfterAirSupplyItemBoxTakeItem(
							playerSession.Character, worldObject.ItemBox as WorldItemBox);
					}
				}

				playerSession.Character.SendInventoryUpdate(UpdateInventoryType.TakeItem);
				Singleton<ItemRoutingRecoder>.inst.AddRoutingLog(playerSession.userId, playerSession.OpenBoxId,
					item.id);
			}
		}
	}
}