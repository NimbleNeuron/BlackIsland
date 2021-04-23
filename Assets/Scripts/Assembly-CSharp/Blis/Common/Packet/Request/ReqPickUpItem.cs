using Blis.Server;
using MessagePack;

namespace Blis.Common
{
	[MessagePackObject]
	[PacketAttr(PacketType.ReqPickUpItem, false)]
	public class ReqPickUpItem : ReqPacketForResponse
	{
		[Key(1)] public int targetId;


		public override ResPacket Action(GameServer gameServer, GameService gameService, PlayerSession playerSession)
		{
			WorldItem worldItem = null;
			if (!gameService.World.TryFind<WorldItem>(targetId, ref worldItem))
			{
				return new ResPickUpItem();
			}

			if (playerSession.Character.SkillController.OnlyMoveInputWhileSkillPlaying())
			{
				playerSession.Character.Controller.MoveTo(worldItem.GetPosition(), false);
				return new ResPickUpItem();
			}

			if (!playerSession.Character.CanAnyAction(ActionType.ItemPickup))
			{
				return new ResPickUpItem();
			}

			if (!playerSession.Character.IsInInteractableDistance(worldItem))
			{
				return new ResPickUpItem();
			}

			Item item = worldItem.GetItem();
			ItemData itemData = item.ItemData;
			int inventoryInsertableCount = playerSession.Character.GetInventoryInsertableCount(itemData);
			bool flag = playerSession.Character.Equipment.CanEquip(item) &&
			            playerSession.Character.CanAnyAction(ActionType.ItemEquipOrUnequip);
			if (!flag && inventoryInsertableCount <= 0)
			{
				return new ResPickUpItem
				{
					errorCode = 1
				};
			}

			int num = 0;
			bool flag2 = false;
			bool flag3 = false;
			if (flag)
			{
				if (itemData.itemType == ItemType.Weapon)
				{
					Item weapon = playerSession.Character.GetWeapon();
					if (weapon == null)
					{
						flag2 = true;
					}
					else if (item.ItemData.code == weapon.ItemData.code &&
					         item.Amount <= weapon.ItemData.stackable - weapon.Amount)
					{
						flag3 = true;
						weapon.Merge(item);
					}
				}
				else if (itemData.itemType == ItemType.Armor)
				{
					ItemArmorData subTypeData = itemData.GetSubTypeData<ItemArmorData>();
					if (playerSession.Character.GetArmor(subTypeData.armorType) == null)
					{
						flag2 = true;
					}
				}
			}

			SingletonMonoBehaviour<BattleEventCollector>.inst.OnBeforePickupItem(playerSession.Character, item);
			if (flag2)
			{
				if (playerSession.Character.EquipItem(item))
				{
					playerSession.Character.SendEquipmentUpdate();
				}
				else
				{
					playerSession.Character.AddInventoryItem(item, out num);
				}
			}
			else if (flag3)
			{
				playerSession.Character.SendEquipmentUpdate();
			}
			else
			{
				if (item.ItemData.IsThrowType())
				{
					playerSession.Character.WeaponItemBulletCooldown(item);
				}

				playerSession.Character.AddInventoryItem(item, out num);
			}

			playerSession.Character.SendInventoryUpdate(UpdateInventoryType.PickupItem);
			if (num == 0)
			{
				gameService.Spawn.DestroyWorldObject(worldItem);
			}

			return new ResPickUpItem
			{
				errorCode = 0
			};
		}
	}
}